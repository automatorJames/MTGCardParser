using Glyphotype.GlyphAnalysisDTOs.WordTrees;

namespace DocumentAnalysisInterface.Components.DocumentLines;

/// <summary>
/// One active echo lane at a specific word (or inter-word gap) position — which packed lane it's
/// in (its vertical offset), which corpus span it belongs to (for the click target), and whether
/// this is the one position along that span where its count badge should render.
/// </summary>
public record EchoLaneEntry(int Lane, EchoMatch Echo, bool IsBadgeAnchor);
