using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.GABoss
{
    public class BubbleBlastProjectile : ModProjectile
    {
        private const float RiseSpeed = -5f;
        private const float ExplosionRadius = 80f;
        private const int ExplosionDelay = 180;

        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/GABoss/BubbleBig";
        private static readonly string TelegraphTexturePath = "TalesoftheEntropicSea/Content/Projectiles/GABoss/BubbleTelegraph";

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = ExplosionDelay + 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.velocity = new Vector2(0f, RiseSpeed);

            if (PlayerIsInRange() || Projectile.timeLeft <= 1)
            {
                Explode();
            }

            if (Projectile.Hitbox.Intersects(Main.player[Projectile.owner].Hitbox))
                Explode();
        }

        private bool PlayerIsInRange()
        {
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && player.Center.Distance(Projectile.Center) <= ExplosionRadius)
                    return true;
            }
            return false;
        }

        private void Explode()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];
                    if (p.active && !p.dead && p.Center.Distance(Projectile.Center) <= ExplosionRadius)
                    {
                        p.Hurt(PlayerDeathReason.ByProjectile(Main.myPlayer, Projectile.whoAmI), Projectile.damage, 0);
                    }
                }

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<BubbleExplosionProjectile>(),
                    0, 0f, Main.myPlayer
                );
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D tex = ModContent.Request<Texture2D>(TelegraphTexturePath).Value;

            float scale = (ExplosionRadius * 2f) / tex.Width; 
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = tex.Size() * 0.5f;

            float fadeIn = Utils.GetLerpValue(0f, 30f, Projectile.timeLeft, clamped: true); 
            Color ringColor = Color.White * 0.8f * fadeIn;

            Main.spriteBatch.Draw(
                tex,
                drawPos,
                null,
                ringColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );

            return true;
        }

    }
}
