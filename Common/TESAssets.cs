using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace TalesofttheEntropicSea.Common
{
    public static class TESAssets
    {
        public static Asset<Texture2D> Line;

        public static void Load(Mod mod)
        {
            Line = mod.Assets.Request<Texture2D>("Content/Line");
        }

        public static void Unload()
        {
            Line = null;
        }
    }
}
