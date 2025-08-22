using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables; 
using CalamityMod.Items.Placeables.Ores; 
using CalamityMod.Tiles.Furniture.CraftingStations; 
using TalesoftheEntropicSea.Content.Projectiles.Weapons;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Items.Weapons
{
    public class Stingray : ModItem
    {
        public override void SetStaticDefaults()
        {

        }



        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<StingrayProjectile>(), 5000, 2, 4);
            Item.shootSpeed = 4f;
            

            Item.useTime = 90;         
            Item.useAnimation = 90;     
            Item.channel = false;        

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.DamageType = DamageClass.SummonMeleeSpeed;
         
            Item.rare = ModContent.RarityType<OceanBloomRarity>();
        }


        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChallengersIngot>(5)       
                .AddIngredient<EndothermicEnergy>(15)     
                .AddIngredient<ExodiumCluster>(15)        
                .AddIngredient<SeaPrism>(40)              
                .AddTile(ModContent.TileType<DraedonsForge>()) 
                .Register();
        }
    }
}
