using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class BowWeapon : ModProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/Heart/BowWeapon";

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 5000; 
        }

        private int shootCooldown = 0;
        private int fireAnimTimer = 0;

        private enum BowState
        {
            NormalFire,
            FanSpin,
            SideFanShot,
            SideFanShotDown,
            SideFanShotUp,

            StaggeredShots,
            SweepBarrage 
        }



        private BowState state = BowState.NormalFire;

        private int normalShotsFired = 0;
        
        //Phase 2
        private int spinTimer = 0;
        private int spinShotIndex = 0;
        private const int FanProjectileCount = 16;

        //Phase 3
        private int sideAttackTimer = 0;
        private bool sideAttackReturning = false;
        private const int FanProjectiles = 5;

        //Barrage
        private int barrageShotTimer = 0;
        private int barrageShotsFired = 0;
        private const int BarrageCount = 15;
        private bool barrageReturning = false;
        private bool barrageLockedIn = false;


        public override void AI()
        {
            Player target = Main.player[Projectile.owner];
            if (!target.active || target.dead)
            {
                Projectile.Kill();
                return;
            }

            Vector2 hoverOffset = new Vector2(-300, -300);
            Vector2 targetPos = target.Center + hoverOffset;
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.1f);

            switch (state)
            {
                case BowState.NormalFire:
                    Projectile.rotation = MathHelper.PiOver2;
                    shootCooldown++;

                    if (shootCooldown >= 90)
                    {
                        shootCooldown = 0;
                        fireAnimTimer = 10;
                        normalShotsFired++;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 toPlayer = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                            float arrowSpeed = 12f;

                            Projectile.NewProjectile(
                                Projectile.GetSource_FromThis(),
                                Projectile.Center,
                                toPlayer * arrowSpeed,
                                ModContent.ProjectileType<HeartArrow>(),
                                50, 0f, Main.myPlayer
                            );

                            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HeavenlyGaleFire"), Projectile.Center);
                        }

                        if (normalShotsFired >= 3)
                        {
                            state = BowState.FanSpin;
                            spinTimer = 0;
                            normalShotsFired = 0;
                            spinShotIndex = 0;
                        }
                    }

                    break;

                case BowState.FanSpin:

                    Projectile.rotation += 0.15f;

                    spinTimer++;
                    if (spinTimer % 6 == 0 && spinShotIndex < FanProjectileCount && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float angle = MathHelper.TwoPi * (spinShotIndex / (float)FanProjectileCount);
                        Vector2 direction = angle.ToRotationVector2();
                        float speed = 10f;

                        Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            direction * speed,
                            ModContent.ProjectileType<HeartArrow>(),
                            40, 0f, Main.myPlayer
                        );

                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HeavenlyGaleFire"), Projectile.Center);
                        spinShotIndex++;
                    }

                    if (spinShotIndex >= FanProjectileCount)
                    {
                        state = BowState.SideFanShot;
                        spinTimer = 0;
                        sideAttackTimer = 0;
                        sideAttackReturning = false;
                        Projectile.rotation = MathHelper.PiOver2;
                    }


                    break;

                //Phase3 
                case BowState.SideFanShot:
                    {
                        Vector2 sideTargetPos;

                        if (!sideAttackReturning)
                        {
                            
                            sideTargetPos = new Vector2(Main.screenPosition.X + 100f, target.Center.Y);
                        }
                        else
                        {
                            
                            sideTargetPos = target.Center + new Vector2(-300, -300);
                        }

                        
                        Projectile.Center = Vector2.Lerp(Projectile.Center, sideTargetPos, 0.1f);
                        Projectile.rotation = MathHelper.PiOver2 - MathHelper.ToRadians(30f); // 90° + 30°

                        sideAttackTimer++;

                        if (!sideAttackReturning && sideAttackTimer == 30)
                        {
                            
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float totalSpread = MathHelper.ToRadians(40f);
                                float baseAngle = (target.Center - Projectile.Center).ToRotation();
                                float startAngle = baseAngle - totalSpread / 2f;

                                for (int i = 0; i < FanProjectiles; i++)
                                {
                                    float angle = startAngle + i * (totalSpread / (FanProjectiles - 1));
                                    Vector2 velocity = angle.ToRotationVector2() * 12f;

                                    Projectile.NewProjectile(
                                        Projectile.GetSource_FromThis(),
                                        Projectile.Center,
                                        velocity,
                                        ModContent.ProjectileType<HeartArrow>(),
                                        50, 0f, Main.myPlayer
                                    );

                                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HeavenlyGaleFire"), Projectile.Center);
                                }
                            }

                            sideAttackReturning = true;
                            sideAttackTimer = 0;
                        }

                        if (sideAttackReturning && Vector2.Distance(Projectile.Center, target.Center + new Vector2(-300, -300)) < 10f)
                        {
                            state = BowState.SideFanShotDown;
                            sideAttackReturning = false;
                            sideAttackTimer = 0;
                        }


                        break;
                    }

                //Phase4 and 5
                case BowState.SideFanShotDown:
                    {
                        Vector2 downPos = new Vector2(Main.screenPosition.X + 100f, target.Center.Y + 200f);
                        if (!sideAttackReturning)
                        {
                            Projectile.Center = Vector2.Lerp(Projectile.Center, downPos, 0.1f);
                            Projectile.rotation = MathHelper.PiOver2 + MathHelper.ToRadians(15f); 
                        }
                        else
                        {
                            Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center + new Vector2(-300, -300), 0.1f);
                        }

                        sideAttackTimer++;

                        if (!sideAttackReturning && sideAttackTimer == 30)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float totalSpread = MathHelper.ToRadians(40f);
                                float baseAngle = (target.Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(10f); 
                                float startAngle = baseAngle - totalSpread / 2f;

                                for (int i = 0; i < FanProjectiles; i++)
                                {
                                    float angle = startAngle + i * (totalSpread / (FanProjectiles - 1));
                                    Vector2 velocity = angle.ToRotationVector2() * 12f;

                                    Projectile.NewProjectile(
                                        Projectile.GetSource_FromThis(),
                                        Projectile.Center,
                                        velocity,
                                        ModContent.ProjectileType<HeartArrow>(),
                                        50, 0f, Main.myPlayer
                                    );

                                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HeavenlyGaleFire"), Projectile.Center);
                                }
                            }

                            sideAttackReturning = true;
                            sideAttackTimer = 0;
                        }

                        if (sideAttackReturning && Vector2.Distance(Projectile.Center, target.Center + new Vector2(-300, -300)) < 10f)
                        {
                            sideAttackReturning = false;
                            sideAttackTimer = 0;
                            state = BowState.SideFanShotUp;
                        }

                        break;
                    }
                case BowState.SideFanShotUp:
                    {
                        Vector2 upPos = new Vector2(Main.screenPosition.X + 100f, target.Center.Y - 200f);
                        if (!sideAttackReturning)
                        {
                            Projectile.Center = Vector2.Lerp(Projectile.Center, upPos, 0.1f);
                            Projectile.rotation = MathHelper.PiOver2 - MathHelper.ToRadians(15f); 
                        }
                        else
                        {
                            Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center + new Vector2(-300, -300), 0.1f);
                        }

                        sideAttackTimer++;

                        if (!sideAttackReturning && sideAttackTimer == 30)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float totalSpread = MathHelper.ToRadians(40f);
                                float baseAngle = (target.Center - Projectile.Center).ToRotation() - MathHelper.ToRadians(10f); 
                                float startAngle = baseAngle - totalSpread / 2f;

                                for (int i = 0; i < FanProjectiles; i++)
                                {
                                    float angle = startAngle + i * (totalSpread / (FanProjectiles - 1));
                                    Vector2 velocity = angle.ToRotationVector2() * 12f;

                                    Projectile.NewProjectile(
                                        Projectile.GetSource_FromThis(),
                                        Projectile.Center,
                                        velocity,
                                        ModContent.ProjectileType<HeartArrow>(),
                                        50, 0f, Main.myPlayer
                                    );

                                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HeavenlyGaleFire"), Projectile.Center);
                                }
                            }

                            sideAttackReturning = true;
                            sideAttackTimer = 0;
                        }

                        if (sideAttackReturning && Vector2.Distance(Projectile.Center, target.Center + new Vector2(-300, -300)) < 10f)
                        {
                            state = BowState.StaggeredShots; 
                            sideAttackReturning = false;
                            sideAttackTimer = 0;
                            shootCooldown = 0;
                            normalShotsFired = 0;
                        }


                        break;
                    }

                //phase 6
                case BowState.StaggeredShots:
                    Projectile.rotation = MathHelper.PiOver2;
                    shootCooldown++;

                    if (shootCooldown >= 90)
                    {
                        shootCooldown = 0;
                        fireAnimTimer = 10;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);

                            if (normalShotsFired == 1)
                                direction = (target.Center - new Vector2(0, 80f) - Projectile.Center).SafeNormalize(Vector2.UnitY); 
                            else if (normalShotsFired == 2)
                                direction = (target.Center + new Vector2(0, 80f) - Projectile.Center).SafeNormalize(Vector2.UnitY); 

                            float arrowSpeed = 12f;

                            Projectile.NewProjectile(
                                Projectile.GetSource_FromThis(),
                                Projectile.Center,
                                direction * arrowSpeed,
                                ModContent.ProjectileType<HeartArrow>(),
                                50, 0f, Main.myPlayer
                            );

                            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HeavenlyGaleFire"), Projectile.Center);
                        }

                        normalShotsFired++;

                        if (normalShotsFired >= 3)
                        {
                            state = BowState.SweepBarrage; 
                            normalShotsFired = 0;
                            barrageShotTimer = 0;
                            barrageShotsFired = 0;
                            barrageReturning = false;
                        }

                    }
                    break;

                //phase 7
                case BowState.SweepBarrage:
                    {
                        Projectile.frame = 4; 


                        Vector2 barrageTargetPos;
                        if (!barrageReturning)
                        {
                           
                            barrageTargetPos = target.Center + new Vector2(-400f, 1000f);
                        }
                        else
                        {
                            barrageTargetPos = target.Center + new Vector2(-300f, -300f);
                        }

                        
                        Projectile.Center = Vector2.Lerp(Projectile.Center, barrageTargetPos, 0.1f);

                        
                        Projectile.rotation = 0f;

                        
                        if (!barrageReturning && barrageShotTimer > 30)
                        {
                            Vector2 expectedStartOffset = new Vector2(-400f, 1000f);
                            Vector2 currentOffsetFromTarget = Projectile.Center - target.Center;

                            
                            if (Vector2.Distance(currentOffsetFromTarget, expectedStartOffset) < 20f)
                            {
                                
                                Projectile.position.X += 2f; 
                            }
                        }

                        barrageShotTimer++;

                        if (!barrageReturning && barrageShotTimer % 6 == 0 && barrageShotsFired < BarrageCount)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                
                                float t = barrageShotsFired / (float)(BarrageCount - 1); 
                                float angleDegrees = MathHelper.Lerp(-90f, -45f, t);
                                float angleRadians = MathHelper.ToRadians(angleDegrees);

                                
                                Vector2 shootDir = angleRadians.ToRotationVector2();

                                
                                float xJitter = Main.rand.NextFloat(-1.5f, 1.5f);
                                shootDir.X += xJitter * 0.1f;

                                
                                Projectile.NewProjectile(
                                    Projectile.GetSource_FromThis(),
                                    Projectile.Center,
                                    shootDir * 10f,
                                    ModContent.ProjectileType<HeartArrow>(),
                                    50, 0f, Main.myPlayer
                                );

                                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HeavenlyGaleFire"), Projectile.Center);
                            }

                            barrageShotsFired++;
                        }

                        if (barrageShotsFired >= BarrageCount)
                            barrageReturning = true;

                        if (barrageReturning && Vector2.Distance(Projectile.Center, target.Center + new Vector2(-300f, -300f)) < 10f)
                        {
                            state = BowState.NormalFire;
                            barrageReturning = false;
                            barrageShotTimer = 0;
                            barrageShotsFired = 0;
                        }

                        break;
                    }










            }

            
            if (state == BowState.FanSpin)
            {
                
                Projectile.frame = 4;
                Projectile.frameCounter = 0;
            }
            else
            {
                if (fireAnimTimer > 0)
                {
                    fireAnimTimer--;
                    if (++Projectile.frameCounter >= 2)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame >= Main.projFrames[Projectile.type])
                            Projectile.frame = 1;
                    }
                }
                else
                {
                    Projectile.frame = 0;
                    Projectile.frameCounter = 0;
                }
            }
        }





        public override bool PreDraw(ref Color lightColor)
        {
            
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frame.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(
                texture,
                drawPos,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Heart/BowWeapon"
            ).Value;

            Rectangle glowFrame = glowmask.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            Main.spriteBatch.Draw(
                glowmask,
                drawPos,
                glowFrame,
                Color.White, 
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false; 
        }



        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5; 
        }

    }
}
