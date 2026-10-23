namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hundred-Battle Veteran, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hundred-Battle Veteran</exampledoc>
[TokenizationOrder(-1)]
public class HundredBattleVeteranRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as there are three or more different kinds of counters among creatures you control, this creature gets +2/+4"];
}
