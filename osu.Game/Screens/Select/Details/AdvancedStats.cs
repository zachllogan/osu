﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using System;
using osu.Game.Beatmaps;
using osu.Framework.Configuration;
using System.Collections.Generic;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets;

namespace osu.Game.Screens.Select.Details
{
    public class AdvancedStats : Container
    {
        private readonly StatisticRow firstValue, hpDrain, accuracy, approachRate, starDifficulty;

        private BeatmapInfo beatmap;
        public BeatmapInfo Beatmap
        {
            get { return beatmap; }
            set
            {
                if (value == beatmap) return;
                beatmap = value;

                //mania specific
                if ((Beatmap?.Ruleset?.ID ?? 0) == 3)
                {
                    firstValue.Title = "Key Amount";
                    firstValue.Value = (int)Math.Round(Beatmap?.BaseDifficulty?.CircleSize ?? 0);
                }
                else
                {
                    firstValue.Title = "Circle Size";
                    firstValue.Value = Beatmap?.BaseDifficulty?.CircleSize ?? 0;
                }

                hpDrain.Value = Beatmap?.BaseDifficulty?.DrainRate ?? 0;
                accuracy.Value = Beatmap?.BaseDifficulty?.OverallDifficulty ?? 0;
                approachRate.Value = Beatmap?.BaseDifficulty?.ApproachRate ?? 0;
                WBeatmap = Manager.GetWorkingBeatmap(beatmap, WBeatmap);
                if (WBeatmap == null || (beatmap.RulesetID != 0 && beatmap.RulesetID != Ruleset.Value.ID)) return;
                starDifficulty.Value = (float)(instance.CreateDifficultyCalculator(WBeatmap)?.Calculate(SelectedMods.Value.ToArray()).StarRating ?? 0);
            }
        }

        public AdvancedStats()
        {
            Child = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(4f),
                Children = new[]
                {
                    firstValue = new StatisticRow(), //circle size/key amount
                    hpDrain = new StatisticRow { Title = "HP Drain" },
                    accuracy = new StatisticRow { Title = "Accuracy" },
                    approachRate = new StatisticRow { Title = "Approach Rate" },
                    starDifficulty = new StatisticRow(10, true) { Title = "Star Difficulty" },
                },
            };
        }

        protected Ruleset instance;

        protected readonly Bindable<IEnumerable<Mod>> SelectedMods = new Bindable<IEnumerable<Mod>>(new Mod[] { });

        protected readonly IBindable<RulesetInfo> Ruleset = new Bindable<RulesetInfo>();

        protected WorkingBeatmap WBeatmap;

        protected BeatmapManager Manager;

        [BackgroundDependencyLoader]
        private void load(OsuColour colours, BeatmapManager manager, IBindable<RulesetInfo> ruleset, Bindable<IEnumerable<Mod>> selectedMods)
        {
            starDifficulty.AccentColour = colours.Yellow;
            Ruleset.BindTo(ruleset);
            if (selectedMods != null) SelectedMods.BindTo(selectedMods);
            Manager = manager;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Ruleset.BindValueChanged(rulesetChanged, true);
            SelectedMods.BindValueChanged(selectedModsChanged, false);
        }

        private void rulesetChanged(RulesetInfo newRuleset)
        {
            if (newRuleset == null) return;

            instance = newRuleset.CreateInstance();

            if (WBeatmap == null || (beatmap.RulesetID != 0 && beatmap.RulesetID != Ruleset.Value.ID)) return;

            starDifficulty.Value = (float)(instance.CreateDifficultyCalculator(WBeatmap).Calculate(WBeatmap.Mods.Value.ToArray()).StarRating);
        }

            private void selectedModsChanged(IEnumerable<Mod> obj)
        {
            if (WBeatmap == null || WBeatmap.Mods.Value==obj || (beatmap.RulesetID != 0 && beatmap.RulesetID != Ruleset.Value.ID)) return;

            starDifficulty.Value = (float)(instance.CreateDifficultyCalculator(WBeatmap).Calculate(obj.ToArray()).StarRating);
        }

        private class StatisticRow : Container, IHasAccentColour
        {
            private const float value_width = 25;
            private const float name_width = 70;

            private readonly float maxValue;
            private readonly bool forceDecimalPlaces;
            private readonly OsuSpriteText name, value;
            private readonly Bar bar;

            public string Title
            {
                get { return name.Text; }
                set { name.Text = value; }
            }

            private float difficultyValue;
            public float Value
            {
                get { return difficultyValue; }
                set
                {
                    difficultyValue = value;
                    bar.Length = value / maxValue;
                    this.value.Text = value.ToString(forceDecimalPlaces ? "0.00" : "0.##");
                }
            }

            public Color4 AccentColour
            {
                get { return bar.AccentColour; }
                set { bar.AccentColour = value; }
            }

            public StatisticRow(float maxValue = 10, bool forceDecimalPlaces = false)
            {
                this.maxValue = maxValue;
                this.forceDecimalPlaces = forceDecimalPlaces;
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                Children = new Drawable[]
                {
                    new Container
                    {
                        Width = name_width,
                        AutoSizeAxes = Axes.Y,
                        Child = name = new OsuSpriteText
                        {
                            TextSize = 13,
                        },
                    },
                    bar = new Bar
                    {
                        Origin = Anchor.CentreLeft,
                        Anchor = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.X,
                        Height = 5,
                        BackgroundColour = Color4.White.Opacity(0.5f),
                        Padding = new MarginPadding { Left = name_width + 10, Right = value_width + 10 },
                    },
                    new Container
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Width = value_width,
                        RelativeSizeAxes = Axes.Y,
                        Child = value = new OsuSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            TextSize = 13,
                        },
                    },
                };
            }
        }
    }
}
