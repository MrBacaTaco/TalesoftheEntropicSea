
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.Buffs;
using TalesoftheEntropicSea.Content.Projectiles.Heart;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace TalesoftheEntropicSea.Content.Projectiles.Weapons
{
    public class StingrayProjectile : ModProjectile
    {
        public override string Texture => "TalesoftheEntropicSea/Content/Projectiles/Weapons/StingrayProjectile";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 40; 
            Projectile.WhipSettings.RangeMultiplier = 2.5f;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
           
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float ChargeTime
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private static Dictionary<int, int> projectilesThisSecond = new();
        private static Dictionary<int, float> timerPerPlayer = new();


        public override void AI()
        {
            int playerID = Projectile.owner;

            if (!timerPerPlayer.ContainsKey(playerID))
            {
                timerPerPlayer[playerID] = 0f;
                projectilesThisSecond[playerID] = 0;
            }

            timerPerPlayer[playerID] += 1f / 60f;

            if (timerPerPlayer[playerID] >= 1f)
            {
                timerPerPlayer[playerID] = 0f;
                projectilesThisSecond[playerID] = 0;
            }

            Player player = Main.player[playerID];
            Timer++;

            float swingTime = player.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime || player.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            player.heldProj = Projectile.whoAmI;

            if (Timer == swingTime / 2)
            {
                List<Vector2> points = new();
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[^1]);
            }
        }

       


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.7f);

            SpawnSkyProjectiles(target.Center);

            target.AddBuff(ModContent.BuffType<ThalassophobiaDebuff>(), 240); 

            if (target.active && !target.friendly)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_OnHit(target),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<StingrayMarker>(),
                    0, 
                    0,
                    Projectile.owner,
                    target.whoAmI 
                );
            }

        }

        private void SpawnSkyProjectiles(Vector2 targetCenter)
        {
            Player player = Main.player[Projectile.owner];
            int owner = player.whoAmI;

            if (projectilesThisSecond.TryGetValue(owner, out int count) && count >= 3)
                return; 

            IEntitySource source = player.GetSource_ItemUse(player.HeldItem);

            int amount = Main.rand.Next(2, 4);

            amount = Math.Min(amount, 3 - count);

            for (int i = 0; i < amount; i++)
            {
                float offsetX = Main.rand.NextFloat(-100f, 100f);
                Vector2 spawnPos = targetCenter + new Vector2(offsetX, -600f);

                Vector2 velocity = Vector2.UnitY.RotatedByRandom(0.15f) * Main.rand.NextFloat(8f, 12f);

                int type = Main.rand.NextBool()
                    ? ModContent.ProjectileType<AnchorDebris>()
                    : ModContent.ProjectileType<ShipDebris>();

                int proj = Projectile.NewProjectile(source, spawnPos, velocity, type, Projectile.damage / 2, 0, player.whoAmI);

                if (Main.projectile.IndexInRange(proj))
                {
                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].DamageType = DamageClass.Summon;
                }

                projectilesThisSecond[owner]++;
            }
        }




        private void DrawLine(List<Vector2> points)
        {
            Texture2D tex = TextureAssets.FishingLine.Value;
            Rectangle frame = tex.Frame();
            Vector2 origin = new(frame.Width / 2f, 2f);

            Vector2 pos = points[0];
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 diff = points[i + 1] - points[i];
                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(points[i].ToTileCoordinates());
                Vector2 scale = new(1f, (diff.Length() + 2f) / frame.Height);

                Main.EntitySpriteDraw(tex, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None);
                pos += diff;
            }
        }

       


        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> points = new();
            Projectile.FillWhipControlPoints(Projectile, points);

            DrawLine(points);

            SpriteEffects flip = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 pos = points[0];

            for (int i = 0; i < points.Count - 1; i++)
            {
                Rectangle frame;
                Vector2 origin;
                float scale = 1f;

                if (i == 0)
                {
                    // Handle
                    frame = new Rectangle(0, 0, 22, 30);
                    origin = new Vector2(11, 10);
                }
                else if (i == 1)
                {
                    frame = new Rectangle(22, 0, 28, 30);
                    origin = new Vector2(14, 10);
                }
                else if (i == 2)
                {
                    frame = new Rectangle(50, 0, 30, 30);
                    origin = new Vector2(15, 10);
                }
                else if (i == 3)
                {
                    frame = new Rectangle(80, 0, 30, 30);
                    origin = new Vector2(15, 10);
                }
                else if (i == points.Count - 2)
                {
                    frame = new Rectangle(110, 0, 38, 30);
                    origin = new Vector2(19, 10);

                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f,
                        Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else
                {
                    frame = new Rectangle(80, 0, 30, 30);
                    origin = new Vector2(15, 10);
                }

                Vector2 diff = points[i + 1] - points[i];
                float rotation = diff.ToRotation(); 

                Color color = Lighting.GetColor(points[i].ToTileCoordinates());

                Main.EntitySpriteDraw(tex, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip);
                pos += diff;
            }

            return false;
        }
    }
}
