namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spellweaver Volute, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spellweaver Volute</exampledoc>
[Dependent]
public class SpellweaverVoluteEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy the enchanted instant card"];
}
