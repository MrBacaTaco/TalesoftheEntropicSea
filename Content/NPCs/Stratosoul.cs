using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using TalesoftheEntropicSea.Content.Projectiles;
using TalesoftheEntropicSea.Content.World; 
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TalesoftheEntropicSea.World;

namespace TalesoftheEntropicSea.Content.NPCs
{
    public class Stratosoul : ModNPC
    {
        public enum SoulState { Calm, Angered }
        private SoulState state = SoulState.Calm;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 120;
            NPC.defense = 100;
            NPC.lifeMax = 12000;
            NPC.knockBackResist = 0.2f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.friendly = false;
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                NPC.frame.Y = 0;
        }

        public override void AI()
        {
            NPC.TargetClosest(false);
            Player target = Main.player[NPC.target];


            
            NPC.velocity.Y = (float)(System.Math.Sin(NPC.ai[0] / 60f) * 0.6f);
            NPC.velocity.X = 0;
            NPC.ai[0]++;

            
            float dist = Vector2.Distance(target.Center, NPC.Center);
            if (dist < 400f && target.active && !target.dead)
            {
                state = SoulState.Angered;

                
                NPC.ai[1]++;
                if (NPC.ai[1] > 90)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/PhantomSpirit"), NPC.Center);

                        Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            NPC.Center,
                            dir,
                            ModContent.ProjectileType<StratosoulShot>(),
                            100, 1f,
                            Main.myPlayer
                        );
                    }
                    NPC.ai[1] = 0;
                }
            }
            else
            {
                state = SoulState.Calm;
                NPC.ai[1] = 0;
            }
        }

        
        public override string Texture
        {
            get
            {
                
                Player player = Main.player[Player.FindClosest(NPC.Center, NPC.width, NPC.height)];
                float dist = Vector2.Distance(player.Center, NPC.Center);
                
                if (dist < 1600f && player.active && !player.dead)
                    return "TalesoftheEntropicSea/Content/NPCs/Stratosoul_Angered";
                return "TalesoftheEntropicSea/Content/NPCs/Stratosoul_Calm";
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            
            if (SubworldLibrary.SubworldSystem.Current is SkyAbyssSubworld)
            {
                return 2f; 
            }

            return 0f; 
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            
            npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ItemID.GoldCoin, 1, 20, 30));
        }

        public override void OnKill()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbyssDrown"), NPC.Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            Texture2D baseTexture;
            Texture2D glowTexture;

            if (state == SoulState.Angered)
            {
                baseTexture = ModContent.Request<Texture2D>(
                    "TalesoftheEntropicSea/Content/NPCs/Stratosoul_Angered"
                ).Value;

                glowTexture = ModContent.Request<Texture2D>(
                    "TalesoftheEntropicSea/Content/NPCs/Stratosoul_Angered"
                ).Value;
            }
            else
            {
                baseTexture = ModContent.Request<Texture2D>(
                    "TalesoftheEntropicSea/Content/NPCs/Stratosoul_Calm"
                ).Value;

                glowTexture = ModContent.Request<Texture2D>(
                    "TalesoftheEntropicSea/Content/NPCs/Stratosoul_Calm"
                ).Value;
            }

            
            int frameHeight = baseTexture.Height / Main.npcFrameCount[NPC.type];
            Rectangle frame = new Rectangle(0, NPC.frame.Y, baseTexture.Width, frameHeight);

            Vector2 origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            
            spriteBatch.Draw(
                baseTexture,
                NPC.Center - screenPos,
                frame,
                drawColor,
                NPC.rotation,
                origin,
                NPC.scale,
                effects,
                0f
            );

            
            spriteBatch.Draw(
                glowTexture,
                NPC.Center - screenPos,
                frame,
                Color.White,
                NPC.rotation,
                origin,
                NPC.scale,
                effects,
                0f
            );

            return false; 
        }

    }
}
