﻿namespace ImproveGame.Interface.Common
{
    public class UIColor : ModSystem
    {
        public override void Load()
        {
            if (Main.dedServ)
                return;
            SetColorOne();
        }

        public static Color PanelBorder { get; private set; }
        public static Color PanelBackground { get; private set; }

        public static Color SlotFavoritedBorder { get; private set; }
        public static Color SlotNoFavoritedBorder { get; private set; }
        public static Color SlotFavoritedBackground { get; private set; }
        public static Color SlotNoFavoritedBackground { get; private set; }

        public static Color ButtonBackground { get; private set; }

        public static Color TitleBackground { get; private set; }

        public static Color ScrollBarBackground { get; private set; }

        // 边框
        public static Color SwitchBorder { get; private set; }
        public static Color SwitchBorderHover { get; private set; }
        // 圆
        public static Color SwitchRound { get; private set; }
        public static Color SwitchRoundHover { get; private set; }
        // 背景色
        public static Color SwitchBoxBackground { get; private set; }
        public static Color SwitchBackgroundHover { get; private set; }

        public static Color Fork { get; private set; }

        public static void SetColorOne()
        {
            PanelBorder = new Color(18, 18, 38);
            PanelBackground = new Color(44, 57, 105, 160);

            SlotFavoritedBorder = new Color(233, 176, 0, 200);
            SlotNoFavoritedBorder = new Color(18, 18, 38, 200);
            SlotFavoritedBackground = new Color(83, 88, 151, 200);
            SlotNoFavoritedBackground = new Color(63, 65, 151, 200);

            ButtonBackground = new Color(54, 56, 130);

            TitleBackground = new Color(35, 40, 83);

            ScrollBarBackground = new Color(44, 57, 105);

            // 边框
            SwitchBorder = new Color(22, 25, 55);
            SwitchBorderHover = new Color(233, 176, 0);
            // 圆
            SwitchRound = new Color(22, 25, 55);
            SwitchRoundHover = new Color(233, 176, 0);
            // 背景色
            SwitchBoxBackground = new Color(22, 25, 55, 127);
            SwitchBackgroundHover = new Color(72, 63, 63, 127);

            Fork = new Color(200, 40, 40);
        }

        public static void ModColorsTwo()
        {
            PanelBorder = new Color(27, 50, 57, 210);
            PanelBackground = new Color(11, 14, 15, 127);

            SlotFavoritedBorder = new Color(230, 230, 230, 200);
            SlotNoFavoritedBorder = new Color(27, 50, 57, 210);
            SlotFavoritedBackground = new Color(8, 25, 30, 100);
            SlotNoFavoritedBackground = new Color(11, 14, 15, 127);

            ButtonBackground = new Color(17, 40, 47);

            TitleBackground = new Color(8, 25, 30);

            // 滚动条
            ScrollBarBackground = new Color(11, 14, 15, 127);

            // 边框
            SwitchBorder = new Color(27, 50, 57, 210);
            SwitchBorderHover = new Color(230, 230, 230, 200);
            // 开关中的圆形
            SwitchRound = new Color(27, 50, 57, 210);
            SwitchRoundHover = new Color(230, 230, 230, 200);
            // 背景色
            SwitchBoxBackground = new Color(11, 14, 15, 127);
            SwitchBackgroundHover = new Color(11, 14, 15, 127);

            Fork = new Color(200, 40, 40);
        }
    }
}