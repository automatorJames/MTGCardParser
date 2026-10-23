namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rampaging Cyclops, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rampaging Cyclops</exampledoc>
[TokenizationOrder(-1)]
public class RampagingCyclopsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets -2/-0 as long as two or more creatures are blocking it"];
}
