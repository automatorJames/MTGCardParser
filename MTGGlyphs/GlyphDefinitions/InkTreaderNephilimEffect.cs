namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ink-Treader Nephilim, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ink-Treader Nephilim</exampledoc>
/// <examplecapture>if that spell targets only this creature, copy the spell for each other creature that spell could target</examplecapture>
[Dependent]
public class InkTreaderNephilimEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that spell targets only this creature, copy the spell for each other creature that spell could target"];
}
