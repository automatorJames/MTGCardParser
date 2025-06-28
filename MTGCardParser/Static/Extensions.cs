namespace MTGCardParser.Static;

public static class Extensions
{
    public static List<PropertyInfo> GetPropertiesForCapture(this Type type) =>
         type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(p => !p.GetMethod.IsVirtual)
        .Where(x => (Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType).IsEnum || x.PropertyType == typeof(bool) || x.PropertyType == typeof(TokenSegment) || x.PropertyType.IsAssignableTo(typeof(ITokenCapture)))
        .ToList();
}

