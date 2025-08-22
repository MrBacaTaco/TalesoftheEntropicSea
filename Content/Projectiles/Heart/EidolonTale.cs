using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class EidolonTale : ModProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/Heart/EidolonTale";

        public override void SetDefaults()
        {
            Projectile.width = 250;   
            Projectile.height = 122; 
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {

            Projectile.timeLeft = 2;
        }

        public override bool? CanDamage()
        {

            return true; 
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 origin = new Vector2(0f, texture.Height / 2f);

            SpriteEffects flip = Projectile.ai[2] == 1f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                flip,
                0
            );

            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Heart/EidolonTale"
            ).Value;

            Main.EntitySpriteDraw(
                glowmask,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                flip,
                0
            );

            return false; 
        }





    }
}
