using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PowerOfOne
{
    public abstract class Ability
    {
        public Ability(Entity owner)
        {
            Owner = owner;
        }

        public Entity Owner { get; set; }

        public virtual void ActivateBasicAbility()
        {
        }

        public virtual void ActivateSecondaryAbility()
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void Load()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}