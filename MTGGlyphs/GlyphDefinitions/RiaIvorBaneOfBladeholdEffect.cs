namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ria Ivor, Bane of Bladehold, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ria Ivor, Bane of Bladehold</exampledoc>
/// <examplecapture>the next time target creature would deal combat damage to one or more players this combat, prevent that damage</examplecapture>
[Dependent]
public class RiaIvorBaneOfBladeholdEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time target creature would deal combat damage to one or more players this combat, prevent that damage"];
}
