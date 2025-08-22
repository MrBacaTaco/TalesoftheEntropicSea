using CalamityMod.Tiles.Furniture.CraftingStations;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Items
{
    public class ChallengersIngot : ModItem
    {
        public override void SetStaticDefaults()
        {
            
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 9)); 
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; 
        }

        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 68;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(silver: 10); 
            Item.rare = ModContent.RarityType<OceanBloomRarity>(); 
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChallengerRockItem>(4) 
                .AddIngredient<MonkshoodIcon>(1)     
                .AddTile(ModContent.TileType<DraedonsForge>()) 
                .Register();
        }
    }
}
