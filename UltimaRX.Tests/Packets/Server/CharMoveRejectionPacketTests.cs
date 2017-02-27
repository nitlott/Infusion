﻿using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class CharMoveRejectionPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x21, // packet
                0x0B, // sequence key
                0x12, 0x9A, // xloc
                0x05, 0x51, // yloc
                0x06, // direction
                0x0A, // zloc
            });

            var packet = new CharMoveRejectionPacket();
            packet.Deserialize(rawPacket);

            packet.SequenceKey.Should().Be(0x0B);
            packet.Location.X.Should().Be(0x129A);
            packet.Location.Y.Should().Be(0x0551);
            packet.Location.Z.Should().Be(0x0A);
            packet.Movement.Direction.Should().Be(Direction.West);
            packet.Movement.Type.Should().Be(MovementType.Walk);
        }
    }
}
