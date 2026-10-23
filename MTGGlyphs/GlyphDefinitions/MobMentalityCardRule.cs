namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mob Mentality, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mob Mentality</exampledoc>
[TokenizationOrder(-1)]
public class MobMentalityCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever all non-wall creatures you control attack, enchanted creature gets +x/+0 until end of turn, where x is the number of attacking creatures"];
}
