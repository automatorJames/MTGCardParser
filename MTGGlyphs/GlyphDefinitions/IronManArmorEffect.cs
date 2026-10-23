namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Iron Man Armor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Iron Man Armor</exampledoc>
/// <examplecapture>if this equipment isn't a creature, it becomes a 0/0 construct hero artifact creature with flying and "this creature gets +1/+1 for each artifact you control" until end of turn</examplecapture>
[Dependent]
public class IronManArmorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this equipment isn't a creature, it becomes a 0/0 construct hero artifact creature with flying and \"this creature gets +1/+1 for each artifact you control\" until end of turn"];
}
