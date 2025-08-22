using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using TalesoftheEntropicSea.Common.Utils;

namespace TalesoftheEntropicSea.Content.Projectiles.GABoss
{
    public class UrchinTelegraphLine : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public ref float Lifetime => ref Projectile.ai[1];

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; 

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
        }

        public override void AI()
        {
            float progress = Time / Lifetime;
            Projectile.Opacity = Utils.GetLerpValue(0f, 0.2f, progress) * (1f - Utils.GetLerpValue(0.8f, 1f, progress));
            Projectile.Opacity *= 3f;
            if (Projectile.Opacity > 1f)
                Projectile.Opacity = 1f;

            if (++Time >= Lifetime)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitY);
            float length = Projectile.localAI[0] > 0 ? Projectile.localAI[0] : 300f;

            Vector2 start = Projectile.Center - direction * length / 2f;
            Vector2 end = Projectile.Center + direction * length / 2f;

            float width = MathHelper.Lerp(0.3f, 3f, Projectile.Opacity);

            Main.spriteBatch.DrawLineBetter(start, end, Color.DeepSkyBlue * Projectile.Opacity, width); // ⬅ color adjusted
            return false;
        }
    }
}
