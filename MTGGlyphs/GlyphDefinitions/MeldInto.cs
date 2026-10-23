namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Meld's action: "exile them, then meld them into brisela, voice of nightmares" - the two exiled cards become the named melded permanent. The melded card's name is open text.</summary>
/// <exampledoc>Graf Rats // Chittering Host</exampledoc>
/// <examplecapture>exile them, then meld them into chittering host</examplecapture>
public class MeldInto : Glyph, IPredicate
{
    public override Nib[] Nibs => ["exile them, then meld them into", Pattern(@"[a-z][a-z'\-]*(?:,? [a-z][a-z'\-]*)*")];
}
