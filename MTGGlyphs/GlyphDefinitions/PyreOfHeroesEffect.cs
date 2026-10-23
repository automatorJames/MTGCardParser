namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pyre of Heroes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pyre of Heroes</exampledoc>
/// <examplecapture>search your library for a creature card that shares a creature type with the sacrificed creature and has mana value equal to 1 plus that creature's mana value</examplecapture>
[Dependent]
public class PyreOfHeroesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a creature card that shares a creature type with the sacrificed creature and has mana value equal to 1 plus that creature's mana value"];
}
