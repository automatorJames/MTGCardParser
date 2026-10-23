namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vren, the Relentless, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vren, the Relentless</exampledoc>
/// <examplecapture>create x 1/1 black rat creature tokens with "this token gets +1/+1 for each other rat you control," where x is the number of creatures that were exiled under your opponents' control this turn</examplecapture>
[Dependent]
public class VrenTheRelentlessEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create x 1/1 black rat creature tokens with \"this token gets +1/+1 for each other rat you control,\" where x is the number of creatures that were exiled under your opponents' control this turn"];
}
