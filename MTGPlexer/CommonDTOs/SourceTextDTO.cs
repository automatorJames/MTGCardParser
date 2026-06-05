namespace MTGPlexer.CommonDTOs;

public record SourceTextDTO
{
    public string FormattedText { get; init; }
    public string CorpusMemberName { get; init; }
    public int LineIndex { get; init; }

    public SourceTextDTO(
        string formattedText,
        string corpusMemberName,
        int lineIndex)
    {
        FormattedText = formattedText;
        CorpusMemberName = corpusMemberName;
        LineIndex = lineIndex;
    }
}