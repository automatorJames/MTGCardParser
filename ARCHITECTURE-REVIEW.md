# Architecture Review — Glyphotype / DocumentAnalysisInterface

**Date:** 2026-09-04
**Scope:** `Glyphotype`, `DocumentAnalysisInterface`, plus `MTGGlyphs` and `ConsoleUtility` where they are inseparable from the design.
**Baseline commit:** `b53adf7` — solution builds clean, 0 warnings, 0 errors.

This is an analysis-only document. Nothing here has been implemented.

---

## Contents

- [The architecture as it stands](#the-architecture-as-it-stands)
- [Tier 1 — Structural integrity](#tier-1--structural-integrity)
- [Tier 2 — Performance and scalability](#tier-2--performance-and-scalability)
- [Tier 3 — Maintainability](#tier-3--maintainability)
- [Suggested sequencing](#suggested-sequencing)

---

## The architecture as it stands

```
MTGGlyphs (declarations: ~50 Glyph classes + attributes + SQL repo)
        ^ discovered by AppDomain scan, not project reference
Glyphotype (library)
   |- StaticRegistry ----- GlyphTypeRegistry: static ctor builds everything
   |- GlyphPrimitives ---- Glyph / OneOf / ManyOf / CompoundOf / GlyphFused / DynamicGlyph
   |- RegexGeneration
   |    |- Graph --------- Navigation -> RegexNode tree -> RegexBrick[] -> compiled Regex
   |    |- CaptureContext/CaptureTrace -- hydration + display tree
   |    \- Presentation -- FormattingPipeline -> SmartLineRenderer (mutates bricks)
   |- Tokenizers --------- linear scan: every type x every position
   |- GlyphAnalysisDTOs -- CorpusAnalyzer / ProcessedLine / DigestedText (suffix automaton)
   \- GlyphEditor -------- ~800 LOC, currently inert
DocumentAnalysisInterface (Blazor Server)
   \- Pages render the singleton corpus directly, no virtualization
```

The core idea — declare a grammar as C# types, derive a named-capture regex from reflection, then rehydrate the CLR object graph out of the match — is genuinely good, and the seam between graph-building and presentation is well drawn. The problems are concentrated in three places: **shared mutable state**, **the tokenizer's inner loop**, and **the corpus analysis algorithms**.

---

## Tier 1 — Structural integrity

### 1. `RegexBrick` objects are shared per type but mutated during rendering

`RegexBrick.cs` carries `RegexFormatted`, `CommentFormatted`, `_depthOffset`, `_fullyQualifiedNameOverride`, and `_namedGroupParentOverride` — all written by `RegexBrickFormattingPipeline.Format()` (`RegexBrickFormattingPipeline.cs:40-52`) on the *same* `BuiltRegex.Bricks` list cached on the singleton registry.

`GlyphRegexPage.razor:35` calls `ToSmartRegex(...)` inside the render body. In Blazor **Server**, two circuits rendering that page concurrently — or one circuit with a different `RegexDisplayMode` / `HideBlankLines` than another — will interleave writes to the same brick objects.

The code already knows this is fragile: `ResetEmbeddingOverrides()` exists solely to undo the previous render's leftovers, and its doc comment says *"Bricks are shared/cached per type and re-formatted on every render, so this must overwrite rather than add."* That is a mutex-shaped problem being solved with a reset call.

**Fix:** make `RegexBrick` immutable (regex text + node lineage only) and have the formatting pipeline emit a separate `FormattedBrick` view model. This also removes the need for `ResetEmbeddingOverrides` entirely.

---

### 2. `[Flags] enum Proptions` has non-flag values

```csharp
// Glyphotype/Enums.cs:6-13
[Flags]
public enum Proptions { None, Plural, Optional, OneOrMore, NoPrecedingSpace }
//                       0      1        2          3              4
```

`OneOrMore == 3 == Plural | Optional`. So `PropertyNib.cs:33` (`Proptions |= Proptions.OneOrMore`) silently sets Plural *and* Optional, and every `HasFlag` check downstream (`Glyph.cs:33-34`, `NibContextAction.cs:44,49`, `EditorPropertyNib.GetSetFlags()`) reports both.

Latent today — nothing uses `[OneOrMore]` or passes `Proptions.OneOrMore` — but it will misfire the first time someone does.

**Fix:** explicit powers of two (`None=0, Plural=1, Optional=2, OneOrMore=4, NoPrecedingSpace=8`). The existing `// Todo: we should be using Quantifier to express quantifiers, not Proptions` at `PropertyNib.cs:30` is the better long-term answer.

---

### 3. `EnumNode.GetValue` throws the wrong exception on its own error path

```csharp
// EnumNode.cs:56-60
return Children.OfType<EnumMemberNode>()
    .FirstOrDefault(x => x.Regex.IsMatch(captureTrace.CaptureValue))
    .ScalarValue                     // <- NRE here when FirstOrDefault returns null
    ?? throw new Exception($"Found no matching values for enum ...");
```

The `??` binds to `.ScalarValue`, which is only reached after dereferencing the null. The carefully written diagnostic is unreachable; you get a bare `NullReferenceException` instead.

**Fix:** `FirstOrDefault(...)?.ScalarValue ?? throw ...`.

---

### 4. `CaptureContext` reflects into the private `_regex` field of `Match`

```csharp
// CaptureContext.cs:118-124
var regex = (Regex)typeof(Match)
    .GetField("_regex", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(match);
if (regex == null) return new();   // silently produces an empty capture dictionary
```

This runs on **every match** in the hot path, depends on a BCL implementation detail, and its failure mode is silent: an empty dictionary makes `CaptureContext[node]` throw `"Name '...' does not appear in the dictionary"` from somewhere unrelated.

**Fix:** `match.Groups` implements `IReadOnlyDictionary<string, Group>`, so `match.Groups.Keys` gives the names with no reflection at all. Alternatively pass `BuiltRegex.Regex` into the `CaptureContext` constructor — `RegexGraph.TryMatchWithinScope` already has it in hand.

---

### 5. `CaptureTrace` is a mutable dictionary key

`Equals` / `GetHashCode` (`CaptureTrace.cs:401-408`) are computed from `(CaptureContext, SourceNode, Index)`, but `Rebase()` (`CaptureTrace.cs:255-270`) mutates both `Index` and `CaptureContext`. Instances are used as keys in `CaptureContext._scopedViews` and in `ProcessedLine.GetPositionalPalettes`. Any trace rebased after being cached becomes unreachable in its own dictionary. `AdoptDynamicChildren` rebases whole subtrees.

Today the inner context is discarded so this does not bite, but it is one refactor away from being a very hard bug to find.

**Fix:** give `CaptureTrace` a stable immutable identity (an assigned sequence or `Guid`), or make rebasing produce a new instance rather than mutating in place.

---

### 6. `RuntimeSettings.DisposeAsync` will throw on circuit teardown

```csharp
// RuntimeSettings.cs:298-306
_saveCts?.Cancel();
if (_saveCts != null) await DebouncedSaveAsync();  // waits 200ms, then hits ProtectedLocalStorage
```

By the time a scoped service is disposed, the circuit is gone. `_pls.SetAsync` will throw `JSDisconnectedException`, which the `catch (TaskCanceledException)` inside `DebouncedSaveAsync` does not cover. It also stalls teardown by 200ms per circuit. `_saveCts` is never disposed and is written from arbitrary threads.

**Fix:** in `DisposeAsync`, save directly (no debounce delay) inside a `catch (JSDisconnectedException)`, or drop the flush entirely — each setter already persists.

---

### 7. Minor but real correctness items

| Location | Issue |
|---|---|
| `OneOfBase.cs:57` | `Assembly.GetExecutingAssembly()` is Glyphotype; glyph types live in MTGGlyphs, so `DescribeUsageSite` always falls back to the raw generic type name — the whole point of the method never fires. |
| `Glyph.cs:13-14` | `var name = lastDot == -1 ? memberExpression : memberExpression;` — both branches identical. `Prop(this.Foo)` would resolve `propInfo` to null and NRE on the next line. |
| `OneOf.cs:15,33` | Selects a property by ordinal from `GetProperties()`, whose order is explicitly undefined by the CLR. (Dead code today — no callers.) |
| `OptionalOf.cs:17` | `if (item.GetType() is not T)` tests whether a `Type` *object* is a `Glyph` — always true, so this constructor always throws. (Also dead code.) |
| `RuntimeSettings.cs:325-326` | `RuntimeSettingsDto.MinSpanWords` / `MinSpanOccurences` default to `0`; the live properties default to `3`. Any DTO shape change resets them to 0. |
| `EnumNode.cs:48` | `positionAmongSynonyms: enumMembers.Count > 1 ? j : null` — reads like it should be `patterns.Count > 1`. |
| `ManyOf.cs:19` | `.. SecondPlus?.Select(...)` — the `?.` implies null is expected, but spreading null into a collection expression throws. Currently unreachable (the initializer keeps it non-null), so this is a defensive marker that does not defend. |

---

## Tier 2 — Performance and scalability

### 8. The tokenizer's regexes are unanchored — this is the dominant cost

```csharp
// RegexGraph.cs:176
var match = BuiltRegex.Regex.Match(sourceText, currentIndex);
...
&& match.Index == currentIndex   // <- result discarded unless it started exactly here
```

`Regex.Match(input, startat)` **searches forward** from `startat`. So for every one of ~50 candidate types at every position in the line, .NET scans to the end of the line looking for a match that is then thrown away because it did not start at `currentIndex`. That makes tokenization roughly `O(types x n^2)` per line instead of `O(types x n)`.

**Fix:** prefix every compiled pattern with `\G` (or use `Match(input, start, length)` with an explicit start anchor). This is likely the single largest win in the whole system and is nearly free.

**Also:** the compiled regexes carry no `matchTimeout` (`BuiltRegex.cs:52`). `RegexMatchDebugger` uses a 1-second timeout for exactly this reason; the production path deserves the same, given greedy `[^.]+` dynamic patterns nested inside optional groups.

---

### 9. `UnmatchedString` compiles a fresh `Regex` per instance

```csharp
// UnmatchedString.cs:12-13
var regexForLength = new Regex($".{{{unmatchedLength}}}", RegexOptions.Singleline);
var match = regexForLength.Match(sourceText, unmatchedStart, unmatchedLength);
```

This constructs and parses a regex just to obtain a `Capture` for a known substring — once per unmatched span, corpus-wide. (`Tokenizer._unmatchedRegexCache` at line 7 was clearly meant to solve this and is never referenced.)

**Fix:** do not use a regex. `CaptureContext` only needs index/length/value; introduce a small `CaptureSpan` abstraction (or a `Match`-free `CaptureTrace` constructor) so `UnmatchedString` can be built directly.

---

### 10. The suffix automaton in `DigestedText` allocates a dense transition array per state

```csharp
// DigestedText.cs -- AutomatonState ctor
Next = new int[alphabetSize];   // alphabetSize = distinct words across the corpus
```

A suffix automaton has `O(N)` states where `N` is total word count, so memory is `O(N x |alphabet|)`. At the current `MaxSetSequence: 1` (~1 set) that is tolerable; across the full card corpus it is `~10^5 states x ~10^4 words x 4 bytes` — tens of gigabytes. **This is the hard ceiling on the whole corpus feature.**

**Fix:** use `Dictionary<int,int>` (or a sorted small array) for transitions — automaton edges are `O(N)` total regardless of alphabet size.

Two further quadratic terms in the same file:

- `FindAllOccurrences` is `O(spans x totalWords x patternLength)`. The automaton already knows every occurrence position; harvesting them from the link tree instead of re-scanning removes this.
- `BuildAdjacencyTree`'s repeated `Skip(1).ToList()` copies the remaining sequence at every level.

---

### 11. `FindEchoes` runs a full corpus scan inside the render loop, twice per line

- `CaptureTraceDisplayContext` is constructed per line, per render (`DocumentBlock.razor:31`) and calls `GetMaxEchoLaneCount` -> `FindEchoes` for every unmatched occurrence on the line.
- `SpanView.razor` then calls `GetEchoData()` -> `FindEchoes` **again** for the same occurrences, in its render body.

Each call iterates all of `DigestedText.Spans` with a nested word-window comparison. With echoes enabled this is by far the most expensive thing the UI does, and it recomputes identically on every keystroke in the search bar.

**Fix:** memoize echoes per `UnmatchedTextOccurrence`, keyed by `minWords`/`minOccurrences`. Better: precompute once in `CorpusAnalyzer.EnsureInitializedAsync`, since those two knobs are the only variables.

---

### 12. No virtualization; filtering happens in the render body

`CorpusCapturesPage.razor:11` renders `GetFilteredDocuments()` in full — every document, every line, every `SpanView` and `PropertyTable`. `GlyphRegexPage.razor:16` does the same for every type. Neither uses `<Virtualize>`.

The search predicate in `GetFilteredDocuments()` scans every line of every document on every render, and `RuntimeSettings.OnChanged += StateHasChanged` means every settings toggle and every search keystroke triggers a full re-render plus re-scan.

`appsettings.json` has `"MaxSetSequence": 1`, which is what keeps this survivable — a throttle masking the problem, not a solution.

**Fix:** `<Virtualize>` on both page loops; hoist the filtered list into a field recomputed only when `SearchTerm`/filters change, not per render.

---

### 13. `GetCopyText` builds a second complete `SmartRegex` per card, per render

`GlyphRegexPage.razor:97` runs the entire formatting pipeline again just to strip the comment column for the clipboard button — for every card on the page, on every render, whether or not anyone clicks copy.

**Fix:** make it lazy (compute inside the copy handler).

---

### 14. Allocation in hydration hot paths

- `NamedGroupNode.NamedGroupChildren` is `Children.OfType<NamedGroupNode>().ToList()` — a fresh list every call, and `TryHydrate` calls it once per node per match. It is derived from immutable structure; cache it.
- `CaptureTrace.EffectiveChildren` re-allocates a narrowed view per access, and `IsCollapsible` / `HasOwnBoundary` / `GetEffectiveDepth` each enumerate it. `SpanView.FindEmergentDescendant()` walks a chain calling `IsCollapsible` at each step, inside the render loop.

---

## Tier 3 — Maintainability

### 15. `GlyphTypeRegistry` is a static god-object with public mutable state

```csharp
public static Dictionary<Type, RegexGraph> RegexGraphs { get; set; } = [];
public static Dictionary<Type, Regex> TypeRegexes { get; set; } = [];
public static Dictionary<Type, GlyphTypeConfiguration> TypeConfigurations { get; set; } = [];
public static Dictionary<string, Type> NameToType { get; set; } = [];
public static List<Type> AppliedOrderTypes { get; set; } = [];
public static Tokenizer ClassTokenizer { get; set; }
```

Every one of these is publicly reassignable and publicly mutable. Combined with a static constructor that does assembly loading, reflection, validation, IL-emission setup, and regex compilation, the consequences are:

- **Untestable.** You cannot construct a registry with three test types; you get whatever is in the AppDomain. This is why there is no test project.
- **Uncontrollable init order.** The static ctor's own comments document two stack-overflow bugs that were fixed by careful pass ordering. That fragility is inherent to lazy self-reentrant static init.
- **Failure mode is a `TypeInitializationException`** wrapping an `AggregateException`, thrown from wherever the first touch happens.
- `CreateAndRegisterNewTypeAndSaveToDisk` mutates all of it at runtime *and* writes `.cs` files to `AppContext.BaseDirectory/../../../../MTGGlyphs` (`GlyphTypeRegistry.cs:13`) — a path that only exists on a dev box.

**Fix (highest maintainability leverage):** extract an `IGlyphRegistry` interface with an instance implementation built by an explicit `GlyphRegistryBuilder(params Assembly[])`. Register the singleton in DI and inject it. Keep a thin static facade during migration if needed. This single change unlocks testability for everything below it.

---

### 16. There are no tests at all

For a system whose behavior is "reflection produces a regex, which produces a capture tree, which rehydrates an object graph," this is the biggest structural gap. The invariants that most need pinning:

- **Golden regex tests:** type -> expected `MinifiedRegex` string. These would have caught every joiner-placement subtlety that the 60 lines of doc comments on `RegexNode` describe.
- **Round-trip tests:** text -> tokenize -> assert the hydrated object graph. Especially the `DynamicGlyph` narrowing retry loop in `RegexGraph.TryMatch`, which is the most intricate logic in the codebase and has zero coverage.
- **Validation tests:** every rule in `ValidateStructure` should have a fixture type that trips it.

`IsolateForTestingAttribute` is a testing hook wired into production discovery paths (`GetAllTopLevelGlyphTypes`, `GetAllTypesForValidation`) — a real test harness replaces it. `ConsoleUtility` currently exists only to print `GetStructuralValidationErrors()`; that is a test assertion wearing a console app costume.

---

### 17. `RuntimeSettings` is 365 lines of four-way-duplicated boilerplate

15 settings x (backing field + property with identical getter/setter shape + DTO property + DTO ctor assignment + `EnsureLoadedAsync` assignment). Adding one setting means editing four places, and the `MinSpanWords` default divergence in Tier 1 item 7 is exactly the bug that pattern produces.

**Fix:** a `Setting<T>` wrapper (value, `OnChanged`, debounced save) reduces each setting to one line, and serializing the settings object directly removes the hand-written parallel DTO.

---

### 18. Dead and disabled subsystems carrying weight

| Item | Status |
|---|---|
| `DynamicTypeEmitter.cs` (285 LOC, IL opcode parsing/rewriting) | **Zero references anywhere.** |
| `EditorGlyph.Update()` | Entire body commented out — the glyph editor is inert. |
| `RegexEditorDialog` + `regex-editor.ts/.js` (~800 LOC) | Reachable only from the commented-out click handler in `DocumentBlock.razor`. |
| `MTGPlexer/`, `MTGTokenUnits/`, `CardAnalysisInterface/` | Empty directories, not in the solution. |
| `Tokenizer._unmatchedRegexCache` | Declared, never used. |
| `Quantifier.TwoOrMore`, `CaptureGroupJoinStrategy`, `MultiItemOrdinal`, `OneOfItemOrdinal` | Declared, never used. |
| `CardDataGetter.GetDocumentsAsync` | 12 lines of commented-out debug filters. |

The GlyphEditor is a real feature mid-flight, so it is a judgment call — but it belongs on a branch or behind a feature flag, not half-commented in `master`. The rest is deletable today.

---

### 19. Async and threading patterns in the UI layer

- `RuntimeSettings.OnChanged += StateHasChanged` (`CorpusCapturesPage.razor:29`) invokes `StateHasChanged` directly from whatever thread set the property; it should be `InvokeAsync(StateHasChanged)` (`GlyphRegexPage` does this correctly).
- `private async void OnSettingsChanged()` (`GlyphRegexPage.razor:85`) — `async void`; an exception here is unobservable and will tear down the circuit.
- `_ = DebouncedSaveAsync();` x 15 in `RuntimeSettings` — fire-and-forget with no exception observation.
- `DebugRegexDialog.RunAnalysisAsync` awaits `InvokeAsync(StateHasChanged)` once per graph inside `Task.Run` — hundreds of full re-renders for a progress bar. Throttle to every N graphs, or to a timer.

---

### 20. Project structure

- **TFM split:** `Glyphotype` targets `net8.0`, `DocumentAnalysisInterface` targets `net10.0`. It works, but it leaves two major versions of runtime/regex/LINQ improvements on the floor in the library that does all the work.
- **`<Nullable>disable</Nullable>`** in both projects. For a codebase this dense in reflection and nullable-by-design capture results (`GetValue` returning null to signal "did not match"), NRTs would be genuinely load-bearing. `SpanView.razor` already uses `= null!` annotations, so the intent is there.
- **Implicit assembly coupling:** `Glyphotype` has no reference to `MTGGlyphs`, but `LoadAllAssemblyTypes()` (`GlyphTypeRegistry.cs:60-98`) calls `Assembly.LoadFrom` on every DLL in the output directory and swallows failures. It works only because `DocumentAnalysisInterface` happens to reference both. Making registry construction take explicit assemblies (item 15) fixes this too.
- **Build artifacts in source control:** `.js` and `.js.map` are committed next to their `.ts` sources, while `corpus-captures.js`, `regex-debug.js`, `search.js`, and `general.js` are hand-authored JS with no TS twin. Pick one authoring model; gitignore the outputs.

---

### 21. Data access

`select * from Card` (`CardDataGetter.cs:31`) with no projection and no paging, materialized entirely into memory at startup. `Card` has 13 columns; the analyzer uses `Name` and `Text`. `MaxSetSequence: 1` is doing load-bearing work here.

---

## Suggested sequencing

**Do first — high value, low risk, no design change:**

1. `\G`-anchor the tokenizer regexes (item 8) — biggest single perf win
2. Fix `Proptions` flag values (item 2) and `EnumNode.GetValue` (item 3)
3. Replace the `Match._regex` reflection with `match.Groups.Keys` (item 4)
4. Drop the `Regex` from `UnmatchedString` (item 9)
5. Delete the confirmed-dead code (item 18)

**Then — the two things that unlock everything else:**

6. **Extract `IGlyphRegistry` from the static registry (item 15).** Nothing else is properly testable until this exists.
7. **Stand up a test project** with golden-regex and round-trip-hydration suites (item 16). Do this immediately after item 6, before any of the refactors below — those refactors are exactly where the safety net is needed.

**Then — with tests in place:**

8. Make `RegexBrick` immutable + separate formatted view model (item 1) — removes the concurrency hazard and simplifies the pipeline
9. Rework the suffix automaton's transitions and occurrence harvesting (item 10) — removes the corpus-size ceiling
10. Memoize echoes; add `<Virtualize>`; hoist filtering out of render bodies (items 11, 12, 13)
11. Give `CaptureTrace` a stable identity (item 5)
12. Collapse `RuntimeSettings` boilerplate (item 17); unify the TFMs and enable NRTs (item 20)

---

## A closing note

The doc comments throughout `RegexNode`, `CaptureTrace`, and `RegexGraph.TryMatch` are unusually good — they explain *why* rather than *what*, and several of them correctly identify the exact fragility flagged above (shared brick mutation, the missing cycle guard, the reentrancy that caused stack overflows). The design reasoning is sound; what is missing is the mechanism to enforce it, which is what items 6 and 7 are for.
