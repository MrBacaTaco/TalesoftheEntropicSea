using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TalesoftheEntropicSea.Content.NPCs.GABoss
{
    public class TopWingLeft : ModNPC
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            NPC.width = 356;
            NPC.height = 296;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.lifeMax = 10000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;

        }

        public override void AI()
        {
            
            NPC.timeLeft = 600;
            

            int parentID = (int)NPC.ai[1];
            int side = -1; 

            if (!Main.npc.IndexInRange(parentID) || !Main.npc[parentID].active)
            {
                NPC.active = false;
                return;
            }

            NPC parent = Main.npc[parentID];

            Vector2 offset = new Vector2(190f, 90f) * side;
            Vector2 rotatedOffset = offset.RotatedBy(parent.rotation);

            NPC.Center = parent.Center + rotatedOffset;
            NPC.rotation = parent.rotation;
            NPC.spriteDirection = side; 
        }

        /*
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            Texture2D glowTexture = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/NPCs/GABoss/TopWingLeft"
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
