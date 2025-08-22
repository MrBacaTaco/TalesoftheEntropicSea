using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TalesoftheEntropicSea.Content.NPCs.GABoss
{
    public class MiddleWingLeft : ModNPC
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            NPC.width = 336;  
            NPC.height = 322;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 9999;
            NPC.dontTakeDamage = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
        }

        public override void AI()
        {
            
            NPC.timeLeft = 600;
            

            int followID = (int)NPC.ai[1];
            if (!Main.npc.IndexInRange(followID) || !Main.npc[followID].active)
            {
                NPC.active = false;
                return;
            }

            NPC parent = Main.npc[followID];

            int side = (int)NPC.ai[2]; 

            
            Vector2 offset = new Vector2(185f, -45f) * side;
            Vector2 rotatedOffset = offset.RotatedBy(parent.rotation);

            NPC.Center = parent.Center + rotatedOffset;
            NPC.rotation = parent.rotation;
            NPC.spriteDirection = side;
        }


        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;

        /*
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            Texture2D glowTexture = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/NPCs/GABoss/MiddleWingLeft"
            ).Value;

            Rectangle frame = NPC.frame; // Uses same frame as main texture
            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(
                glowTexture,
                NPC.Center - screenPos,
                frame,
                Color.White, // No lighting applied
                NPC.rotation,
                origin,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }
        */
    }


}
