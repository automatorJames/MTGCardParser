namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Void Stalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Void Stalker</exampledoc>
[Dependent]
public class VoidStalkerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this creature and target creature on top of their owners' libraries, then those players shuffle their libraries"];
}
