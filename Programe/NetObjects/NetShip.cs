﻿using System;
using Lidgren.Network;
using Programe.Network;
using SFML.Graphics;
using SFML.Window;

namespace Programe.NetObjects
{
    public class NetShip : DrawableNetObject
    {
        private float x;
        private float y;
        private float rotation;

        public override NetObjectType Type
        {
            get { return NetObjectType.Ship; }
        }

        protected override void Write(NetOutgoingMessage message)
        {
            throw new NotImplementedException();
        }

        protected override void Read(NetIncomingMessage message)
        {
            message.ReadString();
            x = message.ReadFloat();
            y = message.ReadFloat();
            rotation = message.ReadFloat();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            sprite.Position = new Vector2f(x * Constants.PixelsPerMeter, y * Constants.PixelsPerMeter);
            sprite.Rotation = rotation * (180f / (float)Math.PI);
            target.Draw(sprite);
        }

        private static Sprite sprite;
        static NetShip()
        {
            var texture = new Texture("Data/ship.png");
            sprite = new Sprite(texture);
            sprite.Origin = new Vector2f((float)texture.Size.X / 2, (float)texture.Size.Y / 2);
        }
    }
}
