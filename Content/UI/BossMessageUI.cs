

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TalesoftheEntropicSea.Content.UI
{
    public class BossMessageUI : ModSystem
    {
        private class QueuedMessage
        {
            public string Text;
            public Color[] Gradient;
            public int Time;
        }

        private static readonly Queue<QueuedMessage> messageQueue = new Queue<QueuedMessage>();

        private static string activeMessage;
        private static Color[] activeGradient;
        private static int timer;

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (timer > 0 && !string.IsNullOrEmpty(activeMessage))
            {
                DrawGradientText(spriteBatch, activeMessage, timer);
            }
            else if (messageQueue.Count > 0)
            {

                var next = messageQueue.Dequeue();
                activeMessage = next.Text;
                activeGradient = next.Gradient;
                timer = next.Time;
            }
        }

        public static void QueueMessage(string message, Color[] gradient, int displayTime = 300)
        {
            messageQueue.Enqueue(new QueuedMessage
            {
                Text = message,
                Gradient = gradient,
                Time = displayTime
            });
        }

        private void DrawGradientText(SpriteBatch spriteBatch, string text, int timeLeft)
        {
            var font = ModContent.Request<SpriteFont>("TalesoftheEntropicSea/Content/UI/OldEngGothic").Value;
            float scale = 1f;
            Vector2 size = font.MeasureString(text) * scale;
            Vector2 position = new Vector2(Main.screenWidth / 2f - size.X / 2f, 100f);

            float time = (float)Main.GameUpdateCount / 60f;
            int waveSize = 20;

            Vector2 drawPos = position;

            for (int i = 0; i < text.Length; i++)
            {
                string charStr = text[i].ToString();
                float wave = (float)Math.Sin((time * 2f) + (i * 0.3f)) * 0.5f + 0.5f;
                Color c1 = activeGradient[(i / waveSize) % activeGradient.Length];
                Color c2 = activeGradient[(i / waveSize + 1) % activeGradient.Length];
                Color final = Color.Lerp(c1, c2, wave);

                if (timeLeft < 60)
                {
                    float fade = timeLeft / 60f;
                    final *= fade;
                }

                spriteBatch.DrawString(font, charStr, drawPos, final, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                drawPos.X += font.MeasureString(charStr).X * scale;
            }

            timer--;
        }


        public static readonly Color[] GradLightPurpleDarkPurple =
        {
            new Color(200, 150, 255),
            new Color(170, 110, 240),
            new Color(140, 80, 220),
            new Color(120, 60, 200)
        };

        public static readonly Color[] GradCurrent =
        {
            new Color(200, 150, 255), 
            new Color(120, 60, 200),  
            new Color(140, 200, 255), 
            new Color(60, 100, 200)   
        };

        public static readonly Color[] GradLightBlueDarkBlue =
        {
            new Color(140, 200, 255),
            new Color(100, 160, 240),
            new Color(80, 130, 220),
            new Color(60, 100, 200)
        };
    }
}

