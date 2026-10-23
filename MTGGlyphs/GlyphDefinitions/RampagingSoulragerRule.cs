namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rampaging Soulrager, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rampaging Soulrager</exampledoc>
[TokenizationOrder(-1)]
public class RampagingSoulragerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +3/+0 as long as there are two or more unlocked doors among rooms you control"];
}
