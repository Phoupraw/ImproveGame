﻿namespace ImproveGame.Interface.ExtremeStorage
{
    public class GroupTab : View
    {
        private readonly Asset<Texture2D> _tabTexture;
        private readonly Asset<Texture2D> _groupTexture;
        private readonly ItemGroup _group;
        private readonly Rectangle _groupFrame;
        private Rectangle _tabFrame;

        public GroupTab(ItemGroup group)
        {
            // 作为定位和frame的x值
            int x = group switch
            {
                ItemGroup.Weapon => 0,
                ItemGroup.Tool => 1,
                ItemGroup.Ammo => 2,
                ItemGroup.Armor => 3,
                ItemGroup.Accessory => 4,
                ItemGroup.Furniture => 5,
                ItemGroup.Block => 6,
                ItemGroup.Material => 7,
                ItemGroup.Alchemy => 8,
                ItemGroup.Misc => 9,
                ItemGroup.Setting => 10,
                _ => 0 // 不可能的情况
            };

            this.SetSize(42f, 48f);
            this.SetPos(x * 40 + 8, 0f);

            _group = group;
            _tabTexture = Main.Assets.Request<Texture2D>("Images/UI/Creative/Infinite_Tabs_B");
            _groupTexture = GetTexture("UI/ExtremeStorage/Icons");
            _tabFrame = new Rectangle(0, 0, 42, 48);
            _groupFrame = new Rectangle(x * 30, 0, 28, 28);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            var dimensions = GetDimensions();
            var pos = dimensions.Position();
            var center = dimensions.Center() - new Vector2(0f, 4f);
            spriteBatch.Draw(position: pos, texture: _tabTexture.Value, sourceRectangle: _tabFrame, color: Color.White);
            spriteBatch.Draw(_groupTexture.Value, center, _groupFrame, Color.White, 0f, _groupFrame.Size() / 2f, 1f,
                SpriteEffects.None, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bool mouseHover = IsMouseHovering;
            bool groupSelected = ExtremeStorageGUI.CurrentGroup == _group;

            if (!mouseHover && !groupSelected)
                _tabFrame = _tabTexture.Frame(2, 4, 0, 0).OffsetSize(-2, -2);
            if (mouseHover && !groupSelected)
                _tabFrame = _tabTexture.Frame(2, 4, 0, 1).OffsetSize(-2, -2);
            if (!mouseHover && groupSelected)
                _tabFrame = _tabTexture.Frame(2, 4, 1, 2).OffsetSize(-2, -2);
            if (mouseHover && groupSelected)
                _tabFrame = _tabTexture.Frame(2, 4, 1, 3).OffsetSize(-2, -2);

            if (IsMouseHovering)
                Main.instance.MouseText(GetText($"UI.ExtremeStorage.ItemGroup.{_group}"));
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            ExtremeStorageGUI.SetGroup(_group);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
}