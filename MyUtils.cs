﻿using ImproveGame.Common.Configs;
using ImproveGame.Entitys;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static ImproveGame.Common.GlobalItems.ImproveItem;
using static Microsoft.Xna.Framework.Vector2;

namespace ImproveGame
{
    /// <summary>
    /// 局长自用工具
    /// </summary>
    public class MyUtils
    {
        public static Color[] GetColors(Texture2D texture)
        {
            var w = texture.Width;
            var h = texture.Height;
            var cs = new Color[w * h]; // 创建一个能容下整个贴图颜色信息的 Color[]
            texture.GetData(cs); // 获取颜色信息
            return cs;
        }

        /// <summary>
        /// 旋转物品使用时候的贴图
        /// </summary>
        /// <param name="player"></param>
        public static void ItemRotation(Player player)
        {
            // 旋转物品
            Vector2 rotaion = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
            player.direction = Main.MouseWorld.X < player.Center.X ? -1 : 1;
            player.itemRotation = MathF.Atan2(rotaion.Y * player.direction, rotaion.X * player.direction);
        }

        /// <summary>
        /// 你猜干嘛用的，bongbong！！！
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void BongBong(Vector2 position, int width, int height)
        {
            if (Main.rand.NextBool(6))
            {
                Gore.NewGore(null, position + new Vector2(Main.rand.Next(16), Main.rand.Next(16)), Vector2.Zero, Main.rand.Next(61, 64));
            }
            if (Main.rand.NextBool(2))
            {
                int index = Dust.NewDust(position, width, height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                Main.dust[index].velocity *= 1.4f;
            }
            if (Main.rand.NextBool(3))
            {
                int index = Dust.NewDust(position, width, height, DustID.Torch, 0f, 0f, 100, default, 2.5f);
                Main.dust[index].noGravity = true;
                Main.dust[index].velocity *= 5f;
                index = Dust.NewDust(position, width, height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[index].velocity *= 3f;
            }
        }

        /// <summary>
        /// 限制 Rect 的大小
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Point LimitRect(Point start, Point end, int width, int height)
        {
            width--;
            height--;
            if (end.X - start.X < -width)
                end.X = start.X - width;
            else if (end.X - start.X > width)
                end.X = start.X + width;

            if (end.Y - start.Y < -height)
                end.Y = start.Y - height;
            else if (end.Y - start.Y > height)
                end.Y = start.Y + height;
            return end;
        }

        /// <summary>
        /// 尝试破坏物块，需要有镐子，并且挖的动。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="player"></param>
        /// <param name="BestPickaxe"></param>
        /// <returns></returns>
        public static bool TryKillTile(int x, int y, Player player, Item BestPickaxe)
        {
            if (!Main.tileHammer[Main.tile[x, y].TileType])
            {
                if (player.HasEnoughPickPowerToHurtTile(x, y) && WorldGen.CanKillTile(x, y))
                {
                    WorldGen.KillTile(x, y);
                }
            }
            return Main.tile[x, y].TileType == 0;
        }

        public static void DrawBorderRect(Rectangle tileRectangle, Color backgroundColor, Color borderColor)
        {
            Texture2D texture = TextureAssets.MagicPixel.Value;
            Vector2 position = tileRectangle.TopLeft() * 16f - Main.screenPosition;
            Vector2 scale = new(tileRectangle.Width, tileRectangle.Height);
            Main.spriteBatch.Draw(
                    texture,
                    position,
                    new(0, 0, 1, 1),
                    backgroundColor,
                    0f,
                    Zero,
                    16f * scale,
                    SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(
                texture,
                position + UnitX * -2f + UnitY * -2f,
                new(0, 0, 1, 1),
                borderColor, 0f, Zero,
                new Vector2(2f, 16f * scale.Y + 4),
                SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture,
                position + UnitX * 16f * scale.X + UnitY * -2f,
                new(0, 0, 1, 1),
                borderColor, 0f, Zero,
                new Vector2(2f, 16f * scale.Y + 4), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture,
                position + UnitY * -2f,
                new(0, 0, 1, 1),
                borderColor, 0f, Zero,
                new Vector2(16f * scale.X, 2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture,
                position + UnitY * 16f * scale.Y,
                new(0, 0, 1, 1),
                borderColor, 0f, Zero,
                new Vector2(16f * scale.X, 2f), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// 获取 HJson 文字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetText(string str)
        {
            return Language.GetTextValue($"Mods.ImproveGame.{str}");
        }

        public static Asset<Texture2D> GetTexture(string path)
        {
            return ModContent.Request<Texture2D>($"ImproveGame/Assets/Images/{path}", AssetRequestMode.ImmediateLoad);
        }

        /// <summary>
        /// 绘制一个方框
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="dimensions"></param>
        /// <param name="texture"></param>
        /// <param name="color"></param>
        public static void DrawPanel(SpriteBatch sb, CalculatedStyle dimensions, Texture2D texture, Color color)
        {
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Point point2 = new Point(point.X + (int)dimensions.Width - 12, point.Y + (int)dimensions.Height - 12);
            int width = point2.X - point.X - 12;
            int height = point2.Y - point.Y - 12;
            sb.Draw(texture, new Rectangle(point.X, point.Y, 12, 12), new Rectangle(0, 0, 12, 12), color);
            sb.Draw(texture, new Rectangle(point2.X, point.Y, 12, 12), new Rectangle(12 + 4, 0, 12, 12), color);
            sb.Draw(texture, new Rectangle(point.X, point2.Y, 12, 12), new Rectangle(0, 12 + 4, 12, 12), color);
            sb.Draw(texture, new Rectangle(point2.X, point2.Y, 12, 12), new Rectangle(12 + 4, 12 + 4, 12, 12), color);
            sb.Draw(texture, new Rectangle(point.X + 12, point.Y, width, 12), new Rectangle(12, 0, 4, 12), color);
            sb.Draw(texture, new Rectangle(point.X + 12, point2.Y, width, 12), new Rectangle(12, 12 + 4, 4, 12), color);
            sb.Draw(texture, new Rectangle(point.X, point.Y + 12, 12, height), new Rectangle(0, 12, 12, 4), color);
            sb.Draw(texture, new Rectangle(point2.X, point.Y + 12, 12, height), new Rectangle(12 + 4, 12, 12, 4), color);
            sb.Draw(texture, new Rectangle(point.X + 12, point.Y + 12, width, height), new Rectangle(12, 12, 4, 4), color);
        }

        /// <summary>
        /// 堆叠物品到仓库
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="item"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Item StackItemToInv(int plr, Item[] inv, Item item, GetItemSettings settings, bool hint = true)
        {
            List<int> list = new();
            // 先填充和物品相同的
            for (int i = 0; i < inv.Length; i++)
            {
                if (inv[i].type == item.type && inv[i].stack > 0 && inv[i].stack < inv[i].maxStack &&
                    ItemLoader.CanStack(inv[i], item))
                {
                    item = StackItemToItem(plr, inv, i, item, settings, hint);
                    if (item.type == ItemID.None)
                    {
                        return item;
                    }
                }
            }
            // 后填充空位
            for (int i = 0; i < inv.Length; i++)
            {
                if (inv[i].type == ItemID.None || inv[i].stack < 1)
                {
                    item = StackItemToItem(plr, inv, i, item, settings, hint);
                    if (item.type == ItemID.None)
                    {
                        return item;
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// 堆叠物品到仓库[i]
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="i"></param>
        /// <param name="cItem"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Item StackItemToItem(int plr, Item[] inv, int i, Item cItem, GetItemSettings settings, bool hint)
        {
            if (inv[i].type == ItemID.None)
            {
                if (hint)
                    PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, cItem, cItem.stack, noStack: false, settings.LongText);
                inv[i] = cItem;
                if (plr == Main.myPlayer)
                {
                    Recipe.FindRecipes();
                }
                return new Item();
            }
            if (inv[i].stack + cItem.stack > inv[i].maxStack)
            {
                if (hint)
                    PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, cItem, inv[i].maxStack - inv[i].stack, noStack: false, settings.LongText);
                cItem.stack -= inv[i].maxStack - inv[i].stack;
                inv[i].stack = inv[i].maxStack;
                if (plr == Main.myPlayer)
                {
                    Recipe.FindRecipes();
                }
                return cItem;
            }
            else
            {
                if (hint)
                    PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, cItem, cItem.stack, noStack: false, settings.LongText);
                inv[i].stack += cItem.stack;
                if (plr == Main.myPlayer)
                {
                    Recipe.FindRecipes();
                }
                return new Item();
            }
        }

        /// <summary>
        /// 判断指定 Item[] 中是否有 item 能用的空间
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasItemSpace(Item[] inv, Item item, int indexMax = 0)
        {
            for (int i = 0; i < (indexMax > 0 ? indexMax : inv.Length); i++)
            {
                if (inv[i].type == ItemID.None || (inv[i].type == item.type && inv[i].stack > 0 && inv[i].stack < inv[i].maxStack))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断指定 Item[] 中是否有 item
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasItem(Item[] inv, Item item, int indexMax = 0)
        {
            for (int i = 0; i < (indexMax > 0 ? indexMax : inv.Length); i++)
            {
                if (inv[i].type == item.type && inv[i].stack > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // 获取配置
        public static ImproveConfigs Config()
        {
            return ModContent.GetInstance<ImproveConfigs>();
        }

        /// <summary>
        /// 获取平台总数
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="count">平台的数量</param>
        /// <returns>是否有不会被消耗的平台</returns>
        public static bool GetPlatformCount(Item[] inv, ref int count)
        {
            bool consumable = true;
            for (int i = 0; i < 50; i++)
            {
                Item item = inv[i];
                if (item.createTile != -1 && TileID.Sets.Platforms[item.createTile])
                {
                    count += item.stack;
                    if (!item.consumable || !ItemLoader.ConsumeItem(item, Main.player[item.playerIndexTheItemIsReservedFor]))
                    {
                        consumable = false;
                    }
                }
            }
            return consumable;
        }

        // 获取墙总数
        public static bool GetWallCount(Item[] inv, ref int count)
        {
            bool consumable = true;
            for (int i = 0; i < 50; i++)
            {
                Item item = inv[i];
                if (item.createWall > 0)
                {
                    count += item.stack;
                    if (!item.consumable || !ItemLoader.ConsumeItem(item, Main.player[item.playerIndexTheItemIsReservedFor]))
                    {
                        consumable = false;
                    }
                }
            }
            return consumable;
        }

        // 获取背包第一个平台
        public static Item GetFirstPlatform(Player player)
        {
            for (int i = 0; i < 50; i++)
            {
                Item item = player.inventory[i];
                if (item.stack > 0 && item.createTile > -1 && TileID.Sets.Platforms[item.createTile])
                {
                    return item;
                }
            }
            return new Item();
        }

        // 获取背包第一个平台
        public static Item GetFirstWall(Player player)
        {
            for (int i = 0; i < 50; i++)
            {
                Item item = player.inventory[i];
                if (item.stack > 0 && item.createWall > 0)
                {
                    return item;
                }
            }
            return new Item();
        }

        // 加载前缀
        public static void LoadPrefixInfo()
        {
            //
            PrefixLevel.Add(1, 1);
            PrefixLevel.Add(2, 1);
            PrefixLevel.Add(3, 1);
            PrefixLevel.Add(4, 2);
            PrefixLevel.Add(5, 1);
            PrefixLevel.Add(6, 1);
            PrefixLevel.Add(7, 0);
            PrefixLevel.Add(8, 0);
            PrefixLevel.Add(9, 0);
            PrefixLevel.Add(10, 0);
            PrefixLevel.Add(11, 0);
            PrefixLevel.Add(12, 1);
            PrefixLevel.Add(13, 0);
            PrefixLevel.Add(14, 1);
            PrefixLevel.Add(15, 1);
            // 射手
            PrefixLevel.Add(16, 1);
            PrefixLevel.Add(17, 2);
            PrefixLevel.Add(18, 2);
            PrefixLevel.Add(19, 1);
            PrefixLevel.Add(20, 2);
            PrefixLevel.Add(21, 1);
            PrefixLevel.Add(22, 0);
            PrefixLevel.Add(23, 0);
            PrefixLevel.Add(24, 0);
            PrefixLevel.Add(25, 1);
            // 法师
            PrefixLevel.Add(26, 2);
            PrefixLevel.Add(27, 1);
            PrefixLevel.Add(28, 2);
            PrefixLevel.Add(29, 0);
            PrefixLevel.Add(30, 0);
            PrefixLevel.Add(31, 0);
            PrefixLevel.Add(32, 0);
            PrefixLevel.Add(33, 1);
            PrefixLevel.Add(34, 1);
            PrefixLevel.Add(35, 1);
            // 通用
            PrefixLevel.Add(36, 1);
            PrefixLevel.Add(37, 2);
            PrefixLevel.Add(38, 1);
            PrefixLevel.Add(39, 0);
            PrefixLevel.Add(40, 0);
            PrefixLevel.Add(41, 0);
            // 公共
            PrefixLevel.Add(42, 1);
            PrefixLevel.Add(43, 2);
            PrefixLevel.Add(44, 1);
            PrefixLevel.Add(45, 1);
            PrefixLevel.Add(46, 1);
            PrefixLevel.Add(47, 0);
            PrefixLevel.Add(48, 0);
            PrefixLevel.Add(49, 0);
            PrefixLevel.Add(50, 0);
            PrefixLevel.Add(51, 1);

            PrefixLevel.Add(52, 1);

            PrefixLevel.Add(53, 1);
            PrefixLevel.Add(54, 1);
            PrefixLevel.Add(55, 1);
            PrefixLevel.Add(56, 0);
            PrefixLevel.Add(57, 1);

            // 暴怒
            PrefixLevel.Add(58, 0);
            // 公共
            PrefixLevel.Add(59, 2);
            PrefixLevel.Add(60, 2);
            PrefixLevel.Add(61, 1);

            // 顶级前缀
            PrefixLevel.Add(81, 3);
            PrefixLevel.Add(82, 3);
            PrefixLevel.Add(83, 3);
            PrefixLevel.Add(84, 3);
            // 饰品
            PrefixLevel.Add(62, 1);
            PrefixLevel.Add(69, 1);
            PrefixLevel.Add(73, 1);
            PrefixLevel.Add(77, 1);
            PrefixLevel.Add(63, 2);
            PrefixLevel.Add(70, 2);
            PrefixLevel.Add(74, 2);
            PrefixLevel.Add(78, 2);
            PrefixLevel.Add(67, 2);
            PrefixLevel.Add(64, 3);
            PrefixLevel.Add(71, 3);
            PrefixLevel.Add(75, 3);
            PrefixLevel.Add(79, 3);
            PrefixLevel.Add(65, 4);
            PrefixLevel.Add(72, 4);
            PrefixLevel.Add(76, 4);
            PrefixLevel.Add(80, 4);
            PrefixLevel.Add(68, 4);
            PrefixLevel.Add(66, 4);
        }

        public delegate void MyDelegate(int i, int j);

        // 魔法移除物块方法
        public static void NormalKillTiles(Player player, Rectangle rect, MyDelegate myDelegate = null)
        {
            // 获得背包中最好的镐子
            Item item = player.GetBestPickaxe();
            int minI = rect.X;
            int maxI = rect.X + rect.Width - 1;
            int minJ = rect.Y;
            int maxJ = rect.Y + rect.Height - 1;
            for (int i = 0; i < player.hitTile.data.Length; i++)
            {
                HitTile.HitTileObject hitTileObject = player.hitTile.data[i];
                hitTileObject.timeToLive = 10000;
            }

            for (int i = minI; i <= maxI; i++)
            {
                for (int j = minJ; j <= maxJ; j++)
                {
                    if (myDelegate is not null)
                    {
                        myDelegate(i, j);
                    }
                    Tile tile = Main.tile[i, j];
                    if (!Main.tileAxe[tile.TileType] && !Main.tileHammer[tile.TileType])
                    {
                        player.PickTile(i, j, item != null ? item.pick : 1);
                        player.hitTile.data[player.hitTile.HitObject(i, j, 1)].timeToLive = 10000;
                    }
                }
            }
            for (int i = 0; i < player.hitTile.data.Length; i++)
            {
                HitTile.HitTileObject hitTileObject = player.hitTile.data[i];
                hitTileObject.timeToLive = 60;
            }
        }

        /// <summary>
        /// 遍历 Tile
        /// </summary>
        /// <param name="player"></param>
        /// <param name="rect"></param>
        /// <param name="myDelegate"></param>
        public static void ForechTile(Rectangle rect, MyDelegate myDelegate)
        {
            int minI = rect.X;
            int maxI = rect.X + rect.Width - 1;
            int minJ = rect.Y;
            int maxJ = rect.Y + rect.Height - 1;
            for (int i = minI; i <= maxI; i++)
            {
                for (int j = minJ; j <= maxJ; j++)
                {
                    myDelegate(i, j);
                }
            }
        }

        // 判断物块是否相同
        public static bool IsSameTile(int i, int j, int tileType, int tileStyle)
        {
            return (Main.tile[i, j].TileType == tileType && Main.tile[i, j].TileFrameY != tileStyle * 18)
                             || Main.tile[i, j].TileType != tileType;
        }

        public delegate bool JudgeItem(Item item);

        /// <summary>
        /// 判断有没有足量的此类物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="judge"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool EnoughItem(Player player, JudgeItem judge, int amount)
        {
            int num = 0;
            for (int i = 0; i < 50; i++)
            {
                Item item = player.inventory[i];
                if (item.type != ItemID.None && item.stack > 0 && judge(item))
                {
                    if (!item.consumable || !ItemLoader.ConsumeItem(item, player))
                        return true;
                    num += item.stack;
                }
            }
            if (num >= amount)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 消耗物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="judge"></param>
        /// <param name="amount"></param>
        public static void ConsumeItem(Player player, JudgeItem judge, int amount = 1)
        {
            for (int i = 0; i < 50; i++)
            {
                Item item = player.inventory[i];
                if (item.type != ItemID.None && item.stack > 0 && judge(item))
                {
                    if (item.consumable && ItemLoader.ConsumeItem(item, player))
                    {
                        if (item.stack >= amount)
                        {
                            item.stack -= amount;
                            amount = 0;
                        }
                        else
                        {
                            amount -= item.stack;
                            item.stack = 0;
                        }
                        if (item.stack < 1)
                            player.inventory[i] = new Item();
                        if (amount < 1)
                            return;
                    }
                }
            }
        }
    }
}