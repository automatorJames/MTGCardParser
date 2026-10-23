namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tribal Golem, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tribal Golem</exampledoc>
/// <examplecapture>this creature has trample as long as you control a beast, haste as long as you control a goblin, first strike as long as you control a soldier, flying as long as you control a wizard, and "{b}: regenerate this creature" as long as you control a zombie</examplecapture>
[TokenizationOrder(-1)]
public class TribalGolemRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has trample as long as you control a beast, haste as long as you control a goblin, first strike as long as you control a soldier, flying as long as you control a wizard, and \"{b}: regenerate this creature\" as long as you control a zombie"];
}
