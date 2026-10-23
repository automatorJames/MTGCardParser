namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Strionic Resonator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Strionic Resonator</exampledoc>
[Dependent]
public class StrionicResonatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a triggered ability uses the words \"when,\" \"whenever,\" or \"at"];
}
