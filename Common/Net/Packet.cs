using System;
using System.IO;
using System.Net;
using System.Text;

namespace NineToFive.IO {
    public class Packet : IDisposable {
        private MemoryStream Stream { get; set; }
        private BinaryWriter Writer { get; set; }
        private BinaryReader Reader { get; set; }

        /// <summary>
        /// <para>capacity of the array
        /// used to determine how much information the <see cref="Array"/> can fit</para>
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// <para>amount of information stored in the array meaning if the
        /// <see cref="Capacity"/> is 512 and there's 58 bytes stored by the socket, the Size will be 58</para>
        /// </summary>
        public int Size { get; set; }

        public int Position {
            get => (int) Stream.Position;
            set => Stream.Position = value;
        }

        /// <summary>
        /// constructor for read access
        /// </summary>
        /// <param name="data">information to store in the underlying stream</param>
        public Packet(byte[] data = null) {
            Capacity = data?.Length ?? int.MaxValue;
            Size = data?.Length ?? 0;
            if (data != null) {
                // import data, we are reading an array of bytes
                Stream = new MemoryStream(data);
            } else {
                // writing data, it's important that this constructor is used
                // so we can have an expandable capacity for our stream
                Stream = new MemoryStream();
            }

            Position = 0;
            Writer = new BinaryWriter(Stream);
            Reader = new BinaryReader(Stream);
        }

        public override string ToString() {
            return Encoding.ASCII.GetString(Stream.ToArray());
        }

        public string ToArrayString(bool hex = false) {
            if (hex) {
                return BitConverter.ToString(Stream.ToArray()).Replace("-", " ");
            }

            StringBuilder sb = new StringBuilder("{ ");
            foreach (byte b in Stream.ToArray()) {
                sb.Append(b).Append(", ");
            }

            if (Capacity > 0) sb.Remove(sb.Length - 2, 2);
            return sb.Append(" }").ToString();
        }

        /// <summary>
        /// create an array of bytes from the underlying Stream
        /// </summary>
        /// <param name="relative">if the array should contain only information relative to the Position of the stream</param>
        public byte[] ToArray(bool relative = false) {
            return relative ? Stream.GetBuffer() : Stream.ToArray();
        }

        public void Dispose() {
            Reader.Dispose();
            Writer.Dispose();
            Stream.Dispose();
        }

        #region write methods

        public Packet WriteBytes(params byte[] collection) {
            Writer.Write(collection);
            if (Position > Size) Size = Position;
            return this;
        }

        public byte WriteByte(byte item = 0) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return item;
        }

        public Packet WriteSByte(sbyte item = 0) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public short WriteShort(short item = 0) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return item;
        }

        public Packet WriteUShort(ushort item = 0) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public int WriteInt(int item = 0) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return item;
        }

        public Packet WriteUInt(uint item = 0) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteLong(long item = 0) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteFloat(float item = 0) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return this;
        }

        public bool WriteBool(bool item) {
            Writer.Write(item);
            if (Position > Size) Size = Position;
            return item;
        }

        public Packet WriteString(string item = "") {
            byte[] buffer = Encoding.ASCII.GetBytes(item);
            Writer.Write((short) buffer.Length);
            Writer.Write(buffer);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteStringFixed(string item, int length) {
            if (item.Length > length) throw new ArgumentException($"input string exceeds ({item.Length}) the length limit ({length})");
            Writer.Write(item.ToCharArray());
            for (int i = item.Length; i < length; i++) {
                Writer.Write((byte) 0);
            }
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteDateTime(DateTime item) {
            Writer.Write((long) (item.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteKoreanDateTime(DateTime item) {
            Writer.Write((long) (item.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds * 10000 + 116444592000000000L);
            if (Position > Size) Size = Position;
            return this;
        }

        public Packet WriteIPAddress(IPAddress value) {
            Writer.Write(value.GetAddressBytes());
            if (Position > Size) Size = Position;
            return this;
        }

        #endregion

        #region read methods

        public byte[] ReadBytes(int count) {
            return Reader.ReadBytes(count);
        }

        public byte ReadByte() {
            return Reader.ReadByte();
        }

        public sbyte ReadSByte() {
            return Reader.ReadSByte();
        }

        public short ReadShort() {
            return Reader.ReadInt16();
        }

        public ushort ReadUShort() {
            return Reader.ReadUInt16();
            ;
        }

        public int ReadInt() {
            return Reader.ReadInt32();
        }

        public uint ReadUInt() {
            return Reader.ReadUInt32();
        }

        public long ReadLong() {
            return Reader.ReadInt64();
        }

        public float ReadFloat() {
            return Reader.ReadSingle();
        }

        public bool ReadBool() {
            return Reader.ReadBoolean();
        }

        public string ReadString(int? length = null) {
            char[] result = new char[length ?? ReadShort()];
            for (int i = 0; i < result.Length; i++) {
                result[i] = (char) Reader.ReadByte();
            }
            return new string(result);
        }

        public IPAddress ReadIPAddress() {
            IPAddress result = new IPAddress(Reader.ReadBytes(4));
            return result;
        }

        #endregion
    }
}