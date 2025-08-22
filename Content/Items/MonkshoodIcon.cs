using TalesoftheEntropicSea.Content.Tiles;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Items
{
    public class MonkshoodIcon : ModItem
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Items/MonkshoodIcon";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = 999;
            Item.value = 100;
            Item.rare = ModContent.RarityType<OceanBloomRarity>();
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Monkshood>();
        }
        public override bool? UseItem(Player player)
        {
            Item.placeStyle = Main.rand.Next(3); 
            return base.UseItem(player);
        }

    }
}
