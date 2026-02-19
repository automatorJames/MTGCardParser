namespace Tests;

public class RegexParsingTests
{
    [Fact]
    public void ManyOf()
    {
        var text = "This melody contains the notes C, E, G, and A";
        var template = RegexGraph.Create(typeof(ManyOfMusicNotes));
    }
}
