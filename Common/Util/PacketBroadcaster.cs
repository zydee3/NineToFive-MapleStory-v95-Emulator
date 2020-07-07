using System;
using System.Collections.Generic;
using System.Linq;
using NineToFive.Game.Entity;

namespace NineToFive.Util {
    public abstract class PacketBroadcaster {
        private static readonly Func<Client, User, bool> ClientCanReceive = (client, user) => !user.IsHidden || client.GradeCode >= user.GradeCode;

        public abstract IEnumerable<Client> GetClients();

        /// <summary>
        /// Forwards the packet to all clients received via <see cref="GetClients"/>
        /// </summary>
        public void BroadcastPacket(byte[] packet) {
            foreach (var client in GetClients()) {
                client.Session.Write(packet);
            }
        }

        /// <summary>
        /// Forwards the packet to all clients under the conditions:
        /// <para>1. <paramref name="origin"/> is not hidden</para>
        /// <para>2. <paramref name="origin"/> is hidden and other clients have a higher or equal <see cref="Client.GradeCode"/></para>
        /// </summary>
        /// <param name="origin">the user origin (packet invoker)</param>
        /// <param name="packet">packet buffer to forward</param>
        public void BroadcastPacket(User origin, byte[] packet) {
            foreach (var client in GetClients().Where(c => ClientCanReceive.Invoke(c, origin))) {
                client.Session.Write(packet);
            }
        }

        /// <summary>
        /// Forwards the packet to all clients under the conditions:
        /// <para>1. <paramref name="origin"/> is not hidden</para>
        /// <para>2. <paramref name="origin"/> is hidden and other clients have a higher or equal <see cref="Client.GradeCode"/></para>
        /// <para>3. the receiving client is not the owner of the <paramref name="origin"/> user</para>
        /// </summary>
        /// <param name="origin">the user origin (packet invoker)</param>
        /// <param name="packet">packet buffer to forward</param>
        public void BroadcastPacketExclude(User origin, byte[] packet) {
            foreach (var client in GetClients().Where(c => ClientCanReceive.Invoke(c, origin) && c.Id != origin.AccountId)) {
                client.Session.Write(packet);
            }
        }
    }
}