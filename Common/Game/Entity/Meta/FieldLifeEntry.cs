namespace NineToFive.Game.Entity.Meta {
    /// <summary>
    /// This holds the data from map/life as a template for reference when initializing new fields. 
    /// </summary>
    public class FieldLifeEntry {
        public int X   { get; set; }
        public int Y   { get; set; }
        public int Cy  { get; set; }
        public int Rx0 { get; set; }
        public int Rx1 { get; set; }
        public int MobTime { get; set; }
        public int FootholdID { get; set; }
        
        public bool Flipped { get; set; }
        public bool Hidden  { get; set; }

        public int ID { get; set; }
        
        public FieldLifeEntry() { }
    }
}