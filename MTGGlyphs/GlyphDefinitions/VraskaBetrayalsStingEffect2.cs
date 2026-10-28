namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vraska, Betrayal's Sting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vraska, Betrayal's Sting</exampledoc>
/// <examplecapture>target creature becomes a treasure artifact with "{t}, sacrifice this artifact: add one mana of any color" and loses all other card types and abilities</examplecapture>
[Dependent]
public class VraskaBetrayalsStingEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature becomes a treasure artifact with \"{t}, sacrifice this artifact: add one mana of any color\" and loses all other card types and abilities"];
}
