namespace MTGPlexer.RegexGeneration.Composers;

public class AlternatingComposer : ISegmentComposer
{
    public static readonly AlternatingComposer Instance = new();
    private AlternatingComposer() { }

    public void Compose(RegexBuilder collector, List<RegexSegmentBase> segments)
    {
        // If there are no text segments, the named group parentheses are a sufficient wrapper to isolate
        // the alterantive properties. If not, we must render the alternate properties within supplemental
        // parentheses to isolate them from the text segments on either side.
        bool shouldWrapAlternatives = segments.Any(x => x is TextSegment);

        // Tracks the number of alternatives that have been rendered to open/close groups and render "|" pipes
        int renderedAlternatives = 0;

        foreach (var segment in segments)
        {
            if (segment is TextSegment)
            {
                if (renderedAlternatives > 0)
                    // Close the alternations group before the trailing text segments
                    collector.CloseGroup();

                segment.ComposeRegexLines(collector);

            }
            else if (segment is CaptureGroupSegmentBase)
            {
                if (renderedAlternatives == 0 && shouldWrapAlternatives)
                    collector.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);

                if (renderedAlternatives > 0)
                    collector.AddGroupAlternativePipe();

                segment.ComposeRegexLines(collector);
                renderedAlternatives++;
            }
        }

        if (shouldWrapAlternatives && renderedAlternatives > 0)
            // Close the alternations group because we're done
            collector.CloseGroup();
    }

    public void Compose(RegexBuilder collector, List<Node> nodes)
    {
        // If there are no text segments, the named group parentheses are a sufficient wrapper to isolate
        // the alterantive properties. If not, we must render the alternate properties within supplemental
        // parentheses to isolate them from the text segments on either side.
        bool shouldWrapAlternatives = nodes.Any(x => x is TextNode);

        // Tracks the number of alternatives that have been rendered to open/close groups and render "|" pipes
        int renderedAlternatives = 0;

        foreach (var node in nodes)
        {
            if (node is TextNode)
            {
                if (renderedAlternatives > 0)
                    // Close the alternations group before the trailing text segments
                    collector.CloseGroup();

                node.ComposeRegexLines(collector);

            }
            else if (node is CaptureNode)
            {
                if (renderedAlternatives == 0 && shouldWrapAlternatives)
                    collector.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);

                if (renderedAlternatives > 0)
                    collector.AddGroupAlternativePipe();

                node.ComposeRegexLines(collector);
                renderedAlternatives++;
            }
        }

        if (shouldWrapAlternatives && renderedAlternatives > 0)
            // Close the alternations group because we're done
            collector.CloseGroup();
    }

}