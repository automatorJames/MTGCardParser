namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"any time they could cast a sorcery", "any time you could cast a sorcery": the timing of a sorcery - during its caster's main phase, with an empty stack.</summary>
/// <exampledoc>Teferi, Mage of Zhalfir</exampledoc>
/// <examplecapture>any time they could cast a sorcery</examplecapture>
[Dependent]
public class SorcerySpeed : Glyph
{
    public override Nib[] Nibs => ["any time", Prop(Player), "could cast a sorcery"];

    public OneOf<WhichPlayer?, They> Player { get; set; }
}
