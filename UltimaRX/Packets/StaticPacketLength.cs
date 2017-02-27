﻿using Infusion.IO;

namespace Infusion.Packets
{
    internal class StaticPacketLength : PacketLength
    {
        private readonly int length;

        public StaticPacketLength(int length)
        {
            this.length = length;
        }

        public override int GetSize(IPacketReader reader)
        {
            return length;
        }
    }
}