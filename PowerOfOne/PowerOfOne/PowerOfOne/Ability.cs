using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace PowerOfOne
{
    public abstract class Ability
    {
        public Entity Owner { get; set; }

        public Ability(Entity owner)
        {
            Owner = owner;
        }

        public virtual void Load() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void ActivateBasicAbility() { }
    }
}
