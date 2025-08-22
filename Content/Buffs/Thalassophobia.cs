using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;


namespace TalesoftheEntropicSea.Content.Buffs
{
    public class ThalassophobiaDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; 
            Main.pvpBuff[Type] = true; 
            Main.buffNoSave[Type] = false; 
            Main.buffNoTimeDisplay[Type] = false; 
        }

        public override string Texture => "TalesoftheEntropicSea/Assets/Buffs/ThalassophobiaDebuff";


    }
}
