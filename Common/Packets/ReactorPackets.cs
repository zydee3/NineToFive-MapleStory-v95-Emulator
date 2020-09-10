using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class ReactorPool {
        public static byte[] GetReactorChangeState(Reactor reactor) {
            using Packet w = new Packet();
            w.WriteShort((short) CReactorPool.OnReactorChangeState);
            w.WriteUInt(reactor.Id);
            w.WriteByte(); // CReactorPool *pThis[4]
            w.WriteShort((short) reactor.Location.X);
            w.WriteShort((short) reactor.Location.Y);
            w.WriteByte(); // CReactorPool *pThis[9]
            w.WriteByte(); // CReactorPool *pThis[11]
            return w.ToArray();
        }

        public static byte[] GetReactorMove(Reactor reactor) {
            using Packet w = new Packet();
            w.WriteShort((short) CReactorPool.OnReactorMove);
            w.WriteUInt(reactor.Id);
            w.WriteShort((short) reactor.Location.X);
            w.WriteShort((short) reactor.Location.Y);
            return w.ToArray();
        }

        public static byte[] GetReactorEnterField(Reactor reactor) {
            using Packet w = new Packet();
            w.WriteShort((short) CReactorPool.OnReactorEnterField);
            w.WriteUInt(reactor.Id);
            w.WriteInt(reactor.TemplateId);
            w.WriteByte(); // CReactorPool *pThis[4] , // CReactorPool *pThis[3]
            w.WriteShort((short) reactor.Location.X);
            w.WriteShort((short) reactor.Location.Y);
            w.WriteByte(); // CReactorPool *pThis[16] 
            w.WriteString();
            return w.ToArray();
        }

        public static byte[] GetReactorLeaveField(Reactor reactor) {
            using Packet w = new Packet();
            w.WriteShort((short) CReactorPool.OnReactorLeaveField);
            w.WriteUInt(reactor.Id);
            return w.ToArray();
        }
    }
}