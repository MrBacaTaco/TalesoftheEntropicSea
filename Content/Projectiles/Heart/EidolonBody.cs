using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class EidolonBody : ModProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/Heart/EidolonBody";

        public override void SetDefaults()
        {
            Projectile.width = 86;
            Projectile.height = 288;
            Projectile.timeLeft = 1200; 
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
        }

        public override void AI()
        {

            if (Projectile.ai[0] == 1f)
                return; 

            if (Projectile.ai[0] == 2f) 
            {

                Projectile.velocity.Y += 0.05f; 
                Projectile.velocity.X *= 0.98f; 
                Projectile.rotation += 0.005f; 
                return;
            }

            Projectile.velocity *= 0.95f;
            if (Projectile.velocity.Length() > 0.05f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2(texture.Width / 2f, 0f); 
                                                                  
            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
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
