﻿using System.Collections.Generic;
using System.Reflection;

namespace ImproveGame.Common.Systems
{
    public class TownNPCSystem : ModSystem
    {
        public MethodInfo SpawnTownNPCs;
        public List<int> TownNPCIDs = new();

        public override void Load()
        {
            On.Terraria.Main.UpdateTime_SpawnTownNPCs += TweakNPCSpawn;
        }

        public override void PostSetupContent()
        {
            SpawnTownNPCs = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.Static | BindingFlags.NonPublic);
            SetupTownNPCList();
        }

        /// <summary>设立城镇NPC列表</summary>
        private void SetupTownNPCList()
        {
            TownNPCIDs.AddRange(NPCID.Sets.TownNPCBestiaryPriority);
            // 不用这个，我们要参考Priority对入住NPC优先进行排序
            //foreach ((int netID, NPC npc) in ContentSamples.NpcsByNetId)
            //{
            //    if (npc.townNPC && NPC.TypeToDefaultHeadIndex(netID) >= 0)
            //    {
            //        TownNPCIDs.Add(netID);
            //    }
            //}

            // 你个浓眉大眼的到底是不是城镇NPC?
            TownNPCIDs.RemoveAll(id =>
            {
                var npc = new NPC();
                npc.SetDefaults(id);
                int head = NPC.TypeToDefaultHeadIndex(id);
                return !npc.townNPC || head < 0 || head >= NPCHeadLoader.NPCHeadCount || NPCHeadID.Sets.CannotBeDrawnInHousingUI[head] || npc.ModNPC?.TownNPCStayingHomeless is true;
            });

            var modNPCs = typeof(NPCLoader).GetField("npcs", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as IList<ModNPC>;
            foreach (ModNPC modNPC in modNPCs)
            {
                var npc = modNPC.NPC;
                int head = NPC.TypeToDefaultHeadIndex(npc.type);
                if (npc.townNPC && head >= 0 && !NPCHeadID.Sets.CannotBeDrawnInHousingUI[head] && !modNPC.TownNPCStayingHomeless)
                {
                    TownNPCIDs.Add(npc.type);
                }
            }
        }

        private void TweakNPCSpawn(On.Terraria.Main.orig_UpdateTime_SpawnTownNPCs orig)
        {
            orig.Invoke();

            int moneyCount = 0;
            for (int l = 0; l < Main.maxPlayers; l++)
            {
                if (!Main.player[l].active)
                    continue;

                for (int m = 0; m < 40; m++)
                {
                    if (Main.player[l].bank.item[m] is null || Main.player[l].bank.item[m].stack <= 0)
                        continue;

                    if (moneyCount < 2000000000)
                    {
                        //Patch context: this is the amount of money.
                        int itemType = Main.player[l].bank.item[m].type;
                        if (itemType == ItemID.CopperCoin)
                            moneyCount += Main.player[l].bank.item[m].stack;

                        if (itemType == ItemID.SilverCoin)
                            moneyCount += Main.player[l].bank.item[m].stack * 100;

                        if (itemType == ItemID.GoldCoin)
                            moneyCount += Main.player[l].bank.item[m].stack * 10000;

                        if (itemType == ItemID.PlatinumCoin)
                            moneyCount += Main.player[l].bank.item[m].stack * 1000000;
                    }
                }
            }

            if (moneyCount > 5000)
                SetNPCSpawn(NPCID.Merchant);

            if (Config.TownNPCGetTFIntoHouse)
            {
                if (NPC.downedGoblins)
                    SetNPCSpawn(NPCID.GoblinTinkerer);
                if (NPC.downedBoss2)
                    SetNPCSpawn(NPCID.DD2Bartender);
                if (NPC.downedBoss3)
                    SetNPCSpawn(NPCID.Mechanic);
                if (Main.hardMode)
                {
                    SetNPCSpawn(NPCID.Wizard);
                    SetNPCSpawn(NPCID.TaxCollector);
                }

                if (TownNPCIDs is null || TownNPCIDs.Count < 0)
                {
                    SetupTownNPCList();
                }

                for (int i = 0; i < TownNPCIDs.Count; i++)
                {
                    int id = TownNPCIDs[i];
                    // 已经被解锁
                    if (Main.BestiaryTracker.Chats.GetWasChatWith(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[id]))
                    {
                        SetNPCSpawn(id);
                    }
                }
            }
        }

        public override void PostUpdateTime()
        {
            if (!Main.dayTime && Config.TownNPCSpawnInNight)
                SpawnTownNPCs.Invoke(null, null);
        }

        public static void SetNPCSpawn(int npcID)
        {
            if (NPC.AnyNPCs(npcID))
                return;

            Main.townNPCCanSpawn[npcID] = true;
            if (WorldGen.prioritizedTownNPCType == 0)
            {
                WorldGen.prioritizedTownNPCType = npcID;
            }
        }
    }
}
