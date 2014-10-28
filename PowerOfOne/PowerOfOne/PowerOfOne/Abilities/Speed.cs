using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerOfOne
{
    public struct FlashStat
    {
        public Texture2D Texture;
        public Vector2 Position;
        public float Alpha;

        public FlashStat(Texture2D texture, Vector2 position, float alpha)
        {
            Texture = texture;
            Position = position;
            Alpha = alpha;
        }
    }

    public class Speed : Passive
    {
        

        private List<FlashStat> flashes;
        private TimeSpan flashTimer;

        public Speed()
            : base() 
        {
            flashes = new List<FlashStat>();
        }

        public override void ActivatePassive()
        {
            Owner.ChangeSpeed(Owner.BaseSpeed + 3 * Owner.AbilityPower);
            Owner.WeaponTime -= 30 * Owner.AbilityPower;
            Owner.AttackSpeed -= 50 * Owner.AbilityPower;
            base.ActivatePassive();
        }

        public override void DeactivatePassive()
        {
            Owner.ChangeSpeed(Owner.BaseSpeed);
            Owner.WeaponTime = Owner.BaseWeaponTime;
            Owner.AttackSpeed = Owner.BaseAttackSpeed;
            base.DeactivatePassive();
        }

        public override void Update(GameTime gameTime)
        {
            if(Activated)
            {
                if (flashTimer.TotalMilliseconds <= 0)
                {
                    Texture2D texture = Owner.walkingAnimation[Owner.currentDirection].GetCurrentTexture();
                    Vector2 position = Owner.Position - new Vector2(Owner.EntityWidth / 2, Owner.EntityHeight / 2);
                    flashes.Add(new FlashStat(texture, position, 0.8f));
                    flashTimer = new TimeSpan(0, 0, 0, 0, 15);
                }
            }

            if (flashes.Count > 11)
            {
                RemoveFlash();
            }

            if (flashes.Count > 0)
            {
                if (flashTimer.TotalMilliseconds <= 0)
                {
                    RemoveFlash();
                }
                else
                {
                    flashTimer = flashTimer.Subtract(gameTime.ElapsedGameTime);
                }

                DowngradeAlpha();
            }
        }

        private void DowngradeAlpha()
        {
            for (int i = 0; i < flashes.Count; i++)
            {
                flashes[i] = new FlashStat(flashes[i].Texture, flashes[i].Position, flashes[i].Alpha - 0.04f);
            }
        }

        private void RemoveFlash()
        {
            FlashStat flash = flashes.First();
            flashes.Remove(flash);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (FlashStat flash in flashes)
            {
                spriteBatch.Draw(flash.Texture, flash.Position , null, Color.White * flash.Alpha, 0, new Vector2(), 1f, SpriteEffects.None, Owner.baseDepth - 0.000001f);
            }
        }
    }
}