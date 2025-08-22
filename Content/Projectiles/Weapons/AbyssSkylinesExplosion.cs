
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class AbyssSkylinesExplosion : ModProjectile
    {
        private struct ShaderParticle
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Scale;
            public Color Color;
            public int TimeLeft;
        }

        private static List<ShaderParticle> particles = new();
        private bool shockwaveSpawned = false;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) *
                              Main.rand.NextFloat(0.5f, 1.2f);

                ShaderParticle p = new ShaderParticle
                {
                    Position = Projectile.Center,
                    Velocity = vel,
                    Scale = Main.rand.NextFloat(0.01f, 0.1f),
                    Color = Main.rand.NextBool()
                        ? new Color(30, 30, 160)
                        : (Main.rand.NextBool()
                            ? new Color(100, 180, 255)
                            : new Color(160, 80, 200)),
                    TimeLeft = 30
                };

                particles.Add(p);
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                ShaderParticle p = particles[i];
                p.Position += p.Velocity;
                p.Velocity *= 0.95f;
                p.TimeLeft--;

                if (p.TimeLeft <= 0)
                    particles.RemoveAt(i);
                else
                    particles[i] = p;
            }

            
            if (!shockwaveSpawned)
            {
                shockwaveSpawned = true;

                Particle pulse = new DirectionalPulseRing(
                    Projectile.Center,
                    Vector2.Zero,
                    Color.Aqua,
                    new Vector2(2f, 2f),
                    0f, 0.1f, 0.85f, 36
                );
                GeneralParticleHandler.SpawnParticle(pulse);

                Particle explosion = new DetailedExplosion(
                    Projectile.Center,
                    Vector2.Zero,
                    Color.Magenta,
                    Vector2.One,
                    Main.rand.NextFloat(-5, 5),
                    0f, 0.65f, 26
                );
                GeneralParticleHandler.SpawnParticle(explosion);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (particles.Count == 0)
                return false;

            Effect particleShader = ModContent.Request<Effect>(
                "TalesoftheEntropicSea/Effects/AbyssSkylineParticle",
                AssetRequestMode.ImmediateLoad
            ).Value;

            Texture2D tex = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Assets/Textures/SoftCircle",
                AssetRequestMode.ImmediateLoad
            ).Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive,
                                   SamplerState.LinearClamp, DepthStencilState.None,
                                   RasterizerState.CullNone);

            particleShader.Parameters["globalTime"].SetValue(Main.GlobalTimeWrappedHourly);

            foreach (var p in particles)
            {
                float alpha = p.TimeLeft / 30f;
                Vector2 drawPos = p.Position - Main.screenPosition;

                particleShader.Parameters["baseColor"].SetValue(p.Color.ToVector4());
                particleShader.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(
                    tex,
                    drawPos,
                    null,
                    Color.White * alpha,
                    0f,
                    tex.Size() * 0.5f,
                    p.Scale,
                    SpriteEffects.None,
                    0f
                );
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                                   SamplerState.LinearClamp, DepthStencilState.Default,
                                   RasterizerState.CullCounterClockwise);

            return false;
        }
    }
}


