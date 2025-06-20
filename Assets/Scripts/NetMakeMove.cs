using Unity.Collections;
using Unity.Networking.Transport;

public class NetMakeMove : NetMessage
{
    public int positionPressed;
    public int teamId;

    // For sending a message
    public NetMakeMove()
    {
        Code = OpCode.MAKE_MOVE;
    }

    // For receiving message
    public NetMakeMove(DataStreamReader reader)
    {
        Code = OpCode.MAKE_MOVE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(positionPressed);
        writer.WriteInt(teamId);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        positionPressed = reader.ReadInt();
        teamId = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_MAKE_MOVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
    }
}
