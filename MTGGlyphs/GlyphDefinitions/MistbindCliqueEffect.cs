namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mistbind Clique, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mistbind Clique</exampledoc>
[Dependent]
public class MistbindCliqueEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all lands target player controls"];
}
