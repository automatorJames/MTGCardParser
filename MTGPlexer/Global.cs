// library name idea: ParLex

global using MTGPlexer.Attributes;
global using MTGPlexer.CommonDTOs;
global using MTGPlexer.Data;
global using MTGPlexer.RegexGeneration.RegexSegments;
global using MTGPlexer.RegexGeneration.RegexTemplateLines;
global using MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Boundaries;
global using MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;
global using MTGPlexer.StaticRegistry;
global using MTGPlexer.Colors;
global using MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;
global using MTGPlexer.TokenAnalysisDTOs.TypeExpressions;
global using MTGPlexer.TokenAnalysisDTOs.WordTrees;
global using MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;
global using MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;
global using MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Alternates;
global using MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Helpers;
global using MTGPlexer.SnippetHelpers;
global using MTGPlexer.Tokenizers;
global using MTGPlexer.RegexGeneration.Composers;
global using MTGPlexer.TokenUnitGraphComponents;
global using MTGPlexer.TokenUnits;
global using MTGPlexer.TokenEditor;
global using System.Reflection;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Diagnostics;

global using System.Text.RegularExpressions;
global using static MTGPlexer.Extensions;
global using static MTGPlexer.SnippetHelpers.SnippetShortcuts;