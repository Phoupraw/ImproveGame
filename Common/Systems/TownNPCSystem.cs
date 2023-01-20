﻿using System.Reflection;

namespace ImproveGame.Common.Systems
{
    public class TownNPCSystem : ModSystem
    {
        private static bool _skipFindHome;
        private static MethodInfo _spawnTownNPCs;
        private static List<int> _townNPCIDs = new();
        private static HashSet<int> _activeTownNPCs = new();

        public override void Load()
        {
            // 更好的NPC生成机制 + NPC生成加速
            On.Terraria.Main.UpdateTime_SpawnTownNPCs += orig =>
            {
                _skipFindHome = false;
                orig.Invoke();
                
                int times = Config.TownNPCSpawnSpeed is 0 ? 0 : (int)Math.Pow(2, Config.TownNPCSpawnSpeed);
                // 在跳过寻找家的步骤的情况下调用 orig
                for (int i = 0; i < times; i++)
                {
                    TrySetNPCSpawn(orig);
                }
                Main.NewText(times);
            };
            // 在加速的NPC生成中，跳过寻找家的步骤
            On.Terraria.WorldGen.QuickFindHome += (orig, npc) =>
            {
                if (!_skipFindHome)
                    orig.Invoke(npc);
            };
        }

        public override void PostSetupContent()
        {
            _spawnTownNPCs =
                typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.Static | BindingFlags.NonPublic);
            SetupTownNPCList();
        }

        /// <summary>设立城镇NPC列表</summary>
        private static void SetupTownNPCList()
        {
            _townNPCIDs.AddRange(NPCID.Sets.TownNPCBestiaryPriority);
            // 不用这个，我们要参考Priority对入住NPC优先进行排序
            //foreach ((int netID, NPC npc) in ContentSamples.NpcsByNetId)
            //{
            //    if (npc.townNPC && NPC.TypeToDefaultHeadIndex(netID) >= 0)
            //    {
            //        TownNPCIDs.Add(netID);
            //    }
            //}

            // 你个浓眉大眼的到底是不是城镇NPC?
            _townNPCIDs.RemoveAll(id =>
            {
                var npc = new NPC();
                npc.SetDefaults(id);
                int head = NPC.TypeToDefaultHeadIndex(id);
                return !npc.townNPC || head < 0 || head >= NPCHeadLoader.NPCHeadCount ||
                       NPCHeadID.Sets.CannotBeDrawnInHousingUI[head] || npc.type is NPCID.SantaClaus ||
                       npc.ModNPC?.TownNPCStayingHomeless is true;
            });

            var modNPCs =
                typeof(NPCLoader).GetField("npcs", BindingFlags.Static | BindingFlags.NonPublic)
                    ?.GetValue(null) as IList<ModNPC> ?? new List<ModNPC>();

            foreach (var modNPC in modNPCs)
            {
                var npc = modNPC.NPC;
                int head = NPC.TypeToDefaultHeadIndex(npc.type);
                if (npc.townNPC && head >= 0 && !NPCHeadID.Sets.CannotBeDrawnInHousingUI[head] &&
                    !modNPC.TownNPCStayingHomeless)
                {
                    _townNPCIDs.Add(npc.type);
                }
            }
        }

        private static void TrySetNPCSpawn(On.Terraria.Main.orig_UpdateTime_SpawnTownNPCs orig)
        {
            // 在跳过寻找家的步骤的情况下调用 orig
            _skipFindHome = true;
            orig.Invoke();

            // 确保原版在生成NPC的阶段
            double worldUpdateRate = WorldGen.GetWorldUpdateRate();
            if (Main.netMode is NetmodeID.MultiplayerClient || worldUpdateRate <= 0 || Main.checkForSpawns != 0)
                return;

            // 设立存活城镇NPC速查表
            SetupActiveTownNPCList();

            // 钱币包括猪猪存钱罐里面的
            if (HasEnoughMoneyForMerchant())
                TrySetNPCSpawn(NPCID.Merchant);

            // 更好的生成机制
            if (!Config.TownNPCGetTFIntoHouse)
            {
                return;
            }

            if (NPC.downedGoblins)
                TrySetNPCSpawn(NPCID.GoblinTinkerer);
            if (NPC.downedBoss2)
                TrySetNPCSpawn(NPCID.DD2Bartender);
            if (NPC.downedBoss3)
                TrySetNPCSpawn(NPCID.Mechanic);
            if (Main.hardMode)
            {
                TrySetNPCSpawn(NPCID.Wizard);
                TrySetNPCSpawn(NPCID.TaxCollector);
            }

            if (_townNPCIDs is null || _townNPCIDs.Count < 0)
            {
                SetupTownNPCList();
            }

            if (_townNPCIDs == null)
            {
                return;
            }

            foreach (var id in _townNPCIDs.Where(id =>
                         Main.BestiaryTracker.Chats.GetWasChatWith(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[id])))
            {
                TrySetNPCSpawn(id);
            }
        }

        private static bool HasEnoughMoneyForMerchant()
        {
            int moneyCount = 0;
            for (int l = 0; l < Main.maxPlayers; l++)
            {
                var player = Main.player[l];
                if (!player.active)
                    continue;

                for (int m = 0; m < 40; m++)
                {
                    if (player.bank.item[m] is null || player.bank.item[m].stack <= 0)
                        continue;

                    var item = player.bank.item[m];
                    switch (item.type)
                    {
                        case ItemID.CopperCoin:
                            moneyCount += item.stack;
                            break;
                        case ItemID.SilverCoin:
                            moneyCount += item.stack * 100;
                            break;
                        case ItemID.GoldCoin:
                            return true;
                        case ItemID.PlatinumCoin:
                            return true;
                    }

                    if (moneyCount >= 5000) return true;
                }
            }

            return false;
        }

        public override void PostUpdateTime()
        {
            if (!Main.dayTime && Config.TownNPCGetTFIntoHouse)
                _spawnTownNPCs.Invoke(null, null);
        }

        private static void TrySetNPCSpawn(int npcId)
        {
            if (_activeTownNPCs.Contains(npcId))
                return;

            Main.townNPCCanSpawn[npcId] = true;
            if (WorldGen.prioritizedTownNPCType == 0)
            {
                WorldGen.prioritizedTownNPCType = npcId;
            }
        }

        private static void SetupActiveTownNPCList()
        {
            _activeTownNPCs = new HashSet<int>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && npc.townNPC && npc.friendly)
                {
                    _activeTownNPCs.Add(npc.type);
                }
            }
        }
    }
}