namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you or a creature you control", "an opponent or a permanent an opponent controls": a player, or one permanent - as replacement and prevention effects name what damage is dealt to.</summary>
/// <exampledoc>Hedron-Field Purists</exampledoc>
/// <examplecapture>you or a creature you control</examplecapture>
[Dependent]
public class PlayerOrPermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "or", Prop(Permanent)];

    public WhichPlayer Player { get; set; }
    public IndefinitePermanent Permanent { get; set; }
}
