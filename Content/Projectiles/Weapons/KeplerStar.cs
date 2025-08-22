using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class KeplerStar : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 104;
            Projectile.height = 126;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.4f, 0.6f, 1f);
        }

        public override void OnKill(int timeLeft)
        {
            
            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit,
                                            Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f),
                                            0, new Color(150, 200, 255), 2f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Weapons/KeplerStarGlowmask"
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
