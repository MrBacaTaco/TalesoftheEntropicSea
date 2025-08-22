using TalesoftheEntropicSea.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace TalesoftheEntropicSea.Content.Items
{
    public class Pillar1Item : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 96;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Pillar1>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SkyAbyssStoneItem>(40) 
                .AddTile(ModContent.TileType<VoidCondenser>()) 
                .Register();
        }
    }
}
