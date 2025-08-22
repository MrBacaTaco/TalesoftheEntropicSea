using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TalesoftheEntropicSea.Content.UI
{
    public class SkyAbyssIntroUI : ModSystem
    {
        private static string[] activeLines;
        private static int timer;
        private static int totalTime;
        private static int revealSpeed = 2; 

        private static readonly Color[] gradientColors = new Color[]
        {
            new Color(200, 150, 255), 
            new Color(120, 60, 200),  
            new Color(140, 200, 255), 
            new Color(60, 100, 200)   
        };

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (timer > 0 && activeLines != null && activeLines.Length > 0)
            {
                DrawGradientText(spriteBatch, activeLines, timer);
                timer--; 
            }
        }

        public static void ShowMessage(string[] lines, int displayTime = 300)
        {
            activeLines = lines;
            timer = displayTime;
            totalTime = displayTime;
        }

        private void DrawGradientText(SpriteBatch spriteBatch, string[] lines, int timeLeft)
        {
            var font = ModContent.Request<SpriteFont>("TalesoftheEntropicSea/Content/UI/OldEngGothic").Value;
            float scale = 1.2f;
            float lineSpacing = 10f;

            float startY = 200f;

            float time = (float)Main.GameUpdateCount / 60f;
            int waveSize = 20;

            int elapsed = totalTime - timeLeft;
            int charsToShow = elapsed / revealSpeed;
            int drawnChars = 0;

            for (int l = 0; l < lines.Length; l++)
            {
                string text = lines[l];
                Vector2 size = font.MeasureString(text) * scale;
                Vector2 position = new Vector2(Main.screenWidth / 2f - size.X / 2f, startY + l * (font.LineSpacing * scale + lineSpacing));
                Vector2 drawPos = position;

                for (int i = 0; i < text.Length; i++)
                {
                    if (drawnChars >= charsToShow)
                        return; 

                    string charStr = text[i].ToString();
                    float wave = (float)Math.Sin((time * 2f) + (i * 0.3f)) * 0.5f + 0.5f;
                    Color c1 = gradientColors[(i / waveSize) % gradientColors.Length];
                    Color c2 = gradientColors[(i / waveSize + 1) % gradientColors.Length];
                    Color final = Color.Lerp(c1, c2, wave);

                    if (timeLeft < 60)
                    {
                        float fade = timeLeft / 60f;
                        final *= fade;
                    }

                    spriteBatch.DrawString(font, charStr, drawPos, final, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    drawPos.X += font.MeasureString(charStr).X * scale;
                    drawnChars++;
                }
            }
        }
    }
}
