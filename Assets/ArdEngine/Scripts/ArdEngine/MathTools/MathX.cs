using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ArdEngine.MathTools
{
    public static class MathX
    {
        [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mod(this int value, int mod)
        {
            return (value %= mod) < 0 ? value + mod : value;
        }
    }
}