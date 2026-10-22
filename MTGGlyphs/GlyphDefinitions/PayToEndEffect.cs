namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A Licid's way out: "you may pay {r} to end this effect" - paying a cost ends the continuous effect the ability just made.</summary>
/// <exampledoc>Convulsing Licid</exampledoc>
/// <examplecapture>you may pay {r} to end this effect</examplecapture>
public class PayToEndEffect : Glyph
{
    public override Nib[] Nibs => ["you may pay", Prop(Cost), "to end this effect"];

    public ManaCost Cost { get; set; }
}
