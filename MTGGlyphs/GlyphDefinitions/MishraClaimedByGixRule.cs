namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mishra, Claimed by Gix // Mishra, Lost to Phyrexia, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mishra, Claimed by Gix // Mishra, Lost to Phyrexia</exampledoc>
/// <examplecapture>if mishra, claimed by gix and a creature named phyrexian dragon engine are attacking, and you both own and control them, exile them, then meld them into mishra, lost to phyrexia</examplecapture>
[TokenizationOrder(-1)]
public class MishraClaimedByGixRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if mishra, claimed by gix and a creature named phyrexian dragon engine are attacking, and you both own and control them, exile them, then meld them into mishra, lost to phyrexia"];
}
