using Microsoft.Xna.Framework;

namespace PowerOfOne
{
    public class Flying : Passive
    {
        public Flying()
            : base() { }

        public override void ActivatePassive()
        {
            Owner.Size = 1.2f;
            Owner.ChangeSpeed(Owner.BaseSpeed + 4);
            Owner.walkingAnimation[Owner.currentDirection].ChangeAnimatingState(false);
            Owner.baseDepth = 0.8f;
            Owner.noClip = true;
            base.ActivatePassive();
        }

        public override void Update(GameTime gameTime)
        {
            if (Activated)
            {
                Owner.walkingAnimation[Owner.currentDirection].ChangeAnimatingState(false);
            }
        }

        public override void DeactivatePassive()
        {
            if (!Owner.CheckForCollision())
            {
                Owner.Size = 1f;
                Owner.ChangeSpeed(Owner.BaseSpeed);
                Owner.baseDepth = 0.2f;
                Owner.noClip = false;
                base.DeactivatePassive();
            }
        }
    }
}