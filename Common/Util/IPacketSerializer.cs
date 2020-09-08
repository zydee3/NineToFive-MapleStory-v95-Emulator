using NineToFive.Net;

namespace NineToFive.Util {
    public interface IPacketSerializer {

        void Encode(Packet p);

        void Decode(Packet p);
    }
}