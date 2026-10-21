namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Equip/fortify reminder tail: "this card enters unattached and stays on the battlefield if the creature leaves" (or "if the land leaves").</summary>
/// <exampledoc>Slagwurm Armor</exampledoc>
/// <examplecapture>this card enters unattached and stays on the battlefield if the creature leaves</examplecapture>
public class EntersUnattached : Glyph
{
    public override Nib[] Nibs => ["this card enters unattached and stays on the battlefield if the", Prop(Attachee), "leaves"];

    public CardType Attachee { get; set; }
}
