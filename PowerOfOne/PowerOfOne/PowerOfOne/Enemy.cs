using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        public Enemy(Vector2 pos,int id)
            : base(pos)
        {
            moveSpeed = 4;
            idleMovementSteps = new Queue<Direction>();
            rand = new Random();
            ability = new Telekinesis(this);
            this.id = id;
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

            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            healthBar.Update(Position - healtBarPositionOffset, health, maxHealth);
            base.Update(gameTime);
        }

        private void IdleMovement()
        {
            int moveRand = rand.Next(100);

            if (moveRand <= MoveChance)
            {
                int moveSteps = rand.Next(20);
                Direction directionToMove = (Direction)rand.Next(4);

                for (int i = 0; i < moveSteps; i++)
                {
                    idleMovementSteps.Enqueue(directionToMove);
                }
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

            base.Draw(spriteBatch);
        }

        public override void TakeDamage(int damageToBeTaken)
        {
            showHealthBar = true;
            base.TakeDamage(damageToBeTaken);
        }
    }
}