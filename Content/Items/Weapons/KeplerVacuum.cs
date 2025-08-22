using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using TalesoftheEntropicSea.Content.Projectiles.Weapons;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Items.Weapons
{
    public class KeplerVacuum : ModItem
    {
        public override void SetStaticDefaults()
        {
            
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 1000;
            Item.DamageType = DamageClass.Melee;
            Item.width = 112;
            Item.height = 116; 
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 56);
            Item.rare = ModContent.RarityType<OceanBloomRarity>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true; 
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<KeplerVacuumProj>();
            Item.shootSpeed = 2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChallengersIngot>(5)    
                .AddIngredient<EndothermicEnergy>(12)  
                .AddIngredient<GalileoGladius>(1)      
                .AddTile(ModContent.TileType<DraedonsForge>()) 
                .Register();
        }
    }
}
