namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pygmy Hippo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pygmy Hippo</exampledoc>
/// <examplecapture>you may have defending player activate a mana ability of each land they control and lose all unspent mana</examplecapture>
[Dependent]
public class PygmyHippoEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have defending player activate a mana ability of each land they control and lose all unspent mana"];
}
