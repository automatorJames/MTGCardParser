namespace Glyphotype.NibHelpers;

/// <summary>An optional plural suffix on the nib before it: what a <see cref="PluralNib"/> is matched as (see <see cref="PluralNib.Flatten"/>).</summary>
public record OptionalPluralNib : Nib
{
    const string _suffixes = "(s|es|ies)?";

    public OptionalPluralNib()
        : base(_suffixes, _suffixes) { }
}
