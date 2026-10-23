namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Greater Realm of Preservation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Greater Realm of Preservation</exampledoc>
[Dependent]
public class GreaterRealmOfPreservationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time a black or red source of your choice would deal damage to you this turn, prevent that damage"];
}
