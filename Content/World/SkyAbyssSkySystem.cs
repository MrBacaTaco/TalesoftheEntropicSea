using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.World
{
    public class SkyAbyssSkySystem : ModSystem
    {
        public override void Load()
        {
            if (Main.dedServ) 
                return;

            SkyManager.Instance["TalesoftheEntropicSea:SkyAbyssSky"] = new SkyAbyssSky();
            SkyManager.Instance["TalesoftheEntropicSea:SkyAbyssMediumSky"] = new SkyAbyssMediumSky();
            SkyManager.Instance["TalesoftheEntropicSea:SkyAbyssCloseSky"] = new SkyAbyssCloseSky();
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            
            SkyManager.Instance.Deactivate("TalesoftheEntropicSea:SkyAbyssSky");
            SkyManager.Instance.Deactivate("TalesoftheEntropicSea:SkyAbyssMediumSky");
            SkyManager.Instance.Deactivate("TalesoftheEntropicSea:SkyAbyssCloseSky");
        }
    }
}
