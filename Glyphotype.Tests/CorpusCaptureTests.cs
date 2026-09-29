namespace Glyphotype.Tests;

/// <summary>Each <see cref="TestCorpus"/> document must tokenize into exactly its expected captures.</summary>
[Collection(CorpusCollection.Name)]
public class CorpusCaptureTests(CorpusFixture corpus)
{
    public static TheoryData<string, string> Documents()
    {
        var data = new TheoryData<string, string>();

        foreach (var document in TestCorpus.Documents)
            data.Add(document.Feature, document.Text);

        return data;
    }

    [Theory]
    [MemberData(nameof(Documents))]
    public void Document_produces_its_expected_captures(string feature, string text)
    {
        _ = feature; // only there to group cases by feature in the test runner

        var expected = TestCorpus.Documents.Single(x => x.Text == text).ExpectedLines;

        // Compared as one newline-joined string so a failure shows the exact point of divergence.
        Assert.Equal(string.Join("\n", expected), string.Join("\n", corpus.ActualLines(text)));
    }
}
