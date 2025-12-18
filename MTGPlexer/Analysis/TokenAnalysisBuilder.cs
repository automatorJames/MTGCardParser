namespace MTGPlexer.Analysis;

public static class TokenAnalysisBuilder
{
    public static TokenAnalysisNode BuildGraph(TokenUnit rootUnit, string fullText, string cardName, int lineIndex)
    {
        // 1. Create the Root Node
        var rootNode = new TokenAnalysisNode
        {
            Name = rootUnit.Type.Name.ToFriendlyCase(TitleDisplayOption.Title),
            NodeType = rootUnit is DefaultUnmatchedString ? AnalysisNodeType.Unmatched : AnalysisNodeType.Root,
            ClrType = rootUnit.Type,
            FriendlyTypeName = "Root",
            Text = rootUnit.Match.RegexMatch.Value,
            Value = rootUnit,
            StartIndex = rootUnit.Match.RegexMatch.Index,
            Length = rootUnit.Match.RegexMatch.Length,
            RegexEnclosurePath = rootUnit.Type.Name,
            Palette = TokenTypeRegistry.Palettes[rootUnit.Type],
            IsTerminal = false,
            IsCollapsed = false
        };

        // 2. Recursively Populate Children
        foreach (var capture in rootUnit.IndexedPropertyCaptures)
        {
            var childNode = CreateNodeFromCapture(capture, rootUnit, fullText);
            if (childNode != null)
            {
                rootNode.Children.Add(childNode);
            }
        }

        // 3. Post-Process: Distilled Values (Virtual Nodes)
        if (rootUnit is TokenUnitDistilled distilled)
        {
            AddDistilledNodes(rootNode, distilled);
        }

        return rootNode;
    }

    private static TokenAnalysisNode CreateNodeFromCapture(IndexedPropertyCapture indexedPropertyCapture, TokenUnit parentUnit, string fullText)
    {
        var val = indexedPropertyCapture.Value;
        if (val == null) return null;

        var node = new TokenAnalysisNode
        {
            Name = indexedPropertyCapture.RegexPropInfo.FriendlyPropName,
            Text = indexedPropertyCapture.Capture.Value,
            Value = val,
            StartIndex = indexedPropertyCapture.Capture.Index,
            Length = indexedPropertyCapture.Capture.Length,
            RegexEnclosurePath = indexedPropertyCapture.CaptureGroupPropPath.PropPath,
            Palette = indexedPropertyCapture.Palette ?? DeterministicPalette.GetFixedRainbowPalette(indexedPropertyCapture.Ordinal),
            ClrType = indexedPropertyCapture.RegexPropInfo.UnderlyingType,
            FriendlyTypeName = indexedPropertyCapture.RegexPropInfo.FriendlyTypeName
        };

        // Determine Type and Recurse
        if (val is TokenUnitOneOf oneOf)
        {
            // OneOfs are transparent structural nodes usually, but we keep them for the tree
            node.NodeType = AnalysisNodeType.Structural;
            node.IsTerminal = false;

            // Recurse: OneOfs have exactly one active child in their IndexedPropertyCaptures
            foreach (var childCap in oneOf.IndexedPropertyCaptures)
            {
                var childNode = CreateNodeFromCapture(childCap, oneOf, fullText);
                if (childNode != null) node.Children.Add(childNode);
            }
        }
        else if (val is TokenUnit childUnit)
        {
            node.NodeType = AnalysisNodeType.Structural;
            node.Palette = TokenTypeRegistry.Palettes.TryGetValue(childUnit.Type, out var p) ? p : node.Palette;
            node.IsTerminal = false;

            // Recurse children
            foreach (var childCap in childUnit.IndexedPropertyCaptures)
            {
                var childNode = CreateNodeFromCapture(childCap, childUnit, fullText);
                if (childNode != null) node.Children.Add(childNode);
            }

            // Handle Distilled values for child token units
            if (childUnit is TokenUnitDistilled distilled)
            {
                AddDistilledNodes(node, distilled);
            }
        }
        else if (val is ManyOf manyOf)
        {
            node.NodeType = AnalysisNodeType.Collection;
            node.IsTerminal = false;
            // ManyOf palette logic
            node.Palette = TokenTypeRegistry.Palettes.TryGetValue(manyOf.ItemType, out var mp) ? mp : node.Palette;

            ProcessManyOfChildren(node, manyOf, indexedPropertyCapture, fullText);
        }
        else if (val is DynamicCapture dyn)
        {
            // Dynamic captures are wrappers. We want to represent the wrapper, then the content.
            node.NodeType = AnalysisNodeType.Structural;
            // Use the dynamic capture's inner value for further processing if it's a TokenUnit
            if (dyn.ValueObject is TokenUnit dynTu)
            {
                foreach (var childCap in dynTu.IndexedPropertyCaptures)
                {
                    // Important: Dynamic captures often have offset issues in the current code base
                    // Ideally, the IndexedPropertyCapture inside the dynamic unit is absolute. 
                    // If not, offsets apply here. Assuming absolute for this "finished" class:
                    var childNode = CreateNodeFromCapture(childCap, dynTu, fullText);
                    if (childNode != null) node.Children.Add(childNode);
                }
            }
            // If it's a dynamic enum
            else
            {
                node.IsTerminal = true;
                node.NodeType = AnalysisNodeType.Terminal;
            }
        }
        else
        {
            // Terminals (Enums, Bools, Strings)
            node.NodeType = AnalysisNodeType.Terminal;
            node.IsTerminal = true;
            node.FriendlyTypeName = val is bool ? "bool" : "enum";
            if (val is PlaceholderCapture) node.FriendlyTypeName = "text";
        }

        return node;
    }

