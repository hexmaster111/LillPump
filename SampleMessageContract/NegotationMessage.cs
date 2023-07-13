using System.Runtime.InteropServices;
using System.Text;
using DataCom;

namespace SampleMessageContract;

public enum NegotiationMessageId : UInt16
{
    HelloWorld = 0,
    OtherMessage = 1,
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct HandshakeMessageData
{
    public HandshakeMessageData(byte version)
    {
        Version = version;
        Reserved = new byte[7];
    }

    public readonly byte Version;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
    public readonly byte[] Reserved;
}

public class SharedStream
{
    public object ReadLock { get; } = new();
    public object WriteLock { get; } = new();
    public Stream Stream { get; }

    public SharedStream(Stream stream)
    {
        Stream = stream;
    }
}

/// <summary>
///     This is any message that will be talking over any stream
/// </summary>
public abstract class TcpMessagePostBase : AbstractMessagePost<NegotiationMessageId, HandshakeMessageData>
{
    public override StreamReader Reader { get; }
    public override StreamWriter Writer { get; }
    public override object ReadLock => _sharedStream.ReadLock;
    public override object WriteLock => _sharedStream.WriteLock;
    private readonly SharedStream _sharedStream;

    protected TcpMessagePostBase(SharedStream s)
    {
        _sharedStream = s;
        Reader = new StreamReader(_sharedStream.Stream, Encoding.UTF8);
        Writer = new StreamWriter(_sharedStream.Stream, Encoding.UTF8);
    }
}

/// <summary>
///     Top level, this of this like a dependency property
/// </summary>
public class HelloWorldMessagePost : TcpMessagePostBase
{
    public override ushort MessageNumbBase => 0;
    public override NegotiationMessageId MessageId => NegotiationMessageId.HelloWorld;

    public HelloWorldMessagePost(SharedStream s) : base(s)
    {
    }
}