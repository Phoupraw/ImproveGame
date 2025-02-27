﻿using ImproveGame.Common.Configs;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;

namespace ImproveGame.Common.GlobalItems;

// 两个单词
// Grab 抓住（游戏内指代使物品飞向玩家的操作）
// Pickup 捡起

public class GrabAndPickup : GlobalItem
{
    public override void Load()
    {
        // 已废弃
        // On_Player.PickupItem += PickupItem;
        On_Player.GetItem += On_Player_GetItem;
    }

    /// <summary>
    /// 修改抓取速度 <br/>
    /// 禁用原版吸附速度返回: <see langword="true"/> <br/>
    /// 允许原版吸附速度返回 <see langword="false"/>
    /// </summary>
    public override bool GrabStyle(Item item, Player player)
    {
        if (Config.GrabDistance > 0)
        {
            item.velocity = Vector2.Normalize(player.Center - item.Center) * Math.Clamp(item.velocity.Length() + 1f, 0f, Math.Max(player.velocity.Length() + 5f, 15f));
            return true;
        }

        return false;
    }

    private Item On_Player_GetItem(On_Player.orig_GetItem orig, Player self, int plr, Item newItem, GetItemSettings settings)
    {
        newItem = orig.Invoke(self, plr, newItem, settings);

        if (settings.LongText == false && settings.NoText == false && settings.CanGoIntoVoidVault == true)
        {
            if (newItem.IsAir)
            {
                return newItem;
            }

            Item cloneItem = newItem.Clone();

            // 背包溢出堆叠至其他容器
            if (!newItem.IsACoin)
            {
                // 大背包
                if (Config.SuperVault && self.GetModPlayer<UIPlayerSetting>().SuperVault_OverflowGrab)
                {
                    newItem.StackToArray(self.GetModPlayer<DataPlayer>().SuperVault);
                }

                if (newItem.IsAir) goto Finish;

                // 猪猪 保险箱 ...
                if (Config.SuperVoidVault && self.TryGetModPlayer(out ImprovePlayer improvePlayer))
                {
                    if (improvePlayer.HasPiggyBank)
                    {
                        newItem.StackToArray(self.bank.item);
                    }

                    if (newItem.IsAir) goto Finish;

                    if (improvePlayer.HasSafe)
                    {
                        newItem.StackToArray(self.bank2.item);
                    }

                    if (newItem.IsAir) goto Finish;

                    if (improvePlayer.HasDefendersForge)
                    {
                        newItem.StackToArray(self.bank3.item);
                    }
                }
            }

            Finish:

            PickupPopupText(cloneItem, newItem);

            return newItem;
        }

        return newItem;
    }

    /// <summary>
    /// 拾取的物品溢出背包后, 已废弃
    /// </summary>
    /*private static Item PickupItem(On_Player.orig_PickupItem orig, Player player, int playerIndex, int worldItemArrayIndex, Item itemToPickUp)
    {
        itemToPickUp = orig(player, playerIndex, worldItemArrayIndex, itemToPickUp);

        if (itemToPickUp.IsAir)
        {
            return itemToPickUp;
        }

        Item cloneItem = itemToPickUp.Clone();

        // 背包溢出堆叠至其他容器
        if (!itemToPickUp.IsACoin)
        {
            // 大背包
            if (Config.SuperVault && player.GetModPlayer<UIPlayerSetting>().SuperVault_OverflowGrab)
            {
                itemToPickUp.StackToArray(player.GetModPlayer<DataPlayer>().SuperVault);
            }

            if (itemToPickUp.IsAir) goto Finish;

            // 猪猪 保险箱 ...
            if (Config.SuperVoidVault && player.TryGetModPlayer(out ImprovePlayer improvePlayer))
            {
                if (improvePlayer.HasPiggyBank)
                {
                    itemToPickUp.StackToArray(player.bank.item);
                }

                if (itemToPickUp.IsAir) goto Finish;

                if (improvePlayer.HasSafe)
                {
                    itemToPickUp.StackToArray(player.bank2.item);
                }

                if (itemToPickUp.IsAir) goto Finish;

                if (improvePlayer.HasDefendersForge)
                {
                    itemToPickUp.StackToArray(player.bank3.item);
                }
            }
        }

        Finish:

        if (itemToPickUp.stack < cloneItem.stack)
        {
            SoundEngine.PlaySound(SoundID.Grab);
        }

        Main.item[worldItemArrayIndex] = itemToPickUp;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, worldItemArrayIndex);
        }
        return itemToPickUp;
    }
*/

