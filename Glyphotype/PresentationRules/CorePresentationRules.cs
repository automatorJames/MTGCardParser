// The single home for every presentation knob that Glyphotype's OWN renderers read while building
// their output (formatted regex, the C# class view, rainbow palettes). Consolidated here from
// RegexGeneration/Presentation/RegexPresentationRules.cs, RegexGeneration/Presentation/GlyphClassRenderRules.cs,
// Colors/GlyphKeyPaletteKnobs.cs, and the private constants that used to sit inside
// Colors/DeterministicPalette.cs and Colors/ColorWheelOptions.cs.
//
// This is one of exactly two knob files in the solution. The other is
// DocumentAnalysisInterface/PresentationRules/InterfacePresentationRules.cs, which holds every knob
// only the Blazor UI reads. The split is not stylistic — it's the project reference direction:
// DocumentAnalysisInterface references Glyphotype, never the reverse, so anything Glyphotype's own
// render pipeline consumes (SmartLineRenderer, GlyphClassRenderer, EnumSectionBuilder,
// CommentBoxMetrics, DeterministicPalette, ColorKnobs, AnalyzedText) has to live on this side of the
// line. Knobs that only ever reach a .razor file or a CSS custom property belong in the interface
// file instead.
//
// Nothing outside these two files should hardcode a layout, color, font, or spacing constant.

namespace Glyphotype.PresentationRules;

/// <summary>
/// Layout knobs for the formatted/commented regex output: spacing, buffer widths, the font family, and
/// the box-drawing character sets used to render group borders.
/// </summary>
public static class SmartRegexStaticRules
{
    /// <summary>Spaces of indentation per level of group nesting in the regex column.</summary>
    public static int IndentSpaces = 4;

    /// <summary>
    /// Spaces of indentation for an enum member row (and its synonym header/footer/omitted-count siblings)
    /// from its enclosing enum group's own bookends — narrower than <see cref="IndentSpaces"/> since these
    /// rows don't themselves introduce another level of group nesting.
    /// </summary>
    public static int EnumMemberIndentSpaces = 2;

    /// <summary>Extra columns of padding before the '#' comment separator.</summary>
    public static int CommentBorderLineBuffer = 2;

    /// <summary>Spaces between a group box's wall character and its contents.</summary>
    public static int GroupWallInnerBuffer = 1;

    /// <summary>Spaces between a group bookend's box corner and its comment text.</summary>
    public static int GroupBookendCommentBuffer = 1;

    /// <summary>Spaces on either side of the colon separating an enum member's name from its occurrence count.</summary>
    public static int EnumMemberOccurrenceCountColonBuffer = 1;

    /// <summary>Spaces between an enum member row's leading pipe/space and its regex pattern.</summary>
    public static int EnumMemberBufferAfterPipe = 1;

    /// <summary>The literal "  #  "-style prefix that separates a line's regex column from its comment column.</summary>
    public static string CommentBorderLineWithBuffer =
        $"{string.Empty.PadLeft(CommentBorderLineBuffer)}#{string.Empty.PadLeft(CommentBorderLineBuffer)}";

    /// <summary>
    /// The font family for all formatted regex output, applied to the whole block. There is deliberately
    /// no per-role alternate font: <c>CommentBoxMetrics</c> and every padding/column calculation
    /// around it measure text by character count, which only lines up on screen if every span shares one
    /// truly monospace font.
    /// </summary>
    public static string PrimaryFontFamily = "'Fira Code', monospace";

    /// <summary>
    /// Centers text within the given width. Any odd leftover space goes to the right,
    /// so repeated centering (e.g. centering an already-centered string within a wider
    /// outer width) stays visually balanced.
    /// </summary>
    public static string CenterPad(string text, int width)
    {
        var (leftPad, rightPad) = CenterPadSplit(text, width);
        return $"{leftPad}{text}{rightPad}";
    }

