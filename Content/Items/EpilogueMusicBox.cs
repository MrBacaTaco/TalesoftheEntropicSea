using Terraria.ID;
using Terraria.ModLoader;
using TalesoftheEntropicSea.Content.Tiles;

namespace TalesoftheEntropicSea.Content.Items
{
    public class EpilogueMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(
                Mod,
                MusicLoader.GetMusicSlot(Mod, "Assets/Music/BeachEpilogue"), 
                ModContent.ItemType<EpilogueMusicBox>(),
                ModContent.TileType<EpilogueMusicBoxTile>()
            );
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<EpilogueMusicBoxTile>(), 0);
        }
    }
}
