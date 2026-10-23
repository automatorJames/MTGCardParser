namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pyromancer Ascension, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pyromancer Ascension</exampledoc>
/// <examplecapture>you may put a quest counter on this enchantment</examplecapture>
[Dependent]
public class PyromancerAscensionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a quest counter on this enchantment"];
}
