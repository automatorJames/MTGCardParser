namespace MTGPlexer.CommonDTOs;

public record SourceTextDTO
{
    public string FormattedText { get; init; }
    public string OriginalText { get; init; }
    public string CorpusMemberName { get; init; }
    public int LineIndex { get; init; }

    public SourceTextDTO(
        string formattedText,
        string originalText,
        string corpusMemberName,
        int lineIndex)
    {
        FormattedText = formattedText;
        OriginalText = originalText;
        CorpusMemberName = corpusMemberName;
        LineIndex = lineIndex;
    }
}