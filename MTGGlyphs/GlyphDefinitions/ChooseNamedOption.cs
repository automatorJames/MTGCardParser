namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A choice between named options: "choose khans or dragons" (Siege cycle), "choose black or red", "choose artifact or enchantment". The options are game terms (factions, colors, land, card or creature types), captured as such.</summary>
/// <exampledoc>Citadel Siege</exampledoc>
/// <examplecapture>choose khans or dragons</examplecapture>
public class ChooseNamedOption : Glyph, IPredicate
{
    public override Nib[] Nibs => ["choose", Prop(Options)];

    public ManyOf<ChoiceOption> Options { get; set; }
}
