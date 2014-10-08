﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PowerOfOne
{
    public class Telekinesis : Ability
    {
        private Texture2D pushTexture;
        private Vector2 pushDirection;
        private Vector2 pushStart;
        private Vector2 pushEnd;
        private float pushSpeed;
        private bool push;
        private float pushRotation;
        private TimeSpan pushTime;
        private int pushMiliSeconds;
        private List<Vector2> pushCollision;
        private int pullRadius;
        private bool pull;
        private float pullstrength;
        private TimeSpan pullTime;
        private int pullMiliSeconds;
        private Vector2 pullCenter;
        private Vector2 pullTextureOrigin;
        private Texture2D pullTexture;

        public Telekinesis(Entity owner):base(owner)
        {
            push = false;
            pushSpeed = 12f;
            pullstrength = 8f;
            pushMiliSeconds = 400;
            pullMiliSeconds = 200;
            pushCollision = new List<Vector2>();
        }

        public override void Load()
        {
            pushTexture = Scripts.LoadTexture(@"Abilities\Telekinesis\Push");
            pullTexture = Scripts.LoadTexture(@"Abilities\Telekinesis\Pull");
            pullTextureOrigin = new Vector2(pullTexture.Width / 2, pullTexture.Height / 2);
        }

        public override void ActivateBasicAbility()
        {
            Push();
        }

        public override void ActivateSecondaryAbility()
        {
            Pull();
        }

        public override void Update(GameTime gameTime)
        {
            if(push)
            {
                pushStart += pushDirection * pushSpeed;
                pushEnd += pushDirection * pushSpeed;
                pushTime = pushTime.Subtract(gameTime.ElapsedGameTime);
                if(pushTime.TotalMilliseconds<=0)
                {
                    push = false;
                }
                for (int i = 0; i < pushCollision.Count; i++)
                {
                    pushCollision[i] += pushDirection * pushSpeed;
                }
                CheckPushCollision();
            }
            if(pull)
            {
                //pullTime = pullTime.Subtract(gameTime.ElapsedGameTime);
                //if(pullTime.TotalMilliseconds <=0)
                //{
                //    pull = false;
                //}
                if(Main.mouse.RightReleased())
                {
                    pull = false;
                }
                else
                {
                    pullCenter.X = (float)Math.Round(Main.mouse.RealPosition.X);
                    pullCenter.Y = (float)Math.Round(Main.mouse.RealPosition.Y);
                }
                CheckForPull();
            }
        }

        private void CheckForPull()
        {
            foreach (Entity entity in Main.Entities)
            {
                float distance = Vector2.Distance(pullCenter, entity.Position);
                if (distance <= pullRadius)
                {
                    if(distance<pullstrength)
                    {
                        entity.MoveByPosition(pullCenter - entity.Position);
                    }
                    else
                    {
                        Vector2 direction = Vector2.Normalize(pullCenter - entity.Position);
                        entity.MoveByPosition(direction * pullstrength);
                    }
                }
            }
        }

        private void CheckPushCollision()
        {
            foreach (Entity entity in Main.Entities)
            {
                //if (entity != Owner)
                //{
                    foreach (Vector2 point in pushCollision)
                    {
                        if (Vector2.Distance(point, entity.Position) < entity.EntityHeight / 2)
                        {
                            if (entity.rect.Contains(point))
                            {
                                entity.MoveByPosition(pushDirection * pushSpeed);
                            }
                        }
                    }
                //}
            }
        }

        private void Push()
        {
            if (!push)
            {
                push = true;
                pushCollision.Clear();
                pushDirection = Vector2.Normalize(Main.mouse.RealPosition - Owner.Position);
                Vector2 teleCenter = Owner.Position + pushDirection * (Owner.EntityHeight / 2);
                float teleAngle = MathAid.FindRotation(teleCenter, Main.mouse.RealPosition);
                pushRotation = teleAngle;
                pushStart = Owner.Position - MathAid.AngleToVector(teleAngle + 90) * pushTexture.Height / 2;
                pushEnd = Owner.Position - MathAid.AngleToVector(teleAngle - 90) * pushTexture.Height / 2;
                pushTime = new TimeSpan(0, 0, 0, 0, pushMiliSeconds);
                Vector2 startToEndDirection = Vector2.Normalize(pushEnd - pushStart);
                for (int i = 0; i < pushTexture.Height ; i++)
                {
                    pushCollision.Add(pushStart + startToEndDirection * i);
                }
            }
        }

        private void Pull()
        {
            if (!pull)
            {
                pull = true;
                pullTime = new TimeSpan(0, 0, 0, 0, pullMiliSeconds);
                pullCenter.X = (float)Math.Round(Main.mouse.RealPosition.X);
                pullCenter.Y = (float)Math.Round(Main.mouse.RealPosition.Y);
                pullRadius = pullTexture.Width/2;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (push)
            {
                spriteBatch.Draw(pushTexture, pushStart, null, Color.White, pushRotation, new Vector2(), 1f, SpriteEffects.None, 1f);
            }
            if(pull)
            {
                spriteBatch.Draw(pullTexture, pullCenter, null, Color.White, 0, pullTextureOrigin, 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
