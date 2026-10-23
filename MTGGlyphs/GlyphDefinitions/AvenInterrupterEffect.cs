namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aven Interrupter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aven Interrupter</exampledoc>
[Dependent]
public class AvenInterrupterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its owner may cast it as a sorcery on a later turn without paying its mana cost"];
}
