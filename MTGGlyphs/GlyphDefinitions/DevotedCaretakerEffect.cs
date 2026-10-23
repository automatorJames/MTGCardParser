namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Devoted Caretaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Devoted Caretaker</exampledoc>
[Dependent]
public class DevotedCaretakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target permanent you control gains protection from instant spells and from sorcery spells until end of turn"];
}
