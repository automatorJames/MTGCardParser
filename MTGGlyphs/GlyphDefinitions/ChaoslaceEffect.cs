namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chaoslace, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaoslace</exampledoc>
[Dependent]
public class ChaoslaceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its mana symbols remain unchanged"];
}
