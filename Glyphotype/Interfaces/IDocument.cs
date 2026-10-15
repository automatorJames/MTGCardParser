namespace Glyphotype.Interfaces;

public interface IDocument
{
    public const string ThisToken = "{this}";

    public string Name { get; }
    public string Text { get; }

    /// <summary>The number of the volume this document is in (see <see cref="IDocumentRepository.GetVolumes"/>) - null for a corpus that isn't divided into volumes.</summary>
    public int? Volume => null;

    public string[] GetFormattedLines()
    {
        if (Text is null)
            return [];

        var text = Text.ToLower();

        if (!string.IsNullOrEmpty(Name))
            text = text.Replace(Name.ToLower(), ThisToken);
        var lines = text.Split('\n').Select(x => x.Trim()).ToArray();

        return lines;
    }
}
