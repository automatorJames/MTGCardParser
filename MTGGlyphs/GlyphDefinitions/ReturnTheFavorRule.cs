namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Return the Favor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Return the Favor</exampledoc>
[TokenizationOrder(-1)]
public class ReturnTheFavorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["+ {1} — copy target instant spell, sorcery spell, activated ability, or triggered ability"];
}
