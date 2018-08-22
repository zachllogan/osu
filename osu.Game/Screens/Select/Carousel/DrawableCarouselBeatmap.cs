// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.States;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Configuration;
using System.Collections.Generic;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets;
using System.Linq;

namespace osu.Game.Screens.Select.Carousel
{
    public class DrawableCarouselBeatmap : DrawableCarouselItem, IHasContextMenu
    {
        private readonly BeatmapInfo beatmap;

        private Sprite background;

        private Action<BeatmapInfo> startRequested;
        private Action<BeatmapInfo> editRequested;
        private Action<BeatmapInfo> hideRequested;

        private Triangles triangles;
        private StarCounter starCounter;

        private BeatmapSetOverlay beatmapOverlay;

        public DrawableCarouselBeatmap(CarouselBeatmap panel) : base(panel)
        {
            beatmap = panel.Beatmap;
            Height *= 0.60f;
        }

        protected Ruleset Instance;

        protected readonly Bindable<IEnumerable<Mod>> SelectedMods = new Bindable<IEnumerable<Mod>>(new Mod[] { });

        protected readonly IBindable<RulesetInfo> Ruleset = new Bindable<RulesetInfo>();

        protected WorkingBeatmap Beatmap;

        [BackgroundDependencyLoader(true)]
        private void load(SongSelect songSelect, BeatmapManager manager, BeatmapSetOverlay beatmapOverlay, IBindable<RulesetInfo> ruleset, Bindable<IEnumerable<Mod>> selectedMods)
        {
            this.beatmapOverlay = beatmapOverlay;

            if (songSelect != null)
            {
                startRequested = b => songSelect.FinaliseSelection(b);
                editRequested = songSelect.Edit;
            }

            if (manager != null)
                hideRequested = manager.Hide;

            Ruleset.BindTo(ruleset);
            if (selectedMods != null) SelectedMods.BindTo(selectedMods);
            Beatmap = manager?.GetWorkingBeatmap(beatmap, Beatmap);
            Instance = Ruleset.Value.CreateInstance();

            float starCount = (float)beatmap.StarDifficulty;
            if (!(Beatmap == null || beatmap.RulesetID != 0 && beatmap.RulesetID != Ruleset.Value.ID))
            {
                starCount = (float)Instance.CreateDifficultyCalculator(Beatmap).Calculate(SelectedMods.Value.ToArray()).StarRating;
            }
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                },
                triangles = new Triangles
                {
                    TriangleScale = 2,
                    RelativeSizeAxes = Axes.Both,
                    ColourLight = OsuColour.FromHex(@"3a7285"),
                    ColourDark = OsuColour.FromHex(@"123744")
                },
                new FillFlowContainer
                {
                    Padding = new MarginPadding(5),
                    Direction = FillDirection.Horizontal,
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Children = new Drawable[]
                    {
                        new DifficultyIcon(beatmap)
                        {
                            Scale = new Vector2(1.8f),
                        },
                        new FillFlowContainer
                        {
                            Padding = new MarginPadding { Left = 5 },
                            Direction = FillDirection.Vertical,
                            AutoSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(4, 0),
                                    AutoSizeAxes = Axes.Both,
                                    Children = new[]
                                    {
                                        new OsuSpriteText
                                        {
                                            Font = @"Exo2.0-Medium",
                                            Text = beatmap.Version,
                                            TextSize = 20,
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                        new OsuSpriteText
                                        {
                                            Font = @"Exo2.0-Medium",
                                            Text = "mapped by",
                                            TextSize = 16,
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                        new OsuSpriteText
                                        {
                                            Font = @"Exo2.0-MediumItalic",
                                            Text = $"{(beatmap.Metadata ?? beatmap.BeatmapSet.Metadata).Author.Username}",
                                            TextSize = 16,
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                    }
                                },
                                starCounter = new StarCounter
                                {
                                    CountStars = starCount,
                                    Scale = new Vector2(0.8f),
                                }
                            }
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Ruleset.BindValueChanged(rulesetChanged, true);
            SelectedMods.BindValueChanged(selectedModsChanged);
        }

        private void rulesetChanged(RulesetInfo newRuleset)
        {
            if (newRuleset == null) return;

            Instance = newRuleset.CreateInstance();

            if (Beatmap == null || beatmap.RulesetID != 0 && beatmap.RulesetID != Ruleset.Value.ID) return;

            starCounter.CountStars = (float)Instance.CreateDifficultyCalculator(Beatmap).Calculate(Beatmap.Mods.Value.ToArray()).StarRating;
        }

        private void selectedModsChanged(IEnumerable<Mod> obj)
        {
            if (Beatmap == null || beatmap.RulesetID != 0 && beatmap.RulesetID != Ruleset.Value.ID) return;

            starCounter.CountStars = (float)Instance.CreateDifficultyCalculator(Beatmap).Calculate(obj.ToArray()).StarRating;
        }

        protected override void Selected()
        {
            base.Selected();

            background.Colour = ColourInfo.GradientVertical(
                new Color4(20, 43, 51, 255),
                new Color4(40, 86, 102, 255));

            triangles.Colour = Color4.White;
        }

        protected override void Deselected()
        {
            base.Deselected();

            background.Colour = new Color4(20, 43, 51, 255);
            triangles.Colour = OsuColour.Gray(0.5f);
        }

        protected override bool OnClick(InputState state)
        {
            if (Item.State == CarouselItemState.Selected)
                startRequested?.Invoke(beatmap);

            return base.OnClick(state);
        }

        protected override void ApplyState()
        {
            if (Item.State.Value != CarouselItemState.Collapsed && Alpha == 0)
                starCounter.ReplayAnimation();

            base.ApplyState();
        }

        public MenuItem[] ContextMenuItems => new MenuItem[]
        {
            new OsuMenuItem("Play", MenuItemType.Highlighted, () => startRequested?.Invoke(beatmap)),
            new OsuMenuItem("Edit", MenuItemType.Standard, () => editRequested?.Invoke(beatmap)),
            new OsuMenuItem("Hide", MenuItemType.Destructive, () => hideRequested?.Invoke(beatmap)),
            new OsuMenuItem("Details", MenuItemType.Standard, () =>
            {
                if (beatmap.OnlineBeatmapID.HasValue) beatmapOverlay?.FetchAndShowBeatmap(beatmap.OnlineBeatmapID.Value);
            }),
        };
    }
}
