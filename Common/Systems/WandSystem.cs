﻿using Terraria.ModLoader;

namespace ImproveGame.Common.Systems
{
    public class WandSystem : ModPlayer
    {
        // 爆破法杖
        public static bool FixedMode = true;
        public static bool TileMode = true;
        public static bool WallMode = true;

        // 液体法杖
        public static bool AbsorptionMode = false;
        public static byte LiquidMode = 0;

        // 自动钓鱼机
        public static bool SelectPoolMode = false;

        // 画画魔杖
        public enum PaintMode : byte { Tile, Wall, Remove };
        public static PaintMode PaintWandMode = PaintMode.Tile;

        /// <summary>
        /// 切换吸收模式
        /// </summary>
        /// <returns>切换后当前模式</returns>
        public static bool ChangeAbsorptionMode() => AbsorptionMode = !AbsorptionMode;
    }
}
