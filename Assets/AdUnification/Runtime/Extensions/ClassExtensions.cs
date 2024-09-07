using System;

namespace DTech.AdUnification
{
    internal static class ClassExtensions
    {
        public static T ThrowIfNull<T>(this T source)
        {
            if (source == null)
            {
                throw new NullReferenceException("Instance is null!");
            }
            
            return source;
        }
    }
}