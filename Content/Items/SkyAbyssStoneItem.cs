using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace TalesoftheEntropicSea.Content.Items
{
    public class SkyAbyssStoneItem : ModItem
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Items/SkyAbyssStoneIcon";

        public override void SetDefaults()
        {
            Item.width = 36; 
            Item.height = 36;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.SkyAbyssStone>();
        }
    }
}
