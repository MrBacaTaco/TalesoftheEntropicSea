using SubworldLibrary;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.UI;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace TalesoftheEntropicSea.World
{
    public class SkyAbyssSubworld : Subworld
    {
        public override int Width => 1000;
        public override int Height => 500;

        public override List<GenPass> Tasks => new()
        {
            new PassLegacy("Sky Abyss Gen", Generate)
        };

        private void Generate(GenerationProgress progress, GameConfiguration _)
        {
            progress.Message = "Generating the Sky Abyss...";

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Main.tile[x, y].ClearEverything();

            Main.worldSurface = Height;
            Main.rockLayer = Height;



            SubworldSkyAbyssGen.GenerateIslands(Width, Height);
            SubworldSkyAbyssGen.FloodSubworldWithVoidWater(this.Width, this.Height);

        }






        public override void OnEnter()
        {
            Main.NewText("Entering the Sky Abyss...");

            if (Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient) 
            {
                SkyAbyssIntroUI.ShowMessage(
                    new string[] { "Hadal Zone", "Sky Abyss" },
                    300 
                );
            }
        }

        public override void OnExit() => Main.NewText("Returning to the surface...");
    }
}
