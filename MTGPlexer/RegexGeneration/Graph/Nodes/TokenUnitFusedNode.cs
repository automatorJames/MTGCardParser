namespace MTGPlexer.RegexGeneration.Graph.Nodes;

using MTGPlexer.SnippetHelpers;
using System.Reflection;

public class TokenUnitFusedNode : TokenUnitNode
{
    // 1. Context check: Are we serving as the inner property or the outer root class?
    private bool IsInnerContent => Navigation.Prop?.Name == "FusedContent";

    // 2. The Inner Content alternates properties via '|'. The Outer Wrapper joins normally.
    protected override Joiner Joiner =>
        IsInnerContent ? Joiner.Pipe : base.Joiner;

    // 3. The Outer Wrapper receives the quantifier (e.g. ')+'). The Inner Content does not.
    protected override GroupQuantifier? Quantifier =>
        !IsInnerContent ? GroupQuantifier.OneOrMore : null;

    public TokenUnitFusedNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        if (!IsInnerContent)
        {
            // OUTER WRAPPER MODE:
            // Behave normally. This will evaluate
            base.AddReflectedChildren(children);
        }
        else
        {
            // INNER CONTENT MODE: Break the loop!
            // Do NOT evaluate T's Snippets. Instead, directly embed the primitive properties declared on T.

            foreach (var prop in GetDeclaredNonBaseProperties())
            {
                var propSnippet = new PropertySnippet(prop.Name, prop, Proptions.None);

                // Recursively generate the child primitive node
                children.Add(GetNodeForPropertySnippetType(this, propSnippet));
            }
        }
    }

    IEnumerable<PropertyInfo> GetDeclaredNonBaseProperties()
    {
        var props = Navigation.NodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            var getter = prop.GetMethod;

            if (getter == null)
                continue;

            // non-virtual property → genuinely belongs to the type that declares it
            if (!getter.IsVirtual && prop.DeclaringType == Navigation.NodeType)
            {
                yield return prop;
                continue;
            }

            // virtual property
            var baseDef = getter.GetBaseDefinition();

            // THIS is the important line:
            if (baseDef.DeclaringType == Navigation.NodeType)
                yield return prop;
        }
    }

    public override void SetPropertyValue(TokenUnit instance, CaptureContext context)
    {
        if (IsInnerContent)
        {
            // FLATTENED HYDRATION:
            // We bypass the standard hydration that would set instance.FusedContent = new ManaValue().
            // Instead, we grab our scoped captures and instruct our child properties (Colorless, White) 
            // to hydrate their parsed values directly onto the outer root `instance`.
            var scopedContext = context;

            foreach (var childGroup in Children.OfType<NamedGroupNode>())
            {
                childGroup.SetPropertyValue(instance, scopedContext);
            }
        }
        else
        {
            // Outer Wrapper Mode: standard behavior
            base.SetPropertyValue(instance, context);
        }
    }
}