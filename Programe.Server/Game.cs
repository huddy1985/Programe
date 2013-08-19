﻿using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Programe.Machine;
using Programe.Network;
using Programe.Network.Packets;
using Programe.Server.NetObjects;

namespace Programe.Server
{
    public static class Game
    {
        private static World world;
        private static List<Ship> ships;
        private static int timer;

        public static void Start()
        {
            // TODO: investivate fix for farseer quadreee: http://farseerphysics.codeplex.com/workitem/30555
            world = new World(new Vector2(0, 0));
            ships = new List<Ship>();

            CreateBounds(20, 12);

            // TODO: random asteroids
            var asteroid = CreateAsteroid();
            asteroid.Position = new Vector2(8, 2);
        }

        public static void Update()
        {
            // remove dead ships
            foreach (var ship in ships.Where(s => s.Dead).ToList())
            {
                world.RemoveBody(ship.Body);
                ships.Remove(ship);
            }

            //TODO: spawn if theres room

            foreach (var ship in ships)
            {
                ship.Update();
            }

            world.Step((float)Constants.SecondsPerUpdate);

            timer++;
            if (timer >= 2)
            {
                var scene = new Scene();

                foreach (var body in world.BodyList)
                {
                    var data = (RadarData)body.UserData;

                    if (data.UserData == null)
                        continue;

                    var netObj = (NetObject)data.UserData;
                    scene.Objects.Add(netObj);
                }

                Server.Broadcast(scene, NetDeliveryMethod.UnreliableSequenced, 0);
                timer = 0;
            }
        }

        public static void Spawn(Ship ship)
        {
            var body = CreateShip();
            body.UserData = new RadarData(RadarType.Ship, new NetShip(ship));

            // TODO: ship spawn location
            body.Position = new Vector2(2, 6);
            body.Rotation = 0f;

            ship.Spawn(world, body);
            ships.Add(ship);
        }

        #region Physics Objects
        private static void CreateBullets(Body ship)
        {
            Func<Vector2, Body> createBullet = pos =>
            {
                var body = new Body(world);
                body.BodyType = BodyType.Dynamic;
                body.IsBullet = true;

                var shape = new CircleShape(0.15f / 4, 1);
                body.CreateFixture(shape);

                body.Position = ship.Position + ship.GetWorldVector(pos);
                body.Rotation = ship.Rotation;
                body.ApplyForce(body.GetWorldVector(new Vector2(0.0f, -15.0f)));

                body.OnCollision += (a, b, contact) =>
                {
                    world.RemoveBody(body);
                    return false;
                };

                return body;
            };

            createBullet(new Vector2(-0.575f, -0.20f));
            createBullet(new Vector2(0.575f, -0.20f));
        }

        private static Body CreateShip()
        {
            var body = new Body(world);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 0.5f;
            body.AngularDamping = 1f;

            // tip
            var rect1 = new PolygonShape(1);
            rect1.SetAsBox(0.25f, 0.5f, new Vector2(0, -0.5f), 0);

            // tail
            var rect2 = new PolygonShape(3);
            rect2.SetAsBox(0.75f, 0.5f, new Vector2(0, 0.5f), 0);

            body.CreateFixture(rect1);
            body.CreateFixture(rect2);

            return body;
        }

        private static Body CreateAsteroid()
        {
            var body = new Body(world);
            body.BodyType = BodyType.Dynamic;
            body.LinearDamping = 1f;
            body.AngularDamping = 1f;
            body.UserData = new RadarData(RadarType.Asteroid, new NetAsteroid(body));

            var shape = new CircleShape(1, 10);
            body.CreateFixture(shape);
            return body;
        }

        private static void CreateBounds(float width, float height)
        {
            var body = new Body(world);
            body.BodyType = BodyType.Static;
            body.UserData = new RadarData(RadarType.Wall);

            var bottom = new PolygonShape(5);
            bottom.SetAsBox(width / 2, 0.1f, new Vector2(width / 2, height), 0);

            var top = new PolygonShape(5);
            top.SetAsBox(width / 2, 0.1f, new Vector2(width / 2, 0), 0);

            var left = new PolygonShape(5);
            left.SetAsBox(0.1f, height / 2, new Vector2(0, height / 2), 0);

            var right = new PolygonShape(5);
            right.SetAsBox(0.1f, height / 2, new Vector2(width, height / 2), 0);

            body.CreateFixture(bottom);
            body.CreateFixture(top);
            body.CreateFixture(left);
            body.CreateFixture(right);
        }
        #endregion
    }
}
