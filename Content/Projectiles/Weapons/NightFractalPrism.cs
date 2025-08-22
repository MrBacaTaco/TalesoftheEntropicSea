using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.Projectiles.Weapons;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class NightFractalPrism : ModProjectile
    {
        public const int NumBeams = 6;
        public const float MaxCharge = 120f; 
        public const float DamageStart = 30f;
        private const float AimResponsiveness = 0.9f;
        private const int SoundInterval = 20;
        private const float MaxManaConsumptionDelay = 15f;
        private const float MinManaConsumptionDelay = 5f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 126;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }



        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            Projectile.damage = player.HeldItem is null ? 0 : player.GetWeaponDamage(player.HeldItem);

            Projectile.ai[0] += 1f;
            float chargeRatio = MathHelper.Clamp(Projectile.ai[0] / MaxCharge, 0f, 1f);

            if (Projectile.owner == Main.myPlayer && chargeRatio > 0.2f)
            {
                float shakeIntensity = 3f * chargeRatio; 
                Main.screenPosition += new Vector2(
                    Main.rand.NextFloat(-shakeIntensity, shakeIntensity),
                    Main.rand.NextFloat(-shakeIntensity, shakeIntensity)
                );
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5) 
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 8)
                    Projectile.frame = 0;
            }

            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = SoundInterval;
                if (Projectile.ai[0] > 1f)
                    SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
            }

            UpdatePlayerVisuals(player, rrp);

            if (Projectile.owner == Main.myPlayer)
            {
                float speed = player.HeldItem.shootSpeed * Projectile.scale;
                UpdateAim(rrp, speed);

                if (Projectile.localAI[0] <= 0f)
                {
                    if (!player.CheckMana(player.HeldItem.mana, true))
                    {
                        Projectile.Kill(); 
                        return;
                    }

                    Projectile.localAI[0] = 30f; 
                }
                else
                {
                    Projectile.localAI[0]--;
                }

                bool stillUsing = player.channel && !player.noItems && !player.CCed;

                if (stillUsing && Projectile.ai[0] == 1f)
                {
                    Vector2 beamVelocity = Vector2.Normalize(Projectile.velocity);
                    if (beamVelocity.HasNaNs())
                        beamVelocity = -Vector2.UnitY;
                    int damage = Projectile.damage;
                    float kb = Projectile.knockBack;
                    for (int b = 0; b < NumBeams; b++)
                    {
                        int beam = Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            beamVelocity,
                            ModContent.ProjectileType<NightFractalBeam>(),
                            damage,
                            kb,
                            Projectile.owner,
                            b, 
                            Projectile.whoAmI 
                        );
                        Main.projectile[beam].netUpdate = true;
                    }
                    Projectile.netUpdate = true;
                }
                else if (!stillUsing)
                    Projectile.Kill();
            }

            Projectile.timeLeft = 2;
        }



        private bool ShouldConsumeMana()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[1] = Projectile.localAI[0] = MaxManaConsumptionDelay;
                return true;
            }
            bool consume = Projectile.ai[0] == Projectile.ai[1];
            if (consume)
            {
                Projectile.localAI[0] = MathHelper.Clamp(Projectile.localAI[0] - 1f, MinManaConsumptionDelay, MaxManaConsumptionDelay);
                Projectile.ai[1] += Projectile.localAI[0];
            }
            return consume;
        }

        private void UpdateAim(Vector2 source, float speed)
        {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), AimResponsiveness));
            aimVector *= speed;
            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        private void UpdatePlayerVisuals(Player player, Vector2 rrp)
        {
            Projectile.Center = rrp;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = tex.Height / Main.projFrames[Projectile.type]; 
            int frameWidth = tex.Width;


            Rectangle frame = new Rectangle(0, Projectile.frame * frameHeight, frameWidth, frameHeight);


            Vector2 origin = new Vector2(frameWidth / 2f, frameHeight / 2f);


            Vector2 drawPos = Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition;

            Main.spriteBatch.Draw(
                tex,
                drawPos,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false; 
        }

        public override void DrawBehind(
            int index,
            List<int> behindNPCsAndTiles,
            List<int> behindNPCs,
            List<int> behindProjectiles,
            List<int> overPlayers,
            List<int> overWiresUI)
        {
            overPlayers.Add(index); 
        }



    }
}
