namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Incubation Druid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Incubation Druid</exampledoc>
[TokenizationOrder(-1)]
public class IncubationDruidRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature has a +1/+1 counter on it, add three mana of that type instead"];
}
