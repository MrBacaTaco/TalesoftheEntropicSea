using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using TalesoftheEntropicSea.Content.Tiles;

namespace TalesoftheEntropicSea.Content.Items
{
    public class PurpleWisteriaTree2Item : ModItem
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Items/PurpleWisteriaTree2";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.PurpleWisteriaTree2>();
        }
    }
}
