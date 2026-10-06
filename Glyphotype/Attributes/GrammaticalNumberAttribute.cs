namespace Glyphotype.Attributes;

/// <summary>
/// The number of what a <see cref="BackReference"/> refers to: see <see cref="SingularAttribute"/> and
/// <see cref="PluralAttribute"/>. Left off, it refers to referents of either number. (A referent's own number is
/// <see cref="ReferentAttribute.Number"/>.)
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public abstract class GrammaticalNumberAttribute(GrammaticalNumber number) : Attribute
{
    public GrammaticalNumber Number { get; } = number;

    /// <summary>The number <paramref name="member"/> declares, or <see cref="GrammaticalNumber.Unspecified"/>.</summary>
    public static GrammaticalNumber Of(MemberInfo member) =>
        member?.GetCustomAttributes<GrammaticalNumberAttribute>().FirstOrDefault()?.Number ?? GrammaticalNumber.Unspecified;
}

/// <summary>A back-reference, such as "it", that refers only to one thing.</summary>
public class SingularAttribute() : GrammaticalNumberAttribute(GrammaticalNumber.Singular);

/// <summary>A back-reference, such as "they", that refers only to several things.</summary>
public class PluralAttribute() : GrammaticalNumberAttribute(GrammaticalNumber.Plural);
