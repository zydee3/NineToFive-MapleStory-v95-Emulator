using System;
using System.Numerics;
using NineToFive.Util;

namespace NineToFive.Game {
    public class Foothold {
        public int Next { get; set; }
        public int Prev { get; set; }
        public int X1   { get; set; }
        public int X2   { get; set; }
        public int Y1   { get; set; }
        public int Y2   { get; set; }
        public int Id   { get; set; }

        /// <summary>
        /// The left end point is the point that has the smaller x (further left). The furthest left point is used
        /// to calculate things like item drop position.
        /// </summary>
        public Vector2 LeftEndPoint  { get; set; }
        public Vector2 RightEndPoint { get; set; }
        public Vector2 SlopeForm     { get; set; } // x = m, y = b

        /// <summary>
        /// Checks if position passed in is between the left and right endpoints
        /// </summary>
        /// <param name="position">position being checked</param>
        /// <param name="offset">position + offset, used for when spawning mob drops so they don't clump</param>
        /// <returns>true if the position's x is between the left and right end points</returns>
        public bool InDomain(Vector2 position, int offset = 0) => LeftEndPoint.X <= position.X + offset && RightEndPoint.X >= position.X + offset;
        
        /// <summary>
        /// Finds the distance between a position and the point on the platform directly vertical of the position
        /// </summary>
        /// <param name="position">position being checked</param>
        /// <returns>distance</returns>
        public int GetRange(Vector2 position) => (int) (SlopeForm.X == 0 ? LeftEndPoint.Y - position.Y : GetYFromX(position.X) - position.Y);
        
        /// <summary>
        /// Calculates the y position on a line given x; used to find position y of item when spawning since x is constant
        /// </summary>
        /// <param name="x">horizontal position of an item</param>
        /// <returns>y position</returns>
        public float GetYFromX(float x) => SlopeForm.X * x + SlopeForm.Y;
        
        /// <summary>
        /// Initializes left and right end points as well as the variables needed to calculate position related things.
        /// </summary>
        public void SetVariables() {
            if (X1 < X2) {
                LeftEndPoint = new Vector2(X1, Y1);
                RightEndPoint = new Vector2(X2, Y2);
            } else {
                LeftEndPoint = new Vector2(X2, Y2);
                RightEndPoint = new Vector2(X1, Y1);
            }

            float m = RightEndPoint.X - LeftEndPoint.X == 0 ? 0 : (RightEndPoint.Y - LeftEndPoint.Y) / (RightEndPoint.X - LeftEndPoint.X);
            SlopeForm = new Vector2(m, RightEndPoint.Y - m * RightEndPoint.X);
        }
    }
}