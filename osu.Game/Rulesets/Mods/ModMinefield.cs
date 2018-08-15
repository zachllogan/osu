// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Mods
{
    public abstract class ModMinefield : Mod
    {
        public override string Name => "Minefield";
        public override string ShortenedName => "MF";
        public override ModType Type => ModType.Fun;
        public override FontAwesome Icon => FontAwesome.fa_bomb;
        public override string Description => @"Sudden death if you missaim.";
        public override double ScoreMultiplier => 0;
    }
}
