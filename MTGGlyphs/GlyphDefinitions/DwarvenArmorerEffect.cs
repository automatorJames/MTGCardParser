namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dwarven Armorer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dwarven Armorer</exampledoc>
[Dependent]
public class DwarvenArmorerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +0/+1 counter or a +1/+0 counter on target creature"];
}
