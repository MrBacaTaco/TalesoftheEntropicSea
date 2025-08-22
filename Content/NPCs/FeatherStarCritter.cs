using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using TalesoftheEntropicSea.Content.Buffs;
using TalesoftheEntropicSea.Content.Projectiles;
using TalesoftheEntropicSea.Content.World; 
using TalesoftheEntropicSea.World;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.NPCs
{
    public class FeatherStarCritter : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void FindFrame(int frameHeight)
        {
            
            NPC.frameCounter++;
            if (NPC.frameCounter >= 45) 
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                {
                    NPC.frame.Y = 0; 
                }
            }
        }

        public override void SetDefaults()
        {
            NPC.width = 140;
            NPC.height = 134;
            NPC.damage = 120;
            NPC.defense = 100;
            NPC.lifeMax = 150000;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60f;
            NPC.friendly = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.aiStyle = -1; 

            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ThalassophobiaDebuff>()] = true;
        }

        public override void AI()
        {
            NPC.TargetClosest(false);
            Player target = Main.player[NPC.target];
            if (target.dead || !target.active)
                return;

            
            NPC.ai[0]++;

            if (NPC.ai[0] > 120) 
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 shootDirection = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        NPC.Center,
                        shootDirection * 8f,
                        ModContent.ProjectileType<FeatherStarSpell>(),
                        100, 
                        2f, 
                        Main.myPlayer
                    
                    );
                }
                NPC.ai[0] = 0;
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/NPCs/FeatherStarCritter_glow"
            ).Value;

            
            int frameHeight = glowmask.Height / Main.npcFrameCount[NPC.type];
            Rectangle frame = new Rectangle(0, NPC.frame.Y, glowmask.Width, frameHeight);

            
            Vector2 origin = frame.Size() / 2f;

            
            Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, 0); 

            SpriteEffects effects = NPC.spriteDirection == 1
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(
                glowmask,
                drawPos,
                frame,
                Color.White,
                NPC.rotation,
                origin,
                NPC.scale,
                effects,
                0f
            );
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            
            if (SubworldLibrary.SubworldSystem.Current is SkyAbyssSubworld)
            {
                return 1f; 
            }

            return 0f; 
        }
        public override void OnKill()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbyssDrown"), NPC.Center);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ItemID.GoldCoin, 1, 45, 55));
        }


    }
}
