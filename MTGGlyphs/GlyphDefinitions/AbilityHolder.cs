namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of urza", "of planeswalkers you control", "of jace planeswalkers you control": whose abilities a player may activate.</summary>
/// <exampledoc>Oath of Teferi</exampledoc>
/// <examplecapture>of planeswalkers you control</examplecapture>
[Dependent]
public class AbilityHolder : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Holder)];

    public OneOf<Permanents, This, ShortName> Holder { get; set; }
}
