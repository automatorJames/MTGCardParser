namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sarulf, Realm Eater, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarulf, Realm Eater</exampledoc>
[TokenizationOrder(-1)]
public class SarulfRealmEaterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, exile each other nonland permanent with mana value less than or equal to the number of counters removed this way"];
}
