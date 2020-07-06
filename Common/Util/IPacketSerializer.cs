using NineToFive.Net;

namespace NineToFive.Util {
    public interface IPacketSerializer<T> {

        void Encode(T t, Packet p);

        void Decode(T t, Packet p);
    }
}