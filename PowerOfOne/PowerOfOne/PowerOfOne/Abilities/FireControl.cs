using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
namespace PowerOfOne
{
    public class FireControl : Ability
    {
        const int baseDamage = 1;

        private ParticleEngine particleSystem;
        private float damage;
        private Vector2 fireDirection;
        private Vector2 inscribedCircleCenter;
        private List<Texture2D> particles;
        private float hitDistance;
        private Texture2D phoenixSpriteSheet;
        private Dictionary<Direction, Animation> ownerAnimation;
        private Dictionary<Direction, Animation> phoenixAnimation;
        private bool IsPhoenix;
        private int DefaultEntityWidth;
        private int DefaultEntityHeight;
        private bool mouseReleased;
        private int PhoenixWidth;
        private int PhoenixHeight;
        private Rectangle phoenixWalkingRect;
        private Vector2 phoenixWalkingOrigin;
        private Rectangle ownerWalkingRect;
        private Vector2 ownerWalkingOrigin;

        public FireControl()
            :base ()
        {
        }

        public override void Initialize(Entity owner)
        {
            base.Initialize(owner);
            IsPhoenix = false;
            damage = baseDamage * Owner.AbilityPower;
            mouseReleased = true;
            ownerAnimation = new Dictionary<Direction, Animation>();
        }

        public override void Load()
        {
            base.Load();
            phoenixSpriteSheet = Scripts.LoadTexture(@"Abilities\FireControl\Phoenix");
            phoenixAnimation = Scripts.LoadEntityWalkAnimation(phoenixSpriteSheet);

            PhoenixWidth = phoenixSpriteSheet.Width / 4;
            PhoenixHeight = phoenixSpriteSheet.Height / 4;
            phoenixWalkingOrigin = Scripts.GetWalkingOrigin(PhoenixWidth, PhoenixHeight);
            phoenixWalkingRect = Scripts.GetWalkingRect(new Vector2(), PhoenixWidth, PhoenixHeight);

            particles = new List<Texture2D>() { Scripts.LoadTexture(@"Abilities\Circle") };
            particleSystem = new ParticleEngine(particles, Owner.Position);

            foreach (KeyValuePair<Direction, Animation> kvp in Owner.walkingAnimation)
            {
                ownerAnimation.Add(kvp.Key, kvp.Value);
            }
            DefaultEntityWidth = Owner.EntityWidth;
            DefaultEntityHeight = Owner.EntityHeight;
            ownerWalkingOrigin = Owner.WalkingOrigin;
            ownerWalkingRect = Owner.WalkingRect;
        }

        public override void Update(GameTime gameTime)
        {
            particleSystem.Update();
            if (!mouseReleased)
            {
                if (Main.mouse.RightReleased())
                {
                    mouseReleased = true;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            particleSystem.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }

        public override void ActivateBasicAbility()
        {
            damage = baseDamage * Owner.AbilityPower / 2;
            fireDirection = Vector2.Normalize(Main.mouse.RealPosition - Owner.Position);
            particleSystem.EmitterLocation = Owner.Position + fireDirection * (Owner.EntityHeight/2+8);
            float particleSpeed = Owner.AbilityPower;
            particleSystem.GenerateFireParticles(10 + 5*Owner.AbilityPower, fireDirection, Owner.AbilityPower);

            hitDistance =particleSpeed * 20f;
            inscribedCircleCenter = Owner.Position + fireDirection * hitDistance;

            FireHitEnemies();

            base.ActivateBasicAbility();
        }

        public override void ActivateSecondaryAbility()
        {
            if (mouseReleased)
            {
                if (!IsPhoenix)
                {
                    Owner.walkingAnimation = phoenixAnimation;
                    Owner.EntityNoClip = true;
                    Owner.EntityWidth = phoenixSpriteSheet.Width / 4;
                    Owner.EntityHeight = phoenixSpriteSheet.Height / 4;
                    Owner.WalkingRect = phoenixWalkingRect;
                    Owner.WalkingOrigin = phoenixWalkingOrigin;
                    IsPhoenix = true;
                }
                else
                {
                    Owner.walkingAnimation = ownerAnimation;
                    Owner.EntityNoClip = false;
                    Owner.EntityWidth = DefaultEntityWidth;
                    Owner.EntityHeight = DefaultEntityHeight;
                    Owner.WalkingRect = ownerWalkingRect;
                    Owner.WalkingOrigin = ownerWalkingOrigin;
                    IsPhoenix = false;
                }
                mouseReleased = false;
            }
            base.ActivateSecondaryAbility();
        }

        private void FireHitEnemies()
        {
            var EntitiesHit =
                from entity in Main.Entities
                where entity != Owner
                where Vector2.Distance(inscribedCircleCenter, entity.Position) <= hitDistance
                select entity;

            EntitiesHit.ForEach(entity => entity.TakeDamage(damage));
        }
    }
}
