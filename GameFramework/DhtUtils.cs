using System;

namespace GameFramework
{
    public static class DhtUtils
    {
        private static Random random = new Random();

        /// <summary>
        /// Useful for finding out which bucket a node belongs to
        /// </summary>
        /// <param name="idA"></param>
        /// <param name="idB"></param>
        /// <returns>n in: 2^n <= distance(n1, n2) < 2^(n+1), e.g. first bit that differs</returns>
        public static int DistanceExp(Guid idA, Guid idB)
        {
            byte[] bytesA = idA.ToByteArray();
            byte[] bytesB = idB.ToByteArray();

            for (int i = 0; i < bytesA.Length; i++)
            {
                int xor = bytesA[i] ^ bytesB[i];

                if (xor == 0) continue;

                int bit = i * 8;
                for (int b = 7; b >= 0; b--)
                {
                    if (xor >= (1 << b)) return bit + (7 - b);
                }
                return bit;
            }

            return 0;
        }

        public static Guid GeneratePlayerId()
        {
            byte[] bytes = new byte[8];
            random.NextBytes(bytes);

            // For now leave only 2 bytes random non zero, for easy debugging
            bytes[0] = bytes[1] = bytes[2] = bytes[3] = bytes[4] = bytes[5] = 0;

            Guid g = new Guid(0, 0, 0, bytes);

            return g;
        }
    }
}
