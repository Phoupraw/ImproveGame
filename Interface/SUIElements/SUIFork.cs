﻿using ImproveGame.Common.Animations;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.SUIElements
{
    public class SUIFork : HoverSUIE
    {
        public float forkSize;
        public float radius;
        public float border;

        public SUIFork(float forkSize)
        {
            radius = 3.7f;
            border = 2;
            this.forkSize = forkSize;
            Width.Pixels = forkSize + 20;
            Height.Pixels = forkSize + 10;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            Color background = Color.Lerp(UIColor.Default.TitleBackground * 0.5f, UIColor.Default.TitleBackground * 1f, hoverTimer.Schedule);
            Color fork = Color.Lerp(Color.Transparent, UIColor.Default.CloseBackground, hoverTimer.Schedule);

            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();
            PixelShader.DrawRoundRect(pos, size, 10f, background, 3f, UIColor.Default.PanelBorder);
            Vector2 forkPos = pos + size / 2 - new Vector2(forkSize / 2);
            PixelShader.DrawFork(forkPos, forkSize, radius, fork, border, UIColor.Default.PanelBorder);
        }
    }
}
