namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's still a land", "they're still lands", "it's still a shapeshifter", "he's still a planeswalker": a permanent that becomes something else keeps a type it had.</summary>
/// <exampledoc>Mishra's Factory</exampledoc>
/// <examplecapture>it's still a land</examplecapture>
public class StillALand : Glyph
{
    public override Nib[] Nibs => [Alt("it's still a", "it's still an", "they're still", "he's still a", "she's still a"), Plural(Prop(Kind))];

    public OneOf<CardType?, CreatureType?> Kind { get; set; }
}
