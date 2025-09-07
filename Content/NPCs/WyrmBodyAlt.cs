using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.NPCs
{
    public class WyrmBodyAlt : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 140;
            NPC.height = 144;
            NPC.damage = 100;
            NPC.defense = 250;
            NPC.lifeMax = 999999999;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            //NPC.dontTakeDamage = true;
            NPC.netAlways = true;
            NPC.alpha = 255;
            NPC.chaseable = false;
        }

        public override void AI()
        {

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            bool shouldDespawn = true;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == ModContent.NPCType<WyrmHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0) NPC.alpha = 0;
            }

            Vector2 segmentPosition = NPC.Center;
            NPC target = Main.npc[(int)NPC.ai[1]];
            Vector2 targetCenter = target.Center;
            float x = targetCenter.X - segmentPosition.X;
            float y = targetCenter.Y - segmentPosition.Y;
            float dist = (float)System.Math.Sqrt(x * x + y * y);
            int segmentSpacing = (int)(NPC.width * 0.8f); 
            dist = (dist - segmentSpacing) / dist;

            x *= dist;
            y *= dist;

            NPC.velocity = Vector2.Zero;
            NPC.position.X += x;
            NPC.position.Y += y;

            NPC.rotation = (float)System.Math.Atan2(y, x) + MathHelper.PiOver2;
            NPC.spriteDirection = (x < 0f) ? -1 : 1;



            if (NPC.realLife >= 0 && Main.npc[NPC.realLife].active)
            {
                NPC.alpha = Main.npc[NPC.realLife].alpha;
            }

        }
        
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {

            return false;
        }
        

        public override bool CheckActive()
        {
            return false; 
        }


        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/WyrmBodyAlt_glow").Value;

            Rectangle frame = NPC.frame;
            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(
                glowTexture,
                NPC.Center - screenPos,
                frame,
                Color.White,
                NPC.rotation,
                origin,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }

    }
}
