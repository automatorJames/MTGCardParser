namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zimone, All-Questioning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zimone, All-Questioning</exampledoc>
[Dependent]
public class ZimoneAllQuestioning2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["2, 3, 5, 7, 11, 13, 17, 19, 23, 29, and 31 are prime numbers"];
}
