namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Minion of Leshrac, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Minion of Leshrac</exampledoc>
[TokenizationOrder(-1)]
public class MinionOfLeshracRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature deals damage to you this way, tap it"];
}
