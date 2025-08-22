using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.GABoss
{
    public class SharkProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
          
        }

        public override void SetDefaults()
        {
            Projectile.width = 264; 
            Projectile.height = 176;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false; 
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();


            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard);
            }

            Lighting.AddLight(Projectile.Center, 0f, 0.1f, 0.3f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 180);
        }


        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
                }

                SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() / 2f;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor, 
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0
            );

            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/GABoss/SharkProjectile"
            ).Value;

            Main.EntitySpriteDraw(
                glowmask,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White, 
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0
            );

            return false; 
        }


    }
}
