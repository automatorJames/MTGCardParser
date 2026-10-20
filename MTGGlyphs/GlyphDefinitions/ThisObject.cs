namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"this creature", "this spell", "this card": the object with the ability, named by its kind - including "this spell", which ThisCreature leaves out.</summary>
/// <exampledoc>Into the Roil</exampledoc>
/// <examplecapture>this spell</examplecapture>
[Dependent]
public class ThisObject : Glyph
{
    public override Nib[] Nibs => [Prop(Object)];

    public OneOf<ThisSpell, ThisCreature> Object { get; set; }
}
