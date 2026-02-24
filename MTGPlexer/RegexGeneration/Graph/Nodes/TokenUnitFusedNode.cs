namespace MTGPlexer.RegexGeneration.Graph.Nodes;

using MTGPlexer.RegexGeneration.RegexTemplateLines;
using MTGPlexer.SnippetHelpers;
using System.Reflection;

public class TokenUnitFusedNode : TokenUnitNode
{

    public TokenUnitFusedNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        // open wrapper
        collector.Append(new(this, $"(?<FusedNodeWrapper>", "FusedNodeWrapper"));

        // add inner content
        base.AppendRegexBricks(collector);

        // close wraper
        collector.Append(GroupCloseBrick);
    }

    //IEnumerable<PropertyInfo> GetDeclaredNonBaseProperties()
    //{
    //    var props = Navigation.NodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //
    //    foreach (var prop in props)
    //    {
    //        var getter = prop.GetMethod;
    //
    //        if (getter == null)
    //            continue;
    //
    //        // non-virtual property → genuinely belongs to the type that declares it
    //        if (!getter.IsVirtual && prop.DeclaringType == Navigation.NodeType)
    //        {
    //            yield return prop;
    //            continue;
    //        }
    //
    //        // virtual property
    //        var baseDef = getter.GetBaseDefinition();
    //
    //        // THIS is the important line:
    //        if (baseDef.DeclaringType == Navigation.NodeType)
    //            yield return prop;
    //    }
    //}

    //public override void SetPropertyValue(TokenUnit instance, CaptureContext context)
    //{
    //    if (IsInnerContent)
    //    {
    //        // FLATTENED HYDRATION:
    //        // We bypass the standard hydration that would set instance.FusedContent = new ManaValue().
    //        // Instead, we grab our scoped captures and instruct our child properties (Colorless, White) 
    //        // to hydrate their parsed values directly onto the outer root `instance`.
    //        var scopedContext = context;
    //
    //        foreach (var childGroup in Children.OfType<NamedGroupNode>())
    //        {
    //            childGroup.SetPropertyValue(instance, scopedContext);
    //        }
    //    }
    //    else
    //    {
    //        // Outer Wrapper Mode: standard behavior
    //        base.SetPropertyValue(instance, context);
    //    }
    //}
}