namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Decree of Annihilation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Decree of Annihilation</exampledoc>
[TokenizationOrder(-1)]
public class DecreeOfAnnihilationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all artifacts, creatures, and lands from the battlefield, all cards from all graveyards, and all cards from all hands"];
}
