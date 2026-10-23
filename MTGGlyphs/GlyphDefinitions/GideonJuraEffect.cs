namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gideon Jura, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon Jura</exampledoc>
[Dependent]
public class GideonJuraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during target opponent's next turn, creatures that player controls attack", Nib.This, "if able"];
}
