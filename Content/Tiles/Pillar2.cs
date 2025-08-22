using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;

namespace TalesoftheEntropicSea.Content.Tiles
{
    public class Pillar2 : ModTile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Tiles/Pillar2"; 

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;


            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2); 
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 4;

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 8 };

            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(80, 80, 100), Terraria.Localization.Language.GetText("Mods.TalesoftheEntropicSea.Tiles.Pillar2"));
            DustType = DustID.Stone;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 16; 
        }
    }
}
