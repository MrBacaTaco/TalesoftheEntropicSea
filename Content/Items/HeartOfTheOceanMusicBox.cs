using Terraria.ID;
using Terraria.ModLoader;
using TalesoftheEntropicSea.Content.Tiles;

namespace TalesoftheEntropicSea.Content.Items
{
    public class HeartOfTheOceanMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(
                Mod,
                MusicLoader.GetMusicSlot(Mod, "Assets/Music/HelmetBoss"), 
                ModContent.ItemType<HeartOfTheOceanMusicBox>(),
                ModContent.TileType<HeartOfTheOceanMusicBoxTile>()
            );
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<HeartOfTheOceanMusicBoxTile>(), 0);
        }
    }
}
