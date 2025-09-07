using Terraria.ID;
using Terraria.ModLoader;
using TalesoftheEntropicSea.Content.Tiles;

namespace TalesoftheEntropicSea.Content.Items
{
    public class SkyAbyssMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
          
            MusicLoader.AddMusicBox(
                Mod,
                MusicLoader.GetMusicSlot(Mod, "Assets/Music/RuinsBiome"), 
                ModContent.ItemType<SkyAbyssMusicBox>(),
                ModContent.TileType<SkyAbyssMusicBoxTile>()
            );
        }

        public override void SetDefaults()
        {
            
            Item.DefaultToMusicBox(ModContent.TileType<SkyAbyssMusicBoxTile>(), 0);
        }
    }
}
