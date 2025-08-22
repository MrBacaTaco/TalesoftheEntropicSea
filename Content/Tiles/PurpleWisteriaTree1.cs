using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures; 
using Terraria.Enums;         
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;


namespace TalesoftheEntropicSea.Content.Tiles
{
    public class PurpleWisteriaTree1 : ModTile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Tiles/PurpleWisteriaTree1";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileCut[Type] = false; 


            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 7; 
            TileObjectData.newTile.Height = 14; 
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 8 }; 
            TileObjectData.newTile.Origin = new Point16(3, 13);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(160, 100, 200), Language.GetText("Purple Wisteria Tree"));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
            MinPick = 0;

            
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {

        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            
                offsetY = 26; 
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;

            Texture2D glowmask = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/Tiles/PurpleWisteriaTree1GlowMask").Value;

            Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);

            float glowStrength = Main.dayTime ? 0f : 1f; 

            if (glowStrength > 0f)
            {
                spriteBatch.Draw(
                    glowmask,
                    position,
                    frame,
                    Color.White * glowStrength,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
        }

    }
}
