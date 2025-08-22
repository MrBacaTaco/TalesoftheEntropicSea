using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class StingrayMarker : ModProjectile
    {
        private const int lifeTime = 60; 

        public override string Texture => "Terraria/Images/Projectile_0"; 

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false; 
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = lifeTime;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs)
            {
                NPC target = Main.npc[(int)Projectile.ai[0]];
                if (target.active && !target.friendly)
                {
                    Projectile.Center = target.Center;
                }
                else
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Effect starShader = ModContent.Request<Effect>(
                "TalesoftheEntropicSea/Effects/StarCircle",
                ReLogic.Content.AssetRequestMode.ImmediateLoad
            ).Value;

            Texture2D tex = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Assets/Textures/StarShape",
                ReLogic.Content.AssetRequestMode.ImmediateLoad
            ).Value;

            float progress = 1f - (Projectile.timeLeft / (float)lifeTime);

            float fadeIn = Utils.GetLerpValue(0f, 0.2f, progress, true);
            float fadeOut = 1f - Utils.GetLerpValue(0.8f, 1f, progress, true);
            float appearProgress = fadeIn * fadeOut;

            float rotation = MathHelper.TwoPi * progress;

            float scalePulse = 0.8f + 0.2f * (float)Math.Sin(progress * MathHelper.TwoPi);
            float scaleOut = MathHelper.Lerp(1f, 0.5f, Utils.GetLerpValue(0.8f, 1f, progress, true));
            float scale = scalePulse * scaleOut;

            Color startColor = new Color(80, 200, 255);
            Color endColor = new Color(20, 60, 150);
            Color finalColor = Color.Lerp(startColor, endColor, progress);
            float brightness = 0.8f + 0.4f * (float)Math.Sin(progress * MathHelper.Pi);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive,
                                   SamplerState.LinearClamp, DepthStencilState.None,
                                   RasterizerState.CullNone);

            starShader.Parameters["starColor"].SetValue(finalColor.ToVector4() * brightness);
            starShader.Parameters["appearProgress"].SetValue(appearProgress);
            starShader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                rotation,
                tex.Size() / 2f,
                scale,
                SpriteEffects.None,
                0f
            );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                                   SamplerState.LinearClamp, DepthStencilState.Default,
                                   RasterizerState.CullCounterClockwise);

            return false;
        }
    }
}
