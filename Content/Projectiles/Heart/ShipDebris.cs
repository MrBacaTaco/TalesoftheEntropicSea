using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class ShipDebris : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 266; 
            Projectile.height = 142; 
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = 0;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {

            Projectile.velocity.Y += 0.25f;

            Projectile.rotation += Projectile.velocity.X * 0.01f;

            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            }
        }




        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Heart/ShipDebris"
            ).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(glowmask.Width / 2f, glowmask.Height / 2f);

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
