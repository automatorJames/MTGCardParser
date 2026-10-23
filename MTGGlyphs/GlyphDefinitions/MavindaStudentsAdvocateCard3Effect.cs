namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mavinda, Students' Advocate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mavinda, Students' Advocate</exampledoc>
[Dependent]
public class MavindaStudentsAdvocateCard3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you still pay the spell's costs"];
}
