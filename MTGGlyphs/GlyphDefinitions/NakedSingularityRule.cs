namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Naked Singularity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Naked Singularity</exampledoc>
[TokenizationOrder(-1)]
public class NakedSingularityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if tapped for mana, plains produce {r}, islands produce {g}, swamps produce {w}, mountains produce {u}, and forests produce {b} instead of any other type"];
}
