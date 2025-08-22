using Microsoft.Xna.Framework;
using TalesoftheEntropicSea.Content.NPCs.GABoss;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles.Furniture.CraftingStations;
using TalesoftheEntropicSea.Content.UI;
using SubworldLibrary;
using TalesoftheEntropicSea.World; 

namespace TalesoftheEntropicSea.Content.Items
{
    public class SuspiciousTrident : ModItem
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Items/SuspiciousTrident";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13; 
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 64;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<OceanBloomRarity>();
            Item.UseSound = SoundID.Roar;
            Item.maxStack = 1;
            Item.consumable = false; 
            Item.value = Item.buyPrice(gold: 10);
        }

        public override bool? UseItem(Player player)
        {
            
            
            if (!(SubworldSystem.Current is SkyAbyssSubworld))
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.NewText("This item can only be used in the Sky Abyss.", Microsoft.Xna.Framework.Color.Red);
                }
                return false; // Prevent usage
            }


            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.GABoss.Head>());
            }
            else
            {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<NPCs.GABoss.Head>());
            }

            
            BossMessageUI.QueueMessage(
                "The depths stir as something awakens...",
                BossMessageUI.GradCurrent,
                300
            );

            return true;
        }



        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChallengersIngot>(5) 
                .AddIngredient<SkyAbyssStoneItem>(20)    
                .AddTile(ModContent.TileType<DraedonsForge>()) 
                .Register();
        }
    }
}
