
using Luminance.Common.DataStructures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using TalesoftheEntropicSea.Common;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class NightFractalBeam : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public ref float BeamLength => ref Projectile.ai[1];

        private const float MaxBeamLength = 2200f;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 999999;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.hide = true;
        }

        public override bool ShouldUpdatePosition() => false;

        private SoundEffectInstance beamSoundInstance;

        private int soundLifetime;


        public override void AI()
        {
            if (!Owner.active || Owner.dead || !Owner.channel || Owner.noItems || Owner.CCed)
            {

                if (beamSoundInstance != null && !beamSoundInstance.IsDisposed)
                {
                    beamSoundInstance.Stop();
                    beamSoundInstance.Dispose();
                    beamSoundInstance = null;
                }

                Projectile.Kill();
                return;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                if (beamSoundInstance == null || beamSoundInstance.IsDisposed)
                {
                    var effect = ModContent.Request<SoundEffect>(
                        "TalesoftheEntropicSea/Assets/Sounds/Beam",
                        AssetRequestMode.ImmediateLoad
                    ).Value;

                    beamSoundInstance = effect.CreateInstance();
                    beamSoundInstance.IsLooped = true;
                    beamSoundInstance.Volume = 0.5f; 
                    beamSoundInstance.Play();

                    soundLifetime = (int)(effect.Duration.TotalSeconds * 60f);
                    Projectile.timeLeft = soundLifetime;
                }
            }

            Vector2 direction = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX);
            Projectile.velocity = direction;
            Projectile.Center = Owner.Center + direction * 20f;
            BeamLength = MathHelper.Lerp(BeamLength, MaxBeamLength, 0.15f);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.owner == Main.myPlayer)
            {
                var shakePlayer = Owner.GetModPlayer<ScreenShakePlayer>();
                shakePlayer.screenShakeIntensity = MathHelper.Clamp(
                    shakePlayer.screenShakeIntensity + 2f, 0f, 6f
                );
            }
        }



        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 24f, ref _);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            if (beamSoundInstance != null && !beamSoundInstance.IsDisposed)
            {
                beamSoundInstance.Stop();
                beamSoundInstance.Dispose();
                beamSoundInstance = null;
            }
        }



        public override bool PreDraw(ref Color lightColor)
        {
            var graphics = Main.graphics.GraphicsDevice;
            var shader = TalesoftheEntropicSea.NightFractalBeamShader;
            if (shader == null)
                return true; 

            shader.Parameters["globalTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
            shader.Parameters["coreColor"]?.SetValue(new Color(180, 140, 255).ToVector4());
            shader.Parameters["glowColor"]?.SetValue(new Color(50, 80, 200).ToVector4());
            shader.Parameters["flareColor"]?.SetValue(new Color(120, 60, 200).ToVector4());

            Matrix projection = Matrix.CreateOrthographicOffCenter(
                0, graphics.Viewport.Width,
                graphics.Viewport.Height, 0,
                0, 1
            );
            shader.Parameters["uWorldViewProjection"]?.SetValue(projection);

            graphics.Textures[1] =
                ModContent.Request<Texture2D>("TalesoftheEntropicSea/Assets/Textures/Noise", AssetRequestMode.ImmediateLoad).Value;

            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 end = start + Projectile.velocity.SafeNormalize(Vector2.UnitX) * BeamLength;

            float halfWidth = 300f;
            Vector2 dir = Vector2.Normalize(end - start);
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2) * halfWidth;

            VertexPositionColorTexture[] verts = new VertexPositionColorTexture[4];
            verts[0] = new VertexPositionColorTexture(new Vector3(start - normal, 0f), Color.White, new Vector2(0, 0));
            verts[1] = new VertexPositionColorTexture(new Vector3(start + normal, 0f), Color.White, new Vector2(0, 1));
            verts[2] = new VertexPositionColorTexture(new Vector3(end - normal, 0f), Color.White, new Vector2(1, 0));
            verts[3] = new VertexPositionColorTexture(new Vector3(end + normal, 0f), Color.White, new Vector2(1, 1));

            graphics.BlendState = BlendState.Additive;
            graphics.RasterizerState = RasterizerState.CullNone;
            graphics.DepthStencilState = DepthStencilState.None;

            foreach (var pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, verts, 0, 2);
            }

            graphics.BlendState = BlendState.AlphaBlend;
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.DepthStencilState = DepthStencilState.Default;

            Texture2D prismTex = Terraria.GameContent.TextureAssets.Projectile[ModContent.ProjectileType<NightFractalPrism>()].Value;

            int frameCount = Main.projFrames[ModContent.ProjectileType<NightFractalPrism>()];
            int frameHeight = prismTex.Height / frameCount;
            Rectangle frame = new Rectangle(0, (frameCount - 1) * frameHeight, prismTex.Width, frameHeight);

            Vector2 drawPos = Owner.Center - Main.screenPosition;
            Vector2 origin = new Vector2(prismTex.Width / 2f, frameHeight / 2f);

            Vector2 aimDir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX);
            float rotation = aimDir.ToRotation() + MathHelper.PiOver2;

            Main.spriteBatch.Draw(
                prismTex,
                drawPos,
                frame,
                lightColor,
                rotation,
                origin,
                1f,
                SpriteEffects.None,
                0f
            );

            return false; 
        }






        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
