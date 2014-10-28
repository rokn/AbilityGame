using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PowerOfOne
{
    public class Player : Entity
    {

        #region Vars
        private bool hasHit;
        private bool hasPassive;
        private bool weaponIsOut;
        private Dictionary<Direction, Animation> flyingAnimation;
        private float weaponRotation;
        private Passive passive;
        private List<Ability> abilities;
        private Texture2D flySpritesheet;
        private Texture2D weaponTexture;
        private TimeSpan attackTimer;
        private TimeSpan weaponTimer;
        private Vector2 weaponPosition;
        private Vector2 weaponTipPosition;
        private bool weaponSelected;
        private int selectedAbility;
        ParticleEngine engine;
        #endregion

        public Player(Vector2 pos)
            : base(pos)
        {
            health = 100;
            maxHealth = 100;
            weaponTime = 200;
            BaseWeaponTime = weaponTime;
            attackSpeed = 500;
            baseAttackSpeed = attackSpeed;
            BaseSpeed = 4;
            moveSpeed = BaseSpeed;
            weaponIsOut = false;
            weaponSelected = true;
            hasHit = false;
            hasPassive = true;
            baseDamage = 15;

            abilities = new List<Ability>();
            abilities.Add(new FireControl());
            abilities.ForEach(ab => ab.Initialize(this));
            passive = new Speed();
            passive.Initialize(this);


            AbilityPower = 1;
            flyingAnimation = new Dictionary<Direction, Animation>();
            Initialize();
        }


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

            abilities.ForEach(ab => ab.Load());

            base.Load();
        }

        #endregion Load

        public override void Update(GameTime gameTime)
        {
            HandleForInput();

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

            if (hasPassive)
            {
                if (IsFlying())
                {
                    flyingAnimation[currentDirection].Update(Position, 0);
                }

                passive.Update(gameTime);
            }

            abilities.ForEach(ab => ab.Update(gameTime));

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
            if(hasPassive)
            {
                passive.Draw(spriteBatch);
            }

            if (weaponIsOut)
            {
                spriteBatch.Draw(weaponTexture, weaponPosition, null, Color.White, weaponRotation, new Vector2(0, weaponTexture.Height / 2), 1f, SpriteEffects.None, 0.3f);
            }

            base.Draw(spriteBatch);

            if (IsFlying())
            {
                flyingAnimation[currentDirection].Draw(spriteBatch, size, baseDepth, Color.White);
            }
            else
            {
                walkingAnimation[currentDirection].Draw(spriteBatch, size, 0.9f, Color.White * 0.2f);                
            }

            abilities.ForEach(ab => ab.Draw(spriteBatch));
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

        private bool IsFlying()
        {
            if (hasPassive)
            {
                return passive.Activated && passive.GetType() == typeof(Flying);
            }
            else
            {
                return false;
            }
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

        private void HandleForInput()
        {
            CheckForMovementButtons();
            CheckForAttackButtons();
            CheckForPassiveButton();
            CheckForAbilitySwitchButtons();
        }

        private void CheckForPassiveButton()
        {
            if (hasPassive)
            {
                if (Main.keyboard.JustPressed(Keys.LeftShift))
                {
                    if (!passive.Activated)
                    {
                        passive.ActivatePassive();
                    }
                    else
                    {
                        passive.DeactivatePassive();
                    }
                }
            }
        }

        private void CheckForMovementButtons()
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

            if(IsMovementButonsAreReleased())
            {
                StopAnimation(walkingAnimation);

                if (IsFlying())
                {
                    StopAnimation(flyingAnimation);
                }
            }
        }

        private bool IsMovementButonsAreReleased()
        {
            if (Scripts.KeyIsReleased(Keys.W) &&
                Scripts.KeyIsReleased(Keys.A) &&
                Scripts.KeyIsReleased(Keys.S) &&
                Scripts.KeyIsReleased(Keys.D))
            {
                return true;
            }
            return false;
        }

        private void CheckForAttackButtons()
        {
            if (Main.mouse.LeftClick() || Main.mouse.LeftHeld())
            {
                if (Vector2.Distance(Main.mouse.RealPosition, Position) > EntityHeight / 2)
                {
                    if (weaponSelected)
                    {
                        if (canAttack)
                        {
                            if (!weaponIsOut)
                            {
                                StartBasicAttack();
                            }
                        }
                    }
                    else
                    {
                        abilities[selectedAbility].ActivateBasicAbility();
                    }
                }
            }

            if (Main.mouse.RightClick() || Main.mouse.RightHeld())
            {
                if (!weaponSelected)
                {
                    abilities[selectedAbility].ActivateSecondaryAbility();
                }
            }
        }        

        private void CheckForAbilitySwitchButtons()
        {
            if (Scripts.KeyIsPressed(Keys.D1))
            {
                weaponSelected = true;
            }

            if (Scripts.KeyIsPressed(Keys.D2))
            {
                if (abilities.Count >= 1)
                {
                    selectedAbility = 0;
                    weaponSelected = false;
                }
            }

            if (Scripts.KeyIsPressed(Keys.D3))
            {
                if (abilities.Count >= 2)
                {
                    selectedAbility = 1;
                    weaponSelected = false;
                }
            }

            if (Scripts.KeyIsPressed(Keys.D4))
            {
                if (abilities.Count >= 3)
                {
                    selectedAbility = 2;
                    weaponSelected = false;
                }
            }

            if (Scripts.KeyIsPressed(Keys.D5))
            {
                if (abilities.Count >= 4)
                {
                    selectedAbility = 3;
                    weaponSelected = false;
                }
            }
        }
    }
}