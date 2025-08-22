using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.Content.Projectiles;
using TalesoftheEntropicSea.Content.Projectiles.Weapons; 
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Items.Weapons
{
    public class AbyssSkylines : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 244; 
            Item.height = 64; 
            Item.damage = 800;
            Item.knockBack = 10f;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 56, 0, 0); 
            Item.rare = ModContent.RarityType<OceanBloomRarity>();

            
            Item.UseSound = CommonCalamitySounds.LargeWeaponFireSound;
        }

        
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-50, 0); 
        }

        public override void HoldItem(Player player)
        {
            
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<AbyssSkylinesBullet>(); 
        }


        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit += 49; 
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChallengersIngot>(5)   
                .AddIngredient<SuspiciousScrap>(4)   
                .AddIngredient<EndothermicEnergy>(10) 
                .AddIngredient<ExodiumCluster>(15)   
                .AddIngredient<TyrannysEnd>(1)       
                .AddTile(ModContent.TileType<DraedonsForge>()) 
                .Register();
        }
        
    }
}



