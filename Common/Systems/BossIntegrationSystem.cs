using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.NPCs.HeartOfTheOcean;
using Terraria.ModLoader;
using TalesoftheEntropicSea.Content.NPCs;
using TalesoftheEntropicSea.Content.Items;

namespace TalesoftheEntropicSea.Common.Systems
{
    public class BossIntegrationSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            DoBossChecklistIntegration();
        }

        private void DoBossChecklistIntegration()
        {
            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
                return;

            if (bossChecklistMod.Version < new Version(1, 6))
                return;

           
            string internalName = "HeartOfTheOcean";

            
            float weight = 27.5f; // check weight list online

            
            Func<bool> downed = () => DownedBossSystem.downedHeartOfTheOcean;

           
            int bossType = ModContent.NPCType<HeartOfTheOcean>();

            
            int spawnItem = ModContent.ItemType<SuspiciousTrident>();

            
            List<int> collectibles = new List<int>() {
                ModContent.ItemType<EpilogueItem>()
            };

            
            var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>(
                    "TalesoftheEntropicSea/Content/NPCs/HeartOfTheOcean/HeartOfTheOcean"
                ).Value;


                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    ["customPortrait"] = customPortrait
                }
            );
        }
    }
}
