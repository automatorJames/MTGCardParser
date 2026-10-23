namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Legion Angel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Legion Angel</exampledoc>
[Dependent]
public class LegionAngelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may reveal a card you own named", Nib.This, "from outside the game and put it into your hand"];
}
