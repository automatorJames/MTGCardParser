namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Flash's reminder text: "you may cast this spell any time you could cast an instant", "you may cast it any time you could cast an instant", "you may cast them any time …".</summary>
/// <exampledoc>Aether Meltdown</exampledoc>
/// <examplecapture>you may cast this spell any time you could cast an instant</examplecapture>
public class CastAtInstantSpeed : Glyph
{
    public override Nib[] Nibs => ["you may cast", Prop(What), "any time you could cast an instant"];

    public OneOf<ThisSpell, It, Them> What { get; set; }
}
