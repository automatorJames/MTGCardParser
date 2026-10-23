namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rivaz of the Claw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rivaz of the Claw</exampledoc>
[TokenizationOrder(-1)]
public class RivazOfTheClawRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once during each of your turns, you may cast a dragon creature spell from your graveyard"];
}
