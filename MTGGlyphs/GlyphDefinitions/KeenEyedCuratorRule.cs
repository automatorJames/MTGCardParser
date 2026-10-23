namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Keen-Eyed Curator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keen-Eyed Curator</exampledoc>
[TokenizationOrder(-1)]
public class KeenEyedCuratorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as there are four or more card types among cards exiled with this creature, it gets +4/+4 and has trample"];
}
