using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.GABoss
{
    public class IndicatorLine : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 3600;
            Projectile.timeLeft = 60; 
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.hide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(tex.Width / 2f, tex.Height);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, 0f, origin, 1f, SpriteEffects.None, 0);
            return false;
        }

    }
}
