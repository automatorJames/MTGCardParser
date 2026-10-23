namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A legendary card's short name in the possessive, standing for the card itself ("molimo's power and toughness …", "sage of ancient lore's …"); offered only where a possessive opens a power/toughness ability, after every other possessive. The name itself is a <see cref="PossessedShortName"/>.</summary>
/// <exampledoc>Molimo, Maro-Sorcerer</exampledoc>
/// <examplecapture>molimo's</examplecapture>
[Dependent]
public class ShortNamePossessive : Glyph
{
    public override Nib[] Nibs => [Prop(Name), "'s"];

    public PossessedShortName Name { get; set; }
}
