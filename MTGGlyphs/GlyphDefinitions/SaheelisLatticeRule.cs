namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Saheeli's Lattice // Mastercraft Raptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Saheeli's Lattice // Mastercraft Raptor</exampledoc>
/// <examplecapture>craft with one or more dinosaurs {4}{r} ({4}{r}, exile this artifact, exile one or more dinosaurs you control and/or dinosaur cards from your graveyard: return this card transformed under its owner's control</examplecapture>
[TokenizationOrder(-1)]
public class SaheelisLatticeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["craft with one or more dinosaurs {4}{r} ({4}{r}, exile this artifact, exile one or more dinosaurs you control and/or dinosaur cards from your graveyard: return this card transformed under its owner's control"];
}
