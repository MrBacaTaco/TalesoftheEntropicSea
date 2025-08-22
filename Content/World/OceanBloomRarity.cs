using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.World
{
    public class OceanBloomRarity : ModRarity
    {

        private static readonly Color[] GradientColors = new Color[]
        {
            new Color(200, 150, 255), 
            new Color(120, 0, 180),   
            new Color(150, 200, 255), 
            new Color(0, 50, 180)     
        };

        public override Color RarityColor
        {
            get
            {

                const int cycleTime = 240;
                int tick = (int)(Main.GlobalTimeWrappedHourly * 60) % cycleTime;


                float progress = tick / (cycleTime / 4f); 
                int colorIndex = (int)progress % GradientColors.Length;
                int nextColorIndex = (colorIndex + 1) % GradientColors.Length;

                float blend = progress - (int)progress;

                return Color.Lerp(GradientColors[colorIndex], GradientColors[nextColorIndex], blend);
            }
        }
    }
}
