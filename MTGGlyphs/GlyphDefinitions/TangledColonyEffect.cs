namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tangled Colony, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tangled Colony</exampledoc>
/// <examplecapture>create x 1/1 black rat creature tokens with "this token can't block," where x is the amount of damage dealt to it this turn</examplecapture>
[Dependent]
public class TangledColonyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create x 1/1 black rat creature tokens with \"this token can't block,\" where x is the amount of damage dealt to it this turn"];
}
