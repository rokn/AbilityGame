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
            Owner.ChangeSpeed(Owner.DefaultSpeed + 4);
            Owner.walkingAnimation[Owner.currentDirection].ChangeAnimatingState(false);
            Owner.defaultDepth = 0.8f;
            Owner.noClip = true;
            base.Activate();
        }

        public override void Update(GameTime gameTime)
        {
            if(Activated)
            {
                Owner.walkingAnimation[Owner.currentDirection].ChangeAnimatingState(false);
            }
        }

        public override void Deactivate()
        {
            if (!Owner.CheckForCollision())
            {
                Owner.Size = 1f;
                Owner.ChangeSpeed(Owner.DefaultSpeed);
                Owner.defaultDepth = 0.2f;
                Owner.noClip = false;
                base.Deactivate();
            }
        }
    }
}
