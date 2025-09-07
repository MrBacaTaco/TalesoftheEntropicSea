using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using TalesoftheEntropicSea.Content.Items;

namespace TalesoftheEntropicSea.Content.Tiles
{
    public class SkyAbyssMusicBoxTile : ModTile
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

            RegisterItemDrop(ModContent.ItemType<SkyAbyssMusicBox>());
            AddMapEntry(new Color(200, 200, 200));
        }

        public override void EmitParticles(int i, int j, Tile tileCache, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            if (!visible || tileFrameX != 36 || tileFrameY % 36 != 0)
                return;

            if ((int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
                return;

            int noteType = Main.rand.Next(570, 573);

            Vector2 pos = new Vector2(i * 16 + 8, j * 16 - 8);

            Vector2 vel = new Vector2(Main.WindForVisuals * 2f, -0.5f);
            vel.X *= Main.rand.NextFloat(0.5f, 1.5f);
            vel.Y *= Main.rand.NextFloat(0.5f, 1.5f);

            switch (noteType)
            {
                case 572: pos.X -= 8f; break;
                case 571: pos.X -= 4f; break;
            }

            Gore.NewGore(new EntitySource_TileUpdate(i, j), pos, vel, noteType, 0.8f);
        }

    }
}
