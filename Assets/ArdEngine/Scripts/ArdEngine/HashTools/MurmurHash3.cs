using System.Collections.Generic;

namespace ArdEngine.HashTools
{
    public static class MurmurHash3
    {
        public static int Hash(IReadOnlyList<byte> input)
        {
            const uint seed = 258499;
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            uint hash = seed;
            int length = input.Count;
            uint current;

            for (var i = 0; i < length - 3; i += 4)
            {
                current = (uint) (input[i] | input[i + 1] << 8 | input[i + 2] << 16 | input[i + 3] << 24);
                current *= c1;
                current = RotateLeft32(current, 15);
                current *= c2;

                hash ^= current;
                hash = RotateLeft32(hash, 13);
                hash = hash * 5 + 0xe6546b64;
            }

            current = 0;
            int remainder = length - length / 4 * 4;
            int lastChunk = length - remainder;
            for (var i = 0; i < remainder; i++)
            {
                current |= (uint) (input[lastChunk + i] << (8 * i));
            }

            if (current > 0)
            {
                current *= c1;
                current = RotateLeft32(current, 15);
                current *= c2;
                hash ^= current;
            }

            hash ^= (uint) length;
            hash ^= hash >> 16;
            hash *= 0x85ebca6b;
            hash ^= hash >> 13;
            hash *= 0xc2b2ae35;
            hash ^= hash >> 16;
            return (int) hash;

            static uint RotateLeft32(uint x, byte r) => (x << r) | (x >> (32 - r));
        }
    }
}