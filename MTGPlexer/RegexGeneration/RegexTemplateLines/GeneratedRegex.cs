namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record GeneratedRegex
{
    const int _hashSeparatorPadding = 2;
    const int _boxContentLeftPadding = 1; // Padding for left-aligned text inside a box wall.

    public List<RegexCommentedLine> CommentedLines { get; private set; } = [];
    public string FormattedRegex { get; set; }
    public string MinifiedRegex { get; set; }
    public int HashSeparatorColumn { get; private set; }
    public int CommentColumn { get; private set; }
    public int CommentBoxLength { get; private set; }

    public GeneratedRegex(List<RegexTemplateLine> lines)
    {
        if (lines == null || !lines.Any())
            return;

        CalculateColumnWidths(lines);
        FormatCommentedLines(lines);
        FormattedRegex = string.Join(Environment.NewLine, CommentedLines.Select(x => x.FormattedText));
        var regexWithoutComments = string.Join("", CommentedLines.Select(x => x.Regex));
        MinifiedRegex = MinifyRegex(regexWithoutComments);
    }

    /// <summary>
    /// Builds the final formatted string for each line, managing a stack of active named groups
    /// to correctly render nested Unicode boxes.
    /// </summary>
    void FormatCommentedLines(List<RegexTemplateLine> templateLines)
    {
        var activeNamedGroups = new Stack<NamedGroupOpen>();

        foreach (var line in templateLines)
        {
            // For a closing line, its comment is rendered in the context of its parent,
            // so we pop the group from the stack before generating the comment.
            if (line is GroupClose close && activeNamedGroups.Any() && close.Name == activeNamedGroups.Peek().Name)
            {
                activeNamedGroups.Pop();
            }

            var regex = line.IndentedValue.PadRight(CommentColumn);
            var commentPrefix = $"#{new string(' ', _hashSeparatorPadding)}";
            var commentBody = GetFormattedComment(line, activeNamedGroups);

            CommentedLines.Add(new(regex, commentPrefix + commentBody, line.Palette));

            // For an opening line, its comment is rendered, and THEN it's pushed to the stack
            // to become the parent for subsequent lines.
            if (line is NamedGroupOpen open)
            {
                activeNamedGroups.Push(open);
            }
        }
    }

    /// <summary>
    /// Generates the appropriate comment string for a line, including parent box walls for nesting.
    /// </summary>
    /// <param name="line">The RegexTemplateLine to process.</param>
    /// <param name="parentGroups">The current stack of active parent groups.</param>
    /// <returns>A formatted comment string.</returns>
    private string GetFormattedComment(RegexTemplateLine line, Stack<NamedGroupOpen> parentGroups)
    {
        // Handle lines that are never boxed (like boundaries).
        if (line is NegativeLookaheadBoundary or NegativeLookbehindBoundary)
        {
            return line.CommentOne ?? string.Empty;
        }

        var parentPrefix = new StringBuilder();
        var parentSuffix = new StringBuilder();
        int nestingDepth = parentGroups.Count;

        // 1. Build the parent wall prefixes and suffixes based on the current nesting depth.
        for (int i = 0; i < nestingDepth; i++)
        {
            parentPrefix.Append("│ ");
            parentSuffix.Insert(0, " │");
        }

        // 2. Calculate the width available for the content at the current nesting level.
        int currentContentWidth = CommentBoxLength - (nestingDepth * 4);
        string coreContent;

        // 3. Generate the core content based on the line type.
        switch (line)
        {
            case NamedGroupOpen ngo:
                string left = $" {ngo.CommentOne} ";
                string right = $" {ngo.CommentTwo} ";
                int fillerLenOpen = currentContentWidth - 2 - left.Length - right.Length;
                string fillerOpen = new string('─', Math.Max(0, fillerLenOpen));
                coreContent = $"┌{left}{fillerOpen}{right}┐";
                break;

            case GroupClose gc when !string.IsNullOrEmpty(gc.Name):
                string contentClose = $" {gc.CommentTwo} ";
                int fillerLenClose = currentContentWidth - 2 - contentClose.Length;
                string fillerClose = new string('─', Math.Max(0, fillerLenClose));
                coreContent = $"└{fillerClose}{contentClose}┘";
                break;

            // Center-align AlternateValue comments.
            case AlternateValue av:
                string centeredText = av.CommentOne ?? "";
                int availableWidthCenter = Math.Max(0, currentContentWidth);
                int textWidthCenter = centeredText.Length;
                int totalPaddingCenter = Math.Max(0, availableWidthCenter - textWidthCenter);
                int leftPaddingCenter = totalPaddingCenter / 2;
                int rightPaddingCenter = totalPaddingCenter - leftPaddingCenter;
                coreContent = $"{new string(' ', leftPaddingCenter)}{centeredText}{new string(' ', rightPaddingCenter)}";
                break;

            // Left-align other content lines like pipes and blank lines inside boxes.
            case GroupAlternativePipe or BlankLine:
                string leftAlignedText = line.CommentOne ?? "";
                int availableWidthLeft = Math.Max(0, currentContentWidth);
                if (string.IsNullOrEmpty(leftAlignedText))
                {
                    coreContent = new string(' ', availableWidthLeft);
                }
                else
                {
                    string paddedText = (new string(' ', _boxContentLeftPadding) + leftAlignedText).PadRight(availableWidthLeft);
                    coreContent = paddedText;
                }
                break;

            default:
                var plainComment = line.CommentOne ?? string.Empty;
                var fillerTxt = new string(' ', Math.Max(0, currentContentWidth - plainComment.Length));
                coreContent = parentGroups.Any() ? $"{plainComment}{fillerTxt}" : plainComment;
                break;
        }

        // 4. Combine parent walls with the generated core content.
        return parentPrefix.ToString() + coreContent + parentSuffix.ToString();
    }

    /// <summary>
    /// Calculates the required width for all comment boxes. The width is determined by the
    /// named group that requires the most horizontal space, considering both its title length
    /// and its nesting depth.
    /// </summary>
    void CalculateColumnWidths(List<RegexTemplateLine> lines)
    {
        HashSeparatorColumn = lines.Max(x => x.End) + _hashSeparatorPadding;
        CommentColumn = HashSeparatorColumn + _hashSeparatorPadding;

        if (!lines.OfType<NamedGroupOpen>().Any())
        {
            CommentBoxLength = 0;
            return;
        }

        int maxRequiredWidth = 0;

        foreach (var line in lines.OfType<NamedGroupOpen>())
        {
            // The nesting depth is equivalent to the number of named group ancestors.
            // The Path property (e.g., "Parent_Child_Grandchild") reliably tracks this.
            int nestingDepth = line.Path.Count(c => c == '_');

            // Calculate the base width required for the header content itself.
            int headerContentWidth = line.CommentOneLength + line.CommentTwoLength
                + 2 // for ┌ and ┐
                + 1 // for at least one '─' filler character
                + 4; // for " {comment1} " and " {comment2} " spacing

            // Calculate the total visual width needed at its specific depth.
            // Each level of nesting adds 4 characters for the "│ " prefix and " │" suffix.
            int totalVisualWidth = headerContentWidth + (nestingDepth * 4);

            if (totalVisualWidth > maxRequiredWidth)
            {
                maxRequiredWidth = totalVisualWidth;
            }
        }

        CommentBoxLength = maxRequiredWidth;
    }
}