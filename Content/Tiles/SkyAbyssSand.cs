using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace TalesoftheEntropicSea.Content.Tiles
{
    public class SkyAbyssSand : ModTile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Tiles/SkyAbyssSandTileset";

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;      
            Main.tileBlockLight[Type] = true; 
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = false;
            Main.tileSand[Type] = false;      

            TileID.Sets.Conversion.Sand[Type] = true; 


            AddMapEntry(new Color(80, 140, 220), Language.GetText("Sky Abyss Sand"));
            DustType = DustID.BlueCrystalShard; 
            RegisterItemDrop(ModContent.ItemType<Items.SkyAbyssSandItem>());
            MinPick = 250; 
            MineResist = 10f;

            Main.tileMerge[Type][ModContent.TileType<SkyAbyssStone>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ChallengerRock>()] = true;
        }

    }
}
