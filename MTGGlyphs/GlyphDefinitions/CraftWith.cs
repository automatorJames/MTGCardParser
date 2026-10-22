namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Craft, with what must be exiled and its mana cost: "craft with artifact {2}{u}", "craft with two creatures {5}{b}", "craft with a dinosaur, a merfolk, a pirate, and a vampire {4}", "craft with one or more dinosaurs {4}{r}".</summary>
/// <exampledoc>Tithing Blade // Consuming Sepulcher</exampledoc>
/// <examplecapture>craft with creature {4}{b}</examplecapture>
[Dependent]
public class CraftWith : Glyph
{
    public override Nib[] Nibs => ["craft with", Prop(Materials), Prop(Cost)];

    public OneOf<ManyOf<IndefinitePermanent>, CountedPermanents, PermanentKind> Materials { get; set; }
    public ManaCost Cost { get; set; }
}
