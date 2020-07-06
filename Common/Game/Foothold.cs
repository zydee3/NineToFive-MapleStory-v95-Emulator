using System;
using System.Security.Policy;
using NineToFive.Util;

namespace NineToFive.Game {
    public class Foothold {
        public int Next { get; set; }
        public int Prev { get; set; }
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }
        public int ID { get; set; }
        /// <summary>
        /// The left end point is the point that has the smaller x (further left). The furthest left point is used
        /// to calculate things like item drop position.
        /// </summary>
        public Tuple<int, int> LeftEndPoint { get; set; }
        public Tuple<int, int> RightEndPoint { get; set; }

        public SlopeEquation SlopeForm { get; set; }

        //todo: set next, prv, x1, x2, y1, y2 to null after left and right end points are set as well as slope form.
        public Foothold() { }
        
        public void SetEndPoints() {
            if (X1 < X2) {
                LeftEndPoint = new Tuple<int, int>(X1, Y1);
                RightEndPoint = new Tuple<int, int>(X2, Y2);
            } else {
                LeftEndPoint = new Tuple<int, int>(X2, Y2);
                RightEndPoint = new Tuple<int, int>(X1, Y1);
            }
            
            SlopeForm = new SlopeEquation(LeftEndPoint, RightEndPoint);
        }
    }
}