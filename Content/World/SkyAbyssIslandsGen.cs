

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Walls;
using TalesoftheEntropicSea.Content.Tiles;

namespace TalesoftheEntropicSea.Content.World
{
    public class SkyAbyssIslandsGen : ModSystem
    {
        public static int SkyAbyssCenterX;
        public static int SkyAbyssCenterY;
        public static int SkyAbyssRadius = 120;

        /// <summary>
        /// Called by SkyAbyssTrigger when conditions are met.
        /// </summary>
        public void GenerateSkyAbyssIslands()
        {
            bool isDungeonLeft = GenVars.dungeonX < Main.maxTilesX / 2;
            int centerX = isDungeonLeft ? 300 : Main.maxTilesX - 300;
            int centerY = (int)(GenVars.worldSurface * 0.35f);

            SkyAbyssCenterX = centerX;
            SkyAbyssCenterY = centerY;

            GenerateIsland(centerX, centerY, 24, 1.2f); 

            int islandCount = 45;
            for (int i = 0; i < islandCount; i++)
            {
                float angle = WorldGen.genRand.NextFloat(MathHelper.TwoPi);
                float distance = WorldGen.genRand.Next(60, 180);
                int x = centerX + (int)(Math.Cos(angle) * distance);
                int y = centerY + (int)(Math.Sin(angle) * distance * 0.5f);

                int size = WorldGen.genRand.Next(8, 16);
                float sharpness = WorldGen.genRand.NextFloat(1.2f, 1.7f);

                GenerateIsland(x, y, size, sharpness);
            }

            GenerateDebrisIslands(centerX, centerY);
        }

        private void GenerateDebrisIslands(int centerX, int centerY)
        {
            int maxRange = 250;
            int minRange = 80;
            int debrisAttempts = 250;

            int voidstoneID = ModContent.TileType<Voidstone>();
            int voidstoneWallID = ModContent.WallType<VoidstoneWall>();

            for (int i = 0; i < debrisAttempts; i++)
            {
                float angle = WorldGen.genRand.NextFloat(MathHelper.TwoPi);
                float distance = WorldGen.genRand.Next(minRange, maxRange);

                float t = (distance - minRange) / (float)(maxRange - minRange);
                float densityChance = (float)Math.Sin(t * Math.PI);
                if (WorldGen.genRand.NextFloat() > densityChance)
                    continue;

                int debrisX = centerX + (int)(Math.Cos(angle) * distance);
                int debrisY = centerY + (int)(Math.Sin(angle) * distance * 0.5f);

                float sizeTaper = 1f - t;
                int baseSize = WorldGen.genRand.Next(2, 5);
                baseSize = (int)(baseSize * sizeTaper);
                if (baseSize < 2)
                    continue;

                GenerateDebrisShape(debrisX, debrisY, baseSize, voidstoneID, voidstoneWallID);
            }
        }

        private void GenerateDebrisShape(int startX, int startY, int size, int tileType, int wallType)
        {
            int halfSize = size;

            for (int x = -halfSize; x <= halfSize; x++)
            {
                for (int y = -halfSize * 2; y <= halfSize * 2; y++)
                {
                    float dx = Math.Abs(x) / (float)halfSize;
                    float dy = Math.Abs(y) / (float)(halfSize * 2);
                    float distance = dx + dy;

                    if (distance > 1.2f)
                        continue;

                    int worldX = startX + x;
                    int worldY = startY + y;

                    if (!WorldGen.InWorld(worldX, worldY)) continue;

                    Tile tile = Framing.GetTileSafely(worldX, worldY);
                    tile.HasTile = true;
                    tile.TileType = (ushort)tileType;

                    if (distance < 0.9f)
                        tile.WallType = (ushort)wallType;
                }
            }

            GenerateSpikyVoidstoneWall(startX, startY + size + 4, size / 2, size * 2);
        }

