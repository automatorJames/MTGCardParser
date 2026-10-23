namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sword of the Ages, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sword of the Ages</exampledoc>
/// <examplecapture>this artifact deals x damage to any target, where x is the total power of the creatures sacrificed this way, then exile this artifact and those creature cards</examplecapture>
[Dependent]
public class SwordOfTheAgesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this artifact deals x damage to any target, where x is the total power of the creatures sacrificed this way, then exile this artifact and those creature cards"];
}
