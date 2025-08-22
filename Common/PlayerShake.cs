using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TalesoftheEntropicSea.Common
{
    public class ScreenShakePlayer : ModPlayer
    {
        public float screenShakeIntensity;

        public override void ModifyScreenPosition()
        {
            if (screenShakeIntensity > 0f)
            {
                
                Main.screenPosition += new Vector2(
                    Main.rand.NextFloat(-screenShakeIntensity, screenShakeIntensity),
                    Main.rand.NextFloat(-screenShakeIntensity, screenShakeIntensity)
                );

                
                screenShakeIntensity *= 0.9f;
                if (screenShakeIntensity < 0.2f)
                    screenShakeIntensity = 0f;
            }
        }
    }
}
