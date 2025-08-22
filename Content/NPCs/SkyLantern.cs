using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using TalesoftheEntropicSea.Content.Buffs;
using TalesoftheEntropicSea.Content.World;
using TalesoftheEntropicSea.World;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.NPCs
{
    public class SkyLantern : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3; 
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 50;
            NPC.damage = 120;
            NPC.defense = 100;
            NPC.lifeMax = 40000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 80f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = 16;         // 16 - Swimming/Fish AI, 14 - bat ai.
                                      

            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ThalassophobiaDebuff>()] = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10) 
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 180);
        }


        public override void OnKill()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbyssDrown"), NPC.Center);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            
            if (SubworldLibrary.SubworldSystem.Current is SkyAbyssSubworld)
            {
                return 1.4f; 
            }

            return 0f; 
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            
            npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ItemID.GoldCoin, 1, 25, 35));
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/NPCs/SkyLantern_glow"
            ).Value;

            int frameHeight = glowmask.Height / Main.npcFrameCount[NPC.type];
            Rectangle frame = new Rectangle(0, NPC.frame.Y, glowmask.Width, frameHeight);

            Vector2 origin = frame.Size() / 2f;
            Vector2 drawPos = NPC.Center - screenPos;

            spriteBatch.Draw(
                glowmask,
                drawPos,
                frame,
                Color.White, 
                NPC.rotation,
                origin,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }
    }
}
