using System;

namespace DTech.AdUnification
{
    internal static class TypeExtensions
    {
        private const int DefaultComparisonValue = 100;

        public static int Comparison(this Type type, Type childrenType) =>
            type.IsInterface ? type.ComparisonInterface(childrenType) : type.ComparisonClass(childrenType);

        private static int ComparisonInterface(this Type type, Type childrenType)
        {
            if (childrenType == type)
            {
                return 0;
            }

            int weight = DefaultComparisonValue;
            Type[] interfaces = childrenType.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                if (!type.IsAssignableFrom(interfaceType))
                {
                    weight--;
                }
            }

            return weight;
        }

        private static int ComparisonClass(this Type type, Type childrenType)
        {
            if (childrenType == type)
            {
                return DefaultComparisonValue;
            }

            return ComparisonClass(type, childrenType.BaseType) + 1;
        }
    }
}