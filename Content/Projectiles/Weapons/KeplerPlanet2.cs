using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class KeplerPlanet2 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 114;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            
            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 0.8f);
        }

        public override void OnKill(int timeLeft)
        {
            
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.DungeonSpirit,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    0,
                    new Color(0, 167, 255),
                    1.8f
                );
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Weapons/KeplerPlanet2Glowmask"
            ).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = glowmask.Size() / 2f;

            Main.EntitySpriteDraw(
                glowmask,
                drawPosition,
                null,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }
    }
}
