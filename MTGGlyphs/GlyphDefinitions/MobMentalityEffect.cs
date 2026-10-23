namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mob Mentality, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mob Mentality</exampledoc>
[Dependent]
public class MobMentalityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets +x/+0 until end of turn, where x is the number of attacking creatures"];
}
