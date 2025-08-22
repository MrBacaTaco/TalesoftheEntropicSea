using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.GABoss
{
    public class UrchingSpikeProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0f, 0.2f, 0.4f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 120);
        }


        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/GABoss/UrchingSpikeProjectile"
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
