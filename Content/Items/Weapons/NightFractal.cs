

using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
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
    public class NightFractal : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
        }

        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 126;
            Item.damage = 4200;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 200;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item13;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2f;
            Item.shoot = ModContent.ProjectileType<NightFractalBeam>(); 
            Item.shootSpeed = 0f; 
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<OceanBloomRarity>();

            Item.useTurn = false;
            Item.autoReuse = false;

            
        }

        public override bool CanUseItem(Player player)
        {
            
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override bool AltFunctionUse(Player player)
        {
            return false; 
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChallengersIngot>(5)
                .AddIngredient<EndothermicEnergy>(10)
                .AddIngredient<AshesofAnnihilation>(5)
                .AddIngredient<DarkSpark>(1)
                .AddTile(ModContent.TileType<DraedonsForge>())
                .Register();
        }
    }
}

