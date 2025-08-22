using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; 
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent; 
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.NPCs
{
    public class WyrmHead : ModNPC
    {
        private const int SegmentCount = 40;
        private bool tailSpawned = false;

        private enum WyrmAttackState
        {
            Transition = 0,
            Charging = 1,
            Dash = 2,
            Circle = 3
        }

        private const int AttackCount = 3; 

        public override void SetDefaults()
        {
            NPC.width = 430;
            NPC.height = 222;
            NPC.damage = 230;
            NPC.defense = 400;
            NPC.lifeMax = 999999999;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;

            NPC.ai[0] = (float)WyrmAttackState.Transition;
            NPC.localAI[0] = 0; 
            NPC.alpha = 255;
        }

        public override void AI()
        {

            if (Main.netMode != NetmodeID.MultiplayerClient && !tailSpawned)
            {
                int prev = NPC.whoAmI;

                for (int i = 0; i < SegmentCount; i++)
                {
                    int bodyType = (i % 2 == 0)
                        ? ModContent.NPCType<WyrmBody>()
                        : ModContent.NPCType<WyrmBodyAlt>();

                    int body = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, bodyType, NPC.whoAmI);
                    Main.npc[body].realLife = NPC.whoAmI;
                    Main.npc[body].ai[1] = prev;
                    Main.npc[body].ai[2] = NPC.whoAmI;
                    Main.npc[prev].ai[0] = body;
                    Main.npc[body].netUpdate = true;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, body);
                    prev = body;
                }

                int tail = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WyrmTail>(), NPC.whoAmI);
                Main.npc[tail].realLife = NPC.whoAmI;
                Main.npc[tail].ai[1] = prev;
                Main.npc[tail].ai[2] = NPC.whoAmI;
                Main.npc[prev].ai[0] = tail;
                Main.npc[tail].netUpdate = true;
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, tail);

                tailSpawned = true;

                if (NPC.ai[3] > 0)
                {
                    NPC.ai[3]--;
                    if (NPC.ai[3] <= 0)
                    {
                        NPC.active = false;
                        return;
                    }
                }

                NPC.alpha = 0; 
                NPC.ai[0] = (float)WyrmAttackState.Charging; 
                NPC.ai[1] = 0; 

                Player target1 = Main.player[NPC.target];
                Vector2 dir = (target1.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                NPC.velocity = dir * 8f;
                NPC.rotation = dir.ToRotation() + MathHelper.PiOver2;
                NPC.netUpdate = true;
            }


            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            if (!target.active || target.dead)
            {
                NPC.velocity.Y += 0.1f;
                return;
            }

            switch ((WyrmAttackState)NPC.ai[0])
            {
                case WyrmAttackState.Transition:
                    DoTransition();
                    break;
                case WyrmAttackState.Charging:
                    DoCharging(target);
                    break;
                case WyrmAttackState.Dash:
                    DoDash(target);
                    break;
                case WyrmAttackState.Circle:
                    DoCircle(target);
                    break;
            }
        }

        private void GoToTransition()
        {
            NPC.localAI[0] = (NPC.localAI[0] + 1) % AttackCount; 
            NPC.ai[0] = (float)WyrmAttackState.Transition;
            NPC.ai[1] = 0;
            NPC.velocity = Vector2.Zero;
            NPC.netUpdate = true;
        }

        private void DoTransition()
        {
            NPC.velocity *= 0.9f;
            NPC.ai[1]++;

            if (NPC.alpha < 255)
            {
                NPC.alpha += 10;
                if (NPC.alpha > 255) NPC.alpha = 255;
            }

            if (NPC.alpha == 255 && NPC.ai[1] > 30)
            {
                switch ((int)NPC.localAI[0])
                {
                    case 0: NPC.ai[0] = (float)WyrmAttackState.Charging; break;
                    case 1: NPC.ai[0] = (float)WyrmAttackState.Dash; break;
                    case 2: NPC.ai[0] = (float)WyrmAttackState.Circle; break;
                }
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
            }
        }

        private Vector2 GetOffscreenTarget(Player target, float distance)
        {
            Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
            return target.Center + dir * distance;
        }

        private void FadeIn()
        {
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 12;
                if (NPC.alpha < 0) NPC.alpha = 0;
            }
        }

        private void DoCharging(Player target)
        {
            if (NPC.ai[1] == 0)
            {
                NPC.alpha = 255;
                NPC.Center = GetOffscreenTarget(target, -800f);
                NPC.velocity = Vector2.Zero;

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PrimordialWyrmCharge"), NPC.Center);
            }

            FadeIn();
            NPC.ai[1]++;

            if (NPC.ai[1] < 30)
            {
                Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                NPC.rotation = dir.ToRotation() + MathHelper.PiOver2;
                NPC.velocity *= 0.85f;
            }
            else if (NPC.ai[1] == 30)
            {
                Vector2 dashTarget = GetOffscreenTarget(target, 1400f);
                Vector2 dashDir = (dashTarget - NPC.Center).SafeNormalize(Vector2.UnitY);
                NPC.velocity = dashDir * 32f;
                NPC.rotation = dashDir.ToRotation() + MathHelper.PiOver2;
            }

            if (NPC.ai[1] > 180) 
            {
                GoToTransition();
            }
        }

        private void DoDash(Player target)
        {
            if (NPC.ai[1] == 0)
            {
                NPC.alpha = 255;
                NPC.Center = GetOffscreenTarget(target, -900f);
                NPC.velocity = Vector2.Zero;

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PrimordialWyrmCharge"), NPC.Center);
            }

            FadeIn();
            NPC.ai[1]++;

            if (NPC.ai[1] == 20)
            {
                Vector2 dashTarget = GetOffscreenTarget(target, 1600f);
                Vector2 dashDir = (dashTarget - NPC.Center).SafeNormalize(Vector2.UnitY);
                NPC.velocity = dashDir * 36f;
                NPC.rotation = dashDir.ToRotation() + MathHelper.PiOver2;
            }

            if (NPC.ai[1] > 180)
            {
                GoToTransition();
            }
        }

        private void DoCircle(Player target)
        {
            if (NPC.ai[1] == 0)
            {
                NPC.alpha = 255;
                NPC.Center = GetOffscreenTarget(target, 600f);

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PrimordialWyrmCharge"), NPC.Center);
            }

            FadeIn();
            NPC.ai[1]++;

            const float radius = 520f;
            const int circleTime = 75;

            if (NPC.ai[1] == 0)
            {
                NPC.ai[2] = (target.Center - NPC.Center).ToRotation();
            }

            float angle = (float)NPC.ai[2] + (NPC.ai[1] / (float)circleTime) * MathHelper.TwoPi;
            Vector2 circlePos = target.Center + radius * angle.ToRotationVector2();
            Vector2 moveDir = circlePos - NPC.Center;
            float speed = 20f;
            if (moveDir.Length() > speed)
                moveDir = moveDir.SafeNormalize(Vector2.Zero) * speed;
            NPC.velocity = moveDir;

            if (NPC.velocity.Length() > 0.1f)
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            if (NPC.ai[1] > circleTime + 70)
            {
                GoToTransition();
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/NPCs/WyrmHead_glow").Value;

            Rectangle frame = NPC.frame;

            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(
                glowTexture,
                NPC.Center - screenPos, 
                frame,
                Color.White, 
                NPC.rotation,
                origin,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 240);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => true;
        public override bool CheckActive() => false;
    }
}
