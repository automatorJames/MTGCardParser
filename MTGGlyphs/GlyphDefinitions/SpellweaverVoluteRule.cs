namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spellweaver Volute, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spellweaver Volute</exampledoc>
[TokenizationOrder(-1)]
public class SpellweaverVoluteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, exile the enchanted card and attach this aura to another instant card in a graveyard"];
}
