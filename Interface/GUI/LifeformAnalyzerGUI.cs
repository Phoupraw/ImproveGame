﻿using ImproveGame.Common.Configs;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Patches;
using ImproveGame.Interface.UIElements;
using ImproveGame.Interface.SUIElements;
using System.Reflection;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;

namespace ImproveGame.Interface.GUI
{
    public class LifeformAnalyzerGUI : ViewBody
    {
        public static bool Visible { get; private set; }

        public override bool Display { get => Visible; set => Visible = value; }

        public override bool CanPriority(UIElement target) => target != this;

        public override bool CanDisableMouse(UIElement target)
            => (target != this && _basePanel.IsMouseHovering) || _basePanel.KeepPressed;

        private const float PanelLeft = 730f;
        private const float PanelTop = 160f;
        private const float PanelHeight = 330f;
        private const float PanelWidth = 390f;

        private SUIPanel _basePanel; // 背景板
        public SUICross CloseButton;
        public SUIScrollbar Scrollbar; // 拖动条
        public UIList UIList; // 明细列表
        private UISearchBar _searchBar;
        private SUIPanel _searchBoxPanel;
        private string _searchString;
        private bool _didClickSomething;
        private bool _didClickSearchBar;

        public override void OnInitialize()
        {
            Append(_basePanel = new SUIPanel(new Color(29, 34, 70), new Color(44, 57, 105, 160))
            {
                Shaded = true,
                ShadowThickness = 12,
                Draggable = true,
                Left = {Pixels = PanelLeft},
                Top = {Pixels = PanelTop},
                Width = {Pixels = PanelWidth},
                Height = {Pixels = PanelHeight},
            });

            _basePanel.Append(CloseButton = new SUICross
            {
                HAlign = 1f,
                Height = StyleDimension.FromPixels(28f),
                Width = StyleDimension.FromPixels(32f),
                Left = {Pixels = 4},
                BorderColor = Color.Transparent,
                BgColor = Color.Transparent
            });
            CloseButton.OnLeftMouseDown += (_, _) => Close();
            
            UIList = new UIList
            {
                Width = StyleDimension.FromPixelsAndPercent(-6f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(-32f, 1f),
                Top = StyleDimension.FromPixels(32f),
                PaddingBottom = 4f,
                PaddingTop = 4f,
                ListPadding = 4f,
            };
            UIList.SetPadding(2f);
            UIList.ManualSortMethod = _ => { };
            _basePanel.Append(UIList);

            Scrollbar = new()
            {
                Left = {Pixels = -20f, Percent = 1f},
                Top = {Pixels = 32f},
                Height = {Pixels = -32f, Percent = 1f}
            };
            Scrollbar.SetView(100f, 1000f);
            SetupScrollBar();
            _basePanel.Append(Scrollbar);
            
            UIElement searchArea = new()
            {
                Height = new StyleDimension(28f, 0f),
                Width = new StyleDimension(-34f, 1f)
            };
            searchArea.SetPadding(0f);
            _basePanel.Append(searchArea);
            AddSearchBar(searchArea);
            _searchBar.SetContents(null, forced: true);
        }

        #region 搜索栏

		private void AddSearchBar(UIElement searchArea) {
			UIImageButton uIImageButton = new(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search")) {
				VAlign = 0.5f,
				HAlign = 0f
			};

			uIImageButton.OnLeftClick += Click_SearchArea;
			uIImageButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search_Border"));
			uIImageButton.SetVisibility(1f, 1f);
			searchArea.Append(uIImageButton);
            SUIPanel uIPanel = _searchBoxPanel = new SUIPanel(new Color(35, 40, 83), new Color(35, 40, 83)) {
                Left = new StyleDimension(4f, 0f),
				Width = new StyleDimension(0f - uIImageButton.Width.Pixels - 3f, 1f),
				Height = new StyleDimension(0f, 1f),
				VAlign = 0.5f,
				HAlign = 1f
			};
			uIPanel.SetPadding(0f);
			searchArea.Append(uIPanel);
			UISearchBar uISearchBar = _searchBar = new UISearchBar(Language.GetText("UI.PlayerNameSlot"), 0.8f) {
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(0f, 1f),
				HAlign = 0f,
				VAlign = 0.5f,
				IgnoresMouseInteraction = true
			};

			uIPanel.OnLeftClick += Click_SearchArea;
			uISearchBar.OnContentsChanged += OnSearchContentsChanged;
			uIPanel.Append(uISearchBar);
			uISearchBar.OnStartTakingInput += OnStartTakingInput;
			uISearchBar.OnEndTakingInput += OnEndTakingInput;
			uISearchBar.OnCanceledTakingInput += OnCanceledInput;
			UIImageButton uIImageButton2 = new(Main.Assets.Request<Texture2D>("Images/UI/SearchCancel")) {
				HAlign = 1f,
				VAlign = 0.5f,
				Left = new StyleDimension(-2f, 0f)
			};

			uIImageButton2.OnMouseOver += SearchCancelButton_OnMouseOver;
			uIImageButton2.OnLeftClick += SearchCancelButton_OnClick;
			uIPanel.Append(uIImageButton2);
		}

		private void SearchCancelButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			if (_searchBar.HasContents) {
				_searchBar.SetContents(null, forced: true);
				SoundEngine.PlaySound(SoundID.MenuClose);
			}
			else {
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
		}

		private void SearchCancelButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(SoundID.MenuTick);
		}

