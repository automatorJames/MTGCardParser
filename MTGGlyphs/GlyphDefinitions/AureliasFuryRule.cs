namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aurelia's Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aurelia's Fury</exampledoc>
[TokenizationOrder(-1)]
public class AureliasFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players dealt damage this way can't cast noncreature spells this turn"];
}
