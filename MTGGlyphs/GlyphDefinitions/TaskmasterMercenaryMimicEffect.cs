namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Taskmaster, Mercenary Mimic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Taskmaster, Mercenary Mimic</exampledoc>
/// <examplecapture>until your next turn, taskmaster becomes a copy of up to one target creature on the battlefield or creature card in a graveyard, except his name is {this} and he's a legendary human mercenary villain creature</examplecapture>
[Dependent]
public class TaskmasterMercenaryMimicEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, taskmaster becomes a copy of up to one target creature on the battlefield or creature card in a graveyard, except his name is", Nib.This, "and he's a legendary human mercenary villain creature"];
}
