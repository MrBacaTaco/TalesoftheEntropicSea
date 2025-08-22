using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using NoxusBoss.Core.World.WorldSaving; 
using NoxusBoss.Content.NPCs.Bosses.Avatar.FirstPhaseForm; 

namespace TalesoftheEntropicSea.Content.World
{
    /// <summary>
    /// Handles triggering the Sky Abyss biome generation after a specific condition. Currently incorrect - triggers on avatars 2nd phase
    /// </summary>
    public class SkyAbyssTrigger : ModSystem
    {
        public static bool SkyAbyssGenerated;

        public override void OnWorldLoad()
        {
            SkyAbyssGenerated = false;
        }

        public override void OnWorldUnload()
        {
            SkyAbyssGenerated = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["SkyAbyssGenerated"] = SkyAbyssGenerated;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            SkyAbyssGenerated = tag.GetBool("SkyAbyssGenerated");
        }

        public override void PostUpdateWorld()
        {

            if (!SkyAbyssGenerated && BossDownedSaveSystem.HasDefeated<AvatarRift>())
            {
                SkyAbyssGenerated = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {

                    ModContent.GetInstance<SkyAbyssIslandsGen>().GenerateSkyAbyssIslands();
                }

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText("The Sky Abyss has emerged...", Color.MediumPurple);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    Terraria.Chat.ChatHelper.BroadcastChatMessage(
                        NetworkText.FromLiteral("The Sky Abyss has emerged..."),
                        Color.MediumPurple
                    );
                }
            }
        }
    }
}
