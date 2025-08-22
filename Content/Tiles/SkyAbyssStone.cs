
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
    

namespace TalesoftheEntropicSea.Content.Tiles
{
    public class SkyAbyssStone : ModTile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Tiles/SkyAbyssStone";

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            Main.tileShine[Type] = 1100; 

            TileID.Sets.CanBeDugByShovel[Type] = true; 

            AddMapEntry(new Color(30, 60, 100), CreateMapEntryName());

            DustType = DustID.BlueCrystalShard; 
            HitSound = SoundID.Tink;
            MineResist = 20f; 
            MinPick = 250; ; 


            


            RegisterItemDrop(ModContent.ItemType<Items.SkyAbyssStoneItem>());

            Main.tileMerge[Type][ModContent.TileType<SkyAbyssSand>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ChallengerRock>()] = true;

 


        }

       
    }
}
