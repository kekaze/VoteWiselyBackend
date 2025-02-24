namespace VoteWiselyBackend.Extensions
{
    public static class VariableTypeExtensions
    {
        public static bool IsListOfType(this Type type, Type elementType)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(List<>)
                && type.GetGenericArguments()[0] == elementType;
        }
    }
}
