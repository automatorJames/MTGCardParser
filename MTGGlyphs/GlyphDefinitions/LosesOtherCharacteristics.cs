namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent losing every other characteristic of some kinds, usually restated as reminder text after a type-setting effect: "loses all other card types and creature types", "(it also loses all other colors, card types, and creature types", "has defender and loses all other abilities".</summary>
/// <exampledoc>Frogify</exampledoc>
/// <examplecapture>loses all other card types and creature types</examplecapture>
[Dependent]
public class LosesOtherCharacteristics : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("loses?"), "all other", Prop(Kinds)];

    public OneOf<ManyOf<CharacteristicKind>, CharacteristicKind?> Kinds { get; set; }
}
