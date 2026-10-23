namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Case File Auditor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Case File Auditor</exampledoc>
[Dependent]
public class CaseFileAuditorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the top six cards of your library"];
}
