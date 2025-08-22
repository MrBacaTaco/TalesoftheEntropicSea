using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Common.Class
{
    public class TalesGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff(ModContent.BuffType<ThalassophobiaDebuff>()))
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 3000;
                if (damage < 1000)
                    damage = 1000;
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!npc.active) return;
            if (!npc.HasBuff(ModContent.BuffType<ThalassophobiaDebuff>())) return;

            Texture2D icon = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Assets/Buffs/ThalassophobiaDebuff"
            ).Value;

            
            const float scale = 0.5f;

            
            Vector2 origin = icon.Size() * 0.5f;
            float yOffset = 10f; 
            Vector2 worldPos = npc.Top + new Vector2(0f, -yOffset);
            worldPos.Y += npc.gfxOffY; 

            Vector2 drawPos = worldPos - screenPos;

            
            spriteBatch.Draw(icon, drawPos + new Vector2(1f, 1f), null,
                new Color(0, 0, 0, 160), 0f, origin, scale, SpriteEffects.None, 0f);

            
            spriteBatch.Draw(icon, drawPos, null,
                Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
        }




    }
}



