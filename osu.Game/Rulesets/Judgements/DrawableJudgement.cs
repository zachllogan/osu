﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using OpenTK.Graphics;
using osu.Game.Configuration;

namespace osu.Game.Rulesets.Judgements
{
    /// <summary>
    /// A drawable object which visualises the hit result of a <see cref="Judgements.Judgement"/>.
    /// </summary>
    public class DrawableJudgement : Container
    {
        private const float judgement_size = 80;

        private OsuColour colours;

        protected readonly Judgement Judgement;

        public readonly DrawableHitObject JudgedObject;

        protected SpriteText JudgementText;

        private bool HitErrorJudgements;

        /// <summary>
        /// Creates a drawable which visualises a <see cref="Judgements.Judgement"/>.
        /// </summary>
        /// <param name="judgement">The judgement to visualise.</param>
        /// <param name="judgedObject">The object which was judged.</param>
        public DrawableJudgement(Judgement judgement, DrawableHitObject judgedObject)
        {
            Judgement = judgement;
            JudgedObject = judgedObject;

            Size = new Vector2(judgement_size);
        }

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager config, OsuColour colours)
        {
            this.colours = colours;
            HitErrorJudgements = config.GetBindable<bool>(OsuSetting.HitErrorJudgments);

            Child = new SkinnableDrawable($"Play/{Judgement.Result}", _ => JudgementText = new OsuSpriteText
            {
                Text = Judgement.Result.GetDescription().ToUpperInvariant(),
                Font = @"Venera",
                Colour = HitErrorJudgements ? judgementColour(Judgement) : judgementColour(Judgement.Result),
                Scale = new Vector2(0.85f, 1),
                TextSize = 12
            }, restrictSize: false);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            this.FadeInFromZero(100, Easing.OutQuint);

            switch (Judgement.Result)
            {
                case HitResult.None:
                    break;
                case HitResult.Miss:
                    this.ScaleTo(1.6f);
                    this.ScaleTo(1, 100, Easing.In);

                    this.MoveToOffset(new Vector2(0, 100), 800, Easing.InQuint);
                    this.RotateTo(40, 800, Easing.InQuint);

                    this.Delay(600).FadeOut(200);
                    break;
                default:
                    this.ScaleTo(0.9f);
                    this.ScaleTo(1, 500, Easing.OutElastic);

                    this.Delay(100).FadeOut(400);
                    break;
            }

            Expire(true);
        }

        private Color4 judgementColour(HitResult judgement)
        {
            switch (judgement)
            {
                case HitResult.Perfect:
                case HitResult.Great:
                    return colours.Blue;
                case HitResult.Ok:
                case HitResult.Good:
                    return colours.Green;
                case HitResult.Meh:
                    return colours.Yellow;
                case HitResult.Miss:
                    return colours.Red;
            }

            return Color4.White;
        }

        Vector4 baseFastColor = new Vector4(0.5f, 0.0f, 1.0f, 1);
        Vector4 maxFastColor = new Vector4(0.5f, 1.0f, 0.5f, 1);
        Vector4 baseSlowColor = new Vector4(0.0f, 0.0f, 1.0f, 1);
        Vector4 maxSlowColor = new Vector4(0.0f, 1.0f, 0.5f, 1);
        private Color4 judgementColour(Judgement judgement)
        {
            if (judgement.TimeOffset >= 0)
                return Color4.FromHcy(baseSlowColor + (maxSlowColor - baseSlowColor) * (float)(judgement.TimeOffset / JudgedObject.HitObject.HitWindows.HalfWindowFor(HitResult.Meh)));
            else if (judgement.TimeOffset < 0)
                return Color4.FromHcy(baseFastColor + (maxFastColor - baseFastColor) * (float)(-judgement.TimeOffset / JudgedObject.HitObject.HitWindows.HalfWindowFor(HitResult.Meh)));
            return Color4.White;
        }
    }
}
