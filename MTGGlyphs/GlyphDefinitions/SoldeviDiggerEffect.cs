namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Soldevi Digger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soldevi Digger</exampledoc>
[Dependent]
public class SoldeviDiggerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the top card of your graveyard on the bottom of your library"];
}
