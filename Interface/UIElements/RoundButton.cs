﻿using ImproveGame.Common.Animations;

namespace ImproveGame.Interface.UIElements
{
    public class RoundButton : UIElement
    {
        public Asset<Texture2D> background;
        public Asset<Texture2D> hover;
        public Asset<Texture2D> mainImage;

        public float Opacity;

        public Func<bool> Selected;
        public Func<string> text;

        // 针不错, 全都加上渐变真不错!
        public AnimationTimer HoverTimer;
        public AnimationTimer SelectedTimer;

        public string Text => text?.Invoke();

        public RoundButton(Asset<Texture2D> mainImage)
        {
            background = GetTexture("UI/Brust/Background");
            hover = GetTexture("UI/Brust/Hover");

            this.SetSize(background.Size());
            this.mainImage = mainImage;

            HoverTimer = new(3);
            SelectedTimer = new(3);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverTimer.Update();
            SelectedTimer.Update();
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            HoverTimer.OpenAndReset();
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            HoverTimer.CloseAndReset();
        }

        public Color GetColor()
        {
            if (Selected())
                SelectedTimer.Open();
            else
                SelectedTimer.Close();
            return Color.Lerp(Color.Gray, Color.White, SelectedTimer.Schedule);
        }

        /*private readonly Color BorderColor1 = new(233, 176, 0, 200);
        private readonly Color BorderColor2 = new(18, 18, 38, 200);
        private readonly Color Background1 = new(83, 88, 151, 200);
        private readonly Color Background2 = new(63, 65, 151, 200);*/

        public override void DrawSelf(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position() + this.GetSize() / 2f;
            Color color = GetColor() * Opacity;

            sb.Draw(background.Value, position, null, color, 0, this.GetSize() / 2f, 1f, 0, 0f);
            // color * 1.4f => 高光边框贴图应该亮一点
            sb.Draw(hover.Value, position, null, color * 1.4f * HoverTimer.Schedule, 0, this.GetSize() / 2f, 1f, 0, 0f);

            /*Color borderColor = Color.Lerp(BorderColor2, BorderColor1, HoverTimer.Schedule);
            Color background = Color.Lerp(Background2, Background1, SelectedTimer.Schedule);

            PixelShader.DrawBox(Main.UIScaleMatrix, GetDimensions().Position(),
                this.GetSize(), Width.Pixels / 2, 3, borderColor * Opacity, background * Opacity);*/

            sb.Draw(mainImage.Value, position, null, color, 0, mainImage.Size() / 2f, 0.8f, 0, 0f);
        }
    }
}
