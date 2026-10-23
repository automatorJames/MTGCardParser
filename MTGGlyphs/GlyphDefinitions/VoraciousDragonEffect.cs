namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Voracious Dragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Voracious Dragon</exampledoc>
[Dependent]
public class VoraciousDragonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals damage to any target equal to twice the number of goblins it devoured"];
}
