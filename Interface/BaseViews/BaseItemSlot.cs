﻿using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;

namespace ImproveGame.Interface.BaseViews;

public class BaseItemSlot : TimerView
{
    public readonly Item AirItem = new Item();

    public virtual Item Item { get => AirItem; set { } }

    /// <summary>
    /// 允许显示物品信息
    /// </summary>
    public bool DisplayItemInfo;

    /// <summary>
    /// 允许显示物品堆叠数量
    /// </summary>
    public bool DisplayItemStack;

    public int ItemIconMaxWidthAndHeight = 32;

    public BaseItemSlot()
    {
        SetSizePixels(52f, 52f);
        SetRoundedRectangleValues(UIColor.ItemSlotBg, 2f, UIColor.ItemSlotBorder, new Vector4(12f));
    }

    /// <summary>
    /// 设置 BaseItemSlot 基本属性
    /// </summary>
    /// <param name="displayItemInfo"></param>
    /// <param name="displayItemStack"></param>
    public void SetBaseItemSlotValues(bool displayItemInfo, bool displayItemStack)
    {
        DisplayItemInfo = displayItemInfo;
        DisplayItemStack = displayItemStack;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (DisplayItemInfo && IsMouseHovering)
        {
            Main.hoverItemName = Item.Name;
            Main.HoverItem = Item.Clone();
        }

        BigBagItemSlot.DrawItemIcon(Main.spriteBatch, Item, Color.White, GetInnerDimensions(), ItemIconMaxWidthAndHeight);

        if (DisplayItemStack && Item.stack > 1)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(Item.stack.ToString()) * 0.75f;
            Vector2 textPos = pos + new Vector2(size.X * 0.18f, (size.Y - textSize.Y) * 0.9f);
            DrawString(textPos, Item.stack.ToString(), Color.White, Color.Black, 0.75f);
        }
    }
}
