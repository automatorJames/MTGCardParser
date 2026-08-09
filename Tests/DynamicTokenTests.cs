using MTGPlexer.TokenUnits;

namespace Tests;

public class DynamicTokenTests
{
    [Fact]
    public void TargetPlayerAction_HydratesDynamicTokenWithoutDuplicatingCaptureTraceChild()
    {
        var graph = RegexGraph.Create(typeof(TargetPlayerAction));
        var success = graph.TryMatch("target player draws three cards.", out var tokenUnit);

        Assert.True(success);

        var root = tokenUnit.CaptureContext.RootCaptureTrace;
        var actionChildren = root.Children.Where(c => c.FullyQualifiedName.EndsWith("_Action")).ToList();

        Assert.Single(actionChildren);
        Assert.Equal("draws three cards", actionChildren[0].CaptureValue);
    }
}
