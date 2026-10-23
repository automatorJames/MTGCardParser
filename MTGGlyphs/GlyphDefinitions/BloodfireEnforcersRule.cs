namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bloodfire Enforcers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bloodfire Enforcers</exampledoc>
[TokenizationOrder(-1)]
public class BloodfireEnforcersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has first strike and trample as long as an instant card and a sorcery card are in your graveyard"];
}