    /// <summary>
    /// Same padding math as <see cref="CenterPad"/>, but returns the two pad strings separately instead of
    /// a merged result — for callers that need to attach the padding to sub-pieces of <paramref name="text"/>
    /// (e.g. coloring a centered comment's name/count fields differently) rather than to the whole string.
    /// </summary>
    public static (string LeftPad, string RightPad) CenterPadSplit(string text, int width)
    {
        var extra = Math.Max(0, width - text.Length);
        var leftPad = extra / 2;
        var rightPad = extra - leftPad;
        return (string.Empty.PadLeft(leftPad), string.Empty.PadLeft(rightPad));
    }

    // Box-drawing characters
    const char BoxTopLeft = '┌';        // top-left corner
    const char BoxTopRight = '┐';       // top-right corner
    const char BoxBottomLeft = '└';     // bottom-left corner
    const char BoxBottomRight = '┘';    // bottom-right corner
    const char BoxHorizontal = '─';     // horizontal
    const char BoxVertical = '│';       // vertical
    const char BoxVerticalDashed = '┊'; // dashed vertical

    // Ordinary / distinguishing characters
    const char Asterisk = '*';          // asterisk
    const char Plus = '+';              // plus
    const char Hyphen = '-';            // hyphen
    const char Hash = '#';              // hash
    const char Colon = ':';             // colon
    const char Period = '.';            // period
    const char MiddleDot = '·';         // middle dot
    const char Bullet = '•';            // bullet
    const char WhiteCircle = '○';       // white circle
    const char BlackCircle = '●';       // black circle
    const char WhiteDiamond = '◇';      // white diamond
    const char BlackDiamond = '◆';      // black diamond
    const char WhiteSquare = '□';       // white square
    const char BlackSquare = '■';       // black square
    const char RightArrow = '→';        // right arrow
    const char LeftArrow = '←';         // left arrow

    static readonly BoxCharSet Closed = new(
        TopLeft: BoxTopLeft,
        TopRight: BoxTopRight,
        BottomLeft: BoxBottomLeft,
        BottomRight: BoxBottomRight,
        Horizontal: BoxHorizontal,
        Vertical: BoxVertical
    );

    static readonly BoxCharSet Dashed = Closed with { Vertical = BoxVerticalDashed };

    static readonly BoxCharSet AllBullets = new(
        TopLeft: Bullet,
        TopRight: Bullet,
        BottomLeft: Bullet,
        BottomRight: Bullet,
        Horizontal: Bullet,
        Vertical: Bullet
    );

    /// <summary>Which <see cref="BoxCharSet"/> to draw around a named group's border, based on the kind of node it represents.</summary>
    public static BoxCharSet NodeTypeToBoxCharSet(NamedGroupNode node) =>
        node switch
        {
            EnumNode => Closed,
            DynamicGlyphNode => AllBullets,
            _ => Dashed,
        };

    /// <summary>Looks up the box-drawing character set for the group a bookend brick opens or closes.</summary>
    public static BoxCharSet GetBoxCharsForBookendBrick(RegexBrickGroupBookend bookendBrick) =>
        NodeTypeToBoxCharSet(bookendBrick.NamedGroupParent);

    /// <summary>
    /// The regex-column indent for <paramref name="brick"/>: <see cref="EnumMemberIndentSpaces"/> for its
    /// last level of depth when it's a row directly inside an enum group (a member, synonym header/footer,
    /// or omitted-count row — not the group's own bookends, which stay at the ordinary per-level indent),
    /// <see cref="IndentSpaces"/> per level otherwise.
    /// </summary>
    public static int GetIndentSpaces(RegexBrick brick) =>
        brick is not RegexBrickGroupBookend && brick.NamedGroupParent is EnumNode
            ? ((brick.NestedDepth - 1) * IndentSpaces) + EnumMemberIndentSpaces
            : brick.NestedDepth * IndentSpaces;
}

public record BoxCharSet(char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Horizontal, char Vertical);

/// <summary>
/// Centralized fallback values for any <see cref="Colors.ColorKnobs"/> field left unspecified (not just
/// <see cref="SmartSpanControlPanel"/>'s per-role table), so "default" has exactly one definition instead
/// of being re-guessed at every call site.
/// </summary>
public static class ColorKnobDefaults
{
    /// <summary>Default saturation (0..1) for a span's resting, non-highlighted, non-lowlighted color.</summary>
    public const double Saturation = 0.5;

