

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class SwordWeapon : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 2; 
            Projectile.hide = true;
        }

        public override void AI()
        {
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<SwordSlashHitbox>(),
                    Projectile.damage,
                    0f,
                    Main.myPlayer
                );
            }

            Projectile.Kill();
        }
    }
}
