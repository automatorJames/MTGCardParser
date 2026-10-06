namespace Glyphotype.Attributes;

/// <summary>
/// The number of a <see cref="ReferentAttribute"/> referent, or of what a <see cref="BackReference"/> refers to: see
/// <see cref="SingularAttribute"/> and <see cref="PluralAttribute"/>. Left off, either agrees with anything.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public abstract class GrammaticalNumberAttribute(GrammaticalNumber number) : Attribute
{
    public GrammaticalNumber Number { get; } = number;

    /// <summary>The number <paramref name="member"/> declares, or <see cref="GrammaticalNumber.Unspecified"/>.</summary>
    public static GrammaticalNumber Of(MemberInfo member) =>
        member?.GetCustomAttributes<GrammaticalNumberAttribute>().FirstOrDefault()?.Number ?? GrammaticalNumber.Unspecified;
}

/// <summary>One thing: a referent such as "target creature", or a back-reference such as "it" that refers only to one.</summary>
public class SingularAttribute() : GrammaticalNumberAttribute(GrammaticalNumber.Singular);

/// <summary>Several things: a referent such as "all creatures", or a back-reference such as "they" that refers only to several.</summary>
public class PluralAttribute() : GrammaticalNumberAttribute(GrammaticalNumber.Plural);
