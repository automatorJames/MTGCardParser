namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Forgotten Lore, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Forgotten Lore</exampledoc>
/// <examplecapture>if you do, repeat this process except that opponent can't choose a card already chosen for {this}</examplecapture>
[TokenizationOrder(-1)]
public class ForgottenLoreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, repeat this process except that opponent can't choose a card already chosen for", Nib.This];
}
