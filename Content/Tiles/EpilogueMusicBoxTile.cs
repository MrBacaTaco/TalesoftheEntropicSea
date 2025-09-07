using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures; 
using TalesoftheEntropicSea.Content.Items;

namespace TalesoftheEntropicSea.Content.Tiles
{
    public class EpilogueMusicBoxTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.StyleLineSkip = 2; 
            TileObjectData.addTile(Type);

            RegisterItemDrop(ModContent.ItemType<EpilogueMusicBox>());
            AddMapEntry(new Color(200, 200, 200));



        }

        /*
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.netMode == NetmodeID.Server) return; // no gore on server
            if (!closer) return;                           // only when a player is close

            Tile tile = Main.tile[i, j];

            // Only when the box is "open" and this is the top-left subtile
            if (tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0)
                return;

            // Throttle so it doesn't spam
            if ((int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
                return;

            int noteType = Main.rand.Next(570, 573); // vanilla music notes

            Vector2 pos = new(i * 16 + 8, j * 16 - 8);
            Vector2 vel = new(Main.WindForVisuals * 2f, -0.5f);
            vel.X *= Main.rand.NextFloat(0.5f, 1.5f);
            vel.Y *= Main.rand.NextFloat(0.5f, 1.5f);
            if (noteType == 572) pos.X -= 8f; else if (noteType == 571) pos.X -= 4f;

            Gore.NewGore(new EntitySource_TileUpdate(i, j), pos, vel, noteType, 0.8f);
        }

        didnt work for some reason

        */

    }
}
