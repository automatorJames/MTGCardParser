namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a merfolk you control or a merfolk card from your hand": one object that may be either a permanent on the battlefield or a card in another zone (behold-and-exile costs).</summary>
/// <exampledoc>Champions of the Shoal</exampledoc>
/// <examplecapture>a merfolk you control or a merfolk card from your hand</examplecapture>
[Dependent]
public class PermanentOrCard : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), "or", Prop(Card)];

    public IndefinitePermanent Permanent { get; set; }
    public IndefiniteCardInZone Card { get; set; }
}
