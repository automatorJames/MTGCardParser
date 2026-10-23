namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Venser, Corpse Puppet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Venser, Corpse Puppet</exampledoc>
[TokenizationOrder(-1)]
public class VenserCorpsePuppetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• if you don't control a creature named the hollow sentinel, create the hollow sentinel, a legendary 3/3 colorless phyrexian golem artifact creature token"];
}
