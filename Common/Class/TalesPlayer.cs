using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;

using TalesoftheEntropicSea.Content.Buffs; 
using TalesoftheEntropicSea.Content.UI;
using TalesoftheEntropicSea.World;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.WorldBuilding;




namespace TalesoftheEntropicSea.Common
{
    public class TalesPlayer : ModPlayer
    {
        public int keplerCooldown;

        public override void ResetEffects()
        {
            if (keplerCooldown > 0)
                keplerCooldown--;
        }

        public override void UpdateBadLifeRegen()
        {
            if (Player.HasBuff(ModContent.BuffType<ThalassophobiaDebuff>()))
            {
                
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                // -60 HP/sec
                Player.lifeRegen -= 120;
            }
        }
        public override void OnEnterWorld()
        {
            


        }






        public bool ZoneSkyAbyss;

        public override void PreUpdate()
        {
            int tileX = (int)(Player.Center.X / 16f);
            int tileY = (int)(Player.Center.Y / 16f);

            int centerX = SkyAbyssIslandsGen.SkyAbyssCenterX;
            int centerY = SkyAbyssIslandsGen.SkyAbyssCenterY;
            int radius = SkyAbyssIslandsGen.SkyAbyssRadius;

            ZoneSkyAbyss =
                Math.Abs(tileX - centerX) <= radius &&
                Math.Abs(tileY - centerY) <= radius;
        }
        


    }
}
