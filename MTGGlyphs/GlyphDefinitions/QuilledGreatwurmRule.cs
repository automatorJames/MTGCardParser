namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Quilled Greatwurm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quilled Greatwurm</exampledoc>
[TokenizationOrder(-1)]
public class QuilledGreatwurmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard by removing six counters from among creatures you control in addition to paying its other costs"];
}
