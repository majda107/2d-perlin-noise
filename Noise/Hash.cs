using System;
using System.Collections.Generic;
using System.Text;

namespace PerlinNoise.Noise
{
    class Hash
    {
        public static uint Jenkins(string key)
        {
            int i = 0;
            uint hash = 0;
            while (i != key.Length)
            {
                hash += key[i++];
                hash += hash << 10;
                hash ^= hash >> 6;
            }

            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return hash;
        }
    }
}
