namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kytheon, Hero of Akros // Gideon, Battle-Forged, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kytheon, Hero of Akros // Gideon, Battle-Forged</exampledoc>
/// <examplecapture>if kytheon and at least two other creatures attacked this combat, exile kytheon, then return him to the battlefield transformed under his owner's control</examplecapture>
[Dependent]
public class KytheonHeroOfAkrosEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if kytheon and at least two other creatures attacked this combat, exile kytheon, then return him to the battlefield transformed under his owner's control"];
}
