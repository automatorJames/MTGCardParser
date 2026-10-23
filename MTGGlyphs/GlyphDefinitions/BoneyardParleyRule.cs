namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Boneyard Parley, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Boneyard Parley</exampledoc>
[TokenizationOrder(-1)]
public class BoneyardParleyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put all cards from the pile of your choice onto the battlefield under your control and the rest into their owners' graveyards"];
}
