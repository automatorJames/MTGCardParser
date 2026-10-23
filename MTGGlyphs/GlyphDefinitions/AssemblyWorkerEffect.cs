namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Assembly-Worker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Assembly-Worker</exampledoc>
[Dependent]
public class AssemblyWorkerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target", Nib.This, "creature gets +1/+1 until end of turn"];
}
