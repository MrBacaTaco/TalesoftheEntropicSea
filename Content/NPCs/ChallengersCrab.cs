using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.World;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using SubworldLibrary;
using TalesoftheEntropicSea.Content.World; 
using TalesoftheEntropicSea.Content.Buffs;


namespace TalesoftheEntropicSea.Content.NPCs
{
    public class ChallengersCrab : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3; 
        }

        public override void SetDefaults()
        {
            NPC.width = 104;
            NPC.height = 76;
            NPC.damage = 200;
            NPC.defense = 100;
            NPC.lifeMax = 25000;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.value = 120f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = 3;                // Fighter (Zombie) AI
            AIType = NPCID.Crab;          

            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ThalassophobiaDebuff>()] = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter < 10)
                NPC.frame.Y = 0;
            else if (NPC.frameCounter < 20)
                NPC.frame.Y = frameHeight;
            else if (NPC.frameCounter < 30)
                NPC.frame.Y = frameHeight * 2;
            else
                NPC.frameCounter = 0;
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

        public override void OnKill()
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbyssDrown"), NPC.Center);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/NPCs/ChallengersCrab_glow"
            ).Value;

            int frameHeight = glowmask.Height / Main.npcFrameCount[NPC.type];
            Rectangle frame = new Rectangle(0, NPC.frame.Y, glowmask.Width, frameHeight);

            Vector2 origin = frame.Size() / 2f;
            Vector2 drawPos = NPC.Center - Main.screenPosition;

            Main.EntitySpriteDraw(
                glowmask,
                drawPos,
                frame,
                Color.White,
                NPC.rotation,
                origin,
                NPC.scale,
                NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0
            );
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 180);
        }

    }
}
