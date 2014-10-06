using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PowerOfOne
{
    public class Projectile
    {
        private Vector2 target;
        private float moveSpeed;
        private Texture2D texture;

        public Projectile(Vector2 startPosition,Vector2 startTarget,float speed)
        {
            this.Position = startPosition;
            this.target = startTarget;
            this.moveSpeed = speed;
            Speed = new Vector2(2, 2); // TODO: Remove
        }

        public Vector2 Position { get; set; }

        public Vector2 Speed { get; private set; }

        public void Load()
        {
            texture = Scripts.LoadTexture(@"Projectiles\Proj_2");
        }

        public void Update(GameTime gameTime)
        {
            Position += Speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }
    }
}
