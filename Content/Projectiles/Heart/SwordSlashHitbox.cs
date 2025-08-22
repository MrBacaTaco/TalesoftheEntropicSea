

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class SwordSlashHitbox : ModProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/Heart/SwordSlashHitbox";

        private enum AttackState { Idle, Spinning, HorizontalSpin, SwordBarrage }

        private AttackState state = AttackState.Idle;

        private int attackTimer = 0;
        private float spinTimer = 0f;
        private const float MaxSpinTime = 60f;
        private int horizontalSpinTimer = 0;

        //idle
        private bool prepareBarrageAfterIdle = false;

        //barage
        private int barrageTimer = 0;
        private const int BarrageSwords = 6;
        private const int BarrageInterval = 10;
        private int barrageSwordsFired = 0;

        public override void SetDefaults()
        {
            Projectile.width = 240;
            Projectile.height = 240;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 5000;
            Projectile.alpha = 0; 
        }

        public override void AI()
        {
            Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

            if (!target.active || target.dead)
            {
                Projectile.Kill();
                return;
            }

            switch (state)
            {
                case AttackState.Idle:
                    {
                        Projectile.rotation = MathHelper.PiOver2;
                        attackTimer++;

                        Vector2 hoverOffset = new Vector2(300f, -200f);
                        Vector2 targetPos = target.Center + hoverOffset;
                        Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.1f);

                        if (prepareBarrageAfterIdle && attackTimer >= 30)
                        {
                            attackTimer = 0;
                            barrageTimer = 0;
                            barrageSwordsFired = 0;
                            state = AttackState.SwordBarrage;
                            prepareBarrageAfterIdle = false;
                        }
                        else if (!prepareBarrageAfterIdle && attackTimer >= 120)
                        {
                            attackTimer = 0;
                            state = AttackState.Spinning;
                            spinTimer = 0f;
                            Projectile.hostile = true;
                        }

                        break;
                    }


                case AttackState.Spinning:
                    {
                        spinTimer++;

                        float swingProgress = spinTimer / MaxSpinTime;
                        float startAngle = MathHelper.PiOver2;
                        float endAngle = -MathHelper.ToRadians(360f);
                        Projectile.rotation = MathHelper.Lerp(startAngle, endAngle, swingProgress);

                        if (spinTimer >= MaxSpinTime)
                        {
                            state = AttackState.HorizontalSpin;
                            horizontalSpinTimer = 0;
                            Projectile.hostile = true;

                            Projectile.Center = target.Center + new Vector2(0f, 0f); 
                            Projectile.velocity = Vector2.Zero;
                            Projectile.rotation = 0f;
                        }

                        break;
                    }

                case AttackState.HorizontalSpin:
                    {
                        horizontalSpinTimer++;

                        Projectile.rotation += 0.4f;
                        Projectile.velocity = new Vector2(-12f, 0f);
                        Projectile.position.Y += (float)Math.Sin(horizontalSpinTimer * 0.2f) * 1.5f;

                        if (horizontalSpinTimer >= 90)
                        {
                            state = AttackState.Idle;
                            horizontalSpinTimer = 0;
                            Projectile.velocity = Vector2.Zero;
                            prepareBarrageAfterIdle = true;
                            attackTimer = 0; 
                        }



                        break;
                    }

                case AttackState.SwordBarrage:
                    {
                        barrageTimer++;

                        if (barrageTimer % BarrageInterval == 0 && barrageSwordsFired < BarrageSwords)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float yOffset = (BarrageSwords - 1 - barrageSwordsFired) * 80f;
                                Vector2 spawnPos = new Vector2(
                                    Main.screenPosition.X + Main.screenWidth + 100f,
                                    target.Center.Y - yOffset + 200f 
                                );

                                int projID = Projectile.NewProjectile(
                                    Projectile.GetSource_FromThis(),
                                    spawnPos,
                                    new Vector2(-24f, 0f), 
                                    ModContent.ProjectileType<SwordHorizontal>(),
                                    Projectile.damage,
                                    0f,
                                    Main.myPlayer
                                );

                                if (projID >= 0 && projID < Main.maxProjectiles)
                                {
                                    Projectile proj = Main.projectile[projID];

                                    proj.alpha = 204 - (barrageSwordsFired * 25);
                                    proj.alpha = Utils.Clamp(proj.alpha, 0, 255);
                                }
                            }
                            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/ExobladeBeamSlash"), Projectile.Center);
                            barrageSwordsFired++;
                        }

                        if (barrageSwordsFired >= BarrageSwords && barrageTimer > BarrageSwords * BarrageInterval + 30)
                        {
                            state = AttackState.Idle;
                            barrageTimer = 0;
                            barrageSwordsFired = 0;
                        }

                        break;
                    }



            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return Projectile.hostile;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 bladeStart = Projectile.Center;
            float swordLength = 120f;
            Vector2 direction = Projectile.rotation.ToRotationVector2();
            Vector2 bladeEnd = bladeStart + direction * swordLength;

            float collisionWidth = 30f;
            float collisionPoint = 0f;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), targetHitbox.Size(),
                bladeStart, bladeEnd, collisionWidth, ref collisionPoint
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height); 
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteEffects effects = SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture, drawPos, null, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0f);

            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Heart/SwordSlashHitbox"
            ).Value;

            Main.spriteBatch.Draw(glowmask, drawPos, null, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0f);

            return false; 
        }

    }
}