    private static void ProcessManyOfChildren(TokenAnalysisNode parentNode, ManyOf manyOf, IndexedPropertyCapture parentPropertyCapture, string fullText)
    {
        // 1. Items
        for (int i = 0; i < manyOf.ItemObjects.Count; i++)
        {
            var itemObj = manyOf.ItemObjects[i]; // This is a ManyItemCapture

            // We synthesize an analysis node for the item
            var itemNode = new TokenAnalysisNode
            {
                Name = $"{parentNode.Name} #{i + 1}",
                Text = itemObj.Capture.Value,
                Value = itemObj.ItemObject,
                StartIndex = itemObj.Capture.Index,
                Length = itemObj.Capture.Length,
                RegexEnclosurePath = parentNode.RegexEnclosurePath + $".{parentPropertyCapture.RegexPropInfo.Name}{itemObj.Oridinal.ToString()}",
                NodeType = AnalysisNodeType.CollectionItem,
                ClrType = manyOf.ItemType,
                // Assign a distinct rainbow color based on position in list
                Palette = DeterministicPalette.GetFixedRainbowPalette(i),
                IsTerminal = manyOf.ManyItemVariant == ManyItemVariant.Enum
            };

            if (manyOf.ManyItemVariant == ManyItemVariant.TokenUnit && itemObj.ItemObject is TokenUnit childTu)
            {
                // Set the specific palette of the subtype
                itemNode.Palette = TokenTypeRegistry.Palettes[childTu.Type];

                // Recurse properties
                foreach (var subCap in childTu.IndexedPropertyCaptures)
                {
                    var subNode = CreateNodeFromCapture(subCap, childTu, fullText);
                    if (subNode != null) itemNode.Children.Add(subNode);
                }
            }

            parentNode.Children.Add(itemNode);
        }

        // 2. Conjunction (e.g., "and", "or")
        if (manyOf.Conjunction != null)
        {
            var conjNode = new TokenAnalysisNode
            {
                Name = "Conjunction",
                Text = manyOf.ConjunctionCapture.Value,
                Value = manyOf.Conjunction.Value,
                StartIndex = manyOf.ConjunctionCapture.Index,
                Length = manyOf.ConjunctionCapture.Length,
                RegexEnclosurePath = parentNode.RegexEnclosurePath + ".Conjunction",
                NodeType = AnalysisNodeType.Terminal,
                IsTerminal = true,
                Palette = DeterministicPalette.GetFixedRainbowPalette(manyOf.ItemObjects.Count), // Color it as the "next" item
                ClrType = typeof(Conjunction),
                FriendlyTypeName = "conjunction"
            };
            parentNode.Children.Add(conjNode);
        }
    }

    private static void AddDistilledNodes(TokenAnalysisNode parentNode, TokenUnitDistilled distilled)
    {
        // Distilled values usually map to a PlaceholderCapture. 
        // We find the child node corresponding to the placeholder and add distilled values as its children.

        foreach (var (placeholderCap, distilledMap) in distilled.DistilledVals)
        {
            // Find the analysis node that represents this placeholder capture
            var placeholderNode = parentNode.Children
                .FirstOrDefault(c => c.StartIndex == placeholderCap.Start && c.Length == placeholderCap.Length);

            if (placeholderNode == null) continue;

            foreach (var (propInfo, val) in distilledMap)
            {
                var distilledNode = new TokenAnalysisNode
                {
                    Name = propInfo.FriendlyPropName,
                    Text = val.ToString(), // Virtual text
                    Value = val,
                    StartIndex = placeholderNode.StartIndex, // Inherits location
                    Length = placeholderNode.Length,
                    // Distilled values don't have a specific Regex enclosure, they live inside the placeholder's enclosure
                    RegexEnclosurePath = placeholderNode.RegexEnclosurePath,
                    NodeType = AnalysisNodeType.Derived,
                    IsTerminal = true,
                    ClrType = propInfo.UnderlyingType,
                    FriendlyTypeName = "derived " + (val is int ? "int" : val.GetType().Name),
                    Palette = placeholderNode.Palette // Inherit palette
                };

                placeholderNode.Children.Add(distilledNode);
            }
        }
    }
}