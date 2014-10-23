using Microsoft.Xna.Framework;

namespace PowerOfOne
{
    public class Flying : Passive
    {
        public Flying(Entity owner)
            : base(owner) { }

        public override void Activate()
        {
            Owner.Size = 1.2f;
            Owner.ChangeSpeed(Owner.BaseSpeed + 4);
            Owner.walkingAnimation[Owner.currentDirection].ChangeAnimatingState(false);
            Owner.baseDepth = 0.8f;
            Owner.noClip = true;
            base.Activate();
        }

        public override void Update(GameTime gameTime)
        {
            if (Activated)
            {
                Owner.walkingAnimation[Owner.currentDirection].ChangeAnimatingState(false);
            }
        }

        public override void Deactivate()
        {
            if (!Owner.CheckForCollision())
            {
                Owner.Size = 1f;
                Owner.ChangeSpeed(Owner.BaseSpeed);
                Owner.baseDepth = 0.2f;
                Owner.noClip = false;
                base.Deactivate();
            }
        }
    }
}