using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.GABoss
{
    public class FallingDebrisMedium : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 144; 
            Projectile.height = 144;
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
            Projectile.rotation += Projectile.velocity.X * 0.05f;

            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
            }
        }

        public override void OnKill(int timeLeft)
        {

            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            }
        }
    }
}
