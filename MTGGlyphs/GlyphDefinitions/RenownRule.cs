namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Renown's reminder text.</summary>
/// <exampledoc>Acolyte of the Inferno</exampledoc>
/// <examplecapture>when this creature deals combat damage to a player, if it isn't renowned, put a +1/+1 counter on it and it becomes renowned</examplecapture>
public class RenownRule : Glyph
{
    public override Nib[] Nibs => ["when this creature deals combat damage to a player, if it isn't renowned, put", Prop(Counters), "on it and it becomes renowned"];

    public Counters Counters { get; set; }
}
