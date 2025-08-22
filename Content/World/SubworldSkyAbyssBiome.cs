using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using TalesoftheEntropicSea.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.GameContent;



namespace TalesoftheEntropicSea.Content.World
{
    public class SubworldSkyAbyssBiome : ModBiome
    {
        



        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<SubworldSkyAbyssBackgroundStyle>();
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/VoidWater");

        public override bool IsBiomeActive(Player player)
        {
            return SubworldSystem.Current is SkyAbyssSubworld;
        }

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/RuinsBiome");
    }
}
