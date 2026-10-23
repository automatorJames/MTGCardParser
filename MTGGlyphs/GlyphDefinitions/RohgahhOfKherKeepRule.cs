namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rohgahh of Kher Keep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rohgahh of Kher Keep</exampledoc>
[TokenizationOrder(-1)]
public class RohgahhOfKherKeepRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't, tap rohgahh and all creatures named kobolds of kher keep, then an opponent gains control of them"];
}
