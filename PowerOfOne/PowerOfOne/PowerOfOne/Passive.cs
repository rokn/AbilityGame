namespace PowerOfOne
{
    public abstract class Passive : Ability
    {
        public bool Activated { get; set; }

        public Passive(Entity owner)
            : base(owner) 
        {
            Activated = false;
        }

        public virtual void Activate() 
        {
            Activated = true;
        }

        public virtual void Deactivate() 
        {
            Activated = false;
        }
    }
}
