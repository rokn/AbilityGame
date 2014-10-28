namespace PowerOfOne
{
    public abstract class Passive : Ability
    {
        public bool Activated { get; set; }

        public Passive()
            : base() 
        {
            Activated = false;
        }

        public virtual void ActivatePassive()
        {
            Activated = true;
        }

        public virtual void DeactivatePassive()
        {
            Activated = false;
        }
    }
}
