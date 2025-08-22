using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.Buffs;
using TalesoftheEntropicSea.Content.NPCs.GABoss;
using TalesoftheEntropicSea.Content.NPCs.HeartOfTheOcean;
using TalesoftheEntropicSea.Content.Projectiles.GABoss;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.NPCs.GABoss
{
    public class Head : ModNPC
    {
        private Rectangle arenaBounds;
        private bool arenaSet = false;
        private Vector2? fixedPosition = null; 

        
        public int indicatorTimer = 0;
        public int indicatorShootIndex = 0;
        public int indicatorShootCooldown = 0;
        public readonly List<Vector2> indicatorPositions = new();
        public Vector2 lockedAimDirection;
        public const int TelegraphDuration = 60;
        public const int FireRate = 5;
        public const int Steps = 30;
        public const float Radius = 80f;
        

        private float bubbleTimer = 0f;
        




        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;

           
        }

        public override void SetDefaults()
        {
            NPC.width = 214;
            NPC.height = 152;
            NPC.damage = 100;
            NPC.defense = 150;
            NPC.lifeMax = 8000000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.aiStyle = -1;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/HelmetBoss");

            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ThalassophobiaDebuff>()] = true;

        }




        public override void OnSpawn(IEntitySource source)
        {
            NPC.position = Main.screenPosition + new Vector2(Main.screenWidth + 300f, Main.screenHeight - NPC.height);
        }

        public override void AI()
        {
            if (NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int latest = NPC.whoAmI;

                int topBody = SpawnSegment(ModContent.NPCType<TopBody>(), latest);
                int middleBody = SpawnSegment(ModContent.NPCType<MiddleBody>(), topBody);
                int bottomBody = SpawnSegment(ModContent.NPCType<BottomBody>(), middleBody);
                latest = bottomBody;

                for (int i = 0; i < 3; i++) latest = SpawnSegment(ModContent.NPCType<TailTop>(), latest);
                for (int i = 0; i < 2; i++) latest = SpawnSegment(ModContent.NPCType<TailMiddle>(), latest);
                latest = SpawnSegment(ModContent.NPCType<TailBottom>(), latest);
                latest = SpawnSegment(ModContent.NPCType<TailLast>(), latest);
                latest = SpawnSegment(ModContent.NPCType<TailTip>(), latest);

                SpawnWing(ModContent.NPCType<TopWingLeft>(), topBody, -1);
                SpawnWing(ModContent.NPCType<TopWingRight>(), topBody, 1);
                SpawnWing(ModContent.NPCType<MiddleWingLeft>(), middleBody, -1);
                SpawnWing(ModContent.NPCType<MiddleWingRight>(), middleBody, 1);
                SpawnWing(ModContent.NPCType<BottomWingLeft>(), bottomBody, -1);
                SpawnWing(ModContent.NPCType<BottomWingRight>(), bottomBody, 1);

                NPC.localAI[0] = 1f;
            }

            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[1]++;
                if (NPC.localAI[1] == 180f && !fixedPosition.HasValue)
                {
                    fixedPosition = NPC.Center;
                    NPC.netUpdate = true;
                }
            }

            
            if (fixedPosition.HasValue)
            {
                NPC.velocity = Vector2.Zero;
                NPC.Center = fixedPosition.Value;
            }
            else
            {
                Vector2 screenTarget = Main.screenPosition + new Vector2(Main.screenWidth - 300f, 200f);
                float speed = 10f;
                Vector2 toTarget = screenTarget - NPC.Center;

                if (toTarget.Length() > 10f)
                    NPC.velocity = Vector2.Lerp(NPC.velocity, toTarget.SafeNormalize(Vector2.Zero) * speed, 0.08f);
                else
                    NPC.velocity = Vector2.Zero;
            }

            
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            NPC.rotation = (player.Center - NPC.Center).ToRotation() + MathHelper.PiOver2;

            
            if (!arenaSet)
            {
                Vector2 arenaCenter = player.Center;
                int arenaWidth = 1904;
                int arenaHeight = 1000;

                arenaBounds = new Rectangle(
                    (int)(arenaCenter.X - arenaWidth / 2f),
                    (int)(arenaCenter.Y - arenaHeight / 2f),
                    arenaWidth,
                    arenaHeight
                );

                global::TalesoftheEntropicSea.World.WorldSystem.ArenaBounds = arenaBounds;

                ushort tileType = (ushort)ModContent.TileType<Content.Tiles.SkyAbyssStone>();
                SpawnArenaTiles(arenaBounds, tileType);

                arenaSet = true;
            }

            
            if (arenaSet)
            {
                ushort tileType = (ushort)ModContent.TileType<Content.Tiles.SkyAbyssStone>();

                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead) continue;

                    bool inArena = arenaBounds.Contains(p.Center.ToPoint());

                    if (!inArena)
                    {
                        Point tile = p.Center.ToTileCoordinates();
                        Tile t = Framing.GetTileSafely(tile.X, tile.Y);

                        if (t.HasTile && t.TileType == tileType)
                        {
                            Vector2 direction = (arenaBounds.Center.ToVector2() - p.Center).SafeNormalize(Vector2.UnitY);
                            p.velocity = direction * 10f;
                            p.Hurt(PlayerDeathReason.ByCustomReason($"{p.name} was repelled by the abyss."), 100, 0);
                        }

                        p.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 60);
                    }
                }
            }

            
            bool allPlayersDead = true;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead)
                {
                    allPlayersDead = false;
                    break;
                }
            }

            if (allPlayersDead)
            {
                CleanupArena(); 
                NPC.active = false;
                return;
            }
            else
            {
                NPC.timeLeft = 600; 
            }

            
            int currentAttack = (int)NPC.ai[0];
            float attackTimer = NPC.ai[1]++;
            Player target = Main.player[NPC.target];

            switch ((int)NPC.ai[0])
            {
                case 0:
                case 1:
                case 2:
                    DoFanAttack(NPC, target, (int)NPC.ai[0], ref NPC.ai[1]);
                    break;
                case 3:
                    DoDebrisAttack(NPC, target, ref NPC.ai[1]);
                    break;
                case 4:
                    DoSummonCrittersAttack(NPC, ref NPC.ai[1]);
                    break;
                case 5:
                    DoVerticalSharkAttack(NPC, target, ref NPC.ai[1]);
                    break;
                case 6:
                    DoUrchingBurstAttack(NPC, target, ref NPC.ai[1]);
                    break;
                default:
                    SelectNextAttack(NPC); 
                    break;
            }


            
            bubbleTimer++;

            if (bubbleTimer >= 300f) 
            {
                
                if ((int)bubbleTimer % 120 == 0)
                {
                    Vector2 spawnPos = new Vector2(target.Center.X, Main.screenPosition.Y + Main.screenHeight + 200f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            spawnPos,
                            Vector2.Zero,
                            ModContent.ProjectileType<BubbleBlastProjectile>(), 
                            170, 
                            0f,
                            Main.myPlayer
                        );
                    }
                }
            }





        }




        private void DoFanAttack(NPC npc, Player target, int variant, ref float timer)
        {
            int telegraphCount = 30;
            float fanAngle = MathHelper.ToRadians(200f);
            float radius = 650f;

            if (timer == 1f)
            {
                indicatorPositions.Clear();
                lockedAimDirection = (target.Center - npc.Center).SafeNormalize(Vector2.UnitY);
                float baseRotation = lockedAimDirection.ToRotation();

                for (int i = 0; i < telegraphCount; i++)
                {
                    float angleOffset = MathHelper.Lerp(-fanAngle / 2f, fanAngle / 2f, i / (float)(telegraphCount - 1));
                    Vector2 direction = baseRotation.ToRotationVector2().RotatedBy(angleOffset);
                    indicatorPositions.Add(direction);
                }

                indicatorShootIndex = 0;
                indicatorShootCooldown = 0;
                NPC.ai[2] = 0;
            }

            if (NPC.ai[2] == 0)
            {
                if (++NPC.ai[3] >= 2)
                {
                    NPC.ai[3] = 0;
                    if (indicatorShootIndex < indicatorPositions.Count)
                    {
                        Vector2 dir = indicatorPositions[indicatorShootIndex];
                        Vector2 origin = npc.Center + dir * radius;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), origin, dir,
                                ModContent.ProjectileType<ToothTelegraphLine>(), 0, 0f, Main.myPlayer);
                            if (proj.ModProjectile is ToothTelegraphLine line)
                                line.Lifetime = TelegraphDuration;
                        }

                        indicatorShootIndex++;
                    }
                    else
                    {
                        NPC.ai[2] = 1;
                        indicatorTimer = TelegraphDuration;
                        indicatorShootIndex = 0;
                    }
                }
            }
            else if (NPC.ai[2] == 1)
            {
                indicatorTimer--;
                if (indicatorTimer <= 0)
                {
                    NPC.ai[2] = 2;
                }
            }
            else if (NPC.ai[2] == 2)
            {
                if (++indicatorShootCooldown >= FireRate)
                {
                    indicatorShootCooldown = 0;
                    if (indicatorShootIndex < indicatorPositions.Count)
                    {
                        Vector2 dir = indicatorPositions[indicatorShootIndex];
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, dir * 16f,
                                ModContent.ProjectileType<ToothProj>(), 120, 0f, Main.myPlayer);
                        }
                        SoundEngine.PlaySound(SoundID.Item17, npc.Center);
                        indicatorShootIndex++;
                    }
                    else
                    {
                        SelectNextAttack(npc);
                    }
                }
            }
        }

        private void DoDebrisAttack(NPC npc, Player target, ref float timer)
        {
            int debrisCount = 8;
            float height = 600f;

            if (timer == 1f)
            {
                indicatorPositions.Clear();

                Rectangle arena = arenaBounds;
                for (int i = 0; i < debrisCount; i++)
                {
                    float x = Main.rand.NextFloat(arena.Left, arena.Right);
                    float y = arena.Top - height;

                    Vector2 pos = new Vector2(x, y);
                    indicatorPositions.Add(pos);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), pos, Vector2.UnitY,
                            ModContent.ProjectileType<ToothTelegraphLine>(), 0, 0f, Main.myPlayer);

                        if (proj.ModProjectile is ToothTelegraphLine line)
                            line.Lifetime = TelegraphDuration;
                    }
                }

                NPC.ai[2] = 0;
                indicatorShootIndex = 0;
                indicatorShootCooldown = 0;
            }

            if (NPC.ai[2] == 0)
            {
                if (timer >= TelegraphDuration + 20)
                {
                    NPC.ai[2] = 1;
                }
            }
            else if (NPC.ai[2] == 1)
            {
                if (++indicatorShootCooldown >= FireRate)
                {
                    indicatorShootCooldown = 0;

                    if (indicatorShootIndex < indicatorPositions.Count)
                    {
                        Vector2 pos = indicatorPositions[indicatorShootIndex];
                        Vector2 velocity = Vector2.UnitY * 12f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int[] debris = {
                        ModContent.ProjectileType<FallingDebrisSmall>(),
                        ModContent.ProjectileType<FallingDebrisMedium>(),
                        ModContent.ProjectileType<FallingDebrisBig>()
                    };

                            int chosen = Main.rand.Next(debris);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), pos, velocity, chosen, 120, 0f, Main.myPlayer);

                            
                            Main.projectile[proj].tileCollide = false;
                        }
                        SoundEngine.PlaySound(SoundID.Item88, npc.Center);
                        indicatorShootIndex++;
                    }
                    else
                    {
                        SelectNextAttack(npc);
                    }
                }
            }
        }

        private void DoSummonCrittersAttack(NPC npc, ref float timer)
        {
            int critterCount = 3;

            if (timer == 1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < critterCount; i++)
                    {
                        Vector2 spawnPos = new Vector2(
                            Main.rand.NextFloat(arenaBounds.Left, arenaBounds.Right),
                            Main.rand.NextFloat(arenaBounds.Top, arenaBounds.Bottom)
                        );

                        int id = NPC.NewNPC(npc.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y,
                            ModContent.NPCType<FeatherStarCritter>());

                        if (Main.npc.IndexInRange(id))
                        {
                            Main.npc[id].velocity = Vector2.UnitY * 2f; 
                        }
                    }
                }
            }

            
            if (timer >= 300f)
            {
                SelectNextAttack(npc); 
            }
        }


        private void DoVerticalSharkAttack(NPC npc, Player target, ref float timer)
        {
            int sharkCount = 8;
            float lineLength = 800f;
            float sharkSpeed = 14f;
            float angle = MathHelper.ToRadians(15f); 

            Vector2 direction = new Vector2(1f, 0f).RotatedBy(angle).SafeNormalize(Vector2.UnitX); 
            Vector2 offsetPerLine = new Vector2(600f, 0f); 

            if (timer == 1f)
            {
                indicatorPositions.Clear();

                
                Vector2 start = new Vector2(arenaBounds.Left + 200f, arenaBounds.Center.Y);

                for (int i = 0; i < sharkCount; i++)
                {
                    Vector2 telegraphPos = start + offsetPerLine * i;
                    indicatorPositions.Add(telegraphPos); 

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile telegraph = Projectile.NewProjectileDirect(
                            npc.GetSource_FromAI(),
                            telegraphPos,
                            direction,
                            ModContent.ProjectileType<ToothTelegraphLine>(),
                            0, 0f, Main.myPlayer
                        );

                        if (telegraph.ModProjectile is ToothTelegraphLine line)
                        {
                            line.Lifetime = TelegraphDuration;
                            line.Projectile.rotation = direction.ToRotation();
                            line.Projectile.localAI[0] = lineLength;
                        }
                    }
                }

                NPC.ai[2] = 0;
                indicatorShootIndex = 0;
                indicatorShootCooldown = 0;
            }

            if (NPC.ai[2] == 0)
            {
                if (timer >= 60f) 
                {
                    NPC.ai[2] = 1;
                }
            }
            else if (NPC.ai[2] == 1)
            {
                if (++indicatorShootCooldown >= FireRate)
                {
                    indicatorShootCooldown = 0;

                    if (indicatorShootIndex < indicatorPositions.Count)
                    {
                        Vector2 spawn = indicatorPositions[indicatorShootIndex] - direction * 600f;
                        Vector2 velocity = direction * sharkSpeed;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(
                                npc.GetSource_FromAI(),
                                spawn,
                                velocity,
                                ModContent.ProjectileType<SharkProjectile>(),
                                150, 0f, Main.myPlayer
                            );
                        }
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PrimordialWyrmCharge"), npc.Center);

                        indicatorShootIndex++;
                    }
                }
            }

            
            if (timer >= 300f)
            {
                SelectNextAttack(npc);
            }
        }


        private void DoUrchingBurstAttack(NPC npc, Player target, ref float timer)
        {
            int urchinCount = 3;
            float shootSpeed = 6f;
            float spread = MathHelper.ToRadians(20f); 

            if (timer == 1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    
                    float baseAngle = (target.Center - npc.Center).ToRotation();

                    for (int i = 0; i < urchinCount; i++)
                    {
                        float angleOffset = MathHelper.Lerp(-spread / 2f, spread / 2f, i / (float)(urchinCount - 1));
                        Vector2 shootDirection = (baseAngle + angleOffset).ToRotationVector2();

                        
                        Vector2 spawnPos = npc.Center + new Vector2(0f, -20f);

                        Projectile.NewProjectile(
                            npc.GetSource_FromAI(),
                            spawnPos,
                            shootDirection * shootSpeed,
                            ModContent.ProjectileType<SeaUrchingProjectile>(),
                            150, 0f, Main.myPlayer
                        );
                    }
                }

                
                NPC.ai[2] = 0;
                indicatorShootIndex = 0;
                indicatorShootCooldown = 0;
            }

            
            if (timer >= 240f)
            {
                SelectNextAttack(npc);
            }
        }






        private void SelectNextAttack(NPC npc)
        {
            int totalAttacks = 7; 
            npc.ai[0] = (npc.ai[0] + 1) % totalAttacks;
            npc.ai[1] = npc.ai[2] = npc.ai[3] = 0;
        }


        private void CleanupArena()
        {
            ushort tileType = (ushort)ModContent.TileType<Content.Tiles.SkyAbyssStone>();

            int left = arenaBounds.Left / 16;
            int right = arenaBounds.Right / 16;
            int top = arenaBounds.Top / 16;
            int bottom = arenaBounds.Bottom / 16;

            for (int x = left; x <= right; x++)
            {
                TryKillBorderTile(x, top, tileType);
                TryKillBorderTile(x, bottom, tileType);
            }

            for (int y = top + 1; y < bottom; y++)
            {
                TryKillBorderTile(left, y, tileType);
                TryKillBorderTile(right, y, tileType);
            }

            global::TalesoftheEntropicSea.World.WorldSystem.ArenaBounds = Rectangle.Empty;
        }

        public override void OnKill()
        {
            CleanupArena();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC(
                    NPC.GetSource_FromAI(),
                    (int)NPC.Center.X,
                    (int)NPC.Center.Y,
                    ModContent.NPCType<global::TalesoftheEntropicSea.Content.NPCs.HeartOfTheOcean.HeartOfTheOcean>()


                );
            }
        }



        private void SpawnArenaTiles(Rectangle bounds, ushort tileType)
        {
            int left = bounds.Left / 16;
            int right = bounds.Right / 16;
            int top = bounds.Top / 16;
            int bottom = bounds.Bottom / 16;

            for (int x = left; x <= right; x++)
            {
                WorldGen.PlaceTile(x, top, tileType, true, true);
                WorldGen.PlaceTile(x, bottom, tileType, true, true);
            }

            for (int y = top + 1; y < bottom; y++)
            {
                WorldGen.PlaceTile(left, y, tileType, true, true);
                WorldGen.PlaceTile(right, y, tileType, true, true);
            }
        }

        private void TryKillBorderTile(int x, int y, ushort tileType)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile && tile.TileType == tileType)
            {
                WorldGen.KillTile(x, y, false, false, true);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, x, y, 1);
            }
        }

        private int SpawnSegment(int type, int follow)
        {
            int id = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type);
            Main.npc[id].ai[1] = follow;
            Main.npc[id].ai[2] = NPC.whoAmI;
            Main.npc[id].realLife = NPC.whoAmI;
            return id;
        }

        private void SpawnWing(int type, int parentID, int direction)
        {
            int id = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type);
            Main.npc[id].ai[1] = parentID;
            Main.npc[id].ai[2] = direction;
        }

        /*
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/NPCs/GABoss/Head"
            ).Value;

            Rectangle frame = NPC.frame;
            Vector2 origin = frame.Size() / 2f;

            // Match the head's base texture flip
            SpriteEffects effects = NPC.spriteDirection == 1
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(glowTexture, NPC.Center - screenPos, frame, Color.White, NPC.rotation, origin, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f );
        }
        */
    }
}
