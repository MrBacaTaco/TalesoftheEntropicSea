using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.NPCs.GABoss
{
    public class BottomBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 158;
            NPC.damage = 0;
            NPC.defense = 150;
            NPC.lifeMax = 50000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
        }

        public override void AI() => FollowSegment();

        private void FollowSegment()
        {
            
            NPC.timeLeft = 600;
            

            int followID = (int)NPC.ai[1];
            if (!Main.npc.IndexInRange(followID) || !Main.npc[followID].active)
            {
                NPC.active = false;
                return;
            }

            NPC realParent = Main.npc[followID];
            Vector2 toParent = realParent.Center - NPC.Center;
            float spacing = NPC.width * 0.8f;

            if (toParent != Vector2.Zero)
            {
                NPC.Center = realParent.Center - Vector2.Normalize(toParent) * spacing;
            }

            float targetRotation = toParent.ToRotation() + MathHelper.PiOver2;
            float maxRotationChange = MathHelper.ToRadians(20f);
            float angleDifference = MathHelper.WrapAngle(targetRotation - NPC.rotation);
            angleDifference = MathHelper.Clamp(angleDifference, -maxRotationChange, maxRotationChange);
            NPC.rotation += angleDifference;
        }

        /*
        Glowmask - may be will readd this later
        
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/NPCs/GABoss/BottomBody"
            ).Value;

            Rectangle frame = NPC.frame;
            Vector2 origin = frame.Size() / 2f;

            
            SpriteEffects effects = NPC.spriteDirection == 1
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;

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
        }
        */

    }
}
