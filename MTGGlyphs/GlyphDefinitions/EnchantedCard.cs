namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What an aura gives the permanent it enchants: "enchanted creature has flying".</summary>
/// <exampledoc>Flight</exampledoc>
/// <examplecapture>enchanted creature has flying</examplecapture>
public class EnchantedCard : Glyph
{
    public override Nib[] Nibs => ["enchanted", Prop(CardOrCreatureType), Prop(PermanentVerb), Prop(Buff)];

    public OneOf<CardType?, CreatureType?> CardOrCreatureType{ get; set; }
    public PermanentVerb? PermanentVerb { get; set; }   
    public Buff Buff { get; set; }
}