namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Knight of the Holy Nimbus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Knight of the Holy Nimbus</exampledoc>
[Dependent]
public class KnightOfTheHolyNimbusEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap it, remove it from combat, and heal all damage on it"];
}
