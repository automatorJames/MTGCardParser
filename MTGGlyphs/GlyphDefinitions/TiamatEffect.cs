namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tiamat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tiamat</exampledoc>
/// <examplecapture>if you cast it, search your library for up to five dragon cards not named {this} that each have different names, reveal them, put them into your hand, then shuffle</examplecapture>
[Dependent]
public class TiamatEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast it, search your library for up to five dragon cards not named", Nib.This, "that each have different names, reveal them, put them into your hand, then shuffle"];
}
