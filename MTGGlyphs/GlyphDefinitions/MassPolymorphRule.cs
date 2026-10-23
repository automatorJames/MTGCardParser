namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mass Polymorph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mass Polymorph</exampledoc>
[TokenizationOrder(-1)]
public class MassPolymorphRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put all creature cards revealed this way onto the battlefield, then shuffle the rest of the revealed cards into your library"];
}
