using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
namespace PowerOfOne
{
    public class Enemy : Entity
    {
        private const int MoveChance = 1;

        private HealthBar healthBar;
        private Vector2 healtBarPositionOffset;
        private bool showHealthBar;
        private Queue<Direction> idleMovementSteps;
        private Random rand;
        private int id;
        private TimeSpan idleMoveTimer;
        private Ability ability;
        public Enemy(Vector2 pos,int id)
            : base(pos)
        {
            health = 100;
            maxHealth = 100;
            moveSpeed = 4;
            idleMovementSteps = new Queue<Direction>();
            rand = Main.rand;
            ability = new Telekinesis(this);
            this.id = id;
            idleMoveTimer = new TimeSpan();
        }

        protected override void Initialize()
        {
            int healthBarWidth = 60;
            healtBarPositionOffset = new Vector2(healthBarWidth / 2, EntityHeight / 2 + 12);
            Vector2 healthBarPos = Position - healtBarPositionOffset;
            healthBar = new HealthBar(healthBarWidth, healthBarPos, Color.Green, Color.Red);
            showHealthBar = false;
            base.Initialize();
        }

        public override void Load()
        {
            walkSpriteSheet = Scripts.LoadTexture(@"Enemies\Enemy_" + id.ToString());

            EntityWidth = walkSpriteSheet.Width / 4;
            EntityHeight = walkSpriteSheet.Height / 4;

            this.Initialize();

            healthBar.Load(true, "Enemies");

            ability.Load();
            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            healthBar.Update(Position - healtBarPositionOffset, health, maxHealth);
            IdleMovement(gameTime);
            ability.Update(gameTime);
            base.Update(gameTime);
        }

        private void IdleMovement(GameTime gameTime)
        {
            if (idleMoveTimer.TotalMilliseconds <= 0)
            {
                int moveSteps = rand.Next(5,30);
                Direction directionToMove = (Direction)rand.Next(4);

                for (int i = 0; i < moveSteps; i++)
                {
                    idleMovementSteps.Enqueue(directionToMove);
                }

                idleMoveTimer = new TimeSpan(0, 0, 0, 0, moveSteps * 100);
            }
            else
            {
                idleMoveTimer = idleMoveTimer.Subtract(gameTime.ElapsedGameTime);
            }

            if (idleMovementSteps.Count > 0)
            {
                Move(idleMovementSteps.Dequeue(), moveSpeed);
            }
            else
            {
                walkingAnimation[currentDirection].ChangeAnimatingState(false);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (showHealthBar)
            {
                healthBar.Draw(spriteBatch, 0.9f);
            }

            ability.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }

        public override void TakeDamage(int damageToBeTaken)
        {
            showHealthBar = true;
            base.TakeDamage(damageToBeTaken);
        }
    }
}