using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Walls;
using CalamityMod.Waters;

namespace TalesoftheEntropicSea.Content.World
{
    public static class SubworldSkyAbyssGen
    {
        public static void GenerateIslands(int width, int height)
        {
            int centerX = width / 2;
            int centerY = (int)(height * 0.4f); 

            GenerateIsland(centerX, centerY, 24, 1.2f); 
            int ringCount = 8;
            float startDistance = 100f;
            float ringSpacing = 80f;

            float baseCircumference = 2 * MathF.PI * startDistance;
            int baseIslands = 40; 

            for (int ring = 0; ring < ringCount; ring++)
            {
                float ringDistance = startDistance + ring * ringSpacing;
                float ringCircumference = 2 * MathF.PI * ringDistance;

                int islandCount = (int)(baseIslands * (ringCircumference / baseCircumference));

                for (int i = 0; i < islandCount; i++)
                {
                    float angle = WorldGen.genRand.NextFloat(MathHelper.TwoPi);
                    float distance = WorldGen.genRand.NextFloat(ringDistance, ringDistance + ringSpacing * 0.8f);

                    int x = centerX + (int)(MathF.Cos(angle) * distance);
                    int y = centerY + (int)(MathF.Sin(angle) * distance * 0.5f); 

                    int size = WorldGen.genRand.Next(8, 18);
                    float sharpness = WorldGen.genRand.NextFloat(1.2f, 1.8f);

                    GenerateIsland(x, y, size, sharpness);
                }
            }

            GenerateDebrisIslands(centerX, centerY);
        }



        private static void GenerateDebrisIslands(int centerX, int centerY)
        {
            int maxRange = 250;
            int minRange = 80;
            int debrisAttempts = 250;

            int voidstoneID = ModContent.TileType<Tiles.SkyAbyssStone>();
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

        private static void GenerateDebrisShape(int startX, int startY, int size, int tileType, int wallType)
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

        private static void GenerateIsland(int startX, int startY, int size, float sharpness)
        {
            List<Point> placed = new();
            int voidstoneID = ModContent.TileType<Tiles.SkyAbyssStone>();
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
            GenerateOreVeins(placed);
            ReplaceTopLayerWithSandSlabs(placed);
            DropRandomWaterAboveIsland(placed);
            PlaceAbyssalPots(placed);
            PlacePillars(placed);
            





        }

        private static void DropRandomWaterAboveIsland(List<Point> islandBlocks)
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

        private static void PlaceAbyssalPots(List<Point> placedTiles)
        {
            int abyssalPotID = ModContent.TileType<AbyssalPots>();
            HashSet<Point> alreadyPlaced = new();

            foreach (Point pt in placedTiles)
            {
                int x = pt.X;
                int y = pt.Y;
                Point right = new(x + 1, y);
                if (!placedTiles.Contains(right)) continue;

                if (Framing.GetTileSafely(x, y - 1).HasTile ||
                    Framing.GetTileSafely(x + 1, y - 1).HasTile ||
                    Framing.GetTileSafely(x, y - 2).HasTile ||
                    Framing.GetTileSafely(x + 1, y - 2).HasTile)
                    continue;

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

        private static void PlacePillars(List<Point> placedTiles)
        {
            int[] pillarTileTypes = new int[]
            {
                ModContent.TileType<Pillar1>(),
                ModContent.TileType<Pillar2>(),
                ModContent.TileType<Pillar3>()
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
                if (WorldGen.genRand.NextFloat() > 0.2f) continue;

                int tileType = pillarTileTypes[WorldGen.genRand.Next(pillarTileTypes.Length)];
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

        private static void GenerateSpikyVoidstoneWall(int baseX, int baseY, int spikeWidth, int spikeHeight)
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
                Framing.GetTileSafely(pt.X, pt.Y).WallType = (ushort)voidstoneWallID;

            foreach (var pt in wallPoints)
            {
                Tile tile = Framing.GetTileSafely(pt.X, pt.Y);
                if (tile.HasTile && tile.TileType == TileID.Dirt)
                    tile.HasTile = false;
            }
        }

        private static void GenerateOreVeins(List<Point> islandTiles)
        {
            int oreType = ModContent.TileType<Tiles.ChallengerRock>();

            int veinAttempts = islandTiles.Count / 180; 
            for (int i = 0; i < veinAttempts; i++)
            {
                Point origin = islandTiles[WorldGen.genRand.Next(islandTiles.Count)];
                int veinSize = WorldGen.genRand.Next(30, 50); 

                HashSet<Point> veinPoints = new();
                Queue<Point> toCheck = new();
                toCheck.Enqueue(origin);
                veinPoints.Add(origin);

                while (veinPoints.Count < veinSize && toCheck.Count > 0)
                {
                    Point current = toCheck.Dequeue();

                    for (int d = 0; d < 4; d++)
                    {
                        Point offset = current + WorldGen.genRand.NextVector2Circular(3, 2).ToPoint();
                        if (!WorldGen.InWorld(offset.X, offset.Y)) continue;
                        if (veinPoints.Contains(offset)) continue;

                        Tile tile = Framing.GetTileSafely(offset.X, offset.Y);
                        if (tile.HasTile && tile.TileType == ModContent.TileType<Tiles.SkyAbyssStone>())
                        {
                            veinPoints.Add(offset);
                            toCheck.Enqueue(offset);

                            // Set ore
                            tile.TileType = (ushort)oreType;
                        }

                        if (veinPoints.Count >= veinSize)
                            break;
                    }
                }
            }
        }

        private static void ReplaceTopLayerWithSandSlabs(List<Point> islandTiles)
        {
            int sandType = ModContent.TileType<Tiles.SkyAbyssSand>();
            int stoneType = ModContent.TileType<Tiles.SkyAbyssStone>();

            
            Dictionary<int, int> surfaceYAtX = new(); 

            foreach (Point pt in islandTiles)
            {
                Point above = new Point(pt.X, pt.Y - 1);
                if (!Framing.GetTileSafely(above).HasTile)
                {
                    if (!surfaceYAtX.ContainsKey(pt.X) || pt.Y < surfaceYAtX[pt.X])
                    {
                        surfaceYAtX[pt.X] = pt.Y;
                    }
                }
            }

            if (surfaceYAtX.Count < 20)
                return;

            List<int> xList = new(surfaceYAtX.Keys);
            xList.Sort();

            
            int slabCount = WorldGen.genRand.Next(1, 4);

            for (int i = 0; i < slabCount; i++)
            {
                int startIndex = WorldGen.genRand.Next(0, xList.Count - 10);
                int slabWidth = WorldGen.genRand.Next(10, Math.Min(21, xList.Count - startIndex));

                for (int j = 0; j < slabWidth; j++)
                {
                    int x = xList[startIndex + j];
                    int y = surfaceYAtX[x];

                    
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.HasTile && tile.TileType == stoneType)
                    {
                        tile.TileType = (ushort)sandType;
                    }

                    
                    if (WorldGen.genRand.NextBool())
                    {
                        Tile below = Framing.GetTileSafely(x, y + 1);
                        if (below.HasTile && below.TileType == stoneType)
                        {
                            below.TileType = (ushort)sandType;
                        }
                    }
                }
            }
        }

        public static void FloodSubworldWithVoidWater(int width, int height)
        {
            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < height; y++) 
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile)
                    {
                        tile.LiquidAmount = 255;
                        tile.LiquidType = LiquidID.Water;
                    }
                }
            }
        }
    }
}
