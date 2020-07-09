using System.Numerics;

namespace NineToFive.Game {
    public class Portal {
        public Portal(in byte id) {
            Id = id;
        }

        public byte Id { get; }
        public string Name { get; set; }
        public Vector2 Location { get; set; }
        public string TargetPortalName { get; set; }
        public int TargetPortalId { get; set; }
        public int TargetMap { get; set; }
    }
}