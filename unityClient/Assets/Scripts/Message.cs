using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using global::System;
//using global::System.Collections.Generic;
using global::FlatBuffers;

namespace Assets.Scripts
{
    public struct Message : IFlatbufferObject
    {
        private Table __p;
        public ByteBuffer ByteBuffer { get { return __p.bb; } }
        public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
        public static Message GetRootAsMessage(ByteBuffer _bb) { return GetRootAsMessage(_bb, new Message()); }
        public static Message GetRootAsMessage(ByteBuffer _bb, Message obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
        public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
        public Message __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

        public int ProtocolId { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
        public string Content { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetContentBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
        public ArraySegment<byte>? GetContentBytes() { return __p.__vector_as_arraysegment(6); }
#endif
        public byte[] GetContentArray() { return __p.__vector_as_array<byte>(6); }

        public static Offset<Message> CreateMessage(FlatBufferBuilder builder,
            int Protocol_Id = 0,
            StringOffset contentOffset = default(StringOffset))
        {
            builder.StartTable(2);
            Message.AddContent(builder, contentOffset);
            Message.AddProtocolId(builder, Protocol_Id);
            return Message.EndMessage(builder);
        }

        public static void StartMessage(FlatBufferBuilder builder) { builder.StartTable(2); }
        public static void AddProtocolId(FlatBufferBuilder builder, int ProtocolId) { builder.AddInt(0, ProtocolId, 0); }
        public static void AddContent(FlatBufferBuilder builder, StringOffset contentOffset) { builder.AddOffset(1, contentOffset.Value, 0); }
        public static Offset<Message> EndMessage(FlatBufferBuilder builder)
        {
            int o = builder.EndTable();
            return new Offset<Message>(o);
        }
        public static void FinishMessageBuffer(FlatBufferBuilder builder, Offset<Message> offset) { builder.Finish(offset.Value); }
        public static void FinishSizePrefixedMessageBuffer(FlatBufferBuilder builder, Offset<Message> offset) { builder.FinishSizePrefixed(offset.Value); }
    };


}
