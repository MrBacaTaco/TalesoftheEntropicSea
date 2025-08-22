using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.World
{
    public class SkyAbyssMediumSky : CustomSky
    {
        private bool isActive;
        private float intensity;

        public override void Update(GameTime gameTime)
        {
            const float fadeSpeed = 0.01f;

            if (isActive && intensity < 1f)
                intensity = MathHelper.Clamp(intensity + fadeSpeed, 0f, 1f);
            else if (!isActive && intensity > 0f)
                intensity = MathHelper.Clamp(intensity - fadeSpeed, 0f, 1f);
        }

        public override bool IsActive() => intensity > 0f;

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {

            if (intensity <= 0f || maxDepth < 0f || minDepth >= 0f)
                return;

            Texture2D tex = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Assets/Textures/Backgrounds/SkyAbyssMedium", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float parallaxFactor = 0.3f;
            float scale = 1.6f;
            float screenYPercent = 0.7f;
            float xOffset = 150f;

            Vector2 parallaxOffset = -Main.screenPosition * parallaxFactor;
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f + xOffset, Main.screenHeight * screenYPercent) + parallaxOffset;

            float playerY = Main.LocalPlayer.Center.Y / 16f;
            float spaceY = 150f;
            float heightDarkness = MathHelper.Clamp((spaceY - playerY) / 80f, 0f, 1f);

            float timeBrightness;
            if (Main.dayTime)
            {
                if (Main.time < 5400)
                    timeBrightness = (float)(Main.time / 5400);
                else if (Main.time > 48600)
                    timeBrightness = (float)((54000 - Main.time) / 5400);
                else
                    timeBrightness = 1f;
            }
            else
            {
                if (Main.time < 2700)
                    timeBrightness = (float)(1f - Main.time / 2700);
                else if (Main.time > 29700)
                    timeBrightness = (float)((Main.time - 29700) / 2700);
                else
                    timeBrightness = 0f;
            }

            float combinedDarkness = MathHelper.Clamp(heightDarkness + (1f - timeBrightness), 0f, 1f);
            Color drawColor = Color.Lerp(Color.White, Color.Black, combinedDarkness) * intensity;

            spriteBatch.Draw(
                tex,
                drawPos,
                null,
                drawColor,
                0f,
                new Vector2(tex.Width / 2f, tex.Height / 2f),
                scale,
                SpriteEffects.None,
                0f
            );
        }


        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
            intensity = 0f;
        }

        public override Color OnTileColor(Color inColor)
        {
            return Color.Lerp(inColor, Color.Black, intensity * 0.3f);
        }
    }
}
