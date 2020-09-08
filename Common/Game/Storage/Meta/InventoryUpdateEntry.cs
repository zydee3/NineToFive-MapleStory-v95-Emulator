using System;
using System.IO;
using NineToFive.Constants;

namespace NineToFive.Game.Storage.Meta {
    public class InventoryUpdateEntry {
        
        private readonly short _previousBagIndex;
        
        public ItemSlot Item { get; }
        public InventoryOperation Operation { get; }
        public bool Complete { get; set; } = true;
        
        public short PreviousBagIndex => Operation == InventoryOperation.Move ? _previousBagIndex : Item.BagIndex;

        public InventoryUpdateEntry(ref ItemSlot item, InventoryOperation operation, short previousBagIndex = 0) {
            if(operation == InventoryOperation.Move && previousBagIndex == 0) 
                throw new InvalidDataException("PreviousBagIndex cannot be zero");
            
            Item = item ?? throw new NullReferenceException("Item cannot be null");
            Operation = operation;
            _previousBagIndex = previousBagIndex;
        }
    }
}