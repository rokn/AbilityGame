using System;
using Microsoft.Xna.Framework;

namespace PowerOfOne
{
    /// <summary>
    /// A class of static methods for common XNA taskeyboard
    /// </summary>
    public class MathAid
    {
        /// <summary>
        /// Take all aspects of a rectangle and scale them
        /// </summary>
        /// <param name="rectangle">The Rectangle to scale</param>
        /// <param name="scale">The current world scale</param>
        /// <returns>The scaled rectangle</returns>
        public static Rectangle ScaleWholeRectangle(Rectangle rectangle, float scale)
        {
            Rectangle rect = new Rectangle((int)(rectangle.X * scale),
                (int)(rectangle.Y * scale), (int)(rectangle.Width * scale),
                (int)(rectangle.Height * scale));
            return rect;
        }
        /// <summary>
        /// Scale the width and height of a Rectangle
        /// </summary>
        /// <param name="rectangle">The Rectangle to scale</param>
        /// <param name="scale">The current world scale</param>
        /// <returns>The scaled rectangle</returns>
        public static Rectangle ScaleRectangleSize(Rectangle rectangle, float scale)
        {
            Rectangle rect = new Rectangle(rectangle.X, rectangle.Y,
                (int)(rectangle.Width * scale),
                (int)(rectangle.Height * scale));
            return rect;
        }
        /// <summary>
        /// Scale the width and height of a Rectangle
        /// </summary>
        /// <param name="rectangle">The Rectangle to scale</param>
        /// <param name="scale">The current world scale</param>
        /// <returns>The scaled rectangle</returns>
        public static Rectangle ScaleRectangleSize(Rectangle rectangle, Vector2 scale)
        {
            Rectangle rect = new Rectangle(rectangle.X, rectangle.Y,
                (int)(rectangle.Width * scale.X),
                (int)(rectangle.Height * scale.Y));
            return rect;
        }
        /// <summary>
        /// Update the X, and Y of a Rectangle using a Vector2
        /// </summary>
        /// <param name="rectangle">The Rectangle to update</param>
        /// <param name="position">The current position</param>
        public static Rectangle UpdateRectViaVector(Rectangle rectangle, Vector2 position)
        {
            rectangle.X = (int)position.X;
            rectangle.Y = (int)position.Y;
            return rectangle;

        }
        /// <summary>
        /// Turn a Vector2 into a rectangle
        /// </summary>
        /// <param name="position">The X and Y of the new rectangle</param>
        /// <param name="width">the width</param>
        /// <param name="height">the height</param>
        /// <returns>The new rectangle</returns>
        public static Rectangle Vector2ToRectangle(Vector2 position, int width, int height)
        {
            Rectangle rect = new Rectangle((int)position.X, (int)position.Y, width, height);
            return rect;
        }
        /// <summary>
        /// Turn two Vector2's into a rectangle
        /// </summary>
        /// <param name="position">The X and Y of the new rectangle</param>
        /// <param name="size">the new size with x being the width and y the height</param>
        /// <returns></returns>
        public static Rectangle Vector2ToRectangle(Vector2 position, Vector2 size)
        {
            Rectangle rect = new Rectangle((int)position.X, (int)position.Y,
                (int)size.X, (int)size.Y);
            return rect;
        }
        /// <summary>
        /// Movement via a rotated Vector in 2D
        /// </summary>
        /// <param name="position">The current world position</param>
        /// <param name="rotation">The current rotation</param>
        /// <param name="speed">The current speed</param>
        /// <returns>The updated Vector2</returns>
        public static Vector2 RotatedVectorMovement(Vector2 position,
            float rotation, float speed)
        {
            position.X += speed * ((float)Math.Cos(rotation));
            position.Y += speed * ((float)Math.Sin(rotation));
            return position;
        }
        /// <summary>
        /// Movement via a rotated Vector in 2D
        /// </summary>
        /// <param name="position">The current world position</param>
        /// <param name="rotation">The current rotation</param>
        /// <param name="speed">The current speed</param>
        /// <param name="scale">The world scale</param>
        /// <returns>The updated Vector2</returns>
        public static Vector2 RotatedVectorMovement(Vector2 position,
            float rotation, float speed, float scale)
        {
            position.X += speed * ((float)Math.Cos(rotation)) * scale;
            position.Y += speed * ((float)Math.Sin(rotation)) * scale;
            return position;
        }
        /// <summary>
        /// Movement via a rotated Vector in 2D
        /// </summary>
        /// <param name="position">The current world position</param>
        /// <param name="rotation">The current rotation</param>
        /// <param name="speed">The current speed</param>
        /// <param name="scale">The world scale</param>
        /// <returns>The updated Vector2</returns>
        public static Vector2 RotatedVectorMovement(Vector2 position,
            float rotation, float speed, Vector2 scale)
        {
            position.X += speed * ((float)Math.Cos(rotation)) * scale.X;
            position.Y += speed * ((float)Math.Sin(rotation)) * scale.Y;
            return position;
        }
        /// <summary>
        /// Clamp one rectangle inside of another
        /// </summary>
        /// <param name="limitingRectangle">The rectangle to check against</param>
        /// <param name="movingRectangle">The rectangle inside of the limiting one</param>
        /// <returns>The updated moving Rectangle</returns>
        public static Rectangle RectangleClamp(Rectangle limitingRectangle, Rectangle movingRectangle)
        {
            if (movingRectangle.X < limitingRectangle.X)
            {
                movingRectangle.X = limitingRectangle.X;
            }
            else if (movingRectangle.Right > limitingRectangle.Right)
            {
                movingRectangle.X = limitingRectangle.Right - movingRectangle.Width;
            }
            if (movingRectangle.Y < limitingRectangle.Y)
            {
                movingRectangle.Y = limitingRectangle.Y;
            }
            else if (movingRectangle.Bottom > limitingRectangle.Bottom)
            {
                movingRectangle.Y = limitingRectangle.Bottom - movingRectangle.Height;
            }
            return movingRectangle;
        }
        /// <summary>
        /// Clamp a Vector2 inside a Rectangle
        /// </summary>
        /// <param name="limitingRectangle">The rectangle to check against</param>
        /// <param name="movingPosition">The Vector2 to check</param>
        /// <returns>Returns the updated vector2</returns>
        public static Vector2 RectangleClamp(Rectangle limitingRectangle, Vector2 movingPosition)
        {
            if (movingPosition.X < limitingRectangle.X)
            {
                movingPosition.X = limitingRectangle.X;
            }
            else if (movingPosition.X > limitingRectangle.Right)
            {
                movingPosition.X = limitingRectangle.Right;
            }
            if (movingPosition.Y < limitingRectangle.Y)
            {
                movingPosition.Y = limitingRectangle.Y;
            }
            else if (movingPosition.Y > limitingRectangle.Bottom)
            {
                movingPosition.Y = limitingRectangle.Bottom;
            }
            return movingPosition;
        }
        /// <summary>
        /// Find the transformed position of a child position based on
        /// the rotation of a parent object
        /// </summary>
        /// <param name="parentPosition">The real world parent position</param>
        /// <param name="childPosition">This is the normal distance from the child
        /// position to the parent position, not the real world position</param>
        /// <param name="rotation">The current parent rotation</param>
        /// <returns>Returns the transformed position of the child.</returns>
        public static Vector2 ParentChildTransform(Vector2 parentPosition,
            Vector2 childPosition, float rotation)
        {
            Matrix transform = Matrix.Identity;
            transform = Matrix.CreateTranslation(new Vector3(childPosition, 0))
                * Matrix.CreateRotationZ(rotation);
            Vector2 placement = Vector2.Transform(new Vector2(), transform);
            placement += parentPosition;
            return placement;
        }
        /// <summary>
        /// Find the transformed position of a child position based on
        /// the rotation and scale of a parent object
        /// </summary>
        /// <param name="parentPosition">The real world parent position</param>
        /// <param name="childPosition">This is the normal distance from the child
        /// position to the parent position, not the real world position</param>
        /// <param name="rotation">The current parent rotation</param>
        /// <param name="scale">The current scale</param>
        /// <returns>Returns the transformed position of the child.</returns>
        public static Vector2 ParentChildTransform(Vector2 parentPosition,
            Vector2 childPosition, float rotation, float scale)
        {
            Matrix transform = Matrix.Identity;
            transform = Matrix.CreateTranslation(new Vector3(childPosition, 0)) *
                Matrix.CreateScale(scale) *
                Matrix.CreateRotationZ(rotation);

            Vector2 placement = Vector2.Transform(new Vector2(), transform);
            placement += parentPosition;
            return placement;
        }
        /// <summary>
        /// Find the rotation from one position to another
        /// </summary>
        /// <param name="position">The current position</param>
        /// <param name="targetPosition">The position to find the rotation to</param>
        /// <returns>Returns the rotation to the designated position</returns>
        public static float FindRotation(Vector2 position, Vector2 targetPosition)
        {
            Vector2 place = new Vector2(targetPosition.X - position.X,
                targetPosition.Y - position.Y);
            float rotation = (float)Math.Atan2(place.Y, place.X);
            return rotation;
        }
        /// <summary>
        /// Find the rotation from one scaled position to a non scaled position
        /// for instance a scaled object interacting with a mouse.
        /// </summary>
        /// <param name="position">The current position</param>
        /// <param name="targetPosition">The position to find the rotation to</param>
        /// <param name="scale">the scale of the position object</param>
        /// <returns>Returns the rotation to the designated nonscaled position</returns>
        public static float FindRotation(Vector2 position, Vector2 targetPosition, float scale)
        {
            Vector2 place = new Vector2(targetPosition.X - position.X * scale,
                targetPosition.Y - position.Y * scale);

            float rotation = (float)Math.Atan2(place.Y, place.X);
            return rotation;
        }

        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static Rectangle GetIntersectingRectangle(Rectangle rect1, Rectangle rect2)
        {
            Rectangle intersectingRect = new Rectangle();

            if (rect1.Left > rect2.Left)
            {
                intersectingRect.X = rect1.Left;
            }
            else
            {
                intersectingRect.X = rect2.Left;
            }

            if (rect1.Top > rect2.Top)
            {
                intersectingRect.Y = rect1.Top;
            }
            else
            {
                intersectingRect.Y = rect2.Top;
            }

            if (rect1.Right > rect2.Right)
            {
                intersectingRect.Width = rect2.Right - intersectingRect.X;
            }
            else
            {
                intersectingRect.Width = rect1.Right - intersectingRect.X;
            }

            if (rect1.Bottom > rect2.Bottom)
            {
                intersectingRect.Height = rect2.Bottom - intersectingRect.Y;
            }
            else
            {
                intersectingRect.Height = rect1.Bottom - intersectingRect.Y;
            }

            return intersectingRect;
        }
    }
}