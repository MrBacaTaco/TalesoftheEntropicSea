using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;

namespace TalesoftheEntropicSea.Content.Tiles
{
    public class Pillar3 : ModTile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Tiles/Pillar3";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;


            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); 
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 6;

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };

            TileObjectData.newTile.Origin = new Point16(1, 5);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(90, 90, 110), CreateMapEntryName());
            DustType = DustID.Stone;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 8; 
        }
    }
}