    /// <summary>Default brightness (0..1) for a span's resting color.</summary>
    public const double Brightness = 0.5;

    /// <summary>Default fraction (0..1) of the way from resting saturation to fully-saturated (Hi) / fully-desaturated (Lo).</summary>
    public const double SaturationRange = 0.6;

    /// <summary>Default fraction (0..1) of the way from resting brightness to maximum (Hi) / minimum (Lo) brightness.</summary>
    public const double BrightnessRange = 0.45;
}

/// <summary>
/// The centralized "control panel" of color knobs for every <see cref="RegexSpanKind"/>. This is the
/// single place to retune how TypeRegexPage colors any of these span roles; nothing outside this class
/// should hardcode one of their colors. Sits alongside <see cref="SmartRegexStaticRules"/> as the other
/// "knobs" class for formatted regex output — that one governs layout (and font), this one governs color.
/// </summary>
/// <remarks>
/// A role's hue is always whatever the caller passes into <see cref="Resolve"/> as
/// <c>positionalHueDegrees</c> — in practice, the same rainbow hue
/// <see cref="DeterministicPalette.GetPositionalPaletteSet{T}"/> already assigned to the span's enclosing
/// named group. That's deliberate for every role, including <see cref="RegexSpanKind.CommentGroupBorderWall"/>:
/// nested boxes read best when each box's wall is its own container-meaningful color, so a border's hue
/// should never be pinned regardless of which group it belongs to; only its saturation/brightness are
/// hand-tuned below, same as every other role, with no further distinction by the enclosing group's own
/// node type (box shape — solid vs. dashed — already carries that distinction; see
/// <see cref="SmartRegexStaticRules.NodeTypeToBoxCharSet"/>).
/// </remarks>
public static class SmartSpanControlPanel
{
    /// <summary>Per-role knobs. <see cref="ColorKnobs.HueDegrees"/> is ignored here — it's always overwritten with the span's positional rainbow hue in <see cref="Resolve"/> — so only saturation/brightness (and their hi/lo ranges) are hand-tuned per role.</summary>
    static readonly Dictionary<RegexSpanKind, ColorKnobs> _roleSpecs = new()
    {
        // --- Regex column ---

        // |tap
        [RegexSpanKind.RegexEnumMember] = new(Saturation: .35, Brightness: .8),

        // |tap
        // ^
        [RegexSpanKind.RegexEnumMemberJoiner] = new(Saturation: 0, Brightness: 0.25, SaturationRange: 0, BrightnessRange: 0),

        // until[ ]end
        //      ^^^
        [RegexSpanKind.RegexConnectiveSpace] = new(Saturation: 0, Brightness: 0.25, SaturationRange: 0),

        // until end of turn
        [RegexSpanKind.RegexLiteralMatch] = new(Saturation: 0.2, Brightness: 0.45, SaturationRange: 0.2, BrightnessRange: 0.2),

        // [ ]
        [RegexSpanKind.RegexJoiner] = new(Saturation: 0.15, Brightness: 0.3, SaturationRange: 0.2, BrightnessRange: 0.2),

        // --- Regex/comment separator ---

        // (?<CardKeyword>tap)  #  Card Keyword
        //                    ^^^^^
        [RegexSpanKind.RegexCommentSeparator] = new(Saturation: 0, Brightness: 0.25, SaturationRange: 0.2, BrightnessRange: 0.2),

        // --- Comment column ---

        // Tap : 12
        // ^^^
        [RegexSpanKind.CommentEnumMemberName] = new(Saturation: .35, Brightness: .8),

        // Tap : 12
        //       ^^
        [RegexSpanKind.CommentEnumMemberOccurrenceCount] = new(Saturation: .3, Brightness: .35, IsBold: true),

        // ───────Power and Toughness─
        //        ^^^^^^^^^^^^^^^^^^^
        [RegexSpanKind.CommentEnumMemberSynonymFooter] = new(Saturation: 0.3, Brightness: 0.4, SaturationRange: 0.1, BrightnessRange: 0.1, IsItalic: true),

        // 3 omitted
        [RegexSpanKind.CommentOmittedCount] = new(Saturation: 0, Brightness: 0.4, SaturationRange: 0),

        // literal match
        [RegexSpanKind.CommentLiteralMatch] = new(Saturation: 0.2, Brightness: 0.45, SaturationRange: 0.2, BrightnessRange: 0.2),

        // joiner Space
        [RegexSpanKind.CommentJoiner] = new(Saturation: 0.15, Brightness: 0.35, SaturationRange: 0.2, BrightnessRange: 0.2),

        // Token Unit: Card Keyword
        // ^^^^^^^^^^
        [RegexSpanKind.CommentGroupOpenHeaderText] = new(Brightness: .55, Saturation: .4),

        // Token Unit: Card Keyword
        //            ^^^^^^^^^^^^^
        [RegexSpanKind.CommentGroupOpenHeaderDisambiguator] = new(Saturation: 0.35, Brightness: 0.4, SaturationRange: 0.3, BrightnessRange: 0.25),

        // Card Keyword (any number)
        // ^^^^^^^^^^^^
        [RegexSpanKind.CommentGroupFooterText] = new(IsItalic: true, Brightness: .55, Saturation: .4),

        // Card Keyword (any number)
        //              ^^^^^^^^^^^^
        [RegexSpanKind.CommentGroupFooterQuantifierReminder] = new(Saturation: 0.35, Brightness: 0.4, SaturationRange: 0.3, BrightnessRange: 0.25),

        // ┌── Card Keyword ──┐
        // ^                  ^
        [RegexSpanKind.CommentGroupBorderWall] = new(Brightness: .4, Saturation: .4),
    };

