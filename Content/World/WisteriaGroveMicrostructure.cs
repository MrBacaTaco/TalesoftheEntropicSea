
using Microsoft.Xna.Framework;
using TalesoftheEntropicSea.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;
using CalamityMod.Tiles.Abyss;

namespace TalesoftheEntropicSea.Content.World
{
    public class WisteriaGroveGen : ModSystem
    {
        public void GenerateWisteriaGroves()
        {
            int tries = Main.maxTilesX / 200;

            for (int i = 0; i < tries; i++)
            {
                int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int y = FindSurfaceY(x);


                if (y < Main.worldSurface * 0.25f || y > Main.worldSurface * 0.9f)
                    continue;


                if (!IsForestBiome(x, y))
                    continue;


                if (Main.wallDungeon[Main.tile[x, y].WallType] || Main.tile[x, y].WallType != WallID.None)
                    continue;

                int treeType = GetRandomWisteriaTree();

                int treeWidth = 1;
                if (TileObjectData.GetTileData(treeType, 0) != null)
                    treeWidth = TileObjectData.GetTileData(treeType, 0).Width;

                PlaceWisteriaTree(x, y, treeType);
                PlaceFlowersAround(x, y, ModContent.TileType<Monkshood>(), treeWidth);
            }
        }

        private bool IsForestBiome(int centerX, int centerY, int radius = 30)
        {
            ushort[] invalidTiles = new ushort[]
            {
                TileID.SnowBlock,
                TileID.IceBlock,
                TileID.CorruptGrass,
                TileID.HallowedGrass,
                TileID.Sand,
                TileID.Ebonsand,
                TileID.Crimsand,
                TileID.Pearlsand,
                TileID.Mud,
                TileID.JungleGrass,

                TileID.LeafBlock,
                TileID.LivingMahoganyLeaves,
                TileID.Cloud,
                TileID.Crimstone,
                TileID.FleshIce,
                TileID.Ebonstone,
                TileID.CorruptIce,
                TileID.Pearlstone,
                TileID.HallowedIce,

                (ushort)ModContent.TileType<CalamityMod.Tiles.Abyss.Voidstone>(),
                (ushort)ModContent.TileType<CalamityMod.Tiles.Abyss.SulphurousSand>(),
            };

            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    ushort t = Main.tile[x, y].TileType;
                    for (int i = 0; i < invalidTiles.Length; i++)
                    {
                        if (t == invalidTiles[i])
                            return false;
                    }
                }
            }
            return true;
        }

        private int FindSurfaceY(int x)
        {
            for (int y = 0; y < Main.worldSurface; y++)
            {
                if (Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
                    return y - 1;
            }
            return (int)Main.worldSurface;
        }

        private int GetRandomWisteriaTree()
        {
            int[] trees = {
                ModContent.TileType<PurpleWisteriaTree1>(),
                ModContent.TileType<PurpleWisteriaTree2>(),
                ModContent.TileType<PurpleWisteriaTree3>()
            };
            return trees[WorldGen.genRand.Next(trees.Length)];
        }

        private void PlaceWisteriaTree(int centerX, int surfaceY, int treeTileType)
        {
            int platformWidth = 20;
            int platformHeight = 3;
            int clearHeightAbove = 30;


            for (int dx = -platformWidth / 2; dx <= platformWidth / 2; dx++)
            {
                for (int dy = -clearHeightAbove; dy <= 0; dy++)
                {
                    int tileX = centerX + dx;
                    int tileY = surfaceY + dy - 1;

                    if (WorldGen.InWorld(tileX, tileY))
                    {
                        Main.tile[tileX, tileY].ClearTile();
                    }
                }
            }

            for (int dx = -platformWidth / 2; dx <= platformWidth / 2; dx++)
            {
                for (int dy = 0; dy < platformHeight; dy++)
                {
                    int tileX = centerX + dx;
                    int tileY = surfaceY + dy;

                    if (WorldGen.InWorld(tileX, tileY))
                    {
                        Tile tile = Main.tile[tileX, tileY];
                        tile.ClearTile();
                        tile.HasTile = true;
                        tile.TileType = (dy == 0) ? TileID.Grass : TileID.Dirt;
                        tile.Slope = SlopeType.Solid;
                        tile.BlockType = BlockType.Solid;
                    }
                }
            }

            int treeWidth = 1;
            if (TileObjectData.GetTileData(treeTileType, 0) != null)
                treeWidth = TileObjectData.GetTileData(treeTileType, 0).Width;

            int treeX = centerX;
            int treeY = surfaceY - 1;
            WorldGen.PlaceObject(treeX, treeY, treeTileType);

            PlaceFlowersAround(centerX, surfaceY, ModContent.TileType<Monkshood>(), treeWidth);
        }

        private void PlaceFlowersAround(int centerX, int platformTopY, int flowerTileType, int treeWidth)
        {
            int flowersPlaced = 0;
            int flowerY = platformTopY - 2;
            int halfPlatform = 20 / 2;

            int treeLeft = centerX - (treeWidth / 2);
            int treeRight = centerX + (treeWidth / 2);


            for (int x = centerX - halfPlatform; x < treeLeft; x++)
            {
                if (!WorldGen.InWorld(x, flowerY, 5)) continue;

                if (Main.tile[x, platformTopY].HasTile && Main.tile[x, platformTopY].TileType == TileID.Grass)
                {
                    WorldGen.KillTile(x, flowerY);
                    if (WorldGen.PlaceTile(x, flowerY, flowerTileType, mute: true))
                    {
                        short style = (short)(WorldGen.genRand.Next(3) * 18);
                        Main.tile[x, flowerY].TileFrameX = style;
                        Main.tile[x, flowerY + 1].TileFrameX = style;
                        flowersPlaced++;
                    }
                }
            }

            for (int x = treeRight + 1; x <= centerX + halfPlatform; x++)
            {
                if (!WorldGen.InWorld(x, flowerY, 5)) continue;

                if (Main.tile[x, platformTopY].HasTile && Main.tile[x, platformTopY].TileType == TileID.Grass)
                {
                    WorldGen.KillTile(x, flowerY);
                    if (WorldGen.PlaceTile(x, flowerY, flowerTileType, mute: true))
                    {
                        short style = (short)(WorldGen.genRand.Next(3) * 18);
                        Main.tile[x, flowerY].TileFrameX = style;
                        Main.tile[x, flowerY + 1].TileFrameX = style;
                        flowersPlaced++;
                    }
                }
            }
        }
    }
}
