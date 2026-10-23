namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hobgoblin Captain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hobgoblin Captain</exampledoc>
/// <examplecapture>if you attacked with creatures with total power 6 or greater this combat, this creature gains first strike until end of turn</examplecapture>
[Dependent]
public class HobgoblinCaptainEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with creatures with total power 6 or greater this combat, this creature gains first strike until end of turn"];
}
