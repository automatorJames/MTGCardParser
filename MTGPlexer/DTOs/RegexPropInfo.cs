namespace MTGPlexer.DTOs
{
    public record RegexPropInfo
    {
        public PropertyInfo Prop { get; }
        public RegexPropType RegexPropType { get; }
        public bool IsTokenUnitMany { get; }
        public Type BaseType { get; }
        public Type UnderlyingType { get; }
        public string Name { get; }
        public string FriendlyTypeName { get; }
        public string FriendlyPropName { get; }

        public RegexPropInfo(PropertyInfo prop)
        {
            Prop = prop;
            (RegexPropType, IsTokenUnitMany, BaseType) = GetCapturePropType(prop);
            UnderlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            Name = prop.Name;
            FriendlyPropName = prop.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
            FriendlyTypeName = GetFriendlyTypeName(BaseType);
        }

        private static (RegexPropType, bool, Type) GetCapturePropType(PropertyInfo prop)
        {
            Type type = prop.PropertyType;
            bool isArray = false;

            if (type.IsArray)
            {
                isArray = true;
                type = type.GetElementType()!;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(TokenUnitMany<>))
            {
                isArray = true;
                type = type.GetGenericArguments()[0];
            }

            type = Nullable.GetUnderlyingType(type) ?? type;

            RegexPropType regexPropType =
                type.IsEnum ? RegexPropType.Enum :
                type == typeof(PlaceholderCapture) ? RegexPropType.Placeholder :
                type == typeof(bool) ? RegexPropType.Bool :
                typeof(TokenUnitOneOf).IsAssignableFrom(type) ? RegexPropType.TokenUnitOneOf :
                typeof(TokenUnit).IsAssignableFrom(type) ? RegexPropType.TokenUnit :
                prop.GetCustomAttribute<DistilledValueAttribute>() != null ? RegexPropType.DistilledValue :
                throw new Exception($"{prop.PropertyType.Name} is not a valid {nameof(RegexPropType)} type");

            return (regexPropType, isArray, type);
        }

        private static string GetFriendlyTypeName(Type type)
        {
            bool isNullableEnum = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum;

            if (type.IsEnum || isNullableEnum)
                return "enum";

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return $"{type.GetGenericArguments()[0].Name}".ToFriendlyCase(TitleDisplayOption.Sentence);

            if (type == typeof(int))
                return "int";

            if (type == typeof(PlaceholderCapture))
                return "placeholder";

            return type.Name.ToFriendlyCase(TitleDisplayOption.Sentence).ToLower();
        }
    }
}
