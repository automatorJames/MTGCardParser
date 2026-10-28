namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mavinda, Students' Advocate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mavinda, Students' Advocate</exampledoc>
[TokenizationOrder(-1)]
public class MavindaStudentsAdvocateRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["timing rules for the spell still apply"];
}
