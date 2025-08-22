using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class KeplerComet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0"; 



        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false; 
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Magic; 
            Projectile.ignoreWater = true;

            Projectile.alpha = 255; 

        }

        public override void AI()
        {

            if (Projectile.timeLeft > 150)
            {
                Projectile.velocity = Vector2.Zero; 
                Projectile.alpha = 255; 
                return;
            }

            
            if (Projectile.timeLeft == 150)
            {
                Projectile.velocity = new Vector2(Projectile.ai[1], Projectile.localAI[0]);
                Projectile.alpha = 0;
            }

            
            if (Projectile.timeLeft < 150 && Projectile.alpha > 0)
            {
                Projectile.alpha = Math.Max(0, Projectile.alpha - 25); 
            }

            
            int targetIndex = (int)Projectile.ai[0];
            if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
            {
                Projectile.Kill();
                return;
            }

            NPC target = Main.npc[targetIndex];
            if (!target.active || target.friendly)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0.1f, 0.5f, 1f);

            if (Projectile.Hitbox.Intersects(target.Hitbox))
            {
                Projectile.Kill();
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            
            if (Projectile.alpha >= 255)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Assets/Textures/SoftCircle",
                ReLogic.Content.AssetRequestMode.ImmediateLoad
            ).Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive,
                                   SamplerState.LinearClamp, DepthStencilState.None,
                                   RasterizerState.CullNone);

            
            float alphaMultiplier = 1f - (Projectile.alpha / 255f);

            
            Vector2 headPos = Projectile.Center - Main.screenPosition + new Vector2(0f, 120f);

            
            Main.spriteBatch.Draw(
                tex, headPos, null,
                Color.White * 2f * alphaMultiplier, 
                0f, tex.Size() / 2f,
                0.25f, SpriteEffects.None, 0f
            );

            
            Main.spriteBatch.Draw(
                tex, headPos, null,
                new Color(0, 200, 255) * 1.3f * alphaMultiplier,
                0f, tex.Size() / 2f,
                0.5f, SpriteEffects.None, 0f
            );

            
            Vector2 direction = -Projectile.velocity.SafeNormalize(Vector2.UnitY);
            int segments = 50;
            float tailLength = 1000f;

            for (int i = 0; i < segments; i++)
            {
                float progress = i / (float)segments;
                Vector2 offset = direction * progress * tailLength;

                float scaleX = MathHelper.Lerp(0.12f, 0.005f, progress);
                float scaleY = MathHelper.Lerp(1.0f, 0.2f, progress);
                float alpha = (1f - progress) * alphaMultiplier; 

                Color color;
                if (progress < 0.25f)
                    color = Color.Lerp(Color.White, Color.Cyan, progress / 0.25f);
                else
                    color = Color.Lerp(Color.Cyan, new Color(0, 100, 255), (progress - 0.25f) / 0.75f);

                Main.spriteBatch.Draw(
                    tex,
                    headPos + offset,
                    null,
                    color * alpha,
                    0f,
                    tex.Size() / 2f,
                    new Vector2(scaleX, scaleY),
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

