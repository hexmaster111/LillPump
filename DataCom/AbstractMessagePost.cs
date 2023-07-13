using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DataCom;

public abstract class AbstractMessagePost<TMessageId, TMessageData>
    where TMessageId : Enum, IConvertible, IComparable, IFormattable
    where TMessageData : struct
{
    public abstract UInt16 MessageNumbBase { get; }
    public abstract TMessageId MessageId { get; }

    public abstract object ReadLock { get; }
    public abstract object WriteLock { get; }
    public abstract StreamWriter Writer { get; }
    public abstract StreamReader Reader { get; }

    public event Action<TMessageData>? Recevied;

    public UInt16 MessageIdNumber => Convert.ToUInt16(MessageId);

    public UInt32 MessageIdFull => (UInt32)MessageNumbBase << 16 | MessageIdNumber;
    public int MessageDataSize => Marshal.SizeOf<TMessageData>();

    public void Write(TMessageData data)
    {
        var serialized = Serialize(data);
        lock (WriteLock)
        {
            Writer.Write(serialized);
            Writer.Flush();
        }
    }

    public byte[] Serialize(TMessageData messData)
    {
        var bytes = new byte[MessageDataSize];
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        Marshal.StructureToPtr(messData, handle.AddrOfPinnedObject(), false);
        handle.Free();
        return bytes;
    }

    public TMessageData Deserialize(byte[] data)
    {
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        var message = Marshal.PtrToStructure<TMessageData>(handle.AddrOfPinnedObject());
        handle.Free();
        return message;
    }


    //TODO: This should be a background thread
    public bool EnableReader { get; set; }
    
    private void ReadFunc()
    {
        
    }
}