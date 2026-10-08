namespace Glyphotype.Attributes;

/// <summary>
/// A grammatical number - see <see cref="SingularAttribute"/> and <see cref="PluralAttribute"/>. On a
/// <see cref="BackReference"/>, the number of what it refers to: left off, it refers to referents of either number. On a
/// <see cref="ReferentAttribute"/> class or property, the referent's own number: left off, a back-reference of either
/// number can refer to it. Nowhere else.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public abstract class GrammaticalNumberAttribute(GrammaticalNumber number) : Attribute
{
    public GrammaticalNumber Number { get; } = number;

    /// <summary>The number <paramref name="member"/> declares, or <see cref="GrammaticalNumber.Unspecified"/>.</summary>
    public static GrammaticalNumber Of(MemberInfo member) =>
        member?.GetCustomAttributes<GrammaticalNumberAttribute>().FirstOrDefault()?.Number ?? GrammaticalNumber.Unspecified;
}

/// <summary>One thing: a back-reference, such as "it", that refers only to one thing, or a referent that is one.</summary>
public class SingularAttribute() : GrammaticalNumberAttribute(GrammaticalNumber.Singular);

/// <summary>Several things: a back-reference, such as "they", that refers only to several things, or a referent that is several.</summary>
public class PluralAttribute() : GrammaticalNumberAttribute(GrammaticalNumber.Plural);
