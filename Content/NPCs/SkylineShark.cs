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
    public class SkylineShark : ModNPC
    {
        private enum AIState
        {
            Idle,
            Charging
        }

        private AIState currentState;
        private int attackTimer;
        private int frameCounter;

        public override void SetStaticDefaults()
        {
           
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 324;
            NPC.height = 224;
            NPC.damage = 240;
            NPC.defense = 150;
            NPC.lifeMax = 560000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 1000f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

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


            NPC.direction = NPC.spriteDirection = (player.Center.X > NPC.Center.X) ? 1 : -1;


            Vector2 toPlayer = player.Center - NPC.Center;
            float distance = toPlayer.Length();

            switch (currentState)
            {
                case AIState.Idle:
                    IdleMovement(toPlayer);
                    attackTimer++;

                    if (attackTimer > 120 && distance < 600f)
                    {
                        currentState = AIState.Charging;
                        attackTimer = 0;
                        frameCounter = 0;

                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ReaperEnragedRoar"), NPC.Center);
                    }
                    break;

                case AIState.Charging:
                    ChargeMovement(toPlayer);

                    if (attackTimer > 90)
                    {
                        currentState = AIState.Idle;
                        attackTimer = 0;
                        frameCounter = 0;
                    }

                    attackTimer++;
                    break;
            }

            NPC.rotation = NPC.velocity.X * 0.05f;
        }

        private void IdleMovement(Vector2 toPlayer)
        {
            float speed = 2f;
            float turnResistance = 20f;

            Vector2 moveTo = toPlayer.SafeNormalize(Vector2.Zero) * speed;
            NPC.velocity = (NPC.velocity * (turnResistance - 1) + moveTo) / turnResistance;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            
            if (SubworldLibrary.SubworldSystem.Current is SkyAbyssSubworld)
            {
                return 0.6f; 
            }

            return 0f; 
        }

        private void ChargeMovement(Vector2 toPlayer)
        {
            float speed = 8f;
            NPC.velocity = toPlayer.SafeNormalize(Vector2.Zero) * speed;
        }

        public override void FindFrame(int _)
        {
            Texture2D currentTexture = currentState == AIState.Charging
                ? ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/SkylineShark_Attack").Value
                : ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/SkylineShark").Value;

            int frameHeight = currentTexture.Height / Main.npcFrameCount[NPC.type];

            frameCounter++;

            if (currentState == AIState.Charging)
            {
                if (frameCounter < 48)
                {
                    NPC.frame.Y = (frameCounter / 6) * frameHeight;
                }
                else
                {
                    NPC.frame.Y = frameHeight * (Main.npcFrameCount[NPC.type] - 1);
                }
            }
            else
            {
                if (frameCounter >= 6)
                {
                    frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }

            NPC.frame.Height = frameHeight; 
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture;
            Vector2 origin;

            if (currentState == AIState.Charging)
            {
                texture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/SkylineShark_Attack").Value;
            }
            else
            {
                texture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/SkylineShark").Value;
            }

            
            Rectangle sourceRectangle = new Rectangle(0, NPC.frame.Y, texture.Width, NPC.frame.Height);
            origin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height / 2f);

            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(
                texture,
                NPC.Center - screenPos,
                sourceRectangle,
                drawColor,
                NPC.rotation,
                origin,
                1f,
                effects,
                0f
            );

            return false; 
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 240);
        }


        public override void OnKill()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbyssDrown"), NPC.Center);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            
            npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ItemID.GoldCoin, 1, 70, 90));
        }

    }
}
