namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Urgent Necropsy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urgent Necropsy</exampledoc>
[TokenizationOrder(-1)]
public class UrgentNecropsyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell, collect evidence x, where x is the total mana value of the permanents this spell targets"];
}
