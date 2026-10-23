namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Johann, Apprentice Sorcerer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Johann, Apprentice Sorcerer</exampledoc>
[TokenizationOrder(-1)]
public class JohannApprenticeSorcererRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once each turn, you may cast an instant or sorcery spell from the top of your library"];
}
