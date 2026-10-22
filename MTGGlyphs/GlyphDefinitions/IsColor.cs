namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object's color being set by a static effect, as one clause in a list of what a permanent gets, has or is: "gets +3/-1 and is black", "gets +1/+1, is black, and has …", "are black and are zombies in addition to their other creature types". Not a predicate on its own, so "if that creature is red" stays a condition (<see cref="PermanentIs"/>); "becomes black" is <see cref="BecomesCharacteristic"/>.</summary>
/// <exampledoc>Grave Servitude</exampledoc>
/// <examplecapture>is black</examplecapture>
[Dependent]
public class IsColor : Glyph
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Colors)];

    public ColorCopula Verb { get; set; }
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
}
