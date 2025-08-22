using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using TalesoftheEntropicSea.World;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles
{
    public class SkyAbyssPortal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 117;
            Projectile.timeLeft = 999999;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {

            Projectile.velocity = Vector2.Zero;
            Projectile.position.Y += (float)(System.Math.Sin(Main.GameUpdateCount / 20f) * 0.5f);

            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 0.75f);

            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (Projectile.Hitbox.Contains(Main.MouseWorld.ToPoint()))
                {
                    if (!SubworldSystem.IsActive<SkyAbyssSubworld>())
                    {
                        SubworldSystem.Enter<SkyAbyssSubworld>();
                    }
                }
            }
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = tex.Size() / 2f;

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                0f, 
                origin,
                1f,
                SpriteEffects.None
            );

            return false;
        }
    }
}
