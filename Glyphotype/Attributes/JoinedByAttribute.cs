namespace Glyphotype.Attributes;

/// <summary>
/// Declares the separator between a <see cref="CompoundOf{T}"/>'s items. Defaults to
/// <see cref="Joiner.CommaSpace"/> when absent (e.g. "flying, first strike"); declare otherwise where the
/// surrounding English spells the compound differently (e.g. <c>[JoinedBy(Joiner.Space)]</c> for "artifact
/// creature").
/// <para>
/// The joiner is a fact about the usage site, not about <c>T</c> or the compound's meaning: "flying, first
/// strike" and "artifact creature" are both sets of items that all apply, differing only in spelling. So it's
/// declared where the compound is used - on a <see cref="CompoundOf{T}"/> property, or on a
/// <see cref="CompoundOf{T}"/> subclass (e.g. a top-level alias like <c>CardAbilityLine</c>, which has no
/// property to decorate). When both apply, the property wins, being the more specific of the two.
/// </para>
/// <para>
/// A single joiner per site: every item in one match is joined the same way. Only valid on a
/// <see cref="CompoundOf{T}"/> property or subclass, and never <see cref="Joiner.Pipe"/> (which would turn
/// the repetition into an alternation) - see <see cref="Glyph.ValidateStructure"/>.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class JoinedByAttribute : Attribute
{
    public Joiner Joiner { get; }

    public JoinedByAttribute(Joiner joiner)
    {
        Joiner = joiner;
    }
}