    /// <summary>
    /// 物品拾取后提示
    /// </summary>
    private static void PickupPopupText(Item cloneItem, Item self)
    {
        if (self.stack < cloneItem.stack)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, cloneItem, cloneItem.stack - self.stack);
        }
    }

    /// <summary>
    /// 允许你在玩家捡到一项物品时做一些特殊的事情 <br/>
    /// 返回 <see langword="false"/> 会阻止物品进入玩家的 inventoy，默认情况下返回 true。
    /// </summary>
    public override bool OnPickup(Item source, Player player)
    {
        if (UIConfigs.Instance.QoLAutoTrash &&
            player.TryGetModPlayer(out AutoTrashPlayer autoTrashPlayer) && true &&
            autoTrashPlayer.AutoDiscardItems.Any(adItem => adItem.type == source.type))
        {
            autoTrashPlayer.StackToLastItemsWithCleanUp(source);
            SoundEngine.PlaySound(SoundID.Grab);
            return false;
        }

        if (!player.TryGetModPlayer(out ImprovePlayer improvePlayer))
        {
            return true;
        }

        // 旗帜盒
        if (improvePlayer.BannerChest is not null && improvePlayer.BannerChest.AutoStorage && ItemToBanner(source) != -1)
        {
            Item cloneItem = source.Clone();
            improvePlayer.BannerChest.PutInPackage(ref source);
            PickupPopupText(cloneItem, source);
        }

        if (source.IsAir) return false;

        // 药水袋
        if (improvePlayer.PotionBag is not null && improvePlayer.PotionBag.AutoStorage && source.buffType > 0 && source.consumable)
        {
            Item item = source.Clone();
            improvePlayer.PotionBag.PutInPackage(ref source);
            PickupPopupText(item, source);
        }

        if (source.IsAir) return false;

        // 大背包
        if (Config.SuperVault &&
            player.TryGetModPlayer(out UIPlayerSetting uiPlayerSetting) && uiPlayerSetting.SuperVault_SmartGrab &&
            player.TryGetModPlayer(out DataPlayer dataPlayer) && source.TheArrayHas(dataPlayer.SuperVault))
        {
            Item cloneItem = source.Clone();
            source.StackToArray(dataPlayer.SuperVault);
            PickupPopupText(cloneItem, source);
        }

        if (source.IsAir) return false;

        // 虚空保险库 之 智能收纳
        if (Config.SmartVoidVault && !source.IsACoin)
        {
            // 虚空保险库
            if (player.IsVoidVaultEnabled && source.TheArrayHas(player.bank4.item))
            {
                Item cloneItem = source.Clone();
                source.StackToArray(player.bank4.item);
                PickupPopupText(cloneItem, source);
            }

            if (source.IsAir) return false;

            // 猪猪 保险箱 ...
            if (Config.SuperVoidVault)
            {
                if (improvePlayer.HasPiggyBank && source.TheArrayHas(player.bank.item))
                {
                    Item cloneItem = source.Clone();
                    source.StackToArray(player.bank.item);
                    PickupPopupText(cloneItem, source);
                }

                if (source.IsAir) return false;

                if (improvePlayer.HasSafe && source.TheArrayHas(player.bank2.item))
                {
                    Item cloneItem = source.Clone();
                    source.StackToArray(player.bank2.item);
                    PickupPopupText(cloneItem, source);
                }

                if (source.IsAir) return false;

                if (improvePlayer.HasDefendersForge && source.TheArrayHas(player.bank3.item))
                {
                    Item cloneItem = source.Clone();
                    source.StackToArray(player.bank3.item);
                    PickupPopupText(cloneItem, source);
                }

                if (source.IsAir) return false;
            }
        }

        return true;
    }

    // 抓取距离
    public override void GrabRange(Item item, Player player, ref int grabRange)
    {
        grabRange += Config.GrabDistance * 16;
    }

    public override bool ItemSpace(Item item, Player player)
    {
        if (item.IsACoin)
        {
            return false;
        }

        // 大背包
        if (Config.SuperVault &&
            player.TryGetModPlayer(out UIPlayerSetting uiPlayerSetting) &&
            player.TryGetModPlayer(out DataPlayer dataPlayer))
        {
            if (uiPlayerSetting.SuperVault_OverflowGrab &&
                item.CanStackToArray(dataPlayer.SuperVault))
            {
                return true;
            }
            else if (uiPlayerSetting.SuperVault_SmartGrab &&
                item.CanStackToArray(dataPlayer.SuperVault) &&
                item.TheArrayHas(dataPlayer.SuperVault))
            {
                return true;
            }
        }

        if (Config.SuperVoidVault && player.TryGetModPlayer<ImprovePlayer>(out var improvePlayer))
        {
            if (improvePlayer.HasPiggyBank = improvePlayer.HasPiggyBank && item.CanStackToArray(player.bank.item))
            {
                return true;
            }

            if (improvePlayer.HasSafe = improvePlayer.HasSafe && item.CanStackToArray(player.bank2.item))
            {
                return true;
            }

            if (improvePlayer.HasDefendersForge = improvePlayer.HasDefendersForge && item.CanStackToArray(player.bank3.item))
            {
                return true;
            }
        }

        return false;
    }
}
