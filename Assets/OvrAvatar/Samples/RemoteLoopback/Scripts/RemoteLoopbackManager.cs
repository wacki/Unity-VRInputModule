using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class RemoteLoopbackManager : MonoBehaviour {

    public OvrAvatar LocalAvatar;
    public OvrAvatar LoopbackAvatar;

    int packetSequence = 0;

	// Use this for initialization
	void Start () {
        LocalAvatar.RecordPackets = true;
        LocalAvatar.PacketRecorded += OnLocalAvatarPacketRecorded;
	}

    void OnLocalAvatarPacketRecorded(object sender, OvrAvatar.PacketEventArgs args)
    {
        using (MemoryStream outputStream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(outputStream);
            writer.Write(packetSequence++);
            args.Packet.Write(outputStream);
            SendPacketData(outputStream.ToArray());
        }
    }

    void SendPacketData(byte[] data)
    {
        // Loopback by just "receiving" the data
        ReceivePacketData(data);
    }

    void ReceivePacketData(byte[] data)
    {
        using (MemoryStream inputStream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(inputStream);
            int sequence = reader.ReadInt32();
            OvrAvatarPacket packet = OvrAvatarPacket.Read(inputStream);
            LoopbackAvatar.GetComponent<OvrAvatarRemoteDriver>().QueuePacket(sequence, packet);
        }
    }
}