    /// <summary>
    /// The color treatment for <paramref name="kind"/>. <paramref name="positionalHueDegrees"/> is always
    /// used as this span's hue — the resolved spec's own <see cref="ColorKnobs.HueDegrees"/> is a placeholder
    /// and gets overwritten. When <paramref name="forceGrayscale"/> is set (the enclosing group itself has no
    /// hue — e.g. the transparent root), saturation is pinned to zero so role-tagged spans render neutral
    /// instead of reinterpreting the group's undefined hue as red.
    /// </summary>
    public static SpanStylePalette Resolve(RegexSpanKind kind, double positionalHueDegrees, bool forceGrayscale = false)
    {
        var spec = _roleSpecs[kind];

        var knobs = spec with
        {
            HueDegrees = positionalHueDegrees,
            Saturation = forceGrayscale ? 0 : spec.Saturation,
            SaturationRange = forceGrayscale ? 0 : spec.SaturationRange,
        };

        return SpanStylePalette.FromKnobs(knobs);
    }
}

/// <summary>
/// Tunable knobs for <see cref="RegexGeneration.Presentation.GlyphClassRenderer"/>'s output - the "C# Class"
/// footer tray view's own small knobs class, alongside <see cref="SmartRegexStaticRules"/> (formatted regex
/// layout) and <see cref="SmartSpanControlPanel"/> (formatted regex color).
/// </summary>
public static class GlyphClassRenderRules
{
    /// <summary>Spaces of indentation for a class body's members (the Nibs line, attribute lines, property lines) from the class's own braces.</summary>
    public const int BodyIndentSpaces = 4;

    /// <summary>
    /// Resting brightness (0..1) for neutral C# keyword text - <c>public</c>, <c>class</c>, <c>override</c>,
    /// <c>Prop(</c>, <c>get</c>, <c>set</c> - the lightest of the three neutral shades, since keywords read
    /// as the most "sentence-like" of the uninteresting text.
    /// </summary>
    public const double NeutralKeywordBrightness = 0.62;

    /// <summary>Resting brightness (0..1) for neutral brace/bracket/generic-angle-bracket text (<c>{ } [ ] &lt; &gt; ( )</c>), including the quotes around a Nibs-array literal.</summary>
    public const double NeutralBraceBrightness = 0.45;

    /// <summary>Resting brightness (0..1) for neutral fine punctuation (<c>; :</c>) - the darkest of the three neutral shades, since it's the least meaningful.</summary>
    public const double NeutralPunctuationBrightness = 0.32;
}

