namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kher Keep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kher Keep</exampledoc>
[Dependent]
public class KherKeepEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 0/1 red kobold creature token named kobolds of", Nib.This];
}
