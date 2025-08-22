using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class SwordHorizontal : ModProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/Heart/SwordHorizontal";

        public override void SetDefaults()
        {
            Projectile.width = 338;
            Projectile.height = 124;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
     
        }

        public override void AI()
        {
            if (Projectile.velocity == Vector2.Zero)
                Projectile.velocity = new Vector2(-12f, 0f);

            Projectile.rotation = 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 180);
        }


        public override bool CanHitPlayer(Player target)
        {
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return projHitbox.Intersects(targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(texture, drawPos, null, lightColor, 0f, origin, Projectile.scale, SpriteEffects.FlipHorizontally, 0f);

            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Heart/SwordHorizontal"
            ).Value;

            Main.spriteBatch.Draw(glowmask, drawPos, null, Color.White, 0f, origin, Projectile.scale, SpriteEffects.FlipHorizontally, 0f);

            return false; 
        }

    }
}
