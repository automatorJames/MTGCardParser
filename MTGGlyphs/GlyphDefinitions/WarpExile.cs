namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Warp's reminder text, second sentence: the warped permanent ("this creature", "this enchantment", "that permanent") is exiled at the next end step and may be cast again later.</summary>
/// <exampledoc>Starfield Shepherd</exampledoc>
/// <examplecapture>exile this creature at the beginning of the next end step, then you may cast it from exile on a later turn</examplecapture>
public class WarpExile : Glyph
{
    public override Nib[] Nibs => ["exile", Prop(Warped), "at the beginning of the next end step, then you may cast it from exile on a later turn"];

    public PermanentPhrase Warped { get; set; }
}
