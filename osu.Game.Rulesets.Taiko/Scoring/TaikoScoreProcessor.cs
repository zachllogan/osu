﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Taiko.Scoring
{
    internal class TaikoScoreProcessor : ScoreProcessor
    {
        public TaikoScoreProcessor(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        public override HitWindows CreateHitWindows() => new TaikoHitWindows();
    }
}
