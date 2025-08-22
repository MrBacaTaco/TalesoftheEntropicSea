
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class VoidHole : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 88;
            Projectile.height = 42;
            Projectile.aiStyle = -1; 
            Projectile.timeLeft = 180; 
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

       


        public override void AI()
        {
            if (Projectile.frame < Main.projFrames[Projectile.type] - 1)
            {
                Projectile.frameCounter++;

                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;


                }
            }
            else
            {

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= 10) 
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(
                            Projectile.GetSource_FromAI(),
                            Projectile.Center,
                            Vector2.Zero,
                            ModContent.ProjectileType<VoidH>(),
                            80,
                            Projectile.knockBack,
                            Projectile.owner
                        );
                    }

                    Projectile.Kill(); 
                }
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Heart/VoidHole"
            ).Value;

            int frameHeight = glowmask.Height / 6; 
            Rectangle sourceRect = new Rectangle(
                0,
                Projectile.frame * frameHeight, 
                glowmask.Width,
                frameHeight
            );

            
            Vector2 origin = new Vector2(sourceRect.Width / 2f, sourceRect.Height / 2f);

            
            Main.EntitySpriteDraw(
                glowmask,
                Projectile.Center - Main.screenPosition,
                sourceRect,
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
