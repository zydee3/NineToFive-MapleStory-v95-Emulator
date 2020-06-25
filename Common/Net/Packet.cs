using System;
using System.IO;
using System.Net;
using System.Text;

namespace NineToFive.IO {
    public enum PacketAccess { Read, Write }
    public class Packet : IDisposable {
        private MemoryStream Stream { get; set; }
        private BinaryWriter Writer { get; set; }
        private BinaryReader Reader { get; set; }

        /// <summary>
        /// underlying array used in the stream, writer and reader.<br/>
        /// used for socket listeners incoming information
        /// </summary>
        public byte[] Array { get; private set; }
        /// <summary>
        /// capacity of the array<br/>
        /// used to determine how much information the <see cref="Array"/> can fit
        /// </summary>
        public int Capacity { get => Array.Length; set => TryGrow(value); }
        public PacketAccess PacketAccess { get; set; }
        /// <summary>
        /// amount of information stored in the array<br/>
        /// meaning if the Capacity is 512 and there's 58 bytes stored by the socket, the Size will be 58
        /// </summary>
        public int Size { get; set; }

        public int Position {
            get => (int)Stream.Position;
            set => Stream.Position = value;
        }
        /// <summary>
        /// dependant on the PacketAccess mode<br/>
        /// if reading, returns how much information can be read starting from the stream's <see cref="Position"/><br/>
        /// if writing, returns how much information can be written given the current <see cref="Size"/> and <see cref="Capacity"/>
        /// </summary>
        public int Available => (PacketAccess == PacketAccess.Read) ? Size - Position : Capacity - Size;

        /// <summary>
        /// constructor for Write access
        /// </summary>
        /// <param name="capacity">the size of the underlying array (default is 512)</param>
        public Packet(int capacity = 512) : this(new byte[capacity]) {
            PacketAccess = PacketAccess.Write;
            Size = 0;
        }

        /// <summary>
        /// constructor for read access
        /// </summary>
        /// <param name="data">information to store in the underlying stream</param>
        public Packet(byte[] data) {
            Array = data;
            Capacity = data.Length;

            Stream = new MemoryStream(data);
            Writer = new BinaryWriter(Stream);
            Reader = new BinaryReader(Stream);

            Size = data.Length;
            Position = 0;
            PacketAccess = PacketAccess.Read;
        }

        public byte this[int index] {
            get => Array[index];
            set => Array[index] = value;
        }

        /// <summary>
        /// Prevents read and write methods from being used too freely.
        /// </summary>
        /// <param name="require"></param>
        private void CheckAccessAllowed(PacketAccess require) {
            if (PacketAccess == require) return;
            throw new InvalidOperationException(string.Format("{0} required but currently {1}", require, PacketAccess));
        }

        public override string ToString() {
            return ASCIIEncoding.ASCII.GetString(Array);
        }

        public string ToArrayString(bool hex = false) {
            if (hex) {
                return BitConverter.ToString(Array).Replace("-", " ");
            }
            StringBuilder sb = new StringBuilder("{ ");
            foreach (byte b in Array) {
                sb.Append(b).Append(", ");
            }
            if (Capacity > 0) sb.Remove(sb.Length - 2, 2);
            return sb.Append(" }").ToString();
        }

        /// <summary>
        /// create an array of bytes from <see cref="Position"/> to <see cref="Available"/>
        /// </summary>
        /// <param name="relative">if the array should contain only information relative to the Position of the stream</param>
        public byte[] ToArray(bool relative = true) {
            byte[] b;
            switch (PacketAccess) {
                case PacketAccess.Read:
                    b = new byte[Available];
                    int startIndex = relative ? Position : 0;
                    Buffer.BlockCopy(Array, startIndex, b, 0, Available);
                    break;
                case PacketAccess.Write:
                    b = new byte[Size];
                    Buffer.BlockCopy(Array, 0, b, 0, Size);
                    break;
                default:
                    throw new InvalidOperationException(PacketAccess.ToString());
            }
            return b;
        }

        public void TryGrow(int newCapacity) {
            if (newCapacity <= Capacity) return;
            byte[] expandArray = new byte[Capacity];
            Buffer.BlockCopy(Array, 0, expandArray, 0, Array.Length);
            Array = expandArray;

            int currentPosition = this.Position;
            Dispose();
            Stream = new MemoryStream(expandArray) { Position = currentPosition };
            Writer = new BinaryWriter(Stream);
            Reader = new BinaryReader(Stream);
        }

        public void Dispose() {
            Reader.Dispose();
            Writer.Dispose();
            Stream.Dispose();
        }

        #region write methods
        public Packet WriteBytes(params byte[] collection) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(collection);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteByte(byte item = 0) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteSByte(sbyte item = 0) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteShort(short item = 0) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteUShort(ushort item = 0) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteInt(int item = 0) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteUInt(uint item = 0) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteLong(long item = 0) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteFloat(float item = 0) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteBool(bool item) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteString(string item, params object[] args) {
            CheckAccessAllowed(PacketAccess.Write);
            item = item ?? string.Empty;

            if (item != null) {
                item = string.Format(item, args);
            }
            Writer.Write((short)item.Length);
            foreach (char c in item) {
                Writer.Write(c);
            }
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteStringFixed(string item, int length) {
            CheckAccessAllowed(PacketAccess.Write);
            foreach (char c in item) {
                Writer.Write(c);
            }
            for (int i = item.Length; i < length; i++) {
                Writer.Write((byte)0);
            }
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteDateTime(DateTime item) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write((long)(item.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteKoreanDateTime(DateTime item) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write((long)(item.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds * 10000 + 116444592000000000L);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteIPAddress(IPAddress value) {
            CheckAccessAllowed(PacketAccess.Write);
            Writer.Write(value.GetAddressBytes());
            if (Position > Size) Size = Position;
            return this;
        }
        #endregion

        #region read methods
        public byte[] ReadBytes(int count) {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadBytes(count);
        }

        public byte[] ReadBytes() {
            CheckAccessAllowed(PacketAccess.Read);
            return ReadBytes(Available);
        }

        public byte ReadByte() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadByte();
        }

        public sbyte ReadSByte() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadSByte();
        }

        public short ReadShort() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadInt16();
        }

        public ushort ReadUShort() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadUInt16(); ;
        }

        public int ReadInt() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadInt32();
        }

        public uint ReadUInt() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadUInt32();
        }

        public long ReadLong() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadInt64();
        }

        public float ReadFloat() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadSingle();
        }

        public bool ReadBool() {
            CheckAccessAllowed(PacketAccess.Read);
            return Reader.ReadBoolean();
        }

        public string ReadString() {
            CheckAccessAllowed(PacketAccess.Read);
            short count = Reader.ReadInt16();
            char[] result = new char[count];
            for (int i = 0; i < count; i++) {
                result[i] = (char)Reader.ReadByte();
            }
            return new string(result);
        }

        public IPAddress ReadIPAddress() {
            CheckAccessAllowed(PacketAccess.Read);
            IPAddress result = new IPAddress(Reader.ReadBytes(4));
            return result;
        }
        #endregion
    }
}
