using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TalesoftheEntropicSea.Content.Tiles;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace TalesoftheEntropicSea.Content.Items
{
    public class Pillar2Item : ModItem
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Items/Pillar2Item"; 

        public override void SetDefaults()
        {
            Item.width = 44;   
            Item.height = 56;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 100;
            Item.createTile = ModContent.TileType<Tiles.Pillar2>();
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
