using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Items
{
    public class EpilogueItem : ModItem
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Items/EpilogueItem";

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.rare = ModContent.RarityType<OceanBloomRarity>();
            Item.value = Item.sellPrice(silver: 50);
            Item.maxStack = 1;

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string shortLore = "Whispers from the depths echo in your mind...";
            string longLore = "\"The Tides Beyond\"\n\nThe final echo of the Heart's fall rippled across the seas.\nIts beat - once a drum of dominion - was silenced,\nleaving behind the hush of a world ready to breathe again.\n\nFrom the corpse of the Dark One,\nentropy seeped into the heavens and earth alike.\nWhere it touched the sky, new realms bloomed -\nthe Sky Abyss, where starlight dances upon endless waters;\nand the Wisteria Groves, where violet blooms sway in winds born of the void.\n\nThe old scars of calamity still lingered upon the world:\nthe lands warped by the Devourer's hunger,\nthe skies once pierced by the Astral corruption,\nthe seas still haunted by whispers of what came before.\nBut the deep had changed. The abyss was no longer a maw;\nit was a cradle.\n\nAs the tides rose anew,\ncorals bloomed where voidstone once lay barren,\nlight filtered through waters that had forgotten its warmth,\nand the songs of creatures thought lost\nwove themselves into the wind.\n\nYet in that beauty lay the truth the Dark Ones had known:\nevery beginning is born of an ending,\nand every ending stirs the tides of something yet unseen.\n\nAnd the story of this world is almost complete.";

            tooltips.Add(new TooltipLine(Mod, "LoreShort", shortLore)
            {
                OverrideColor = new Color(255, 220, 60)
            });

            if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
            {
               
                string[] loreLines = longLore.Split('\n');
                foreach (string line in loreLines)
                {
                    tooltips.Add(new TooltipLine(Mod, "LoreLongGradient", line));
                }
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "LoreHint", "Press \"Left Shift\" to listen closer")
                {
                    OverrideColor = new Color(160, 160, 160)
                });
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "LoreLongGradient")
            {
                var font = line.Font ?? Terraria.GameContent.FontAssets.MouseText.Value;
                float time = (float)Main.GameUpdateCount / 60f;

                Color[] colors = new Color[]
                {
            new Color(200, 150, 255), 
            new Color(120, 60, 200),  
            new Color(140, 200, 255), 
            new Color(60, 100, 200)   
                };

                int waveSize = 20; 
                Vector2 pos = new Vector2(line.X, line.Y);

                for (int i = 0; i < line.Text.Length; i++)
                {
                    string charStr = line.Text[i].ToString();

                    float wave = (float)Math.Sin((time * 2f) + (i * 0.3f)) * 0.5f + 0.5f;
                    Color c1 = colors[(i / waveSize) % colors.Length];
                    Color c2 = colors[(i / waveSize + 1) % colors.Length];
                    Color final = Color.Lerp(c1, c2, wave);

                    Main.spriteBatch.DrawString(font, charStr, pos, final, 0f, Vector2.Zero, line.BaseScale, SpriteEffects.None, 0f);
                    pos.X += font.MeasureString(charStr).X;
                }

                return false;
            }

            return true;
        }


        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/Items/EpilogueItem").Value;
            Vector2 drawPosition = Item.Center - Main.screenPosition;
            Vector2 origin = glowTexture.Size() * 0.5f;

            spriteBatch.Draw(
                glowTexture,
                drawPosition,
                null,
                Color.White, 
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }


        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            gravity = 0f; 
            maxFallSpeed = 0f; 
            Item.velocity.Y = 0f; 
        }



    }
}

