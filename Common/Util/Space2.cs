using System.Numerics;

namespace NineToFive.Util {
    public class Space2 {
        public int? X1 { get; set; }
        public int? X2 { get; set; }
        public int? X3 { get; set; }
        
        public int? Y1 { get; set; }
        public int? Y2 { get; set; }
        public int? Y3 { get; set; }

        public bool IsInside(Vector2 Position) {
            float X = Position.X;
            float Y = Position.Y;
            
            //todo: evaluate
            return false;
        }
    }
}