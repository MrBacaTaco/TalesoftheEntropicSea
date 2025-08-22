using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.GABoss
{
    public class SeaUrchingProjectile : ModProjectile
    {
        private const int TelegraphTime = 60; 
        private const int ExplodeDelay = 120;
        private const int SpikeCount = 8;

        private bool telegraphed = false;

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 74;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = TelegraphTime + ExplodeDelay;
            Projectile.tileCollide = true; 
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;

            if (!telegraphed && Projectile.timeLeft == ExplodeDelay)
            {
                telegraphed = true;
                Projectile.velocity = Vector2.Zero; 

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < SpikeCount; i++)
                    {
                        float angle = MathHelper.TwoPi * i / SpikeCount;
                        Vector2 dir = angle.ToRotationVector2();

                        Projectile telegraph = Projectile.NewProjectileDirect(
                            Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            dir,
                            ModContent.ProjectileType<UrchinTelegraphLine>(),
                            0, 0f, Main.myPlayer
                        );

                        if (telegraph.ModProjectile is UrchinTelegraphLine line)
                        {
                            line.Lifetime = TelegraphTime;
                            line.Projectile.rotation = dir.ToRotation();
                            line.Projectile.localAI[0] = 3000f;
                        }
                    }
                }
            }

            if (Projectile.timeLeft == 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

                for (int i = 0; i < SpikeCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / SpikeCount;
                    Vector2 velocity = angle.ToRotationVector2() * 12f;

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        velocity,
                        ModContent.ProjectileType<UrchingSpikeProjectile>(),
                        Projectile.damage / 2, 0f, Main.myPlayer
                    );
                }
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 180);
        }


        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WaterCandle);
                }

                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/GABoss/SeaUrchingProjectile"
            ).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(glowmask.Width / 2f, glowmask.Height / 2f);

            Main.EntitySpriteDraw(
                glowmask,
                drawPosition,
                null,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }
    }
}
