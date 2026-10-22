namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether the dragon-reveal additional cost was paid: "you revealed a dragon card or controlled a dragon as you cast this spell".</summary>
/// <exampledoc>Draconic Roar</exampledoc>
/// <examplecapture>you revealed a dragon card or controlled a dragon as you cast this spell</examplecapture>
[Dependent]
public class RevealedOrControlledAsCast : Glyph
{
    public override Nib[] Nibs => ["you revealed a", Prop(Revealed), "card or controlled a", Prop(Controlled), "as you cast this spell"];

    public PermanentKind Revealed { get; set; }
    public PermanentKind Controlled { get; set; }
}
