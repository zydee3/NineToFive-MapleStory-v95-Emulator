namespace NineToFive.Game {
    public class Portal {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public string TargetPortalName { get; set; }
        public int TargetPortalID { get; set; }
        public int TargetMap { get; set; }

        public Portal() {
            
        }
    }
}