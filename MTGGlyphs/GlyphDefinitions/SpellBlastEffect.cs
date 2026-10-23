namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spell Blast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spell Blast</exampledoc>
[Dependent]
public class SpellBlastEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, if that spell's mana cost is {3}{u}{u}, x is 5"];
}
