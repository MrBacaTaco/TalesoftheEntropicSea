

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using TalesoftheEntropicSea.Content.Buffs;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class KeplerPlanet : ModProjectile
    {
        private Vector2 orbitCenter;
        private float orbitRadius = 300f;
        private float orbitSpeed = 0.05f;
        private float angle;
        private bool initialized = false;
        private bool orbitFromAbove = true;

        public override void SetDefaults()
        {
            Projectile.width = 132;
            Projectile.height = 142;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            

        }

        public override void AI()
        {
            // Only run on first tick
            if (!initialized)
            {
                initialized = true;

                // Do NOT override velocity or position here!
                // This keeps the spawn direction controlled from the weapon
            }

            // Rotation and lighting
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.7f);

            // Trail dust
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit, -Projectile.velocity * 0.5f, 0, default, 1.2f);
                d.noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            // Shrink hitbox for visual centering
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 32;
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;

            // Optional: Reduce damage and re-apply to nearby NPCs
            Projectile.damage /= 2;
            Projectile.Damage();

            // 💥 Blue shockwave dust
            for (int i = 0; i < 60; i++)
            {
                Vector2 pos = Projectile.Center - new Vector2(30, 30);
                Dust d = Dust.NewDustDirect(pos, 60, 60, DustID.Flare_Blue, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 0, new Color(0, 167, 255), 2.5f);
                d.noGravity = true;
            }

            // 🌫 Dungeon spirit ring
            for (int i = 0; i < 36; i++)
            {
                float angle = MathHelper.TwoPi * i / 36f;
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 48f;
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.DungeonSpirit, offset * 0.2f, 0, Color.White, 1.5f);
                d.noGravity = true;
            }

            // 💨 Gore explosion
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 3; i++)
                {
                    int goreIndex = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[goreIndex].velocity *= 0.4f;
                    Main.gore[goreIndex].velocity += new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                }
            }

            // 🔊 Sound
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);

            // 💡 Light burst
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1f);
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Apply your debuff
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 240);

            // Spawn star marker
            Projectile.NewProjectile(
                Projectile.GetSource_OnHit(target),
                target.Center,
                Vector2.Zero,
                ModContent.ProjectileType<KeplerStarMarker>(),
                0,
                0,
                Projectile.owner,
                target.whoAmI
            );

            // Spawn comet ABOVE target with random horizontal offset
            float horizontalOffset = Main.rand.NextFloat(-200f, 200f);
            Vector2 spawnPos = target.Center + new Vector2(horizontalOffset, -800f);

            // Velocity → aim at target
            Vector2 velocity = target.Center - spawnPos;
            velocity.Normalize();
            velocity *= 30f;

            // Spawn the comet but set it to be inactive for 2 seconds
            int comet = Projectile.NewProjectile(
                Projectile.GetSource_OnHit(target),
                spawnPos,
                Vector2.Zero, // Start with zero velocity
                ModContent.ProjectileType<KeplerComet>(),
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner,
                target.whoAmI
            );

            // Store the intended velocity in ai[1] for later use
            Main.projectile[comet].ai[1] = velocity.X;
            Main.projectile[comet].localAI[0] = velocity.Y;
            Main.projectile[comet].timeLeft = 180; // Extra time for delay
        }






        public override void PostDraw(Color lightColor)
        {
            // Load the glowmask texture
            Texture2D glowmask = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/Projectiles/Weapons/KeplerPlanet").Value;

            // Calculate the draw position
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(glowmask.Width / 2f, glowmask.Height / 2f);

            // Draw the glowmask with white color (keeps original texture colors)
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


