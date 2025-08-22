using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TalesoftheEntropicSea.Common.Systems
{
    public class DownedBossSystem : ModSystem
    {
        
        public static bool downedHeartOfTheOcean;

        public override void OnWorldLoad()
        {
            downedHeartOfTheOcean = false;
        }

        public override void OnWorldUnload()
        {
            downedHeartOfTheOcean = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedHeartOfTheOcean)
                tag["downedHeartOfTheOcean"] = true;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedHeartOfTheOcean = tag.ContainsKey("downedHeartOfTheOcean");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte
            {
                [0] = downedHeartOfTheOcean
            };
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedHeartOfTheOcean = flags[0];
        }
    }
}
