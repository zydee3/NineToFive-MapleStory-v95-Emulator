using NineToFive.IO;
using NineToFive.Util;

namespace NineToFive.Game.Storage {
    public class Item : IPacketSerializer<Item> {
        public Item(int id) {
            Id = id;
        }

        public virtual byte Type => 2;
        public int Id { get; }
        public short BagIndex { get; set; }
        public long CashItemSn { get; set; }

        public virtual void Encode(Item item, Packet p) {
            p.WriteByte(item.Type);

            p.WriteInt(Id);
            if (p.WriteBool(CashItemSn > 0)) {
                // liCashItemSN
                p.WriteLong(CashItemSn);
            }

            p.WriteLong(); // dateExpire
        }

        public virtual void Decode(Item item, Packet p) { }
    }

    public struct CashItemSn {
        public int LowPart;
        public int HighPart;
    }
}