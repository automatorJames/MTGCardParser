namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vesuvan Drifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vesuvan Drifter</exampledoc>
[TokenizationOrder(-1)]
public class VesuvanDrifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you reveal a creature card this way, this creature becomes a copy of that card until end of turn, except it has flying"];
}
