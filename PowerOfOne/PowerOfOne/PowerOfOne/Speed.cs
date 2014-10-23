namespace PowerOfOne
{
    public class Speed : Passive
    {
        public Speed(Entity owner)
            : base(owner) { }

        public override void Activate()
        {
            Owner.ChangeSpeed(Owner.BaseSpeed + 15);
            base.Activate();
        }

        public override void Deactivate()
        {
            Owner.ChangeSpeed(Owner.BaseSpeed);
            base.Deactivate();
        }
    }
}