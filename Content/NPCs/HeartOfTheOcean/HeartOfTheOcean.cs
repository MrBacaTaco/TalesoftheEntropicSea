using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.Buffs;
using TalesoftheEntropicSea.Content.Items;
using TalesoftheEntropicSea.Content.Projectiles.GABoss;
using TalesoftheEntropicSea.Content.Projectiles.Heart;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers; 
using Terraria.ID;
using Terraria.ModLoader;




namespace TalesoftheEntropicSea.Content.NPCs.HeartOfTheOcean
{
    public class HeartOfTheOcean : ModNPC
    {
        public override string Texture => "TalesoftheEntropicSea/Content/NPCs/HeartOfTheOcean/HeartOfTheOcean";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 100;
            NPC.defense = 150;
            NPC.lifeMax = 4000000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 20, 0, 0);
            NPC.npcSlots = 10f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/HelmetBoss");

            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ThalassophobiaDebuff>()] = true;
        }


        private List<Vector2> indicatorPositions = new();
        private Vector2 lockedAimDirection;
        private int indicatorShootIndex;
        private int indicatorShootCooldown;
        private int indicatorTimer;

        private const int TelegraphDuration = 30;
        private const int FireRate = 4;

        //weapon summon
        private bool weaponsSummoned = false;

        //final phase check
        private bool finalPhaseStarted = false;

        //head stuff
        private Projectile wyrmHead;
        private Projectile wyrmBody;

        //cutscene
        private static readonly int CutsceneTrailLength = 6; 
        private Vector2[] wyrmHeadHistory = new Vector2[CutsceneTrailLength];
        private bool wyrmHistoryInitialized = false;

        //whip

        //Wyrm
        private const int AbyssWyrmDuration = 60 * 20; 
        private int abyssWyrmHeadId = -1;              
        public override bool CheckActive() => false;


        private enum BossState
        {
            FanAttack = 0,
            DebrisAttack = 1,
            UrchinAttack = 2,
            WeaponSummon = 3,
            FinalPhaseCutscene = 4,
            EidolonWhipAttack = 5,
            RegionAttack = 6,
            AnchorShipAttack = 7,
            AbyssWyrmAttack = 8,
            FinalPhaseAttacks = 9

            
        }



        public override void AI()
        {
            NPC.timeLeft = 18000;


            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                if (!player.active || player.dead)
                {
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }

            
            Vector2 toPlayer = player.Center - NPC.Center;
            float speed = 6f;
            Vector2 desiredVelocity = toPlayer.SafeNormalize(Vector2.Zero) * speed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 0.05f);

            
            int currentAttack = (int)NPC.ai[0];
            float timer = NPC.ai[1]++;

            switch ((int)NPC.ai[0])
            {
                case 0:
                    DoFanAttack(NPC, player, 0, ref NPC.ai[1]);
                    break;
                case 1:
                    DoDebrisAttack(NPC, player, ref NPC.ai[1]);
                    break;
                case 2:
                    DoUrchingBurstAttack(NPC, player, ref NPC.ai[1]);
                    break;
                case 3:
                    DoWeaponSummonAttack(NPC, player, ref NPC.ai[1]);
                    break;
                case 4:
                    DoFinalPhaseCutscene(NPC, Main.player[NPC.target]);
                    break;
                case 5:
                    DoEidolonWhipAttack(NPC, player, ref NPC.ai[1]);
                    break;
                case 6:
                    DoRegionAttack(NPC, player, ref NPC.ai[1]);
                    break;
                case 7:
                    DoAnchorShipAttack(NPC, player, ref NPC.ai[1]);
                    break;
                case (int)BossState.AbyssWyrmAttack:
                    DoAbyssWyrmAttack(NPC, player, ref NPC.ai[1]);
                    break;


                default:
                    SelectNextAttack(NPC);
                    break;
            }




            if (!finalPhaseStarted && NPC.life < NPC.lifeMax * 0.4f)
            {
                finalPhaseStarted = true;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active &&
                        (proj.type == ModContent.ProjectileType<SwordWeapon>() ||
                         proj.type == ModContent.ProjectileType<BowWeapon>() ||
                         proj.type == ModContent.ProjectileType<SwordSlashHitbox>()))
                    {
                        proj.Kill();
                    }
                }

                NPC.ai[0] = 4;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.velocity = Vector2.Zero;

                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                return;
            }


        }






        private void DoFanAttack(NPC npc, Player target, int variant, ref float timer)
        {
            int telegraphCount = 30;
            float fanAngle = MathHelper.ToRadians(200f);
            float radius = 650f;
            float desiredDistance = 160f; 

            Vector2 toPlayer = target.Center - npc.Center;
            float currentDistance = toPlayer.Length();

            if (currentDistance > desiredDistance + 20f) 
            {
                npc.velocity = toPlayer.SafeNormalize(Vector2.Zero) * 6f;
            }
            else if (currentDistance < desiredDistance - 20f) 
            {
                npc.velocity = -toPlayer.SafeNormalize(Vector2.Zero) * 6f;
            }
            else
            {
                npc.velocity = Vector2.Zero; 
            }

            
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
                        indicatorTimer = TelegraphDuration / 2; 
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
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, dir * 32f, 
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
            int debrisCount = 15;
            float heightAbove = 1000f;
            int arenaWidth = 2000; 
            int arenaPadding = 100;

            if (timer == 1f)
            {
                indicatorPositions.Clear();

                
                Rectangle dynamicArena = new Rectangle(
                    (int)(npc.Center.X - arenaWidth / 2),
                    (int)(npc.Center.Y - heightAbove),
                    arenaWidth,
                    (int)heightAbove
                );

                for (int i = 0; i < debrisCount; i++)
                {
                    float x = Main.rand.NextFloat(dynamicArena.Left + arenaPadding, dynamicArena.Right - arenaPadding);
                    float y = dynamicArena.Top;

                    Vector2 pos = new Vector2(x, y);
                    indicatorPositions.Add(pos);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var proj = Projectile.NewProjectileDirect(
                            npc.GetSource_FromAI(),
                            pos,
                            Vector2.UnitY,
                            ModContent.ProjectileType<ToothTelegraphLine>(),
                            0, 0f, Main.myPlayer
                        );

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
                if (timer >= TelegraphDuration + 15f)
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
                            int[] debrisOptions = {
                        ModContent.ProjectileType<FallingDebrisSmall>(),
                        ModContent.ProjectileType<FallingDebrisMedium>(),
                        ModContent.ProjectileType<FallingDebrisBig>()
                    };

                            int chosen = Main.rand.Next(debrisOptions);
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


        private void DoUrchingBurstAttack(NPC npc, Player target, ref float timer)
        {
            int urchinCount = 5;
            float minSpeed = 4f;
            float maxSpeed = 7f;

            if (timer == 1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < urchinCount; i++)
                    {
                        float randomAngle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                        Vector2 direction = randomAngle.ToRotationVector2();

                        float speed = Main.rand.NextFloat(minSpeed, maxSpeed);
                        Vector2 velocity = direction * speed;

                       
                        Vector2 spawnOffset = direction * Main.rand.NextFloat(20f, 60f);
                        Vector2 spawnPos = npc.Center + spawnOffset;

                        Projectile.NewProjectile(
                            npc.GetSource_FromAI(),
                            spawnPos,
                            velocity,
                            ModContent.ProjectileType<SeaUrchingProjectile>(),
                            150, 0f, Main.myPlayer
                        );
                    }
                }

                
                npc.ai[2] = 0;
                indicatorShootIndex = 0;
                indicatorShootCooldown = 0;
            }

            if (timer >= 240f)
            {
                SelectNextAttack(npc);
            }
        }

        private void DoWeaponSummonAttack(NPC npc, Player player, ref float timer)
        {
            if (timer == 1f && !weaponsSummoned) 
            {
                weaponsSummoned = true; 

                Vector2 swordPos = player.Center + new Vector2(-80, -500f);
                Vector2 bowPos = player.Center + new Vector2(80, -500f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), swordPos, Vector2.Zero,
                        ModContent.ProjectileType<SwordWeapon>(), 100, 0f, 255);

                    Projectile.NewProjectile(npc.GetSource_FromAI(), bowPos, Vector2.Zero,
                        ModContent.ProjectileType<BowWeapon>(), 120, 0f, Main.myPlayer);
                }

                SoundEngine.PlaySound(SoundID.Item123, npc.Center);
            }

            if (timer >= 240f)
            {
                SelectNextAttack(npc);
            }
        }



        private void DoFinalPhaseCutscene(NPC npc, Player player)
        {
            npc.velocity = Vector2.Zero;

            Vector2 bossCenter = npc.Center;
            Vector2 finalHeadPos = bossCenter + new Vector2(7 * 16f, 5 * 16f);
            float riseStartBelow = 700f;
            const float overlapPx = -50f;

            float riseSpeed = 14f;
            int shakeTime = 40;
            float shakeAmp = 8f;
            int driftTime = 60; 
            float driftSpeed = 1.2f; 

            if (npc.localAI[0] == 0f)
            {
                Vector2 offscreenStart = player.Center + new Vector2(0f, riseStartBelow);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int headIdx = Projectile.NewProjectile(
                        npc.GetSource_FromAI(),
                        offscreenStart,
                        Vector2.Zero,
                        ModContent.ProjectileType<EidolonHead>(),
                        0, 0f, Main.myPlayer);

                    int bodyIdx = Projectile.NewProjectile(
                        npc.GetSource_FromAI(),
                        offscreenStart,
                        Vector2.Zero,
                        ModContent.ProjectileType<EidolonBody>(),
                        0, 0f, Main.myPlayer);

                    npc.localAI[2] = headIdx;
                    npc.localAI[3] = bodyIdx;

                    if (Main.projectile[headIdx].active) { Main.projectile[headIdx].ai[0] = 1f; Main.projectile[headIdx].netUpdate = true; }
                    if (Main.projectile[bodyIdx].active) { Main.projectile[bodyIdx].ai[0] = 1f; Main.projectile[bodyIdx].netUpdate = true; }
                }

                npc.localAI[1] = 0f; // phase
                npc.ai[2] = 0f;      // timer
                npc.localAI[0] = 1f;

                Main.NewText("Wyrm is being summoned!", Color.Cyan);
            }

            // --- Get projectiles ---
            int hi = (int)npc.localAI[2];
            int bi = (int)npc.localAI[3];
            if (hi < 0 || hi >= Main.maxProjectiles || bi < 0 || bi >= Main.maxProjectiles)
                return;

            Projectile h = Main.projectile[hi];
            Projectile b = Main.projectile[bi];
            if (!h.active || h.type != ModContent.ProjectileType<EidolonHead>()) return;
            if (!b.active || b.type != ModContent.ProjectileType<EidolonBody>()) return;

            // Always keep them alive
            h.timeLeft = 60;
            b.timeLeft = 60;

            Vector2 ForwardFromRot(float rot) => new Vector2(0f, -1f).RotatedBy(rot);

            int phase = (int)npc.localAI[1];

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (phase == 0) // Rise
                {
                    Vector2 toTarget = finalHeadPos - h.Center;
                    float dist = toTarget.Length();
                    Vector2 dir = dist > 0.001f ? toTarget / dist : Vector2.Zero;

                    h.velocity = dir * riseSpeed;
                    if (h.velocity.LengthSquared() > 0.01f)
                        h.rotation = h.velocity.ToRotation() + MathHelper.PiOver2;

                    Vector2 fwd = ForwardFromRot(h.rotation);
                    Vector2 desiredBodyFront = h.Center - fwd * (overlapPx * b.scale);
                    b.Center = Vector2.Lerp(b.Center, desiredBodyFront, 0.6f);
                    b.rotation = h.rotation;
                    b.velocity = Vector2.Zero;

                    if (dist < 12f)
                    {
                        h.Center = finalHeadPos;
                        h.velocity = Vector2.Zero;
                        npc.localAI[1] = 1f; 
                        npc.ai[2] = 0f;
                    }
                }
                else if (phase == 1) 
                {
                    float t = npc.ai[2];
                    float w = 0.35f;

                    Vector2 fwd = ForwardFromRot(h.rotation);
                    Vector2 right = fwd.RotatedBy(MathHelper.PiOver2);

                    Vector2 jitter = right * (float)Math.Sin(t * w) * shakeAmp
                                   + fwd * (float)Math.Sin(t * w * 1.7f) * (shakeAmp * 0.35f);

                    h.Center = finalHeadPos + jitter;
                    h.rotation += (float)Math.Sin(t * w * 2.2f) * 0.02f;

                    Vector2 desiredBodyFront = h.Center - fwd * (overlapPx * b.scale);
                    b.Center = Vector2.Lerp(b.Center, desiredBodyFront, 0.25f);
                    b.rotation = h.rotation;

                    if (t >= shakeTime)
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/PrimordialWyrmDeath"), h.Center);

                        npc.localAI[1] = 2f; 
                        npc.ai[2] = 0f;
                    }
                }
                else if (phase == 2) 
                {
                    float t = npc.ai[2] / driftTime;
                    t = MathHelper.Clamp(t, 0f, 1f);

                    Vector2 fwd = ForwardFromRot(h.rotation);

                    
                    h.Center += fwd * driftSpeed;
                    h.rotation += 0.01f;

                    
                    if (npc.ai[2] == 0f)
                    {
                        b.ai[0] = 2f; 
                        b.velocity = Vector2.Zero;
                    }

                    if (npc.ai[2] >= driftTime)
                    {
                       
                        h.velocity = fwd * 4f;
                        h.ai[0] = 2f;
                        b.velocity = fwd * 4f;
                        b.ai[0] = 0f;

                        npc.localAI[0] = 0f; 
                        npc.localAI[1] = 0f; 
                        npc.localAI[2] = 0f; 
                        npc.localAI[3] = 1f; 

                        npc.ai[0] = -1f; 
                        SelectNextAttack(npc);

                        Main.NewText("Final Phase Cutscene Ended", Color.Cyan);
                    }

                }


            }

            npc.ai[2]++;
        }



        private void DoEidolonWhipAttack(NPC npc, Player player, ref float timer)
        {
            float whipAttackTime = 120f; 
            float swingRadius = 320f;

            bool whipOnRight = player.Center.X >= npc.Center.X;
            float sideDir = whipOnRight ? 1f : -1f; 

            Vector2 anchorPoint = npc.Center + new Vector2(sideDir * (npc.width / 2f), 0f);

            if (npc.localAI[1] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int whipProj = Projectile.NewProjectile(
                        npc.GetSource_FromAI(),
                        anchorPoint,
                        Vector2.Zero,
                        ModContent.ProjectileType<EidolonTale>(),
                        150, 0f, Main.myPlayer, npc.whoAmI, 0f
                    );
                    npc.localAI[2] = whipProj;
                }
                npc.localAI[1] = 1f;
            }

            int whipIdx = (int)npc.localAI[2];
            if (whipIdx >= 0 && whipIdx < Main.maxProjectiles)
            {
                Projectile whip = Main.projectile[whipIdx];
                if (whip != null && whip.active && whip.type == ModContent.ProjectileType<EidolonTale>())
                {
                   
                    anchorPoint = npc.Center + new Vector2(sideDir * (npc.width / 2f + 60f), 0f);

                    float swingProgress = (timer % whipAttackTime) / whipAttackTime;

                   
                    float accel = SwingAccelProfile(swingProgress);

                    float swingArc = MathHelper.ToRadians(100f);
                    float swingAngle = (float)(Math.Sin(accel * MathHelper.TwoPi) * swingArc);

                    if (whipOnRight)
                        swingAngle = -swingAngle;

                    whip.rotation = swingAngle + MathHelper.PiOver2;

                    float handleOffset = whip.width * 0.9f;
                    Vector2 rotatedOffset = new Vector2(-handleOffset, 0f).RotatedBy(whip.rotation);
                    whip.Center = anchorPoint + rotatedOffset;

                    whip.ai[0] = npc.whoAmI;               
                    whip.ai[1] = swingAngle;              
                    whip.ai[2] = whipOnRight ? 1f : -1f;  
                    whip.localAI[0] = swingRadius;

                    if (Math.Abs(swingProgress - 0.5f) < 0.02f) 
                    {
                        SoundEngine.PlaySound(SoundID.Item153, whip.Center);
                    }
                }

            }


            timer++;
            if (timer >= whipAttackTime)
                timer = 0f; 


            float totalPhaseTime = 420f; 
            npc.ai[2]++;
            if (npc.ai[2] > totalPhaseTime)
            {
                if (whipIdx >= 0 && whipIdx < Main.maxProjectiles)
                {
                    Projectile whip = Main.projectile[whipIdx];
                    if (whip != null && whip.active && whip.type == ModContent.ProjectileType<EidolonTale>())
                        whip.Kill();
                }


                npc.localAI[1] = 0;
                npc.localAI[2] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 0;

                SelectNextAttack(npc); 

            }
        }

        
        private float SwingAccelProfile(float t)
        {
            
            return 0.5f - 0.5f * MathF.Cos(t * MathHelper.Pi);
        }





        private void DoRegionAttack(NPC npc, Player player, ref float attackTimer)
        {
            int holeCount = 10;
            float spawnRadius = 1000f; 

            if (attackTimer == 1f) 
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) 
                {
                    for (int i = 0; i < holeCount; i++)
                    {

                        Vector2 randomOffset = Main.rand.NextVector2Circular(spawnRadius, spawnRadius);
                        Vector2 spawnPos = npc.Center + randomOffset;

                        Projectile.NewProjectile(
                            npc.GetSource_FromAI(),
                            spawnPos,
                            Vector2.Zero, 
                            ModContent.ProjectileType<VoidHole>(),
                            0, 
                            0f, 
                            Main.myPlayer
                        );
                    }
                }
            }

            attackTimer++;

            if (attackTimer > 60f) 
            {
                SelectNextAttack(npc);
            }
        }


        private void DoAnchorShipAttack(NPC npc, Player target, ref float timer)
        {
            if (timer == 1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int debrisCount = 6; 

                    for (int i = 0; i < debrisCount; i++)
                    {

                        Vector2 spawnPos = target.Center + new Vector2(Main.rand.Next(-1500, 1500), -600);

                        Vector2 velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(4f, 6f));

                        int projType = Main.rand.NextBool()
                            ? ModContent.ProjectileType<AnchorDebris>()
                            : ModContent.ProjectileType<ShipDebris>();

                        Projectile.NewProjectile(
                            npc.GetSource_FromAI(),
                            spawnPos,
                            velocity,
                            projType,
                            120, 
                            0f,
                            Main.myPlayer
                        );
                    }
                }
            }

            timer++;

            if (timer > 180f)
            {
                SelectNextAttack(npc);
            }
        }


        private void DoAbyssWyrmAttack(NPC npc, Player player, ref float timer)
        {

            if (timer == 1f)
            {
                abyssWyrmHeadId = -1;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {

                    Vector2 spawnPos = player.Center + new Vector2(0f, 800f);

                    int head = NPC.NewNPC(
                        npc.GetSource_FromAI(),
                        (int)spawnPos.X, (int)spawnPos.Y,
                        ModContent.NPCType<WyrmHead>());

                    if (head >= 0 && head < Main.maxNPCs)
                    {
                        abyssWyrmHeadId = head;
                        Main.npc[head].TargetClosest();
                        Main.npc[head].netUpdate = true;
                    }
                }

                npc.netUpdate = true;
            }


            bool wyrmGone = false;
            if (abyssWyrmHeadId != -1)
            {
                if (abyssWyrmHeadId < 0 || abyssWyrmHeadId >= Main.maxNPCs)
                    wyrmGone = true;
                else
                {
                    NPC head = Main.npc[abyssWyrmHeadId];
                    if (!head.active || head.type != ModContent.NPCType<WyrmHead>())
                        wyrmGone = true;
                }
            }
            else
            {
                wyrmGone = true;
            }

            if (timer >= AbyssWyrmDuration || wyrmGone)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && abyssWyrmHeadId != -1)
                    DespawnWyrmChain(abyssWyrmHeadId);

                abyssWyrmHeadId = -1;
                SelectNextAttack(npc);
                return;
            }


        }


        private void DespawnWyrmChain(int headIdx)
        {
            if (headIdx < 0 || headIdx >= Main.maxNPCs) return;

            int wyrmHeadType = ModContent.NPCType<WyrmHead>();
            int headTypeCheck = Main.npc[headIdx].type;

            if (headTypeCheck != wyrmHeadType) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (!n.active) continue;

                if (i == headIdx || n.realLife == headIdx)
                {
                    n.active = false;
                    n.netUpdate = true;
                }
            }
        }


        private void SelectNextAttack(NPC npc)
        {
            // Don't change state if in or after cutscene
            if (npc.ai[0] == (float)BossState.FinalPhaseCutscene ||
                npc.ai[0] == (float)BossState.FinalPhaseAttacks) 
                return;

            float healthRatio = npc.life / (float)npc.lifeMax;

            if (healthRatio > 0.7f)
            {
                int[] phase1Attacks = { 0, 1, 2, 6, 7 }; 
                npc.ai[0] = phase1Attacks[Main.rand.Next(phase1Attacks.Length)];
            }
            else if (healthRatio > 0.4f)
            {
                npc.ai[0] = 3; 
            }
            else
            {

                if (npc.localAI[3] == 0f)
                {
                    npc.ai[0] = (float)BossState.FinalPhaseCutscene;
                    npc.localAI[3] = 1f; 
                }
                else
                {
                    int[] phase3Attacks = { 5, 0, 1, 2, 6, (int)BossState.AbyssWyrmAttack }; 
                    npc.ai[0] = phase3Attacks[Main.rand.Next(phase3Attacks.Length)];
                }
            }

            npc.ai[1] = 0;
            npc.ai[2] = 0;
            npc.ai[3] = 0;
        }




        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - screenPos;
            float healthRatio = NPC.life / (float)NPC.lifeMax;
            Vector2 origin;
            Texture2D mainTexture;
            Texture2D glowTexture;

            int crackStage = 0;
            if (healthRatio <= 0.01f)
                crackStage = 5;
            else if (healthRatio <= 0.2f)
                crackStage = 4;
            else if (healthRatio <= 0.4f)
                crackStage = 3;
            else if (healthRatio <= 0.6f)
                crackStage = 2;
            else if (healthRatio <= 0.8f)
                crackStage = 1;

            if (crackStage == 0)
                mainTexture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/HeartOfTheOcean/HeartOfTheOcean").Value;
            else
                mainTexture = ModContent.Request<Texture2D>($"TalesoftheEntropicSea/Content/NPCs/HeartOfTheOcean/HeartCrack{crackStage}").Value;

            if (crackStage == 0)
                glowTexture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/HeartOfTheOcean/HeartOfTheOcean_Glow").Value;
            else
                glowTexture = ModContent.Request<Texture2D>($"TalesoftheEntropicSea/Content/NPCs/HeartOfTheOcean/HeartCrack{crackStage}_Glow").Value;

            origin = mainTexture.Size() / 2f;


            spriteBatch.Draw(mainTexture, position, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);


            spriteBatch.Draw(glowTexture, position, null, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            return false; 
        }



        public override void OnKill()
        {
 
            int slot = MusicLoader.GetMusicSlot(Mod, "Assets/Music/BeachEpilogue"); //doesnt work for some reason


            Main.newMusic = slot;
            Main.curMusic = slot;
            Main.musicFade[slot] = 1f;

            for (int i = 0; i < Main.musicFade.Length; i++)
            {
                if (i != slot)
                    Main.musicFade[i] = 0f;
            }

            
        }




        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            npcLoot.RemoveWhere(rule =>
            {
                if (rule is CommonDrop drop)
                    return drop.itemId == ItemID.LesserHealingPotion;
                return false;
            });


            npcLoot.Add(ItemDropRule.Common(
                ModContent.ItemType<OmegaHealingPotion>(),
                1, 
                10, 
                20  
            ));

            npcLoot.Add(ItemDropRule.Common(
                ModContent.ItemType<EpilogueItem>(),
                1,
                1,
                1
            ));
        }


    }
}
