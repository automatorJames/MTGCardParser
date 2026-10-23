namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Veiled Apparition, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Veiled Apparition</exampledoc>
/// <examplecapture>if this permanent is an enchantment, it becomes a 3/3 illusion creature with flying and "at the beginning of your upkeep, sacrifice this creature unless you pay {1}{u}</examplecapture>
[Dependent]
public class VeiledApparitionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this permanent is an enchantment, it becomes a 3/3 illusion creature with flying and \"at the beginning of your upkeep, sacrifice this creature unless you pay {1}{u}"];
}
