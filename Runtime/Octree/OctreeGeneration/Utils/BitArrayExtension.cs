//+
using System.Collections;


namespace Octree.OctreeGeneration.Utils
{
    static public class BitArrayExtension 
    {
        public static string BitsToString(this BitArray bitarray)
        {
            string strbit = ""; 
            foreach(bool bit in bitarray)
            {
                strbit += (bit ? 1 : 0);
            }
            return strbit;
        }
    }
}