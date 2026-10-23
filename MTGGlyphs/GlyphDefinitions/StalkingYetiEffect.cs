namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Stalking Yeti, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stalking Yeti</exampledoc>
/// <examplecapture>if it's on the battlefield, it deals damage equal to its power to target creature an opponent controls and that creature deals damage equal to its power to this creature</examplecapture>
[Dependent]
public class StalkingYetiEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's on the battlefield, it deals damage equal to its power to target creature an opponent controls and that creature deals damage equal to its power to this creature"];
}
