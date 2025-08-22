using Microsoft.Xna.Framework;
using TalesoftheEntropicSea.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;                
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;


namespace TalesoftheEntropicSea.Content.Tiles
{
    public class ChallengerRock : ModTile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Tiles/ChallengerRock";

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = false;

            Main.tileFrameImportant[Type] = false;

         

            AddMapEntry(new Color(70, 100, 200), Language.GetText("Challenger Rock"));

            MinPick = 250; 
            MineResist = 40f; 

            DustType = DustID.BlueCrystalShard; 
            HitSound = SoundID.Tink;
            RegisterItemDrop(ModContent.ItemType<Items.ChallengerRockItem>());

            Main.tileMerge[Type][ModContent.TileType<SkyAbyssStone>()] = true;
            Main.tileMerge[Type][ModContent.TileType<SkyAbyssSand>()] = true;

            Main.tileOreFinderPriority[Type] = 950; 
            Main.tileSpelunker[Type] = true;        

            
            

        }

    }
}
