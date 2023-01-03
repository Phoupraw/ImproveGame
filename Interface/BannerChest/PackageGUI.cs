﻿using ImproveGame.Interface.BannerChest.Elements;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.BannerChest
{
    public class PackageGUI : UIState
    {
        public enum StorageType { Banners, Potions }
        private static bool visible;
        public static StorageType storageType;
        public IPackageItem package;

        public static bool Visible
        {
            get
            {
                if (!Main.playerInventory)
                    visible = false;
                return visible;
            }
            set => visible = value;
        }

        public SUIPanel mainPanel;
        public SUITitle title;
        public SUISwitch checkbox;
        public SUISwitch checkbox2;
        public SUICross fork;
        public SUIPanel gridPanel;
        public PackageGrid grid;

        public override void OnInitialize()
        {
            Append(mainPanel = new SUIPanel(UIColor.PackgeBorder, UIColor.PackgeBg)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Shaded = true,
                Draggable = true
            });

            mainPanel.Append(title = new SUITitle("中文|Chinese", 0.5f) { PaddingLeft = 25, PaddingRight = 25 });

            mainPanel.Append(checkbox = new SUISwitch(() => package.AutoStorage, state => package.AutoStorage = state, GetText("PackageGUI.AutoStorage"), 0.8f));
            checkbox.Top.Pixels = title.Bottom() + 8f;

            mainPanel.Append(checkbox2 = new SUISwitch(() => package.AutoSort, state => package.AutoSort = state, GetText("PackageGUI.AutoSort"), 0.8f));
            checkbox2.Top.Pixels = checkbox.Top();
            checkbox2.Left.Pixels = checkbox.Right() + 8f;

            mainPanel.Append(fork = new SUICross(30) { HAlign = 1f });
            fork.Height.Pixels = title.Height();
            fork.OnMouseDown += (_, _) => Close();

            mainPanel.Append(gridPanel = new SUIPanel(UIColor.PackgeGridBorder, UIColor.PackgeGridBg, 12, 3, false));
            gridPanel.SetPadding(10);
            gridPanel.Append(grid = new());
            gridPanel.OnMouseDown += GridPanel_OnMouseDown;
            gridPanel.Top.Pixels = checkbox.Bottom() + 8f;
            gridPanel.Width.Pixels = grid.Width.Pixels + gridPanel.HPadding();
            gridPanel.Height.Pixels = grid.Height.Pixels + gridPanel.VPadding();

            mainPanel.Width.Pixels = gridPanel.Width.Pixels + mainPanel.HPadding();
            mainPanel.Height.Pixels = gridPanel.Bottom() + mainPanel.VPadding();
        }

        private void GridPanel_OnMouseDown(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.mouseItem.IsAir)
                return;

            // 旗帜收纳箱
            if (storageType is StorageType.Banners && ItemToBanner(Main.mouseItem) != -1)
                package.PutInPackage(ref Main.mouseItem);
            // 药水袋子
            else if (storageType is StorageType.Potions && Main.mouseItem.buffType > 0 && Main.mouseItem.consumable)
                package.PutInPackage(ref Main.mouseItem);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (mainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: Package GUI");
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public void Open(List<Item> items, string title, StorageType storageType, IPackageItem package)
        {
            PackageGUI.storageType = storageType;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.playerInventory = true;
            Visible = true;
            grid.SetInventory(items);
            grid.scrollbar.ViewPosition = 0;
            this.title.Text = title;
            this.title.RefreshSize();
            this.package = package;
            Recalculate();
        }

        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Visible = false;
        }
    }
}