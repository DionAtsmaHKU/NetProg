using Unity.Collections;
using Unity.Networking.Transport;

public class NetKeepAlive : NetMessage
{
    // For sending a message
    public NetKeepAlive()
    {
        Code = OpCode.KEEP_ALIVE;
    }

    // For receiving message
    public NetKeepAlive(DataStreamReader reader)
    {
        Code = OpCode.KEEP_ALIVE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }
}
