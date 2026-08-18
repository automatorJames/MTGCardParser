using MTGGlyphs;

namespace Tests;

//public class DynamicGlyphTests
//{
//    [Fact]
//    public void TargetPlayerAction_HydratesDynamicGlyphWithoutDuplicatingCaptureTraceChild()
//    {
//        var graph = RegexGraph.Create(typeof(TargetPlayerAction));
//        var success = graph.TryMatch("target player draws three cards.", out var glyph);
//
//        Assert.True(success);
//
//        var root = glyph.CaptureContext.RootCaptureTrace;
//        var actionChildren = root.Children.Where(c => c.FullyQualifiedName.EndsWith("_Action")).ToList();
//
//        Assert.Single(actionChildren);
//        Assert.Equal("draws three cards", actionChildren[0].CaptureValue);
//    }
//}
