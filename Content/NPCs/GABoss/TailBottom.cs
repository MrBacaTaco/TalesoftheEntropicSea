using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.NPCs.GABoss
{
    public class TailBottom : ModNPC
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            NPC.width = 28; 
            NPC.height = 50;
            NPC.damage = 0;
            NPC.defense = 150;
            NPC.lifeMax = 50000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;

            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ThalassophobiaDebuff>()] = true;
        }

        public override void AI()
        {
            
            NPC.timeLeft = 600;
            

            if (!Main.npc.IndexInRange((int)NPC.ai[1]) || !Main.npc[(int)NPC.ai[1]].active)
            {
                NPC.active = false;
                return;
            }

            NPC realParent = Main.npc[(int)NPC.ai[1]];
            Vector2 toParent = realParent.Center - NPC.Center;

            float spacing = GetSpacing();

            if (toParent != Vector2.Zero)
            {
                NPC.Center = realParent.Center - Vector2.Normalize(toParent) * spacing;
            }

            NPC.rotation = toParent.ToRotation() + MathHelper.PiOver2;
        }

        private float GetSpacing()
        {
            return 45f; 
        }

        /*
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            Texture2D glowTexture = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/NPCs/GABoss/TailBottom"
            ).Value;

            Rectangle frame = NPC.frame; // Uses same frame as main texture
            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(
                glowTexture,
                NPC.Center - screenPos,
                frame,
                Color.White, // No lighting applied
                NPC.rotation,
                origin,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }
        */
    }
}

