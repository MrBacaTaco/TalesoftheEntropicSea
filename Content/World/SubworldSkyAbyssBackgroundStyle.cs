using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.World
{
    public class SubworldSkyAbyssBackgroundStyle : ModSurfaceBackgroundStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
                fades[i] = 0f;

            fades[Slot] = 1f;
        }

        public override int ChooseFarTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/SkyAbyssFar");
        }

        public override int ChooseMiddleTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/SkyAbyssMedium");
        }

       
        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/SkyAbyssClose");
        }
    }
}