        private void GenerateIsland(int startX, int startY, int size, float sharpness)
        {
            List<Point> placed = new();
            int voidstoneID = ModContent.TileType<Voidstone>();
            int voidstoneWallID = ModContent.WallType<VoidstoneWall>();

            int halfWidth = size;
            int[] surfaceOffsets = new int[halfWidth * 2 + 1];
            int lastOffset = 0;

            for (int i = 0; i < surfaceOffsets.Length; i++)
            {
                lastOffset += WorldGen.genRand.Next(-1, 2);
                lastOffset = Utils.Clamp(lastOffset, -2, 2);
                surfaceOffsets[i] = lastOffset;
            }

            for (int x = -halfWidth; x <= halfWidth; x++)
            {
                int index = x + halfWidth;
                int surfaceY = surfaceOffsets[index];

                int baseHeight = size - (int)(Math.Abs(x) * sharpness);
                baseHeight = Math.Max(2, baseHeight);

                if (WorldGen.genRand.NextBool(3))
                    baseHeight += WorldGen.genRand.Next(2, 6);

                for (int y = 0; y <= baseHeight; y++)
                {
                    int worldX = startX + x;
                    int worldY = startY + surfaceY + y;

                    if (!WorldGen.InWorld(worldX, worldY)) continue;

                    Tile tile = Framing.GetTileSafely(worldX, worldY);
                    tile.HasTile = true;
                    tile.TileType = (ushort)voidstoneID;

                    placed.Add(new Point(worldX, worldY));
                }
            }

            List<Point> wallPoints = new();
            foreach (Point pt in placed)
            {
                int dx = Math.Abs(pt.X - startX);
                int dy = pt.Y - startY;
                float distRatio = 1f - dx / (float)size;

                if (distRatio > 0.1f)
                {
                    float diamondHeight = size * distRatio * 2.2f;
                    if (dy > diamondHeight * 0.2f && dy < diamondHeight * 1.4f)
                    {
                        wallPoints.Add(pt);
                    }
                }
            }

            HashSet<Point> expanded = new(wallPoints);
            foreach (var pt in wallPoints)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int wx = pt.X + dx;
                        int wy = pt.Y + dy;

                        if (!WorldGen.InWorld(wx, wy)) continue;

                        Tile tile = Framing.GetTileSafely(wx, wy);
                        if (!tile.HasTile)
                        {
                            tile.HasTile = true;
                            tile.TileType = TileID.Dirt;
                        }

                        expanded.Add(new Point(wx, wy));
                    }
                }
            }

            foreach (var pt in expanded)
            {
                Tile tile = Framing.GetTileSafely(pt.X, pt.Y);
                tile.WallType = (ushort)voidstoneWallID;
            }

            foreach (var pt in expanded)
            {
                Tile tile = Framing.GetTileSafely(pt.X, pt.Y);
                if (tile.HasTile && tile.TileType == TileID.Dirt)
                {
                    tile.HasTile = false;
                }
            }

            GenerateSpikyVoidstoneWall(startX, startY + size + 4, size / 2, size * 3);
            DropRandomWaterAboveIsland(placed);
            PlaceAbyssalPots(placed);
            PlacePillars(placed);
        }

        private void DropRandomWaterAboveIsland(List<Point> islandBlocks)
        {
            if (islandBlocks.Count == 0) return;

            int waterDrops = WorldGen.genRand.Next(2, 5);

            for (int i = 0; i < waterDrops; i++)
            {
                Point randomPt = islandBlocks[WorldGen.genRand.Next(islandBlocks.Count)];
                int dropX = randomPt.X;
                int dropY = randomPt.Y - WorldGen.genRand.Next(20, 40);

                for (int y = dropY; y < randomPt.Y; y++)
                {
                    if (!WorldGen.InWorld(dropX, y)) continue;

                    Tile tile = Framing.GetTileSafely(dropX, y);
                    if (!tile.HasTile && tile.LiquidAmount == 0)
                    {
                        tile.LiquidAmount = 255;
                        tile.LiquidType = LiquidID.Water;
                        break;
                    }
                }
            }
        }

        private void PlaceAbyssalPots(List<Point> placedTiles)
        {
            int abyssalPotID = ModContent.TileType<CalamityMod.Tiles.Abyss.AbyssalPots>();
            HashSet<Point> alreadyPlaced = new();

            foreach (Point pt in placedTiles)
            {
                int x = pt.X;
                int y = pt.Y;

                Point right = new(x + 1, y);
                if (!placedTiles.Contains(right)) continue;

                if (Framing.GetTileSafely(x, y - 1).HasTile) continue;
                if (Framing.GetTileSafely(x + 1, y - 1).HasTile) continue;
                if (Framing.GetTileSafely(x, y - 2).HasTile) continue;
                if (Framing.GetTileSafely(x + 1, y - 2).HasTile) continue;

                if (alreadyPlaced.Contains(pt) || alreadyPlaced.Contains(right)) continue;

                if (WorldGen.genRand.NextBool(2))
                {
                    int potStyle = WorldGen.genRand.Next(3);
                    int frameX = potStyle * 36;
                    int frameY = 0;

                    int originX = x;
                    int originY = y - 2;

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            int tileX = originX + i;
                            int tileY = originY + j;

                            if (!WorldGen.InWorld(tileX, tileY)) continue;

                            Tile t = Framing.GetTileSafely(tileX, tileY);
                            t.HasTile = true;
                            t.TileType = (ushort)abyssalPotID;
                            t.TileFrameX = (short)(frameX + i * 18);
                            t.TileFrameY = (short)(frameY + j * 18);
                        }
                    }

                    alreadyPlaced.Add(pt);
                    alreadyPlaced.Add(right);
                }
            }
        }

        private void PlacePillars(List<Point> placedTiles)
        {
            int[] pillarTileTypes = new int[]
            {
                ModContent.TileType<global::TalesoftheEntropicSea.Content.Tiles.Pillar1>(),
                ModContent.TileType<global::TalesoftheEntropicSea.Content.Tiles.Pillar2>(),
                ModContent.TileType<global::TalesoftheEntropicSea.Content.Tiles.Pillar3>()
            };

            HashSet<Point> alreadyPlaced = new();

            foreach (Point pt in placedTiles)
            {
                int x = pt.X;
                int y = pt.Y;

                Point p1 = new(x - 1, y);
                Point p2 = new(x, y);
                Point p3 = new(x + 1, y);

                if (!placedTiles.Contains(p1) || !placedTiles.Contains(p2) || !placedTiles.Contains(p3))
                    continue;

                if (alreadyPlaced.Contains(p1) || alreadyPlaced.Contains(p2) || alreadyPlaced.Contains(p3))
                    continue;

                bool spaceClear = true;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = 1; dy <= 6; dy++)
                    {
                        int checkX = x + dx;
                        int checkY = y - dy;
                        if (!WorldGen.InWorld(checkX, checkY) || Framing.GetTileSafely(checkX, checkY).HasTile)
                        {
                            spaceClear = false;
                            break;
                        }
                    }
                    if (!spaceClear) break;
                }

                if (!spaceClear) continue;

                if (WorldGen.genRand.NextFloat() > 0.20f) continue;

                int pillarType = WorldGen.genRand.Next(pillarTileTypes.Length);
                int tileType = pillarTileTypes[pillarType];

                int originX = x - 1;
                int originY = y - 5;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        int tileX = originX + i;
                        int tileY = originY + j;

                        if (!WorldGen.InWorld(tileX, tileY)) continue;

                        Tile t = Framing.GetTileSafely(tileX, tileY);
                        t.HasTile = true;
                        t.TileType = (ushort)tileType;
                        t.TileFrameX = (short)(i * 18);
                        t.TileFrameY = (short)(j * 18);
                    }
                }

                alreadyPlaced.Add(p1);
                alreadyPlaced.Add(p2);
                alreadyPlaced.Add(p3);
            }
        }

        private void GenerateSpikyVoidstoneWall(int baseX, int baseY, int spikeWidth, int spikeHeight)
        {
            int voidstoneWallID = ModContent.WallType<VoidstoneWall>();
            List<Point> wallPoints = new();

            int startY = baseY - spikeHeight / 2;

            for (int x = -spikeWidth; x <= spikeWidth; x++)
            {
                float widthRatio = Math.Abs(x) / (float)spikeWidth;
                int height = (int)(spikeHeight * (1f - widthRatio));

                for (int y = 0; y < height; y++)
                {
                    int worldX = baseX + x;
                    int worldY = startY + y;

                    if (!WorldGen.InWorld(worldX, worldY)) continue;

                    Tile tile = Framing.GetTileSafely(worldX, worldY);
                    if (!tile.HasTile)
                    {
                        tile.HasTile = true;
                        tile.TileType = TileID.Dirt;
                    }

                    wallPoints.Add(new Point(worldX, worldY));
                }
            }

            foreach (var pt in wallPoints)
            {
                Tile tile = Framing.GetTileSafely(pt.X, pt.Y);
                tile.WallType = (ushort)voidstoneWallID;
            }

            foreach (var pt in wallPoints)
            {
                Tile tile = Framing.GetTileSafely(pt.X, pt.Y);
                if (tile.HasTile && tile.TileType == TileID.Dirt)
                    tile.HasTile = false;
            }
        }
    }
}
