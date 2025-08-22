using Luminance.Core.Graphics; // <-- important for ManagedShader
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using TalesoftheEntropicSea.Common;
using TalesoftheEntropicSea.Content.World;
using TalesofttheEntropicSea.Common;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;


namespace TalesoftheEntropicSea
{
    public class TalesoftheEntropicSea : Mod
    {
        public static Effect NightFractalBeamShader; // make sure this is here

        public static Effect AbyssSkylineShader;

        public static Effect ShockwaveEffect;


        public override void Load()
        {
            if (!Main.dedServ)
            {
                try
                {
                    NightFractalBeamShader = ModContent.Request<Effect>(
                        "TalesoftheEntropicSea/Effects/NightFractalBeamShader",
                        AssetRequestMode.ImmediateLoad
                    ).Value;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load NightFractalBeamShader: {ex}");
                }


                ShockwaveEffect = ModContent.Request<Effect>(
                    "TalesoftheEntropicSea/Effects/Shockwave",
                    AssetRequestMode.ImmediateLoad
                ).Value;


                AbyssSkylineShader = ModContent.Request<Effect>(
                    "TalesoftheEntropicSea/Effects/AbyssSkylineParticle",
                    AssetRequestMode.ImmediateLoad
                ).Value;
            }
        }


        public override void Unload()
        {
            NightFractalBeamShader = null;
        }
    }
}
