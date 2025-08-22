using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using CalamityMod.Tiles.Abyss;

namespace TalesoftheEntropicSea.Content.World
{
    public class SkyAbyssBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/RuinsBiome");
        public override bool IsBiomeActive(Player player)
        {
            int voidstoneCount = 0;
            int radius = 50;
            int required = 50;

            ushort calamityVoidstone = (ushort)ModContent.TileType<CalamityMod.Tiles.Abyss.Voidstone>();
            Point playerTile = player.Center.ToTileCoordinates();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int tileX = playerTile.X + x;
                    int tileY = playerTile.Y + y;

                    if (!WorldGen.InWorld(tileX, tileY))
                        continue;

                    Tile tile = Framing.GetTileSafely(tileX, tileY);

                    if (tile.HasTile && tile.TileType == calamityVoidstone)
                    {
                        voidstoneCount++;
                        if (voidstoneCount >= required)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        public override void OnInBiome(Player player)
        {
            if (!SkyManager.Instance["TalesoftheEntropicSea:SkyAbyssSky"].IsActive())
                SkyManager.Instance.Activate("TalesoftheEntropicSea:SkyAbyssSky");

            if (!SkyManager.Instance["TalesoftheEntropicSea:SkyAbyssMediumSky"].IsActive())
                SkyManager.Instance.Activate("TalesoftheEntropicSea:SkyAbyssMediumSky");

            if (!SkyManager.Instance["TalesoftheEntropicSea:SkyAbyssCloseSky"].IsActive())
                SkyManager.Instance.Activate("TalesoftheEntropicSea:SkyAbyssCloseSky");
        }

        public override void OnLeave(Player player)
        {
            SkyManager.Instance.Deactivate("TalesoftheEntropicSea:SkyAbyssSky");
            SkyManager.Instance.Deactivate("TalesoftheEntropicSea:SkyAbyssMediumSky");
            SkyManager.Instance.Deactivate("TalesoftheEntropicSea:SkyAbyssCloseSky");
        }
    }
}
