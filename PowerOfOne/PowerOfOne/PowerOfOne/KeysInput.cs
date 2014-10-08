using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PowerOfOne
{
    public class KeysInput
    {
        private KeyboardState currentKeyboard;
        private KeyboardState oldKeyboard;

        public KeysInput()
        {
            currentKeyboard = new KeyboardState();
            oldKeyboard = new KeyboardState();
        }

        public void Update(GameTime gameTime)
        {
            oldKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
        }

        /// <summary>
        /// Use this to determine if the key is being held
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>returns true if the key is held.</returns>
        public bool IsHeld(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Use this to check if the key was just released
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Returns true if the key was released</returns>
        public bool IsReleased(Keys key)
        {
            if (currentKeyboard.IsKeyUp(key) && oldKeyboard.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Use this to see if the key was just pressed
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Returns true if the key was just pressed</returns>
        public bool JustPressed(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
