namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Subterranean Schooner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Subterranean Schooner</exampledoc>
[Dependent]
public class SubterraneanSchoonerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature that crewed it this turn explores"];
}
