using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TalesoftheEntropicSea.Content.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace TalesoftheEntropicSea.Content.Projectiles.Heart
{
    public class VoidH : ModProjectile
    {
        private Texture2D overlayTexture;
        private int overlayFrame = 0;
        private int overlayFrameCounter = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3; 
        }

        public override void SetDefaults()
        {
            Projectile.width = 146;
            Projectile.height = 104;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 120; 
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.damage = 80;

        }

        public override void AI()
        {

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            overlayFrameCounter++;
            if (overlayFrameCounter >= 15) 
            {
                overlayFrameCounter = 0;

                if (overlayFrame == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SCalSounds/SCalRumble"), Projectile.Center);
                }

                overlayFrame++;
                if (overlayFrame >= 8) 
                    overlayFrame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            Texture2D glowmask = ModContent.Request<Texture2D>(
                "TalesoftheEntropicSea/Content/Projectiles/Heart/VoidH"
            ).Value;

            Rectangle glowSource = new Rectangle(0, Projectile.frame * frameHeight, glowmask.Width, frameHeight);
            Main.EntitySpriteDraw(
                glowmask,
                Projectile.Center - Main.screenPosition,
                glowSource,
                Color.White, 
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            if (overlayTexture == null)
                overlayTexture = ModContent.Request<Texture2D>(
                    "TalesoftheEntropicSea/Content/Projectiles/Heart/Hands2"
                ).Value;

            int overlayFrameHeight = overlayTexture.Height / 8; 
            Rectangle overlaySource = new Rectangle(0, overlayFrame * overlayFrameHeight, overlayTexture.Width, overlayFrameHeight);
            Vector2 overlayOrigin = overlaySource.Size() / 2f;


            Vector2 handsOffset = new Vector2(0f, -36f);

            Main.EntitySpriteDraw(
                overlayTexture,
                Projectile.Center - Main.screenPosition + handsOffset,
                overlaySource,
                Color.White,
                0f,
                overlayOrigin,
                1f,
                SpriteEffects.None,
                0
            );

            return false; 
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 180);
        }



        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new Rectangle(
                (int)(Projectile.Center.X - 54),  
                (int)(Projectile.Center.Y - 64 - 36), 
                108,
                128
            );
        }


    }
}
