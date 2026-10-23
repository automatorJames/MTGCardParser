namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Case File Auditor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Case File Auditor</exampledoc>
[TokenizationOrder(-1)]
public class CaseFileAuditorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature enters and whenever you solve a case, look at the top six cards of your library"];
}
