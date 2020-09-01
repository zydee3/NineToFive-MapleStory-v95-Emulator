using System.IO;
using NineToFive.Constants;

namespace NineToFive.Game.Storage.Meta {
    public class InventoryUpdateEntry {
        public Item Item { get; set; }
        public InventoryOperation Operation { get; set; }

        private readonly short _previousBagIndex;
        public short PreviousBagIndex => Operation == InventoryOperation.Move ? _previousBagIndex : Item.BagIndex;

        public InventoryUpdateEntry(ref Item item, InventoryOperation operation, short previousBagIndex = 0) {
            if(operation == InventoryOperation.Move && previousBagIndex == 0) 
                throw new InvalidDataException("PreviousBagIndex cannot be zero");
            
            Item = item;
            Operation = operation;
            _previousBagIndex = previousBagIndex;
        }
    }
}