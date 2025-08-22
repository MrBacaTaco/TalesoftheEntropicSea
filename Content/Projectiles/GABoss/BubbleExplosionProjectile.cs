using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace TalesoftheEntropicSea.Content.Projectiles.GABoss
{
    public class BubbleExplosionProjectile : ModProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/GABoss/Explotion"; 

        public override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 140;
            Projectile.hostile = true;
            Projectile.timeLeft = 14; 
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 2 == 0)
            {
                Projectile.frame++;
                if (Projectile.frame >= 7)
                    Projectile.Kill();
            }
        }

        

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / 7;
            Rectangle frame = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
            Vector2 origin = frame.Size() / 2f;

            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                frame,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }
    }
}
