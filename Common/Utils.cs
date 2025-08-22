using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesofttheEntropicSea.Common;
using Terraria;
using Terraria.GameContent;

namespace TalesoftheEntropicSea.Common.Utils
{
    public static class DrawUtils
    {
        public static void DrawLineBetter(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
        {
            if (start == end)
                return;

            start -= Main.screenPosition;
            end -= Main.screenPosition;

            Texture2D line = TESAssets.Line.Value;            
            float rotation = (end - start).ToRotation();
            float length = Vector2.Distance(start, end);
            Vector2 scale = new Vector2(length / line.Width, width);

            
            spriteBatch.Draw(line, start, null, color, rotation, new Vector2(0f, line.Height / 2f), scale, SpriteEffects.None, 0f);
        }

    }
}
