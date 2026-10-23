namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mavinda, Students' Advocate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mavinda, Students' Advocate</exampledoc>
[TokenizationOrder(-1)]
public class MavindaStudentsAdvocateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that spell doesn't target a creature you control, it costs {8} more to cast this way"];
}