/// <summary>
/// The base HSL values every <see cref="Colors.HexPalette"/> that <see cref="DeterministicPalette"/> hands
/// out is built from, plus the defaults of the <see cref="Colors.ColorWheelOptions"/> that decide where on
/// the wheel a rainbow starts and how far it travels. Previously private constants inside
/// <see cref="DeterministicPalette"/> and inline defaults on <see cref="Colors.ColorWheelOptions"/>; hoisted
/// here so the "what does a rainbow actually look like" numbers sit next to
/// <see cref="GlyphKeyPaletteKnobs"/>, which is defined purely as multipliers against them.
/// </summary>
public static class RainbowPaletteKnobs
{
    /// <summary>Resting saturation (0..1) of a rainbow hue's <see cref="Colors.HexPalette.Normal"/> variant.</summary>
    public const double BaseSaturation = 0.66;

    /// <summary>Full saturation — both the ceiling any saturation factor clamps to, and the fixed saturation of the <see cref="Colors.HexPalette.Sat"/> hover-pop variant.</summary>
    public const double FullSaturation = 1.0;

    /// <summary>Saturation of the <see cref="Colors.HexPalette.Dark"/> lowlight variant — desaturated so a dimmed element reads as receding rather than merely darker.</summary>
    public const double DarkSaturation = 0.3;

    /// <summary>Resting lightness (0..1) of a rainbow hue's <see cref="Colors.HexPalette.Normal"/> variant.</summary>
    public const double BaseLightness = 0.6;

    /// <summary>Lightness of the <see cref="Colors.HexPalette.Light"/> highlight variant.</summary>
    public const double LightLightness = 0.8;

    /// <summary>Lightness of the <see cref="Colors.HexPalette.Dark"/> lowlight variant.</summary>
    public const double DarkLightness = 0.3;

    /// <summary>
    /// Ceiling for any lightness a caller-supplied factor scales up to. Past this a hue is
    /// indistinguishable from white, which would defeat the point of a rainbow.
    /// </summary>
    public const double MaxLightness = 0.92;

    /// <summary>Where on the color wheel a rainbow's first hue sits, in degrees.</summary>
    public const double WheelStartingDegree = 280.0;

    /// <summary>
    /// Cap on the wheel arc a rainbow may span, in degrees. Below the full 360 so a set with only two
    /// or three items doesn't land on near-opposite hues that read as unrelated.
    /// </summary>
    public const double WheelMaxDegreesPerRotation = 180.0;
}

/// <summary>
/// The tuning knobs for the *second* rainbow a word tree card renders: the one keyed by top-level
/// <see cref="Glyph"/> type (the captured, bolded sub-spans inside node text, plus the glyph key
/// strip below the tree), as opposed to the document-keyed rainbow above it.
/// <para>
/// Both key strips draw equidistant hues from the same
/// <see cref="DeterministicPalette.GetPositionalPaletteSet(int, double, double)"/> wheel, so hue
/// alone can't tell the reader which of the two signals they're looking at — a green document
/// border and a green glyph border would read as related when they aren't. These factors pull the
/// glyph rainbow off the document rainbow on the other two HSL axes instead: a touch brighter and
/// somewhat less saturated, so glyph color reads as a lighter tint over text while document color
/// stays the heavier, more saturated signal on node outlines.
/// </para>
/// </summary>
public static class GlyphKeyPaletteKnobs
{
    /// <summary>
    /// Multiplier applied to <see cref="RainbowPaletteKnobs.BaseSaturation"/> when building a
    /// glyph-keyed palette. Below 1 softens the hue so bolded capture text doesn't compete with the
    /// document-keyed node outlines drawn around it.
    /// </summary>
    public const double SaturationFactor = 0.85;

    /// <summary>
    /// Multiplier applied to <see cref="RainbowPaletteKnobs.BaseLightness"/> and
    /// <see cref="RainbowPaletteKnobs.LightLightness"/> when building a glyph-keyed palette. Above 1
    /// lifts the hue toward white, which is what keeps capture text legible against the dark node fill
    /// at the reduced saturation above. Kept only slightly above 1 so the light variant doesn't sit on
    /// the near-white lightness ceiling, where any saturation the factor above adds would be invisible.
    /// </summary>
    public const double LightnessFactor = 1.05;
}
