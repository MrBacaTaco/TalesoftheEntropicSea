using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.Content.Items;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace TalesoftheEntropicSea.Content.Tiles
{
    public class Monkshood : ModTile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Tiles/Monkshood";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true; 
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.StyleHorizontal = false; 
            TileObjectData.newTile.StyleWrapLimit = 1;      
            TileObjectData.newTile.StyleMultiplier = 3;     
            TileObjectData.newTile.AnchorBottom = new Terraria.DataStructures.AnchorData(
                AnchorType.SolidTile | AnchorType.PlanterBox,
                TileObjectData.newTile.Width, 0
            );
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 60, 160), Language.GetText("Monkshood"));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
            RegisterItemDrop(ModContent.ItemType<MonkshoodIcon>());
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {

            offsetY = 4;
        }
        public override bool CanPlace(int i, int j)
        {
            if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == Type)
                return false;
            return base.CanPlace(i, j);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            short style = (short)(Main.rand.Next(3) * 18);
            Main.tile[i, j].TileFrameX = style;
            Main.tile[i, j + 1].TileFrameX = style;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Tile tile = Main.tile[i, j];
            Texture2D glowTexture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/Tiles/Monkshood_Glow").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;

            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;

            if (!Main.dayTime)
            {
                Color color = Color.Blue; 
                spriteBatch.Draw(glowTexture, position, new Rectangle(frameX, frameY, 16, 32), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (!Main.dayTime)
            {
                r = 0.2f;
                g = 0.2f;
                b = 0.7f; 
            }
        }



    }
}
