// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.Mods
{
	public class OsuModMinefield : ModMinefield, IApplicableToDrawableHitObjects, IApplicableToScoreProcessor, IApplicableToRulesetContainer<OsuHitObject>
    {
        private RulesetContainer<OsuHitObject> rulesetContainer;
        private bool exploded = false;

        public void ApplyToDrawableHitObjects(IEnumerable<DrawableHitObject> drawables)
        {
            foreach (var d in drawables.OfType<DrawableHitCircle>())
                d.ApplyCustomUpdateState += ApplyMine;
            foreach (var d in drawables.OfType<DrawableSlider>())
                ApplySliderMine(d);
        }

        private void ApplyMine(DrawableHitObject hitObject, ArmedState state)
        {
            if (state == ArmedState.Idle)
            {
                var circle = hitObject as DrawableOsuHitObject;
                var mine = new DrawableMineCircle(circle)
                {
                    Alpha = 0
                };
                mine.Clicked += ClickMine;
                rulesetContainer.Playfield.Add(mine);
                double delay = circle.HitObject.TimeFadeIn;
                using (mine.BeginAbsoluteSequence(hitObject.HitObject.StartTime - delay, true))
                    mine.FadeIn(100);
            }
        }

        private void ApplySliderMine(DrawableSlider slider)
        {
            var mine = new DrawableMineCircle(slider)
            {
                Alpha = 0
            };
            mine.Clicked += ClickMine;
            rulesetContainer.Playfield.Add(mine);
            double delay = 100;
            if (slider is DrawableOsuHitObject)
                delay = (slider as DrawableOsuHitObject).HitObject.TimeFadeIn;
            using (mine.BeginAbsoluteSequence(slider.HitObject.StartTime - delay, true))
                mine.FadeIn(100);
        }

        private void ClickMine()
        {
            if (exploded)
                return;
            exploded = true;
        }

        public void ApplyToScoreProcessor(ScoreProcessor scoreProcessor)
        {
            exploded = false;
            scoreProcessor.FailConditions += FailCondition;
        }

        private bool FailCondition(ScoreProcessor arg) => exploded;

        public void ApplyToRulesetContainer(RulesetContainer<OsuHitObject> rulesetContainer)
        {
            this.rulesetContainer = rulesetContainer;
        }
    }
}
