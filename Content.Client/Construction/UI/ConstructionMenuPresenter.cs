// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 Clyybber <darkmine956@gmail.com>
// SPDX-FileCopyrightText: 2021 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Jezithyr <Jezithyr.@gmail.com>
// SPDX-FileCopyrightText: 2022 Morb <14136326+Morb0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 08A <git@08a.re>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 PixelTK <85175107+PixelTheKermit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2024 Artjom <artjombebenin@gmail.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using System.Numerics;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Whitelist;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Placement;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Utility;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using static Robust.Client.UserInterface.Controls.BaseButton;

namespace Content.Client.Construction.UI
{
    /// <summary>
    /// This class presents the Construction/Crafting UI to the client, linking the <see cref="ConstructionSystem" /> with the
    /// model. This is where the bulk of UI work is done, either calling functions in the model to change state, or collecting
    /// data out of the model to *present* to the screen though the UI framework.
    /// </summary>
    internal sealed class ConstructionMenuPresenter : IDisposable
    {
        [Dependency] private readonly EntityManager _entManager = default!;
        [Dependency] private readonly IEntitySystemManager _systemManager = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IPlacementManager _placementManager = default!;
        [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;

        private readonly IConstructionMenuView _constructionView;
        private readonly EntityWhitelistSystem _whitelistSystem;
        private readonly SpriteSystem _spriteSystem;

        private ConstructionSystem? _constructionSystem;
        private ConstructionPrototype? _selected;
        private List<ConstructionPrototype> _favoritedRecipes = [];
        private Dictionary<string, TextureButton> _recipeButtons = new();
        private string _selectedCategory = string.Empty;
        private string _favoriteCatName = "construction-category-favorites";
        private string _forAllCategoryName = "construction-category-all";
        private bool CraftingAvailable
        {
            get => _uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.Visible;
            set
            {
                _uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.Visible = value;
                if (!value)
                    _constructionView.Close();
            }
        }

        /// <summary>
        /// Does the window have focus? If the window is closed, this will always return false.
        /// </summary>
        private bool IsAtFront => _constructionView.IsOpen && _constructionView.IsAtFront();

        private bool WindowOpen
        {
            get => _constructionView.IsOpen;
            set
            {
                if (value && CraftingAvailable)
                {
                    if (_constructionView.IsOpen)
                        _constructionView.MoveToFront();
                    else
                        _constructionView.OpenCentered();

                    if (_selected != null)
                        PopulateInfo(_selected);
                }
                else
                    _constructionView.Close();
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="ConstructionMenuPresenter" />.
        /// </summary>
        public ConstructionMenuPresenter()
        {
            // This is a lot easier than a factory
            IoCManager.InjectDependencies(this);
            _constructionView = new ConstructionMenu();
            _whitelistSystem = _entManager.System<EntityWhitelistSystem>();
            _spriteSystem = _entManager.System<SpriteSystem>();

            // This is required so that if we load after the system is initialized, we can bind to it immediately
            if (_systemManager.TryGetEntitySystem<ConstructionSystem>(out var constructionSystem))
                SystemBindingChanged(constructionSystem);

            _systemManager.SystemLoaded += OnSystemLoaded;
            _systemManager.SystemUnloaded += OnSystemUnloaded;

            _placementManager.PlacementChanged += OnPlacementChanged;

            _constructionView.OnClose += () => _uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.Pressed = false;
            _constructionView.ClearAllGhosts += (_, _) => _constructionSystem?.ClearAllGhosts();
            _constructionView.PopulateRecipes += OnViewPopulateRecipes;
            _constructionView.RecipeSelected += OnViewRecipeSelected;
            _constructionView.BuildButtonToggled += (_, b) => BuildButtonToggled(b);
            _constructionView.EraseButtonToggled += (_, b) =>
            {
                if (_constructionSystem is null) return;
                if (b) _placementManager.Clear();
                _placementManager.ToggleEraserHijacked(new ConstructionPlacementHijack(_constructionSystem, null));
                _constructionView.EraseButtonPressed = b;
            };

            _constructionView.RecipeFavorited += (_, _) => OnViewFavoriteRecipe();

            PopulateCategories();
            OnViewPopulateRecipes(_constructionView, (string.Empty, string.Empty));
        }

        public void OnHudCraftingButtonToggled(ButtonToggledEventArgs args)
        {
            WindowOpen = args.Pressed;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _constructionView.Dispose();

            SystemBindingChanged(null);
            _systemManager.SystemLoaded -= OnSystemLoaded;
            _systemManager.SystemUnloaded -= OnSystemUnloaded;

            _placementManager.PlacementChanged -= OnPlacementChanged;
        }

        private void OnPlacementChanged(object? sender, EventArgs e)
        {
            _constructionView.ResetPlacement();
        }

        private void OnViewRecipeSelected(object? sender, ItemList.Item? item)
        {
            if (item is null)
            {
                _selected = null;
                _constructionView.ClearRecipeInfo();
                return;
            }

            _selected = (ConstructionPrototype) item.Metadata!;
            if (_placementManager.IsActive && !_placementManager.Eraser) UpdateGhostPlacement();
            PopulateInfo(_selected);
        }

        private void OnGridViewRecipeSelected(object? sender, ConstructionPrototype? recipe)
        {
            if (recipe is null)
            {
                _selected = null;
                _constructionView.ClearRecipeInfo();
                return;
            }

            _selected = recipe;
            if (_placementManager.IsActive && !_placementManager.Eraser) UpdateGhostPlacement();
            PopulateInfo(_selected);
        }

        private void OnViewPopulateRecipes(object? sender, (string search, string catagory) args)
        {
            var (search, category) = args;

            var recipes = new List<ConstructionPrototype>();

            var isEmptyCategory = string.IsNullOrEmpty(category) || category == _forAllCategoryName;

            if (isEmptyCategory)
                _selectedCategory = string.Empty;
            else
                _selectedCategory = category;

            foreach (var recipe in _prototypeManager.EnumeratePrototypes<ConstructionPrototype>())
            {
                if (recipe.Hide)
                    continue;

                if (_playerManager.LocalSession == null
                || _playerManager.LocalEntity == null
                || _whitelistSystem.IsWhitelistFail(recipe.EntityWhitelist, _playerManager.LocalEntity.Value))
                    continue;

                if (!string.IsNullOrEmpty(search))
                {
                    if (!recipe.Name.ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
                        continue;
                }

                if (!isEmptyCategory)
                {
                    if (category == _favoriteCatName)
                    {
                        if (!_favoritedRecipes.Contains(recipe))
                        {
                            continue;
                        }
                    }
                    else if (recipe.Category != category)
                    {
                        continue;
                    }
                }

                recipes.Add(recipe);
            }

            recipes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCulture));

            var recipesList = _constructionView.Recipes;
            recipesList.Clear();

            var recipesGrid = _constructionView.RecipesGrid;
            recipesGrid.RemoveAllChildren();

            _constructionView.RecipesGridScrollContainer.Visible = _constructionView.GridViewButtonPressed;
            _constructionView.Recipes.Visible = !_constructionView.GridViewButtonPressed;

            if (_constructionView.GridViewButtonPressed)
            {
                foreach (var recipe in recipes)
                {
                    var itemButton = new TextureButton
                    {
                        TextureNormal = _spriteSystem.Frame0(recipe.Icon),
                        VerticalAlignment = Control.VAlignment.Center,
                        Name = recipe.Name,
                        ToolTip = recipe.Name,
                        Scale = new Vector2(1.35f),
                        ToggleMode = true,
                    };
                    var itemButtonPanelContainer = new PanelContainer
                    {
                        PanelOverride = new StyleBoxFlat { BackgroundColor = StyleNano.ButtonColorDefault },
                        Children = { itemButton },
                    };

                    itemButton.OnToggled += buttonToggledEventArgs =>
                    {
                        SelectGridButton(itemButton, buttonToggledEventArgs.Pressed);

                        if (buttonToggledEventArgs.Pressed &&
                            _selected != null &&
                            _recipeButtons.TryGetValue(_selected.Name, out var oldButton))
                        {
                            oldButton.Pressed = false;
                            SelectGridButton(oldButton, false);
                        }

                        OnGridViewRecipeSelected(this, buttonToggledEventArgs.Pressed ? recipe : null);
                    };

                    recipesGrid.AddChild(itemButtonPanelContainer);
                    _recipeButtons[recipe.Name] = itemButton;
                    var isCurrentButtonSelected = _selected == recipe;
                    itemButton.Pressed = isCurrentButtonSelected;
                    SelectGridButton(itemButton, isCurrentButtonSelected);
                }
            }
            else
            {
                foreach (var recipe in recipes)
                {
                    recipesList.Add(GetItem(recipe, recipesList));
                }
            }
        }

        private void SelectGridButton(TextureButton button, bool select)
        {
            if (button.Parent is not PanelContainer buttonPanel)
                return;

            button.Modulate = select ? Color.Green : Color.White;
            var buttonColor = select ? StyleNano.ButtonColorDefault : Color.Transparent;
            buttonPanel.PanelOverride = new StyleBoxFlat { BackgroundColor = buttonColor };
        }

        private void PopulateCategories(string? selectCategory = null)
        {
            var uniqueCategories = new HashSet<string>();

            foreach (var prototype in _prototypeManager.EnumeratePrototypes<ConstructionPrototype>())
            {
                var category = prototype.Category;

                if (!string.IsNullOrEmpty(category))
                    uniqueCategories.Add(category);
            }

            var isFavorites = _favoritedRecipes.Count > 0;
            var categoriesArray = new string[isFavorites ? uniqueCategories.Count + 2 : uniqueCategories.Count + 1];

            // hard-coded to show all recipes
            var idx = 0;
            categoriesArray[idx++] = _forAllCategoryName;

            // hard-coded to show favorites if it need
            if (isFavorites)
            {
                categoriesArray[idx++] = _favoriteCatName;
            }

            var sortedProtoCategories = uniqueCategories.OrderBy(Loc.GetString);

            foreach (var cat in sortedProtoCategories)
            {
                categoriesArray[idx++] = cat;
            }

            _constructionView.OptionCategories.Clear();

            for (var i = 0; i < categoriesArray.Length; i++)
            {
                _constructionView.OptionCategories.AddItem(Loc.GetString(categoriesArray[i]), i);

                if (!string.IsNullOrEmpty(selectCategory) && selectCategory == categoriesArray[i])
                    _constructionView.OptionCategories.SelectId(i);

            }

            _constructionView.Categories = categoriesArray;
        }

        private void PopulateInfo(ConstructionPrototype prototype)
        {
            _constructionView.ClearRecipeInfo();

            _constructionView.SetRecipeInfo(
                prototype.Name, prototype.Description, _spriteSystem.Frame0(prototype.Icon),
                prototype.Type != ConstructionType.Item,
                !_favoritedRecipes.Contains(prototype));

            var stepList = _constructionView.RecipeStepList;
            GenerateStepList(prototype, stepList);
        }

        private void GenerateStepList(ConstructionPrototype prototype, ItemList stepList)
        {
            if (_constructionSystem?.GetGuide(prototype) is not { } guide)
                return;


            foreach (var entry in guide.Entries)
            {
                var text = entry.Arguments != null
                    ? Loc.GetString(entry.Localization, entry.Arguments) : Loc.GetString(entry.Localization);

                if (entry.EntryNumber is { } number)
                {
                    text = Loc.GetString("construction-presenter-step-wrapper",
                        ("step-number", number), ("text", text));
                }

                // The padding needs to be applied regardless of text length... (See PadLeft documentation)
                text = text.PadLeft(text.Length + entry.Padding);

                var icon = entry.Icon != null ? _spriteSystem.Frame0(entry.Icon) : Texture.Transparent;
                stepList.AddItem(text, icon, false);
            }
        }

        private ItemList.Item GetItem(ConstructionPrototype recipe, ItemList itemList)
        {
            return new(itemList)
            {
                Metadata = recipe,
                Text = recipe.Name,
                Icon = _spriteSystem.Frame0(recipe.Icon),
                TooltipEnabled = true,
                TooltipText = recipe.Description,
            };
        }

        private void BuildButtonToggled(bool pressed)
        {
            if (pressed)
            {
                if (_selected == null) return;

                // not bound to a construction system
                if (_constructionSystem is null)
                {
                    _constructionView.BuildButtonPressed = false;
                    return;
                }

                if (_selected.Type == ConstructionType.Item)
                {
                    _constructionSystem.TryStartItemConstruction(_selected.ID);
                    _constructionView.BuildButtonPressed = false;
                    return;
                }

                _placementManager.BeginPlacing(new PlacementInformation
                {
                    IsTile = false,
                    PlacementOption = _selected.PlacementMode
                }, new ConstructionPlacementHijack(_constructionSystem, _selected));

                UpdateGhostPlacement();
            }
            else
                _placementManager.Clear();

            _constructionView.BuildButtonPressed = pressed;
        }

        private void UpdateGhostPlacement()
        {
            if (_selected == null)
                return;

            if (_selected.Type != ConstructionType.Structure)
            {
                _placementManager.Clear();
                return;
            }

            var constructSystem = _systemManager.GetEntitySystem<ConstructionSystem>();

            _placementManager.BeginPlacing(new PlacementInformation()
            {
                IsTile = false,
                PlacementOption = _selected.PlacementMode,
            }, new ConstructionPlacementHijack(constructSystem, _selected));

            _constructionView.BuildButtonPressed = true;
        }

        private void OnSystemLoaded(object? sender, SystemChangedArgs args)
        {
            if (args.System is ConstructionSystem system) SystemBindingChanged(system);
        }

        private void OnSystemUnloaded(object? sender, SystemChangedArgs args)
        {
            if (args.System is ConstructionSystem) SystemBindingChanged(null);
        }

        private void OnViewFavoriteRecipe()
        {
            if (_selected is not ConstructionPrototype recipe)
                return;

            if (!_favoritedRecipes.Remove(_selected))
                _favoritedRecipes.Add(_selected);

            if (_selectedCategory == _favoriteCatName)
            {
                if (_favoritedRecipes.Count > 0)
                    OnViewPopulateRecipes(_constructionView, (string.Empty, _favoriteCatName));
                else
                    OnViewPopulateRecipes(_constructionView, (string.Empty, string.Empty));
            }

            PopulateInfo(_selected);
            PopulateCategories(_selectedCategory);
        }

        private void SystemBindingChanged(ConstructionSystem? newSystem)
        {
            if (newSystem is null)
            {
                if (_constructionSystem is null)
                    return;

                UnbindFromSystem();
            }
            else
            {
                if (_constructionSystem is null)
                {
                    BindToSystem(newSystem);
                    return;
                }

                UnbindFromSystem();
                BindToSystem(newSystem);
            }
        }

        private void BindToSystem(ConstructionSystem system)
        {
            _constructionSystem = system;
            system.ToggleCraftingWindow += SystemOnToggleMenu;
            system.FlipConstructionPrototype += SystemFlipConstructionPrototype;
            system.CraftingAvailabilityChanged += SystemCraftingAvailabilityChanged;
            system.ConstructionGuideAvailable += SystemGuideAvailable;
            if (_uiManager.GetActiveUIWidgetOrNull<GameTopMenuBar>() != null)
            {
                CraftingAvailable = system.CraftingEnabled;
            }
        }

        private void UnbindFromSystem()
        {
            var system = _constructionSystem;

            if (system is null)
                throw new InvalidOperationException();

            system.ToggleCraftingWindow -= SystemOnToggleMenu;
            system.FlipConstructionPrototype -= SystemFlipConstructionPrototype;
            system.CraftingAvailabilityChanged -= SystemCraftingAvailabilityChanged;
            system.ConstructionGuideAvailable -= SystemGuideAvailable;
            _constructionSystem = null;
        }

        private void SystemCraftingAvailabilityChanged(object? sender, CraftingAvailabilityChangedArgs e)
        {
            if (_uiManager.ActiveScreen == null)
                return;
            CraftingAvailable = e.Available;
        }

        private void SystemOnToggleMenu(object? sender, EventArgs eventArgs)
        {
            if (!CraftingAvailable)
                return;

            if (WindowOpen)
            {
                if (IsAtFront)
                {
                    WindowOpen = false;
                    _uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.SetClickPressed(false); // This does not call CraftingButtonToggled
                }
                else
                    _constructionView.MoveToFront();
            }
            else
            {
                WindowOpen = true;
                _uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.SetClickPressed(true); // This does not call CraftingButtonToggled
            }
        }

        private void SystemFlipConstructionPrototype(object? sender, EventArgs eventArgs)
        {
            if (!_placementManager.IsActive || _placementManager.Eraser)
            {
                return;
            }

            if (_selected == null || _selected.Mirror == null)
            {
                return;
            }

            _selected = _prototypeManager.Index<ConstructionPrototype>(_selected.Mirror);
            UpdateGhostPlacement();
        }

        private void SystemGuideAvailable(object? sender, string e)
        {
            if (!CraftingAvailable)
                return;

            if (!WindowOpen)
                return;

            if (_selected == null)
                return;

            PopulateInfo(_selected);
        }
    }
}
