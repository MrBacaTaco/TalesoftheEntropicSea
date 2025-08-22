using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class EidolonHead : ModProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/Heart/EidolonHead";

        public override void SetDefaults()
        {
            Projectile.width = 126;
            Projectile.height = 76;
            Projectile.timeLeft = 1200; 
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.timeLeft = 9999; 
        }

        public override void AI()
        {

            if (Projectile.ai[0] == 1f)
                return; 

            if (Projectile.ai[0] == 2f) 
            {
                Projectile.velocity *= 0.98f; 
                Projectile.rotation += 0.05f; 
                if (Projectile.velocity.Length() < 0.2f)
                {
                    Projectile.Kill(); 
                }
                return;
            }

            if (Projectile.localAI[1] == 1f)
            {
                Projectile.rotation += 0.22f;
            }
            else if (Projectile.velocity.Length() > 0.05f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            Projectile.velocity *= 0.995f;
        }

        public override bool? CanDamage() => false; 


        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs,
    List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height); 
            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }
    }
}
