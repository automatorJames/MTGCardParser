namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An attachment condition: an Aura or Equipment being (or having been) attached to an object - "enchanted equipment is attached to a creature", "{this} is attached to a creature", "that equipment was attached to a creature", "an equipment named groom's finery is attached to a creature you control".</summary>
/// <exampledoc>Artificer's Hex</exampledoc>
/// <examplecapture>enchanted equipment is attached to a creature</examplecapture>
[Dependent]
public class PermanentIsAttached : Glyph
{
    public override Nib[] Nibs => [Prop(Attachment), Alt("is", "was"), "attached to", Prop(Host)];

    public OneOf<PermanentPhrase, IndefinitePermanent> Attachment { get; set; }
    public IndefinitePermanent Host { get; set; }
}
