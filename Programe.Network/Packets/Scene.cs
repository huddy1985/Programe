﻿using System.Collections.Generic;
using Lidgren.Network;

namespace Programe.Network.Packets
{
    public class Scene : Packet
    {
        public override PacketId Id
        {
            get { return PacketId.Scene; }
        }

        public float Width;
        public float Height;
        public List<NetObject> Items;

        public Scene()
        {
            Items = new List<NetObject>();
        }

        protected override void Write(NetOutgoingMessage message)
        {
            message.Write(Width);
            message.Write(Height);

            message.Write((ushort)Items.Count);
            foreach (var obj in Items)
            {
                NetObject.WriteToMessage(obj, message);
            }
        }

        protected override void Read(NetIncomingMessage message)
        {
            Width = message.ReadFloat();
            Height = message.ReadFloat();

            Items.Clear();
            var count = message.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                Items.Add(NetObject.ReadFromMessage(message));
            }
        }
    }
}
