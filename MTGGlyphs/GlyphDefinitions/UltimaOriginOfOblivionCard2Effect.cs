namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ultima, Origin of Oblivion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ultima, Origin of Oblivion</exampledoc>
[Dependent]
public class UltimaOriginOfOblivionCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add an additional {c}"];
}
