using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PowerOfOne
{
    public abstract class Entity
    {
        #region Vars
        public int EntityWidth { get; set; }

        public int EntityHeight { get; set; }

        public float Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;

                if(health>maxHealth)
                {
                    health = maxHealth;
                }
            }
        }

        public float MaxHealth
        {
            get
            {
                return maxHealth;
            }
            protected set
            {
                maxHealth = value;
            }
        }
        public bool canWalk { get; set; }

        public Rectangle WalkingRect
        {
            get
            {
                return walkingRect;
            }
            set
            {
                walkingRect = value;
            }
        }

        public Vector2 WalkingOrigin
        {
            get
            {
                return walkingOrigin;
            }
            set
            {
                walkingOrigin = value;
            }
        }

        private float baseSpeed;
        protected bool canAttack;
        protected float moveSpeed;
        protected float size;
        protected int attackSpeed;
        protected int baseAttackSpeed;
        protected int baseDamage;
        protected int baseWeaponTime;
        protected float health;
        protected float maxHealth;
        protected int weaponTime;
        protected Rectangle walkingRect;
        protected Texture2D walkSpriteSheet;
        protected Vector2 origin;
        protected Vector2 position;
        protected Vector2 walkingOrigin;
        public Dictionary<Direction, Animation> walkingAnimation;
        public Direction currentDirection;
        public float baseDepth;
        public Rectangle rect;
        #endregion

        public Entity(Vector2 pos)
        {
            size = 1f;
            baseDepth = 0.2f;
            position = pos;
            walkingAnimation = new Dictionary<Direction, Animation>();
            currentDirection = Direction.Down;
            canWalk = true;
            canAttack = true;
        }

        public bool noClip { get; set; }

        public bool EntityNoClip { get; set; }

        public byte AbilityPower { get; protected set; }

        public int BaseWeaponTime
        {
            get
            {
                return baseWeaponTime;
            }
            protected set
            {
                baseWeaponTime = value;

                if (baseWeaponTime <= 0)
                {
                    baseWeaponTime = 1;
                }
            }
        }

        public int BaseAttackSpeed
        {
            get
            {
                return baseAttackSpeed;
            }
            protected set
            {
                baseAttackSpeed = value;

                if (baseAttackSpeed <= 0)
                {
                    baseAttackSpeed = 1;
                }
            }
        }

        public int AttackSpeed
        {
            get
            {
                return attackSpeed;
            }
            set
            {
                attackSpeed = value;

                if (attackSpeed <= 0)
                {
                    attackSpeed = 1;
                }
            }
        }

        public float BaseSpeed
        {
            get
            {
                return baseSpeed;
            }
            protected set
            {
                baseSpeed = value;
            }
        }

        public float Size
        {
            get
            {
                return size;
            }

            set
            {
                if (size < 0f)
                {
                    throw new ArgumentOutOfRangeException("Size of entities must be greater than zero");
                }
                size = value;
            }
        }

        public int WeaponTime
        {
            get
            {
                return weaponTime;
            }
            set
            {
                weaponTime = value;

                if(weaponTime<= 0)
                {
                    weaponTime = 1;
                }
            }
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
            walkingOrigin = Scripts.GetWalkingOrigin(EntityWidth,EntityHeight);
            origin = new Vector2(EntityWidth / 2, EntityHeight / 2);
            walkingRect = Scripts.GetWalkingRect(Position - walkingOrigin,EntityWidth,EntityHeight);
            rect = new Rectangle((int)position.X - (int)origin.X, (int)position.Y - (int)origin.Y, EntityWidth, EntityHeight);
            UpdateRect();
            baseSpeed = moveSpeed;
        }

        public virtual void Load()
        {
            walkingAnimation = Scripts.LoadEntityWalkAnimation(walkSpriteSheet);

            foreach (KeyValuePair<Direction, Animation> kvp in walkingAnimation)
            {
                kvp.Value.ChangeAnimatingState(false);
                kvp.Value.stepsPerFrame = 15 - (int)moveSpeed;
            }

            
        }

        public virtual void Update(GameTime gameTime)
        {
            walkingAnimation[currentDirection].Update(position, 0);
            if (health < 0)
            {
                Main.removeEntities.Add(this);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            walkingAnimation[currentDirection].Draw(spriteBatch, size, baseDepth + (0.000001f * position.Y), Color.White);

            if (Main.showBoundingBoxes)
                spriteBatch.Draw(Main.BoundingBox, rect, null, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, baseDepth + (0.000001f * position.Y) + 0.000001f);
        }

        public void MoveByPosition(Vector2 movement)
        {
            Vector2 previousPos = position;
            position += movement;
            UpdateRect();

            if (!noClip)
            {
                if (CheckForCollision())
                {
                    position = previousPos;
                    UpdateRect();
                }
            }

            CheckIfWithinBounds();
            RoundPosition();
        }

        protected virtual void Move(Direction direction, float moveDistance)
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

                if (!noClip)
                {
                    if (CheckForCollision())
                    {
                        position = previousPos;
                        UpdateRect();
                        if(moveDistance > 1)
                        {
                            Move(direction, moveDistance - 1);
                        }
                    }
                }
                CheckIfWithinBounds();
                RoundPosition();
            }
        }

        public bool CheckForCollision()
        {
            foreach (Rectangle rect in Main.blockRects)
            {
                if (walkingRect.Intersects(rect))
                {
                    return true;
                }
            }

            if (!EntityNoClip)
            {
                foreach (Entity entity in Main.Entities)
                {
                    if (entity != this)
                    {
                        if (walkingRect.Intersects(entity.walkingRect))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void DirectTowardsMouse()
        {
            float angle = MathAid.FindRotation(Position, Main.mouse.RealPosition);
            DirectTowardsRotation(MathHelper.ToDegrees(angle));
        }

        public void DirectTowardsRotation(float rInDegrees)
        {
            if (rInDegrees > -45 && rInDegrees <= 45)
            {
                currentDirection = Direction.Right;
            }
            else if (rInDegrees > 45 && rInDegrees <= 135)
            {
                currentDirection = Direction.Down;
            }
            else if (rInDegrees > 135 || rInDegrees <= -135)
            {
                currentDirection = Direction.Left;
            }
            else if (rInDegrees > -135 && rInDegrees <= -45)
            {
                currentDirection = Direction.Up;
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

        public virtual void TakeDamage(float damageToBeTaken)
        {
            health -= damageToBeTaken;
            if (health <= 0)
            {
                Main.removeEntities.Add(this);
            }
        }

        public void ChangeSpeed(float newSpeed)
        {
            if (newSpeed > 20)
            {
                newSpeed = 20;
            }

            moveSpeed = newSpeed;

            foreach (KeyValuePair<Direction, Animation> kvp in walkingAnimation)
            {
                kvp.Value.stepsPerFrame = 15 - (int)moveSpeed;
            }
        }

        protected void StopAnimation(Dictionary<Direction,Animation> animation)
        {
            foreach (KeyValuePair<Direction, Animation> kvp in animation)
            {
                kvp.Value.ChangeAnimatingState(false);
            }
        }
    }
}