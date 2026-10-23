namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Infernal Denizen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infernal Denizen</exampledoc>
/// <examplecapture>if you can't, tap this creature, and an opponent may gain control of a creature you control of their choice for as long as this creature remains on the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class InfernalDenizenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you can't, tap this creature, and an opponent may gain control of a creature you control of their choice for as long as this creature remains on the battlefield"];
}
