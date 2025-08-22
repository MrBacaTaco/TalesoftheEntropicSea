using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TalesoftheEntropicSea.Content.Buffs;

namespace TalesoftheEntropicSea.Common.DrawLayers
{
    public class ThalassophobiaPlayerTint : PlayerDrawLayer
    {
        
        const int LAYERS = 4;       
        const float AMP_MIN = 1.5f;    
        const float AMP_MAX = 5.0f;    
        const float SPEED_MIN = 0.7f;   
        const float SPEED_MAX = 1.7f;    
        static readonly Color BASE = new Color(22, 122, 115); 

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            var p = drawInfo.drawPlayer;
            return drawInfo.shadow == 0f && p.active && p.HasBuff(ModContent.BuffType<ThalassophobiaDebuff>());
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var p = drawInfo.drawPlayer;
            float t = (float)Main.GlobalTimeWrappedHourly; 

            
            Func<float, float> wob = y => (float)Math.Sin(t * 4f + y * 0.02f) * 2f;

            for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
            {
                var dd = drawInfo.DrawDataCache[i];
                if (dd.texture == null) continue;

                
                Vector2 baseWobble = new Vector2(wob(dd.position.Y), 0f);

                
                for (int k = 0; k < LAYERS; k++)
                {
                   
                    int seed = Hash3(p.whoAmI, i, k);

                   
                    Vector2 dir = Unit(seed * 73471);
                    float amp = Lerp(AMP_MIN, AMP_MAX, Frac(seed * 12347));
                    float hz = Lerp(SPEED_MIN, SPEED_MAX, Frac(seed * 91291));
                    float ph = Frac(seed * 45161) * MathHelper.TwoPi;

                    
                    float s = (float)Math.Sin((t * MathHelper.TwoPi * hz) + ph);
                    Vector2 drift = dir * (amp * s);

                    
                    float scaleMul = 1f + 0.005f * (k + 1);

                    
                    byte a = (byte)MathHelper.Clamp(160 - 28 * k, 0, 255);
                    Color tint = new Color(BASE.R, BASE.G, BASE.B, a);

                    var overlay = dd;
                    overlay.position += baseWobble + drift;
                    overlay.scale *= scaleMul;
                    overlay.color = tint;

                    overlay.Draw(Main.spriteBatch);
                }
            }
        }

        
        static int Hash3(int a, int b, int c)
        {
            unchecked
            {
                int h = 0x2C1B3C6D;
                h = (h ^ a) * 0x297A2D39;
                h = (h ^ b) * 0x1B56C4E9;
                h = (h ^ c) * 0x2C1B3C6D;
                h ^= (h >> 13);
                return h;
            }
        }
        static float Frac(int x) => (x & 0x7FFFFFFF) / 2147483647f; // [0,1)
        static float Lerp(float a, float b, float t) => a + (b - a) * MathHelper.Clamp(t, 0f, 1f);
        static Vector2 Unit(int seed)
        {
            // map to unit circle
            float ang = Frac(seed) * MathHelper.TwoPi;
            return new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang));
        }
    }
}
