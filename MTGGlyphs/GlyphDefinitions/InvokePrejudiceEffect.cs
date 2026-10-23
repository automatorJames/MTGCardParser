namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Invoke Prejudice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invoke Prejudice</exampledoc>
/// <examplecapture>counter that spell unless that player pays {x}, where x is its mana value</examplecapture>
[Dependent]
public class InvokePrejudiceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter that spell unless that player pays {x}, where x is its mana value"];
}
