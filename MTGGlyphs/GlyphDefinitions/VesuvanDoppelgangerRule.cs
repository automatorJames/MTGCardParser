namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vesuvan Doppelganger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vesuvan Doppelganger</exampledoc>
/// <examplecapture>you may have this creature enter as a copy of any creature on the battlefield, except it doesn't copy that creature's color and it has "at the beginning of your upkeep, you may have this creature become a copy of target creature, except it doesn't copy that creature's color and it has this ability</examplecapture>
[TokenizationOrder(-1)]
public class VesuvanDoppelgangerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature enter as a copy of any creature on the battlefield, except it doesn't copy that creature's color and it has \"at the beginning of your upkeep, you may have this creature become a copy of target creature, except it doesn't copy that creature's color and it has this ability"];
}
