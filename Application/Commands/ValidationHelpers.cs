using System;

namespace MyRental
{
    public static class ValidationHelpers
    {
        public static bool IsEmpty(this string x) => string.IsNullOrWhiteSpace(x);
        public static bool IsEmpty(this Guid x) => x == Guid.Empty;
    }
}