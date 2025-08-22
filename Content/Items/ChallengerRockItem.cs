using TalesoftheEntropicSea.Content.Tiles;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Items
{
    public class ChallengerRockItem : ModItem
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Items/ChallengerDust"; 
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 999;
            Item.value = 0;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ChallengerRock>();
            Item.rare = ModContent.RarityType<OceanBloomRarity>();
        }
    }
}
