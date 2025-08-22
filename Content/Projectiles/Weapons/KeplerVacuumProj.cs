using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using System;
using TalesoftheEntropicSea.Common;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;



namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class KeplerVacuumProj : BaseShortswordProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/Weapons/KeplerVacuumProj";

        private int swingCounter;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float customBladeLength = 16f * Projectile.scale * 3f; 
            Vector2 bladeStart = Owner.MountedCenter + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 12f;
            Vector2 bladeEnd = bladeStart + Projectile.velocity * customBladeLength;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                bladeStart,
                bladeEnd,
                Projectile.width,
                ref collisionPoint
            );
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(112, 116); 
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void ExtraBehavior()
        {
            Player player = Main.player[Projectile.owner];

            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard);
                Main.dust[dust].scale = 1.2f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
            }

            if (Projectile.localAI[1] == 0f && Main.myPlayer == Projectile.owner)
            {
                Projectile.localAI[1] = 1f;

                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 6f;

                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    shootVelocity,
                    ModContent.ProjectileType<KeplerVacuumMoon>(),
                    Projectile.damage / 2,
                    0f,
                    Projectile.owner
                );
            }

            if (player.whoAmI == Main.myPlayer && player.GetModPlayer<TalesPlayer>().keplerCooldown <= 0)
            {
                var source = player.GetSource_FromThis();
                int damage = (int)(Projectile.damage * 0.6f);

                Vector2 spawnOffset = new Vector2(Main.rand.Next(-200, 201), -600f);
                Vector2 spawnPos = Main.MouseWorld + spawnOffset;

                Vector2 velocity = Main.MouseWorld - spawnPos;
                velocity.Normalize();
                velocity *= 30f;

                int[] possibleProjectiles = new int[]
                {
            ModContent.ProjectileType<KeplerPlanet>(),
            ModContent.ProjectileType<KeplerPlanet2>(),
            ModContent.ProjectileType<KeplerStar>()
                };

                int projType = Main.rand.Next(possibleProjectiles);

                Projectile.NewProjectile(
                    source,
                    spawnPos,
                    velocity,
                    projType,
                    damage,
                    0f,
                    player.whoAmI
                );

                player.GetModPlayer<TalesPlayer>().keplerCooldown = 60;
            }
        }



        public override void SetVisualOffsets()
        {
            const int spriteWidth = 112;
            const int spriteHeight = 116;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            int handleOffset = 30;

            DrawOffsetX = -(spriteWidth / 2 - HalfProjWidth) + (Main.player[Projectile.owner].direction * handleOffset);
            DrawOriginOffsetY = -(spriteHeight / 2 - HalfProjHeight);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 240); 
        }

    }
}

//idk y so many errors,fix next time
