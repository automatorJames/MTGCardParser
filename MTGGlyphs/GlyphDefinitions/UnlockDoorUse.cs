namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana on a room's door: "unlock a door", "unlock doors".</summary>
/// <exampledoc>Smoky Lounge // Misty Salon</exampledoc>
/// <examplecapture>unlock doors</examplecapture>
[Dependent]
public class UnlockDoorUse : Glyph
{
    public override Nib[] Nibs => [Opt("to"), "unlock", Alt("a door", "doors")];
}
