using System.Runtime.CompilerServices;

namespace Infrastructure.Postgres.Seeding.Shared
{
    public static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllLetters(this ReadOnlySpan<char> span)
        {
            for (int i = 0; i < span.Length; i++)
                if (!char.IsLetter(span[i])) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllDigits(this ReadOnlySpan<char> span)
        {
            for (int i = 0; i < span.Length; i++)
                if (!char.IsDigit(span[i])) return false;
            return true;
        }
    }


}