		private void OnCanceledInput() {
			Main.LocalPlayer.ToggleInv();
		}

		private void Click_SearchArea(UIMouseEvent evt, UIElement listeningElement) {
			if (evt.Target.Parent != _searchBoxPanel) {
				_searchBar.ToggleTakingText();
                _didClickSearchBar = true;
			}
		}

        private void OnSearchContentsChanged(string contents) {
            _searchString = contents;
            SetupList();
        }

        private void OnStartTakingInput() {
            _searchBoxPanel.BorderColor = Main.OurFavoriteColor;
        }

        private void OnEndTakingInput() {
            _searchBoxPanel.BorderColor = new Color(35, 40, 83);
        }

        private void SetupScrollBar(bool resetViewPosition = true) {
            float height = UIList.GetInnerDimensions().Height;
            Scrollbar.SetView(height, UIList.GetTotalHeight());
            if (resetViewPosition)
                Scrollbar.ViewPosition = 0f;
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            if (_basePanel.GetOuterDimensions().ToRectangle().Contains(evt.MousePosition.ToPoint()))
                Scrollbar.BufferViewPosition += evt.ScrollWheelValue;
        }
        
        public override void LeftClick(UIMouseEvent evt) {
            base.LeftClick(evt);
            AttemptStoppingUsingSearchbar(evt);
        }

        private void AttemptStoppingUsingSearchbar(UIMouseEvent evt) {
            _didClickSomething = true;
        }

        #endregion

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (_didClickSomething && !_didClickSearchBar && _searchBar.IsWritingText)
                _searchBar.ToggleTakingText();

            _didClickSomething = false;
            _didClickSearchBar = false;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Scrollbar is not null)
            {
                UIList._innerList.Top.Set(-Scrollbar.ViewPosition, 0);
            }
            UIList.Recalculate();
            
            if (_basePanel.IsMouseHovering || Scrollbar.IsMouseHovering) {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: Lifeform Analyzer GUI");
            }

            base.DrawSelf(spriteBatch);
        }
        
        public void SetupList()
        {
            UIList.Clear();
            
            foreach (var npc in from npc in LifeAnalyzeCore.RaritiedNpcs orderby npc.rarity descending select npc) // 排序
            {
                if (string.IsNullOrEmpty(_searchString) || Lang.GetNPCNameValue(npc.netID).ToLower().Contains(_searchString.Trim().ToLower()))
                    UIList.Add(new LifeformTab(npc.netID));
            }

            Recalculate();
            SetupScrollBar();
        }
        
        public void Open()
        {
            _basePanel.KeepPressed = false;
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            SetupList();
        }
        
        public void Close()
        {
            Visible = false;
            Main.blockInput = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
            AdditionalConfig.Save();
        }
    }
}
