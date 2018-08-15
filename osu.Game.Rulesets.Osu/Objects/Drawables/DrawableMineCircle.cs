// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics;
using osu.Game.Rulesets.Osu.Objects.Drawables.Pieces;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using System;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
	public class DrawableMineCircle : Container
    {
        public ApproachCircle ApproachCircle;
        private readonly MineCirclePiece circle;
        private readonly DrawableHitObject owner;
        private bool isHit;

        public event Action Clicked;

        public DrawableMineCircle(DrawableOsuHitObject h)
        {
            Origin = Anchor.Centre;

            owner = h;

            Position = h.Position;
            Scale = new OpenTK.Vector2(h.HitObject.Scale * 2.2f);
            Size = h.Size;
            Position = h.Position;
            Depth = h.Depth + 1000;
            isHit = false;

            h.OnJudgement += OwnerHit;
            h.OnJudgementRemoved += OwnerHit;

            InternalChildren = new Drawable[]
            {
                circle = new MineCirclePiece
                {
                    Hit = () =>
                    {
                        if (owner.IsHit)
                            return false;
                        isHit = true;
                        this.ScaleTo(Scale * 8, 400, Easing.InQuad);
                        Clicked?.Invoke();
                        Expire(true);
                        return true;
                    },
                },
            };
        }

        private void OwnerHit(DrawableHitObject hitObject, Judgement judgement)
        {
            if (isHit)
                return;
            this.FadeOut(100, Easing.None);
            Expire(true);
        }

        override protected void Update()
        {
            base.Update();

            Position = owner.Position;
        }
    }
}
