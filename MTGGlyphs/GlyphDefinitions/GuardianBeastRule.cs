namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guardian Beast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guardian Beast</exampledoc>
/// <examplecapture>as long as this creature is untapped, noncreature artifacts you control can't be enchanted, they have indestructible, and other players can't gain control of them</examplecapture>
[TokenizationOrder(-1)]
public class GuardianBeastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this creature is untapped, noncreature artifacts you control can't be enchanted, they have indestructible, and other players can't gain control of them"];
}
