using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PowerOfOne
{
    public abstract class Entity
    {
        public int EntityWidth { get; set; }

        public int EntityHeight { get; set; }

        protected int weaponTime;
        protected int attackSpeed;
        protected Dictionary<Direction, Animation> walkingAnimation;
        protected Vector2 position;
        protected Vector2 walkingOrigin;
        protected Vector2 origin;
        protected Direction currentDirection;
        protected float moveSpeed;
        protected Rectangle walkingRect;
        protected bool canWalk;
        protected bool canAttack;
        protected Texture2D walkSpriteSheet;
        public Rectangle rect;
        protected int health;
        protected int maxHealth;
        public float defaultDepth;
        protected int baseDamage;
        public Ability ability;

        public Entity(Vector2 pos)
        {
            defaultDepth = 0.2f;
            health = 100;
            maxHealth = 100;
            position = pos;
            walkingAnimation = new Dictionary<Direction, Animation>();
            currentDirection = Direction.Down;
            canWalk = true;
            canAttack = true;
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        protected virtual void Initialize()
        {
            walkingOrigin = new Vector2(EntityWidth / 2, EntityHeight/2 - Math.Min(TileSet.tileHeight,EntityHeight/2));
            origin = new Vector2(EntityWidth / 2, EntityHeight / 2);
            walkingRect = new Rectangle((int)position.X - (int)walkingOrigin.X, (int)position.Y - (int)walkingOrigin.Y, EntityWidth, EntityHeight -24);
            rect = new Rectangle((int)position.X - (int)origin.X, (int)position.Y - (int)origin.Y, EntityWidth, EntityHeight);
            UpdateRect();
        }

        public virtual void Load() 
        {
            walkingAnimation = Scripts.LoadEntityWalkAnimation(walkSpriteSheet);
            foreach (KeyValuePair<Direction, Animation> kvp in walkingAnimation)
            {
                kvp.Value.ChangeAnimatingState(false);
                kvp.Value.stepsPerFrame = 15 - (int)moveSpeed;
            }
            ability.Load();
        }

        public virtual void Update(GameTime gameTime) 
        {
            walkingAnimation[currentDirection].Update(position, 0);
            ability.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            walkingAnimation[currentDirection].Draw(spriteBatch, defaultDepth+(0.000001f*position.Y),Color.White);
            ability.Draw(spriteBatch);

            if(Main.showBoundingBoxes)
                spriteBatch.Draw(Main.BoundingBox, rect, null, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, defaultDepth + (0.000001f * position.Y) + 0.000001f);
        }

        public void MoveByPosition(Vector2 movement)
        {
            Vector2 oldPos = position;
            position += movement;
            UpdateRect();
            foreach (Rectangle rect in Main.blockRects)
            {
                if (walkingRect.Intersects(rect))
                {
                    position = oldPos;
                    UpdateRect();
                    break;
                }
            }
            foreach (Entity entity in Main.Entities)
            {
                if (entity != this)
                {
                    if (walkingRect.Intersects(entity.walkingRect))
                    {
                        position = oldPos;
                        UpdateRect();
                        break;
                    }
                }
            }
            CheckIfWithinBounds();
            RoundPosition();
        }

        protected void Move(Direction direction, float moveDistance)
        {
            if (canWalk)
            {
                currentDirection = direction;
                if (!walkingAnimation[currentDirection].isAnimating)
                {
                    walkingAnimation[currentDirection].ChangeAnimatingState(true);
                }
                Vector2 previousPos = position;
                switch (direction)
                {
                    case Direction.Right:
                        position.X += moveDistance;
                        UpdateRect();
                        break;
                    case Direction.Left:
                        position.X -= moveDistance;
                        UpdateRect();
                        break;
                    case Direction.Up:
                        position.Y -= moveDistance;
                        UpdateRect();
                        break;
                    case Direction.Down:
                        position.Y += moveDistance;
                        UpdateRect();
                        break;
                }

                foreach (Rectangle rect in Main.blockRects)
                {
                    if (walkingRect.Intersects(rect))
                    {
                        position = previousPos;
                        UpdateRect();
                        break;
                    }
                }
                foreach (Entity entity in Main.Entities)
                {
                    if (entity != this)
                    {
                        if (walkingRect.Intersects(entity.walkingRect))
                        {
                            position = previousPos;
                            UpdateRect();
                            break;
                        }
                    }
                }
                CheckIfWithinBounds();
                RoundPosition();
            }
        }

        private void CheckIfWithinBounds()
        {
            if (position.X < EntityWidth / 2)
            {
                position.X = EntityWidth / 2;
            }

            if (position.Y < EntityHeight / 2)
            {
                position.Y = EntityHeight / 2;
            }

            if (position.X > (Main.tilemap.Width * TileSet.tileWidth) - EntityWidth / 2)
            {
                position.X = (Main.tilemap.Width * TileSet.tileWidth) - EntityWidth / 2;
            }

            if (position.Y > (Main.tilemap.Height * TileSet.tileHeight) - EntityHeight / 2)
            {
                position.Y = (Main.tilemap.Height * TileSet.tileHeight) - EntityHeight / 2;
            }
        }

        private void RoundPosition()
        {
            position.X = (float)Math.Round(position.X);
            position.Y = (float)Math.Round(position.Y);
        }

        protected void UpdateRect()
        {
            walkingRect = MathAid.UpdateRectViaVector(walkingRect, position - walkingOrigin);
            rect = MathAid.UpdateRectViaVector(rect, position - origin);
        }

        public virtual void TakeDamage(int damageToBeTaken)
        {
            health -= damageToBeTaken;
            if(health<0)
            {
                Main.removeEntities.Add(this);
            }
        }
    }
}
