using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using TalesoftheEntropicSea.Content.Buffs;
using TalesoftheEntropicSea.Content.World; 
using TalesoftheEntropicSea.World;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.NPCs
{
    public class Cephalopod : ModNPC
    {
        private enum AIState { Idle, Attacking }

        private AIState currentState;
        private int frameCounter;
        private int animationFrame;
        private int attackTimer;
        private bool hasFired;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12; 
        }

        public override void SetDefaults()
        {
            NPC.width = 176;
            NPC.height = 300;
            NPC.damage = 250;
            NPC.defense = 125;
            NPC.lifeMax = 300000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;

            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ThalassophobiaDebuff>()] = true;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
            }

            float distance = Vector2.Distance(player.Center, NPC.Center);
            NPC.direction = (player.Center.X > NPC.Center.X) ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            switch (currentState)
            {
                case AIState.Idle:
                    IdleMovement(player);
                    attackTimer++;

                    if (attackTimer > 180 && distance < 600f)
                    {
                        currentState = AIState.Attacking;
                        frameCounter = 0;
                        animationFrame = 0;
                        attackTimer = 0;
                        hasFired = false;
                        NPC.frame.Y = 0;
                    }
                    break;

                case AIState.Attacking:
                    NPC.velocity *= 0.9f;

                    frameCounter++;
                    if (frameCounter >= 5) 
                    {
                        frameCounter = 0;
                        animationFrame++;
                        if (animationFrame >= 12)
                        {
                            currentState = AIState.Idle;
                            animationFrame = 0;
                            frameCounter = 0;
                            attackTimer = 0;
                            NPC.frame.Y = 0;
                            return;
                        }
                        NPC.frame.Y = animationFrame * 282;
                    }

                    if (!hasFired && animationFrame == 6)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                            Projectile.NewProjectileDirect(
                                NPC.GetSource_FromAI(),
                                NPC.Center,
                                direction * 8f,
                                ModContent.ProjectileType<Projectiles.CephalopodShot>(),
                                100,
                                1f,
                                Main.myPlayer
                            );
                        }
                        hasFired = true;
                    }
                    break;
            }

            NPC.rotation = (float)System.Math.Sin(Main.GameUpdateCount * 0.05f) * 0.1f;
        }

        private void IdleMovement(Player player)
        {
            Vector2 toPlayer = player.Center - NPC.Center;
            float speed = 2.5f;
            Vector2 moveTo = toPlayer.SafeNormalize(Vector2.Zero) * speed;
            float turnResistance = 40f;
            NPC.velocity = (NPC.velocity * (turnResistance - 1) + moveTo) / turnResistance;

            frameCounter++;
            if (frameCounter >= 15)
            {
                frameCounter = 0;
                animationFrame++;
                if (animationFrame >= 5)
                {
                    animationFrame = 0;
                }
                NPC.frame.Y = animationFrame * 300;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            
            if (SubworldLibrary.SubworldSystem.Current is SkyAbyssSubworld)
            {
                return 0.6f; // 50% chance
            }

            return 0f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture;
            Texture2D glowmask;
            int frameHeight;

            if (currentState == AIState.Attacking)
            {
                texture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/Cephalopod_Attack").Value;
                glowmask = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/Cephalopod_Attack_glow").Value;
                frameHeight = 282;
            }
            else
            {
                texture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/Cephalopod").Value;
                glowmask = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/Cephalopod_glow").Value;
                frameHeight = 300;
            }

            Rectangle frame = new Rectangle(0, NPC.frame.Y, texture.Width, frameHeight);
            Vector2 origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // --- Draw Base Sprite ---
            spriteBatch.Draw(texture, NPC.Center - screenPos, frame, drawColor, NPC.rotation, origin, 1f, effects, 0f);

            // --- Draw Glowmask on top ---
            spriteBatch.Draw(glowmask, NPC.Center - screenPos, frame, Color.White, NPC.rotation, origin, 1f, effects, 0f);

            return false;
        }

        public override void OnKill()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbyssDrown"), NPC.Center);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 70 gold = 70 * 10000 copper
            npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ItemID.GoldCoin, 1, 65, 75));
        }
    }
}
