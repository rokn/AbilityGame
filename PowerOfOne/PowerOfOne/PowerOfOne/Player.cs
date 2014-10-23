using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PowerOfOne
{
    public class Player : Entity
    {
        private bool hasHit;
        private bool weaponIsOut;
        private Dictionary<Direction, Animation> flyingAnimation;
        private float weaponRotation;
        private Passive passive;
        private Texture2D flySpritesheet;
        private Texture2D weaponTexture;
        private TimeSpan attackTimer;
        private TimeSpan weaponTimer;
        private Vector2 weaponPosition;
        private Vector2 weaponTipPosition;

        public Player(Vector2 pos)
            : base(pos)
        {
            weaponTime = 200;
            attackSpeed = 500;
            moveSpeed = 4;
            weaponIsOut = false;
            hasHit = false;
            baseDamage = 5;
            ability = new Telekinesis(this);
            passive = new Speed(this);
            abilityPower = 1;
            flyingAnimation = new Dictionary<Direction, Animation>();
            Initialize();
        }

        public byte abilityPower { get; private set; }

        protected override void Initialize()
        {
            EntityWidth = 32;
            EntityHeight = 48;
            base.Initialize();
        }

        #region Load

        public override void Load()
        {
            weaponTexture = Scripts.LoadTexture(@"Weapons\Sword");

            walkSpriteSheet = Scripts.LoadTexture(@"Player\Walk");

            flySpritesheet = Scripts.LoadTexture(@"Player\Fly");

            flyingAnimation = Scripts.LoadEntityWalkAnimation(flySpritesheet);

            foreach (KeyValuePair<Direction, Animation> kvp in flyingAnimation)
            {
                kvp.Value.ChangeAnimatingState(false);
                kvp.Value.stepsPerFrame = 10 - (int)moveSpeed;
            }

            base.Load();
        }

        #endregion Load

        public override void Update(GameTime gameTime)
        {
            CheckForInput();

            if (weaponIsOut)
            {
                if (!hasHit)
                {
                    CheckIfHasHit();
                }

                weaponTimer = weaponTimer.Subtract(gameTime.ElapsedGameTime);

                if (weaponTimer.TotalMilliseconds < 0)
                {
                    StopBasicAttack();
                }
            }

            attackTimer = attackTimer.Subtract(gameTime.ElapsedGameTime);

            if (!canAttack)
            {
                if (attackTimer.TotalMilliseconds < 0)
                {
                    canAttack = true;
                }
            }

            if(IsFlying())
            {
                flyingAnimation[currentDirection].Update(Position,0);
            }

            passive.Update(gameTime);
            UpdateCamera();
            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            float posX = Main.camera.zeroPos.X + (Position.X - (Main.width / 2));
            float posY = Main.camera.zeroPos.Y + (Position.Y - (Main.height / 2));
            Main.camera.Position = new Vector2(posX, posY);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (weaponIsOut)
            {
                spriteBatch.Draw(weaponTexture, weaponPosition, null, Color.White, weaponRotation, new Vector2(0, weaponTexture.Height / 2), 1f, SpriteEffects.None, 0.3f);
            }

            base.Draw(spriteBatch);
            if (IsFlying())
            {
                flyingAnimation[currentDirection].Draw(spriteBatch, size, defaultDepth, Color.White);
            }
            else
            {
                walkingAnimation[currentDirection].Draw(spriteBatch, size, 0.9f, Color.White * 0.2f);                
            }
        }

        private bool IsFlying()
        {
            return passive.Activated && passive.GetType() == typeof(Flying);
        }

        private void CheckIfHasHit()
        {
            foreach (Entity entity in Main.Entities)
            {
                if (entity != this)
                {
                    if (Vector2.Distance(weaponTipPosition, entity.Position) <= weaponTexture.Width)
                    {
                        Vector2 direction = Vector2.Normalize(weaponTipPosition - weaponPosition);

                        for (int i = 0; i < weaponTexture.Width; i++)
                        {
                            if (entity.rect.Contains(weaponPosition + direction * i))
                            {
                                entity.TakeDamage(baseDamage);
                                hasHit = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void StartBasicAttack()
        {
            canWalk = false;
            weaponIsOut = true;
            hasHit = false;
            canAttack = false;

            weaponPosition = Vector2.Normalize(Main.mouse.RealPosition - position);

            weaponTipPosition = weaponPosition;
            weaponTipPosition *= (EntityHeight / 2) + weaponTexture.Width;
            weaponTipPosition += position;

            weaponPosition *= EntityHeight / 2;
            weaponPosition += position;

            weaponRotation = MathAid.FindRotation(weaponPosition, Main.mouse.RealPosition);

            float rInDegrees = MathHelper.ToDegrees(weaponRotation);

            DirectTowardsRotation(rInDegrees);

            weaponTimer = new TimeSpan(0, 0, 0, 0, weaponTime);
            attackTimer = new TimeSpan(0, 0, 0, 0, attackSpeed);

            walkingAnimation[currentDirection].ChangeAnimatingState(false);
        }

        private void StopBasicAttack()
        {
            canWalk = true;
            weaponIsOut = false;
        }

        private void CheckForInput()
        {
            if (Scripts.KeyIsPressed(Keys.D))
            {
                Move(Direction.Right, moveSpeed);
            }
            else if (Scripts.KeyIsPressed(Keys.A))
            {
                Move(Direction.Left, moveSpeed);
            }

            if (Scripts.KeyIsPressed(Keys.W))
            {
                Move(Direction.Up, moveSpeed);
            }
            else if (Scripts.KeyIsPressed(Keys.S))
            {
                Move(Direction.Down, moveSpeed);
            }

            if (Scripts.KeyIsReleased(Keys.W) &&
                Scripts.KeyIsReleased(Keys.A) &&
                Scripts.KeyIsReleased(Keys.S) &&
                Scripts.KeyIsReleased(Keys.D))
            {
                StopAnimation(walkingAnimation);
                if (IsFlying())
                {
                    StopAnimation(flyingAnimation);
                }
            }

            if (Main.mouse.LeftClick() || Main.mouse.LeftHeld())
            {
                if (Vector2.Distance(Main.mouse.RealPosition, Position) > EntityHeight / 2)
                {
                    //if (canAttack)
                    //{
                    //    if (!weaponIsOut)
                    //    {
                    //        StartBasicAttack();
                    //    }
                    //}
                    ability.ActivateBasicAbility();
                }
            }

            if (Main.keyboard.JustPressed(Keys.LeftShift))
            {
                if (!passive.Activated)
                {
                    passive.Activate();
                }
                else
                {
                    passive.Deactivate();
                }
            }

            if (Main.mouse.RightClick() || Main.mouse.RightHeld())
            {
                ability.ActivateSecondaryAbility();
            }
        }

        protected override void Move(Direction direction, float moveDistance)
        {
            base.Move(direction, moveDistance);
            if (IsFlying())
            {
                if (!flyingAnimation[currentDirection].isAnimating)
                {
                    flyingAnimation[currentDirection].ChangeAnimatingState(true);
                }
            }
        }
    }
}