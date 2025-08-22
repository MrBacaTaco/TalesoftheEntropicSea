using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace TalesoftheEntropicSea.Content.Items
{
    public class SkyAbyssSandItem : ModItem
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Items/SkyAbyssSandIcon";

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 999;
            Item.value = 0;
            Item.rare = ItemRarityID.White;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.SkyAbyssSand>();
        }
    }
}
