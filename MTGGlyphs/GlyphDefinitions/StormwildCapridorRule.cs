namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stormwild Capridor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stormwild Capridor</exampledoc>
[TokenizationOrder(-1)]
public class StormwildCapridorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if noncombat damage would be dealt to this creature, prevent that damage"];
}
