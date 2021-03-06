﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mania.Scoring
{
    internal class ManiaScoreProcessor : ScoreProcessor
    {
        public ManiaScoreProcessor(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        public override HitWindows CreateHitWindows() => new ManiaHitWindows();
    }
}
