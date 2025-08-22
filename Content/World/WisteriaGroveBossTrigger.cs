

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TalesoftheEntropicSea.Content.UI;
using NoxusBoss.Content.NPCs.Bosses.Avatar.SecondPhaseForm;


namespace TalesoftheEntropicSea.Content.World
{
    public class WisteriaGroveBossTrigger : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            //if (npc.type == NPCID.KingSlime)
            if (npc.type == ModContent.NPCType<AvatarOfEmptiness>())
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    BossMessageUI.QueueMessage("Entropy permeates this world.", BossMessageUI.GradLightPurpleDarkPurple, 300);
                    BossMessageUI.QueueMessage("The flowers of chaos have blossomed.", BossMessageUI.GradCurrent, 300);
                    BossMessageUI.QueueMessage("Chunks of the abyss have risen into the skies.", BossMessageUI.GradLightBlueDarkBlue, 300);

                    ModContent.GetInstance<WisteriaGroveGen>().GenerateWisteriaGroves();
                }
            }

        }
    }
}
