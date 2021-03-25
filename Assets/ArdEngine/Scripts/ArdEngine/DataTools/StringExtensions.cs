using System.Runtime.CompilerServices;
using System.Text;
using ArdEngine.HashTools;

namespace ArdEngine.DataTools
{
    public static class StringExtensions
    {
        private const int SIMILARITY_SCORE = 3;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetStableHash(this string value)
        {
            return MurmurHash3.Hash(Encoding.UTF8.GetBytes(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSimilar(this string stringA, string stringB)
        {
            return stringA.GetDamerauLevenshteinDistance(stringB) < SIMILARITY_SCORE;
        }

        public static bool FuzzyContains(this string text, string query)
        {
            return text.GetSubstringDamerauLevenshteinDistance(query) < SIMILARITY_SCORE;
        }
    }
}