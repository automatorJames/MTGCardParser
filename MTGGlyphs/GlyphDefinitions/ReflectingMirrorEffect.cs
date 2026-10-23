namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reflecting Mirror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reflecting Mirror</exampledoc>
[Dependent]
public class ReflectingMirrorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["change the target of target spell with a single target if that target is you"];
}
