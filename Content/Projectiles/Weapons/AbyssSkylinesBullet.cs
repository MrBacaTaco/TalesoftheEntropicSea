using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TalesoftheEntropicSea.Content.Projectiles.Weapons;
using CalamityMod.Buffs.StatDebuffs;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class AbyssSkylinesBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 10;
            Projectile.scale = 1.18f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 100; i++) 
            {
                Vector2 velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(0.6f, 1.7f);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center + Projectile.velocity * -3,
                    velocity,
                    ModContent.ProjectileType<AbyssSkylinesExplosion>(),
                    (int)(Projectile.damage * (hit.Crit ? 0.35f : 0.2f)),
                    0,
                    Projectile.owner
                );
            }
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);

        }

       
    }
}
