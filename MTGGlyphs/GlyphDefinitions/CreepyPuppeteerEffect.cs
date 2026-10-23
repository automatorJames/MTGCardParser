namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Creepy Puppeteer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Creepy Puppeteer</exampledoc>
/// <examplecapture>if you attacked with exactly one other creature this combat, you may have that creature's base power and toughness become 4/3 until end of turn</examplecapture>
[Dependent]
public class CreepyPuppeteerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with exactly one other creature this combat, you may have that creature's base power and toughness become 4/3 until end of turn"];
}
