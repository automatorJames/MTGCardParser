namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Firemane Angel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firemane Angel</exampledoc>
[Dependent]
public class FiremaneAngelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if", Nib.This, "is in your graveyard or on the battlefield, you may gain 1 life"];
}
