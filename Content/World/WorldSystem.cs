using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using TalesoftheEntropicSea.Content.NPCs.GABoss;
using TalesoftheEntropicSea.Content.Projectiles;
using TalesoftheEntropicSea.Content.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using NoxusBoss.Core.World.WorldSaving; 
using NoxusBoss.Content.NPCs.Bosses.Avatar.FirstPhaseForm; 


//debugs are comed out


namespace TalesoftheEntropicSea.World
{
    public class WorldSystem : ModSystem
    {
        private bool spawnedPortal = false;



        public static Rectangle ArenaBounds;

        public static Texture2D IndicatorLineTex;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                IndicatorLineTex = ModContent.Request<Texture2D>("TalesoftheEntropicSea/Content/Projectiles/GABoss/IndicatorLine").Value;
            }
        }
        private bool shouldSpawnPortalOnNextTick = false;

        private bool portalSpawned = false;
        private bool bossDefeated = false;

        private Vector2? savedPortalPosition = null; 


        public override void PostUpdateWorld()
        {
            if (shouldSpawnPortalOnNextTick && !AnySkyAbyssPortalExists())
            {
                TrySpawnSkyAbyssPortal();
                shouldSpawnPortalOnNextTick = false;
            }

            if (!bossDefeated && BossDownedSaveSystem.HasDefeated<AvatarRift>())
            {
                bossDefeated = true;
                TrySpawnSkyAbyssPortal();
            }
        }



        private bool AnySkyAbyssPortalExists()
        {
            int portalType = ModContent.ProjectileType<SkyAbyssPortal>();

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == portalType)
                    return true;
            }

            return false;
        }


        public override void SaveWorldData(TagCompound tag)
        {
            tag["portalSpawned"] = portalSpawned;
            tag["bossDefeated"] = bossDefeated;

            if (savedPortalPosition.HasValue)
            {
                tag["portalX"] = savedPortalPosition.Value.X;
                tag["portalY"] = savedPortalPosition.Value.Y;
            }
        }


        public override void LoadWorldData(TagCompound tag)
        {
            portalSpawned = tag.GetBool("portalSpawned");
            bossDefeated = tag.GetBool("bossDefeated");

            if (portalSpawned)
            {
                //Main.NewText("portalSpawned was TRUE on load", Color.LightBlue);
                shouldSpawnPortalOnNextTick = true;
            }

            if (tag.ContainsKey("portalX") && tag.ContainsKey("portalY"))
            {
                savedPortalPosition = new Vector2(tag.GetFloat("portalX"), tag.GetFloat("portalY"));
                //Main.NewText($"Loaded saved portal position: {savedPortalPosition}", Color.LightBlue);
            }
        }




        private void TrySpawnSkyAbyssPortal()
        {
            if (portalSpawned && AnySkyAbyssPortalExists())
            {
                //Main.NewText("Portal already exists — skipping spawn", Color.Gray);
                return;
            }

            portalSpawned = true;

            Vector2 spawnPosition;

            if (savedPortalPosition is not null)
            {
                spawnPosition = savedPortalPosition.Value;
                //Main.NewText($"Spawning portal at saved position: {spawnPosition}", Color.Cyan);
            }
            else
            {
                int tileX = SkyAbyssIslandsGen.SkyAbyssCenterX;
                int tileY = SkyAbyssIslandsGen.SkyAbyssCenterY;
                spawnPosition = new Vector2(tileX * 16, (tileY - 5) * 16);
                savedPortalPosition = spawnPosition;
                //Main.NewText($"Spawning portal at new center: {spawnPosition}", Color.LimeGreen);
            }

            IEntitySource source = new EntitySource_Misc("SkyAbyssPortal");

            Projectile.NewProjectile(
                source,
                spawnPosition,
                Vector2.Zero,
                ModContent.ProjectileType<SkyAbyssPortal>(),
                0,
                0,
                Main.myPlayer
            );
        }




        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "TalesoftheEntropicSea:IndicatorLines",
                    delegate
                    {
                        foreach (var npc in Main.npc)
                        {
                            if (npc.active && npc.type == ModContent.NPCType<Head>())
                            {
                                var modNPC = npc.ModNPC as Head;
                                if (modNPC != null && modNPC.indicatorTimer > 0)
                                {
                                    foreach (var pos in modNPC.indicatorPositions)
                                    {
                                        Vector2 drawPos = pos - Main.screenPosition;
                                        Main.spriteBatch.Draw(
                                            IndicatorLineTex,
                                            drawPos,
                                            null,
                                            Color.White * 0.7f,
                                            0f,
                                            new Vector2(IndicatorLineTex.Width / 2f, IndicatorLineTex.Height),
                                            1f,
                                            SpriteEffects.None,
                                            0f
                                        );
                                    }
                                }
                            }
                        }

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

    }
}
