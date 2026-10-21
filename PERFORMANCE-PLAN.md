# Scaling the grammar loop: findings and plan

**Date:** 2026-10-20
**Baseline commit:** `577c857` (MTGGlyphs: 762 glyph types, 253 top-level; corpus: 26,228 cards, 135 sets)
**Scope:** why the app now needs minutes and tens of gigabytes per step of the AI grammar loop, measured on the real grammar and corpus, and what to do about it, in order.

This is an analysis document with one prototype behind it. Nothing in the main branch is changed. The prototype lives in the `perf-spike` worktree (`../MTGCardParser-spike`, uncommitted), and the measurements come from a small benchmark harness described below.

---

## Summary

The slowness is not a hard rule of "big grammar × big corpus × regex". The regexes themselves are cheap to run, even at 8.5 MB apiece. The time and memory go to five specific mechanisms around them, all in the engine, none of which the grammar's authors can see:

1. **Every match materializes every named group of its regex.** A top-level MTG glyph's regex averages 3,546 named groups (max 18,271). `CaptureContext` builds a dictionary and a `Group` object for each one, per match, and keeps the `Match` alive for the life of the corpus. That is about 254 KB per corpus line (12 GB per full-corpus tokenization), and it is also paid on every match whose hydration then fails.
2. **A dynamic slot tokenizes its whole captured text in partial mode but reads only the first token.** With nested dynamic slots (23 frames with `Effect`, several with two slots), the discarded work recurses. One sampled line took 9.9 s and 292,603 tokenizer positions; the slowest 1% of lines take 48% of the time.
3. **Parallel tokenization defeats .NET's regex runner cache.** A `Regex` keeps one cached runner, whose backtracking arrays are sized by the pattern. Sixteen threads calling the same 8.5 MB `Regex` allocate about 1 MB per call and run 40 times slower than one thread. Tokenizing the sample allocated 498 GB. This is why 16 cores were slower than one.
4. **The flattened regex per type is enormous, and rebuilt in bulk.** 104 M characters of regex text across all types; 4.1 GB of node graph; 31 s to build, serially under one lock. Editing a dependent glyph that 39 glyphs use (`PermanentPhrase`) re-emits 161 types (25 s) and marks 85% of lines as needing re-tokenization. That is the shape of nearly every evaluate the agent makes.
5. **Several corpus generations are retained at once**, each pinning its own generation of emitted types and their graphs, so memory ratchets up with every evaluate until the process dies.

A prototype of fixes for (1), (2) and (3), 70 lines in six engine files, changes no parse (378 tests pass; the nested rendering of every sampled line is byte-identical) and gives:

| Measure (1-in-10 sample: 2,623 cards, 4,805 lines) | Baseline | Prototype |
|---|---|---|
| Tokenize sample, 16 cores | 50.8 s / 55.4 s (two runs) | 1.9 s cold, 0.6 s warm |
| Tokenize sample, one core | 31.7 s | 2.4 s |
| Slowest line | 9,905 ms | 65 ms |
| Tokenizer positions per line, mean / max | 90.8 / 292,603 | 5.6 / 413 |
| Allocated while tokenizing the sample | 498 GB | 3.2 GB |
| Retained heap per line (isolated from fixed costs) | 254 KB | 21 KB |
| Full-corpus projection: tokenize | about 9 min | about 10 to 20 s |
| Full-corpus projection: heap for one tokenization's lines | about 12 GB | about 1 GB |
| Re-tokenize after editing `PermanentPhrase` (85% of lines) | 61 s | 16.7 s before the lock fix; a few seconds expected with it |

The rest of this document gives the measurements, the mechanisms, and a phased plan: engine fixes first (they are where the factor-of-10s are), structural engine work second, grammar refinement third and guided by a tool rather than by hand, and a longer-term option last.

---

## How this was measured

A console harness in this session's scratchpad (`Bench/Program.cs`; worth moving into `ConsoleUtility` as a `bench` command, see Phase 0) references `Glyphotype`, `MTGGlyphs` and `Glyphotype.Distiller`, loads every card from the `Magic` database, keeps every 10th card (2,623 cards, 4,805 non-empty lines, 68,557 words) and measures, in order: grammar build; regex sizes; parallel tokenization with retained heap; serial per-line timing; per-type regex cost at clause starts; `MdlScorer.Score`; and an evaluate step for a trivial added glyph and for a one-attribute change to the most-referenced dependent glyph. Machine: 16 logical cores, 128 GB. The running app (started 15:14) sat at 55 GB working set while this ran.

Full outputs: `bench10.txt` (baseline) and `bench10-spike.txt` (prototype) in the scratchpad, reproduced in the appendix.

---

## Findings

### 1. The grammar build: 31 s and 4.1 GB before a single card is read

| | |
|---|---|
| Types | 762 (253 top-level, 322 dependent, the rest reached through properties) |
| Regex text, top-level types | 32.6 M chars |
| Regex text, all types | 104.7 M chars |
| Largest regex | `KeywordWithReminder`: 8,559,276 chars, 18,271 named groups, 568,068 alternations |
| Median / p90 top-level regex | 98 chars / 206,488 chars |
| Named groups per top-level regex | mean 309, max 18,271 |
| `GlyphGrammar.Default` | 31.5 s, 4,113 MB retained |
| Compiling all top-level `Regex` objects | 1.7 s, 373 MB |

So compiling the patterns is cheap; building the node graphs is what costs. Every nested glyph and every vocabulary is inlined as its own subtree each time it appears (a `CreatureType` with 313 members and plurals, inlined once per use), which is why `RegexNode`'s own comment speaks of "millions of member nodes". Each node also leaves a `RegexBrick` in `BuiltRegex.Bricks`, kept for the formatted-regex display whether or not anyone ever opens it. `GlyphTypeCache.GetRegexGraph` builds every graph under one reentrant lock, so the 31 s is single-threaded on a 16-core machine.

The eight largest regexes (`KeywordWithReminder`, `CardAbilityLine`, `ActivationRestriction`, `TokenHasAbility`, `ActivatedAbility`, `AlternativeCost`, `GainControl`, `UnlessPaid`) share one inlined subtree of roughly 184,000 alternations, which is the `Cost`/`Effect`/`PermanentPhrase` family. That family is Phase 3's first target.

### 2. Matching the regexes is fast; the tokenizer around them is not

Running every top-level type's scope-filling regex at every clause start of the sample (282,345 tries) takes **0.5 s in total**. The costliest type, `KeywordWithReminder`, takes 17 µs per try. The `\G` anchor and the start-character filter are doing their job; the regex engine is not the bottleneck.

Tokenizing the same sample takes 50.8 s on 16 cores and 31.7 s on one. The parallel run is *slower* because the work is dominated by a few lines and by allocation:

| Per-line serial timing | |
|---|---|
| Mean / median | 6.6 ms / 2.0 ms |
| p99 / max | 40.8 ms / 9,905 ms |
| Share of time in slowest 1% of lines | 48% |
| Share of time in slowest 10% | 68% |
| Tokenizer positions tried per line, mean / max | 90.8 / 292,603 |

The 9.9-second line is "you may have this creature enter as a copy of any creature on the battlefield, except it doesn't copy that creature's…". Every one of the 292,603 positions is a place the tokenizer offered all candidate types a match. The cause is in `DynamicGlyphNode.TryHydrate`:

- It first asks the tokenizer to resolve the slot's text whole (one position; fine).
- Failing that, it calls `Tokenizer.Tokenize(captureValue, allowPartialClauseMatches: true)`, which ratchets through **every word** of the slot text offering **every candidate type** at each, and then reads only the **first token** of the result. Everything after the first token is discarded.
- Each candidate that matches and itself has a dynamic slot repeats this on its own slot, so frames nested in frames (`TriggeredAbility` has two slots; `EffectIfCondition`, `ConditionalEffect`, `DelayedEffect`, `ThenSequence` and others have slots that resolve to glyphs with slots) multiply the waste geometrically. The narrowing retry in `RegexGraph.TryMatch` then runs the whole enclosing match again against a shorter window, which runs the slot resolutions again.
- A successful regex match whose hydration then fails has already paid finding 1's cost: thousands of `Group` objects and a dictionary, allocated and thrown away.

The prototype's fix is exact: a `Tokenizer.MatchFirstGlyph` that tries the candidates once, at index 0, in partial mode, which is precisely the first token the old call produced. On the sample it cuts positions per line from 90.8 to 5.6 and the serial time from 31.7 s to 2.4 s.

### 2b. Parallel tokenization fights the regex runner cache

With the serial time down to 2.4 s, the parallel run was still 19 s. A micro-test of one `Regex` object called 2,000 times with the same text explains it:

| Regex | Pattern length | 1 thread | 16 threads | Allocated per call, 16 threads |
|---|---|---|---|---|
| `KeywordWithReminder` | 8,559,276 | 34 ms | 1,322 ms | 1,099 KB |
| `ActivatedAbility` | 2,394,092 | 6 ms | 1,290 ms | 934 KB |
| `CreateToken` | 573,333 | 1 ms | 12 ms | 58 KB |
| `EndOfCombatEffect` | 78 | 2 ms | 1 ms | 0.1 KB |

A .NET `Regex` caches exactly one runner; its backtracking track and stack arrays are sized by the pattern's node count, so the big patterns' runners are megabytes. A thread that finds the cache empty (because another thread holds the runner) builds a new runner, uses it once and drops it. Under `AsParallel()` every call to a big regex does this, and the corpus-wide result is 498 GB of garbage and 55 gen-2 collections for 4,805 lines. The matches themselves take microseconds, so serializing them per `Regex` (a `lock` on the `Regex` object around `Match`, prototyped in `RegexGraph.TryMatchRegex`) costs nothing measurable and brings the sample's parallel time to 0.6 s warm. A per-thread runner would be better still but has no public API; a small pool of `Regex` copies per pattern is the alternative if lock contention ever shows.

### 3. Memory: about 254 KB per line, almost all of it `Match` bookkeeping

| | |
|---|---|
| Retained heap after tokenizing the first sample | 2,709 MB (564 KB/line, but this includes compiling the dependent types' regexes and their runner caches, about 1.5 GB) |
| Retained heap for a second, disjoint sample (per-line cost only) | 1,205 MB = 254 KB/line |
| Projected per full-corpus tokenization (about 48,000 lines) | about 12 GB, plus about 6 GB fixed (graphs, regexes, runners); the app shows 20 to 30 GB after startup |
| Named groups per matched top-level type, weighted by matches | 3,546 |

`CaptureContext`'s constructor calls `GetNamedGroupCaptures`, which reflects the `Regex` out of the `Match`, then for **every** group name builds `match.Groups[name].Captures.ToArray()` into a dictionary. For a 3,500-group regex that is 3,500 `Group` objects, 3,500 dictionary entries, the `GroupCollection`'s 3,500-slot array, and the `Match`'s own two arrays sized by group count, all kept alive by the `CaptureUnit` for as long as the corpus is. Hydration touches a few dozen of those groups. Nothing reads the dictionary after hydration: every `CaptureTrace` copies the index, length and text it needs, and every later reader (the UI, the scorer, the corpus queries) walks traces.

The prototype resolves groups on demand through `match.Groups[name]` and drops the `Match` (`CaptureContext.Seal()`) as soon as hydration has run, success or failure. Per-line retention falls to 21 KB. What remains is the traces, the glyph instances, the line's strings and its `TokenizationTrace`; the fixed 1.9 GB (compiled regexes and their cached runners) is Phase 2's concern.

### 4. What an evaluate actually costs, and why editing a shared glyph is the worst case

| Evaluate step on the sample | Build grammar | Lines that may differ | Re-tokenize |
|---|---|---|---|
| Add one trivial glyph | 0.12 s (1 type emitted) | 0 of 4,805 | 0.04 s |
| Add a marker to `PermanentPhrase` (39 direct referrers) | **24.9 s** (161 types re-emitted) | **4,091 of 4,805** (85%) | **61.2 s** |

Scaled to the full corpus the second row is roughly 25 s + 10 min, which is the "evaluate takes minutes" the agent hits whenever it touches anything in the lower layers, and the lower layers are where the guidance tells it to work.

Two separate things are happening:

- **Re-emission.** `GrammarEmitter.GlyphKey` includes the identity of every type a glyph refers to, so a change to `PermanentPhrase` re-emits every glyph that reaches it. For a change that alters the regex this is unavoidable in a flattened-regex design, since the referrers' patterns really do change. For a change that doesn't (a marker, documentation, `TokenizationOrder`), it isn't; and either way the 25 s is graph *building* (finding 1), not regex compiling.
- **Incremental re-tokenization.** `TokenizationChange.MayDiffer` treats each re-emitted type as removed and added, and a line may differ if any removed type's regex matched anywhere in it. With 161 re-emitted types that is most lines. Again, for a regex-changing edit this is close to the truth; the remedy is to make re-tokenizing a line cheap (findings 2 and 3), not to guess better about which lines to skip.

### 5. The evaluate report walks the corpus five or six times

`GrammarAgent.EvaluateAsync` → `DescribeEvaluation` → `CorpusQueries.CompareTokenizations` renders every line of both tokenizations, nested, as strings and compares them; then `FindSteals` does the same again (and renders twice more for `WordOwners`); then `RuleViolations` calls `FindSteals` a second time; `ApplyAsync` repeats `RuleViolations`. On the sample, with nothing changed, `CompareTokenizations` costs 0.39 s and one `FindSteals` 0.10 s, so about 7 s per evaluate on the full corpus for a step that changed nothing. `ProcessedDocument.Retokenize` keeps the very same `ProcessedLine` object for every line it didn't re-tokenize, so a reference-equality check (`ReferenceEquals(beforeLine, afterLine)`) would skip almost all of this work for a typical step.

### 6. Scoring

`MdlScorer.Score` takes 3.5 s on the sample the first time (0.7 s the second, once the pattern regexes are compiled), so roughly 10 to 35 s on the full corpus, single-threaded. The empty-grammar baseline it re-tokenizes every call costs nothing (0.02 s); `ToDefinition` costs nothing (0.01 s). The time is in `CorpusEncoding`, most likely `FindOpenPatterns` → `BuiltRegex.FindPatternCaptures`, which compiles a second copy of each authored-pattern type's regex with extra groups (another 8.5 MB regex for `KeywordWithReminder`) and re-matches every top-level token against it, plus the `ParseRenderer` walks. Every context's code length depends only on final counts, so the encoding can run in parallel with per-thread counts merged.

### 7. Retention: why memory ratchets across the loop

A full tokenization is held by: `CorpusAnalyzer.ProcessedDocuments` (compiled grammar), `WorkspaceManager._source.Documents`, `GrammarWorkbench._committedDocuments` and `_committedTrial`, `LatestWorkingScore.Documents`, up to two `_recentScores`, `GrammarAgent._lastEvaluated` (before and after), `ActiveCorpus._current` and `_building`, and each `CorpusAnalyzer` the `ActiveCorpus` builds also builds `GlyphOccurrenceSummaries` and (in the background, every time) a `DigestedText` suffix automaton. Incremental re-tokenization shares unchanged lines between generations, which helps, but each generation's *changed* lines reference that generation's emitted types, which pin that generation's graphs in `GlyphTypeCache` (a `ConditionalWeakTable`), so three to five generations of 1 to 4 GB graphs can be live at once. With finding 3 fixed the per-generation line cost shrinks by two orders of magnitude; with finding 1's graph memory fixed, the graph generations do too. The retention policy still deserves tightening (Phase 1, item 6).

### 8. Smaller items seen on the way

- `CaptureTrace`'s constructor runs a regex to find the parent name of every trace; `LastIndexOf('_')` does the same (prototyped).
- `TokenizationTrace` keeps one `Attempt` struct per position; the 292k-position line keeps 12 MB of them. Deduplicating attempts, or capping them, bounds this.
- `NamedGroupNode.NamedGroupChildren` allocates a list per call, once per node per hydration.
- The baseline harness process crashed at exit with "Fatal error. Internal CLR error (0x80131506)" in `LoaderAllocatorScout.Finalize`, i.e. while unloading collectible assemblies. The app's crashes are reported as out-of-memory, and nothing in the event log says otherwise, but if a crash ever occurs with memory to spare, collectible-assembly unloading is the first suspect.

---

## Prototype (`perf-spike` worktree)

Six files, about 70 lines added, 37 removed, in `Glyphotype` only (the diff is also saved as `perf-spike.patch` in the session scratchpad):

| File | Change |
|---|---|
| `CaptureContext.cs` | Holds the `Match`; resolves a group on first request through `match.Groups[name]`; `Seal()` drops the `Match`. The eager dictionary and the reflection into `Match._regex` are gone. |
| `RegexGraph.cs`, `CaptureUnit.cs` | Call `Seal()` as soon as hydration has run (both outcomes) and after a span context is made. `TryMatchRegex` takes a `lock` on the `Regex` around each `Match` call. |
| `CaptureTrace.cs` | Parent name by `LastIndexOf('_')` instead of a regex, in the constructor and in `Rebase`. |
| `Tokenizer.cs` | `MatchFirstGlyph(text, scopeToType, includeDependentTypes)`: the first token of a partial tokenization, without the rest. Records the same attempt the full call did. |
| `DynamicGlyphNode.cs` | Uses `MatchFirstGlyph` in place of the full partial tokenization. |

Semantics: the only behavioral change is that a group hydration never asked about now reads as "matched nothing" after sealing, rather than as its captures. No code reads such a group today (the one caller of the indexer is `NamedGroupNode.SetPropertyValue`, during hydration). `TokenizationChange.MayDiffer` sees the same attempts for the positions that decide a slot's resolution, so incremental re-tokenization is unaffected.

Verification: all 378 tests in `Glyphotype.Tests` pass; the nested rendering of every line of the sample is identical between baseline and prototype (`dump-baseline.txt` vs `dump-spike.txt`, 4,805 lines).

Results (sample of 2,623 cards, 4,805 lines):

| | Baseline | Lazy captures + first-token dynamics | + per-regex lock |
|---|---|---|---|
| Tokenize, 16 cores | 50.8 s | 19.3 s | 1.9 s cold, 0.6 s warm |
| Tokenize, 1 core | 31.7 s | 3.1 s | 2.4 s |
| Allocated while tokenizing | 498 GB | 208 GB | 3.2 to 7.7 GB |
| Gen-2 collections | 55 | 28 | 3 |
| Slowest line | 9,905 ms | 65 ms | (not re-measured) |
| Positions per line, mean / max | 90.8 / 292,603 | 5.6 / 413 | same |
| Retained per line (second-sample delta) | 254 KB | (not measured) | 21 KB |
| Re-tokenize after the `PermanentPhrase` edit | 61.2 s | 16.7 s | (not re-measured; expect a few seconds) |

What didn't change, because the prototype doesn't touch it: grammar build 31 to 33 s and 4.1 GB; re-emitting 161 types for the `PermanentPhrase` edit 25 s; `MayDiffer` over the sample 3.8 to 4.7 s; scoring 3.5 s.

---

## Plan

### Phase 0: make it measurable (half a day)

1. Move the harness into `ConsoleUtility` as `bench [every]` with the sections above, and a `dump` mode that writes each line's nested rendering, so every engine change can be checked for identical parses on a real sample before it's trusted.
2. Add counters to `TokenizationTrace` (regex calls, hydrations, dynamic resolutions, narrowing retries) and surface them in `evaluate`'s report as one line, so the agent's steps show their own cost.

### Phase 1: engine fixes that keep every parse identical (a few days)

Measured effect of items 1 to 3 on the sample: tokenization 25 to 80 times faster, retained memory per line 12 times smaller, allocation 150 times smaller. Expected on the full corpus once items 1 to 9 are in: startup tokenization in seconds rather than minutes, an evaluate on a shared dependent down from about 11 minutes (25 s build + 45 s `MayDiffer` + 10 min re-tokenize) to about a minute, dominated by graph building until Phase 2.

1. **Lazy capture lookup and sealing** (prototyped). Also seal in `RegexMatchDebugger` if it builds contexts; add a test that a sealed context returns an empty trace for an unresolved group.
2. **First-token-only dynamic resolution** (prototyped).
3. **One thread per `Regex` at a time** (prototyped as a lock in `TryMatchRegex`). Apply the same to `BuiltRegex.FindPatternCaptures` and `TokenizationChange.CouldMatch` if they ever run in parallel. If contention on the hottest regexes shows up in a profile, replace the lock with a small pool of `Regex` copies per pattern.
4. **Memoize dynamic slot resolution per tokenizer**, keyed by `(slot text, filter type)`, storing which type resolved it whole (or the first-glyph type and length), plus the attempts and matched types recorded while finding out, replayed into the current line's trace on a hit. The corpus is templated, so the same slot texts recur thousands of times. On a hit, only that one type's `TryMatch` runs. Exact, because a tokenizer belongs to one grammar.
5. **Evaluate report**: skip reference-equal lines in `CompareTokenizations` and `FindSteals`; compute `FindSteals` once per evaluation and keep it on the `Evaluation`; render each changed line once.
6. **Scoring**: cache the baseline bits per corpus; parallelize `CorpusEncoding.Encode` by line with merged counts; build `FindPatternCaptures`' regex lazily only for types with open-ended patterns (it already is lazy, but check that it isn't being built for types whose patterns are all closed).
7. **Retention**: keep `Documents` only on the latest working score and the committed trial (older `_recentScores` keep their `MdlScore` and drop their documents); clear `_lastEvaluated.Evaluation.After.Documents` once applied or superseded; make `ActiveCorpus` build the `DigestedText` on first use by an echo view rather than after every step.
8. **Bound `TokenizationTrace`**: deduplicate attempts (a `HashSet`), which also makes `MayDiffer` cheaper (it was 3.8 to 4.7 s on the sample for the `PermanentPhrase` edit, so about 45 s on the full corpus).
9. **Parallel graph building**: build each type's graph outside `GlyphTypeCache._buildGate` and publish under it, with `GlyphGrammar.Build` requesting graphs in parallel. Expected: 31 s → a few seconds at startup, and the 25 s re-emission in finding 4 shrinks accordingly.

### Phase 2: structural engine work (one to two weeks)

1. **Shrink the node graph.** Give `EnumNode` a shared per-enum member table instead of one `EnumMemberNode` per member per use, generating bricks and alternation text from it on demand; make `BuiltRegex.Bricks` lazy (rebuilt from the node tree when the formatted display asks). Target: the 4.1 GB of graphs to well under 1 GB, and graph builds proportionally faster. This is what makes re-emitting 161 types cheap.
2. **Reuse compiled `Regex` objects by pattern text** across emitted generations (a weak cache keyed by `MinifiedRegex`), so a re-emitted referrer whose pattern didn't change doesn't recompile 8 MB of regex.
3. **Behavioral keys for re-emission.** Distinguish edits that change a glyph's matching (nibs, properties, patterns) from ones that don't (markers, documentation, `TokenizationOrder`) in `GrammarEmitter.GlyphKey` and `TokenizationChange`, so the latter re-emit one type and re-tokenize only lines where that type was offered a match. Careful: a marker change *does* change which dynamic slots a glyph can fill; the attempt records already cover that case through `ScopeToType`.
4. **Per-line work distribution.** Tokenize with `Parallel.ForEach` over lines (dynamic partitioning) rather than `AsParallel().AsOrdered()` over documents, reassembling documents afterwards, so one 10-second line doesn't idle fifteen cores. Add a per-line match timeout that logs and leaves the clause unmatched rather than stalling a step.
5. **Dynamic slots that fail to resolve whole and aren't trailing** currently fail the enclosing match without trying other splits (e.g. a different comma in "whenever a, b, c"). Not a performance item, but worth recording while this code is open: the regex can't backtrack into hydration, so a correct fix needs the matcher to enumerate slot boundaries, which is Phase 4's territory.

### Phase 3: grammar refinement, guided by a tool rather than by hand (ongoing, after Phase 1)

The grammar is the input the engine is given; it should not have to be shaped around engine limits, and after Phases 1 and 2 most of it won't be. Two things remain worth doing because they improve the grammar as grammar:

1. **An expansion report.** For each top-level glyph: regex size, named-group count, and which nested glyph contributes most of it (subtree size × times inlined). Expose it as a `list_glyphs` sort (`sort=size`) and in the Grammar Tools page. The `Cost`/`Effect`/`PermanentPhrase` family will top it. This is the 8.5 MB `KeywordWithReminder` story: a reminder-text glyph that nests the whole effect vocabulary several times over.
2. **Targeted refactors the report points at**, each evaluated as an ordinary step: turn a very wide inlined `GlyphOneOf` that appears many times into a `DynamicGlyph` slot with a marker (dynamic slots aren't inlined, and after Phase 1 item 3 they're cheap to resolve); give two-slot frames like `TriggeredAbility` a typed first slot (a marker for triggers) so the greedy `[^.]+` isn't resolved against every glyph; prune nested one-ofs whose members the corpus never uses in that position (the scorer already knows which vocabulary members are used; the same usage data exists for one-of alternatives).

Pruning "unnecessarily wide-scoped captures" by hand is not worth the agent's or the person's time before the report exists: the measurements say the wide regexes cost almost nothing to *run*; they cost to *build* and to *hold*, and Phase 2 attacks that directly.

### Phase 4: a longer-term option

Replace the flattened regex per top-level type with a memoized recursive-descent (packrat) matcher over the same node graph: leaf glyphs keep their regexes, composite glyphs match their children in sequence, and results are memoized by `(node, position)`. This removes the inlining blowup entirely (regex text and graph size become linear in the grammar), makes dynamic slots and alternatives ordinary choice points that can backtrack, and keeps the `CaptureTrace` tree and the formatted-regex display (built from the node graph, not from the matcher). It is a rewrite of `RegexGraph.TryMatch` and the hydration entry points, with the test suite and the `dump` diff as the safety net. Not needed to reach short iterations; worth considering once the grammar passes a few thousand types.

### The AI loop itself

- After Phase 1, an evaluate on a shared dependent should take tens of seconds, which is the "not a bottleneck" regime the loop had on smaller grammars. No sampling of the corpus is proposed; the full corpus stays the thing evaluated and applied against.
- `evaluate` should report its own cost (Phase 0 item 2) so a step that is expensive to *evaluate* is visible as such, separately from one that is expensive in bits.
- `ApplyAsync` already reuses the last evaluation; keep that, and make `RuleViolations` reuse the steals computed for the report.

### Acceptance targets (full corpus, this machine)

| | Today | Target after Phase 1 | Target after Phase 2 |
|---|---|---|---|
| App start to serving | several minutes | under 2 minutes | under 1 minute |
| Heap after startup | 20 to 30 GB | under 4 GB | under 2 GB |
| Evaluate, new top-level glyph | tens of seconds | under 10 s | under 5 s |
| Evaluate, shared dependent edited | minutes | under 60 s | under 20 s |
| Loop lifetime before memory is exhausted | minutes | hours | not a limit |

---

## Implementation status (2026-10-20, same day)

Phases 0 to 3.1 are implemented in the working tree (uncommitted). Every engine change was checked three ways: the 378 engine tests pass; the nested rendering of every line of the 1-in-10 sample is byte-identical to the baseline's (`bench` with a dump path, diffed); and for the graph refactor, a per-type fingerprint of regex text, brick sequence, named-group count and start characters is identical across all 762 types (`ConsoleUtility regexes`). The web app compiles against the changes (built to a scratch directory, since the running app locks its own output).

### Results on the sample (2,623 cards, 4,805 lines; 16 cores)

| | Baseline | Finished |
|---|---|---|
| Grammar build (`GlyphGrammar.Default`) | 31.5 s, 4,113 MB | 13 to 15 s, 1,040 MB |
| Tokenize, parallel | 50.8 s | 2.0 s |
| Tokenize, one core | 31.7 s | 2.2 s |
| Allocated while tokenizing | 498 GB | 2.9 to 4.2 GB |
| Retained per line (isolated) | 254 KB | 21 KB |
| Slowest line | 9,905 ms | 27 ms |
| Positions tried per line, mean / max | 90.8 / 292,603 | 4.0 / 43 |
| Evaluate: add one glyph (build, re-tokenize, report, score) | 0.1 s + 0 + 0.4 s + 3.5 s | 0.15 s + 0 + 0 + 1.9 s |
| Evaluate: edit `PermanentPhrase` (161 types re-emitted, 85% of lines) | 25 s build, 3.8 s `MayDiffer`, 61 s re-tokenize, 3.5 s score | 10 s build, 0.4 s `MayDiffer`, 2.3 s re-tokenize, 2.9 s score |
| Score, first / warm | 3.5 s / 3.5 s | 4.2 s / 1.6 to 2.4 s |

Projected to the full corpus (about ten times the sample): startup tokenization in about 20 s instead of 9 minutes; the heap for the compiled grammar and one tokenization at about 3 GB instead of 20 to 30; an evaluate that edits a widely shared glyph in well under a minute instead of about 11, now dominated by building the 161 re-emitted graphs and compiling their pattern regexes for scoring.

### What was done

**Phase 0.** `ConsoleUtility bench [every] [dump-path]` runs the measurements above and dumps parses for diffing; `ConsoleUtility regexes [path]` fingerprints every type's regex and bricks; `ConsoleUtility expansion [count]` prints the expansion report (Phase 3). `evaluate` reports its own cost on a final line: how long building the grammar, re-tokenizing and scoring took, how many types were rebuilt and how many lines re-tokenized (`ScoringEffort` on `WorkingScore`).

**Phase 1**, all nine items:
1. `CaptureContext` resolves groups on demand and `Seal()`s after hydration (`RegexGraph`, `CaptureUnit`).
2. `Tokenizer.MatchFirstGlyph` replaces the partial tokenization of a dynamic slot's text.
3. One thread at a time per `Regex`, in `RegexGraph.TryMatchRegex` and `BuiltRegex.FindPatternCaptures`.
4. `Tokenizer.ResolveDynamic` memoizes slot resolution per (text, filter type) and replays the attempts it recorded into the current line's trace, so incremental re-tokenization sees the same attempts (`TokenizationTrace.Capture`/`Replay`).
5. `CompareTokenizations` and `FindSteals` skip reference-equal lines; `GrammarAgent` finds steals once per evaluation.
6. `MdlScorer` caches the empty-grammar baseline per corpus (the first-score cost that remains is compiling the pattern regexes).
7. `ActiveCorpus` builds the digested text for a new grammar only if the previous analysis was asked for its own (`CorpusAnalyzer.WasDigestRequested`).
8. `TokenizationTrace` keeps attempts as a set.
9. `GlyphGrammar.Build` builds graphs in parallel; `GlyphTypeCache.GetRegexGraph` builds outside its gate.

**Phase 2.**
1. The node graph no longer holds one `EnumMemberNode` per vocabulary member per use: `EnumNode` keeps a shared per-enum member table (`EnumNode.Members`), emits its bricks from it, hydrates from it, and builds member nodes only when asked for its `Children` (`NamedGroupNode.RetainChildren`). `BuiltRegex` keeps its brick list weakly and rebuilds it from the graph when the display asks after it's been collected. `MinifiedRegex` and the pattern-regex text are kept (the latter so scoring never has to rebuild bricks to find it), but shared by content across emitted generations. Heap after the grammar build: 4.1 GB → 1.0 GB.
2. Compiled `Regex` objects are shared by pattern text across emitted generations (`BuiltRegex.Compile`), so a referrer re-emitted with an unchanged pattern doesn't recompile; the pattern regex's anchored fallback compiles only when first needed.
3. Behavioral keys for re-emission: **not done, by decision.** Treating a re-emitted referrer as the same type for `MayDiffer` would leave kept lines holding instances of types the new grammar doesn't have, and the UI's per-type summaries, palettes and graph lookups key on type identity. The cases it would help (marker-only or order-only edits to a shared glyph) are now a few seconds anyway.
4. Per-line work distribution and a match timeout: **not done.** With items 1 to 3 of Phase 1 the slowest sampled line is 27 ms, so neither buys anything measurable yet.

**Phase 3.**
1. The expansion report exists: `list_glyphs sort=size` (agent) and `ConsoleUtility expansion`. On the current grammar it says: `KeywordWithReminder` (8.6 M chars) is `CardAbilityLine` inlined 3 times, which is `KeywordAbility` inlined twice, which inlines `CumulativeUpkeep`, which inlines `Cost` (1.1 M chars each). `Cost` alone accounts for 27 M of the grammar's 105 M regex characters across 13 glyphs; `PermanentPhrase` is inlined in 62 glyphs (14 M); `SpecificTarget` in 69 (8.8 M).
2. The grammar refactors are agent steps, to be evaluated and applied in the app once it runs on this build: not done here. The report's first candidates are (a) `CumulativeUpkeep`'s cost and `KeywordAbility`'s parameters as a `DynamicGlyph` slot with a marker rather than an inlined `Cost`, which would take `KeywordWithReminder`, `CardAbilityLine` and `KeywordsBeforeSemicolon` from megabytes to kilobytes; (b) the `Cost` one-of itself as a dynamic slot wherever a glyph takes one (`ActivatedAbility`, `AlternativeCost`, `TokenHasAbility`, `AndCost`); (c) `PermanentPhrase` and `SpecificTarget` narrowed, or made dynamic, in the glyphs that inline them most. Each is a step for the loop, judged by the usual rules, not an edit to make blind.

### Still open

- The compiled regexes are now the largest fixed cost: 1.1 GB for all 762 types, 9 s to compile them all (done lazily, as types are first tried). The Phase 3 refactors above shrink this directly; so would a Phase 4 matcher.
- The first score after a build compiles the pattern regexes of every type with authored patterns (about 8 s on the sample's first score). Shared by text across generations now, so an evaluate pays it only for types whose pattern text actually changed.
- Both harness runs still end with the CLR's `LoaderAllocatorScout` fatal error at process exit while unloading collectible assemblies. Watch for it in the app, as agreed.

## Appendix: benchmark output

Both harness runs crashed at process exit with "Fatal error. Internal CLR error (0x80131506)" in `LoaderAllocatorScout.Finalize` while unloading the emitted collectible assemblies; those lines are omitted below.

### Baseline, full run (`bench10.txt`)

```
Sample: every 10th card. Cores: 16

=== Grammar build ===
[   31.50s] GlyphGrammar.Default (discover, build graphs, validate)
Types 762, top-level 253, dependent 322; heap after build 4113 MB

=== Regex size (top-level types) ===
Total regex chars (top-level): 32,615,840; all types: 104,733,766
Median 98, p90 206,488, max 8,559,276; StartChars=Any: 23; SpansClauses: 0
Named groups per top-level regex: avg 309, max 18271
  8,559,276 chars 18271 groups 568068 pipes  any=True  spans=False KeywordWithReminder
  2,593,219 chars  6088 groups 189356 pipes  any=True  spans=False CardAbilityLine
  2,540,692 chars  4923 groups 151167 pipes  any=True  spans=False ActivationRestriction
  2,435,953 chars  5971 groups 184077 pipes  any=True  spans=False TokenHasAbility
  2,394,092 chars  5970 groups 184076 pipes  any=True  spans=False ActivatedAbility
  2,286,692 chars  5968 groups 184077 pipes  any=False spans=False AlternativeCost
  1,909,984 chars  4254 groups 133385 pipes  any=False spans=False GainControl
  1,205,115 chars  3155 groups  97273 pipes  any=True  spans=False UnlessPaid
    576,409 chars  1230 groups  37790 pipes  any=False spans=False CastRestriction
    573,333 chars  1413 groups  41487 pipes  any=False spans=False CreateToken
    433,805 chars  1263 groups  37590 pipes  any=True  spans=False BothSubjectsDoComma
    427,494 chars  1263 groups  37590 pipes  any=True  spans=False BothSubjectsDo
    393,726 chars   979 groups  34273 pipes  any=False spans=False PreventDamage
    357,400 chars   917 groups  26845 pipes  any=False spans=False CostModification
    347,593 chars   851 groups  26231 pipes  any=False spans=False WhileConditionStatic
[    1.73s] Compile all top-level ScopeFilling+Anchored regexes
Heap for compiled top-level regexes: 373 MB

=== Corpus load ===
[    0.23s] Load all cards from SQL
All cards 26,228; sample 2,623 docs, 4,805 lines, 392,030 chars, 68,557 words

=== Tokenize sample (parallel, as the app does) ===
[   50.81s] ProcessedDocument x sample (AsParallel)
Retained heap for sample tokenization: 2709 MB = 563.8 KB/line -> full corpus est. 27.1 GB
Coverage 59.2%; tokens/line 2.5; attempts/line 90.8, max attempts 292603
Top-level matches 4,514; avg regex group names per matched type (= CaptureContext dictionary entries per match): 3546

=== Serial per-line timing (find slow lines) ===
Serial total 31.7s; mean 6.60 ms/line; median 2.01; p99 40.8; max 9905
Top 1% of lines account for 48% of time; top 10%: 68%
      9905 ms  292603 attempts  you may have this creature enter as a copy of any creature on the battlefield, except it doesn't copy that cre...
       388 ms   4654 attempts  {2}{u}{u}: until end of turn, target creature you control gets +1/+0, gains "whenever this creature deals comb...
       258 ms    839 attempts  hideaway 4 (when this land enters, look at the top four cards of your library, exile one face down, then put t...
       256 ms    549 attempts  blitz {2}{r} (if you cast this spell for its blitz cost, it gains haste and "when this creature dies, draw a c...
       240 ms   9743 attempts  pack tactics — whenever this creature attacks, if you attacked with creatures with total power 6 or greater th...
       229 ms   3512 attempts  [−7]: you get an emblem with "whenever you cast a creature spell, you may search your library for a creature c...
       229 ms   1994 attempts  corrupted — at the beginning of your upkeep, if an opponent has three or more poison counters, choose a creatu...
       214 ms    122 attempts  enchanted creature gets +1/+1 and has flying, haste, and "{1}: this creature gets +1/+0 until end of turn." it...
       194 ms   2422 attempts  [−7]: you get an emblem with "at the beginning of your end step, you may search your library for a creature ca...
       179 ms    769 attempts  when this creature enters, you may search your library for an artifact card, exile it, then shuffle.
       170 ms    867 attempts  at the beginning of each player's upkeep, that player may choose any number of tapped creatures without flying...
       167 ms    382 attempts  equipped creature gets +2/+0 and has "whenever this creature attacks, it gains your choice of trample or death...

=== Per-type regex cost at clause starts (top-level ScopeFilling regex only) ===
Sum of per-type regex time 0.5s over 282,345 tries; top 15 types:
       126 ms    7,231 tries     17.4 us/try  len 8,559,276  KeywordWithReminder
        59 ms    7,231 tries      8.1 us/try  len 2,593,219  CardAbilityLine
        29 ms    7,231 tries      3.9 us/try  len 2,394,092  ActivatedAbility
        27 ms    7,231 tries      3.8 us/try  len  433,805  BothSubjectsDoComma
        25 ms    7,231 tries      3.5 us/try  len  427,494  BothSubjectsDo
        22 ms    7,231 tries      3.1 us/try  len  207,734  SubjectDoes
        22 ms    7,231 tries      3.0 us/try  len  283,717  Reveal
        19 ms    7,231 tries      2.7 us/try  len 1,205,115  UnlessPaid
        12 ms    7,231 tries      1.7 us/try  len    5,054  DelayedEffect
        12 ms    7,231 tries      1.7 us/try  len 2,435,953  TokenHasAbility
        12 ms    7,231 tries      1.6 us/try  len    2,504  LevelUpRestriction
        12 ms    7,231 tries      1.6 us/try  len 2,540,692  ActivationRestriction
        11 ms    5,772 tries      1.9 us/try  len  357,400  CostModification
        11 ms    7,231 tries      1.5 us/try  len   15,878  EffectIfCondition
        11 ms    7,231 tries      1.5 us/try  len       78  EndOfCombatEffect
Top 15 types = 87% of per-type time

=== Score sample ===
[    3.49s] MdlScorer.Score (incl. empty-grammar baseline retokenize)
[    0.02s]   of which: baseline retokenize with empty grammar (serial)
[    0.01s]   of which: grammar.ToDefinition()
Score 597,933 bits, coverage 59.2%

=== Evaluate-step simulation ===
[    0.13s] FromDefinition(unchanged) - should reuse every type
  new types emitted: 0
[    0.12s] FromDefinition(+1 trivial glyph)
  new types emitted: 1
[    0.06s] TokenizationChange.MayDiffer over sample lines
  lines that may differ: 0 of 4,805
[    0.04s] Retokenize sample incrementally (+1 glyph)
Most-referenced dependent glyph: PermanentPhrase (39 direct referrers)
[   24.92s] FromDefinition(PermanentPhrase gets a new marker)
  new types emitted (re-emitted referrer closure): 161
[    3.83s] TokenizationChange.MayDiffer over sample lines
  lines that may differ: 4,091 of 4,805
[   61.23s] Retokenize sample incrementally (tweak)
```

### Prototype without the lock, full run (`bench10-spike.txt`)

```
Sample: every 10th card. Cores: 16

=== Grammar build ===
[   32.07s] GlyphGrammar.Default (discover, build graphs, validate)
Types 762, top-level 253, dependent 322; heap after build 4113 MB

=== Regex size (top-level types) ===
Total regex chars (top-level): 32,615,840; all types: 104,733,766
Median 98, p90 206,488, max 8,559,276; StartChars=Any: 23; SpansClauses: 0
Named groups per top-level regex: avg 309, max 18271
  8,559,276 chars 18271 groups 568068 pipes  any=True  spans=False KeywordWithReminder
  2,593,219 chars  6088 groups 189356 pipes  any=True  spans=False CardAbilityLine
  2,540,692 chars  4923 groups 151167 pipes  any=True  spans=False ActivationRestriction
  2,435,953 chars  5971 groups 184077 pipes  any=True  spans=False TokenHasAbility
  2,394,092 chars  5970 groups 184076 pipes  any=True  spans=False ActivatedAbility
  2,286,692 chars  5968 groups 184077 pipes  any=False spans=False AlternativeCost
  1,909,984 chars  4254 groups 133385 pipes  any=False spans=False GainControl
  1,205,115 chars  3155 groups  97273 pipes  any=True  spans=False UnlessPaid
    576,409 chars  1230 groups  37790 pipes  any=False spans=False CastRestriction
    573,333 chars  1413 groups  41487 pipes  any=False spans=False CreateToken
    433,805 chars  1263 groups  37590 pipes  any=True  spans=False BothSubjectsDoComma
    427,494 chars  1263 groups  37590 pipes  any=True  spans=False BothSubjectsDo
    393,726 chars   979 groups  34273 pipes  any=False spans=False PreventDamage
    357,400 chars   917 groups  26845 pipes  any=False spans=False CostModification
    347,593 chars   851 groups  26231 pipes  any=False spans=False WhileConditionStatic
[    1.78s] Compile all top-level ScopeFilling+Anchored regexes
Heap for compiled top-level regexes: 373 MB

=== Corpus load ===
[    0.21s] Load all cards from SQL
All cards 26,228; sample 2,623 docs, 4,805 lines, 392,030 chars, 68,557 words

=== Tokenize sample (parallel, as the app does) ===
[   19.30s] ProcessedDocument x sample (AsParallel)
Allocated during tokenization: 208.3 GB; gen2 GCs: 28
Retained heap for sample tokenization: 1809 MB = 376.6 KB/line -> full corpus est. 18.1 GB
Coverage 59.2%; tokens/line 2.5; attempts/line 5.6, max attempts 413
Top-level matches 4,514; avg regex group names per matched type (= CaptureContext dictionary entries per match): 3546

=== Serial per-line timing (find slow lines) ===
Serial total 3.1s; mean 0.64 ms/line; median 0.38; p99 4.0; max 65
Top 1% of lines account for 13% of time; top 10%: 41%
        65 ms     29 attempts  whenever a player casts a spell, that player creates x 1/1 green squirrel creature tokens, where x is the numb...
        26 ms    237 attempts  corrupted — at the beginning of your upkeep, if an opponent has three or more poison counters, choose a creatu...
        23 ms    101 attempts  hideaway 4 (when this land enters, look at the top four cards of your library, exile one face down, then put t...
        14 ms    128 attempts  creatures you control have "{t}: target player mills x cards, where x is this creature's toughness."
        11 ms    413 attempts  you may have this creature enter as a copy of any creature on the battlefield, except it doesn't copy that cre...
        11 ms    135 attempts  when this creature enters, create a green treefolk warrior creature token with "this token's power and toughne...
        10 ms    156 attempts  when this creature enters, each player chooses an artifact, a creature, an enchantment, and a planeswalker fro...
        10 ms     51 attempts  whenever you cast a spell, if that spell was kicked, put a +1/+1 counter on hallar, then hallar deals damage e...
        10 ms     69 attempts  equipped creature gets +1/+1 and has "whenever this creature deals combat damage, untap it and you gain 2 life...
         9 ms     31 attempts  enchanted creature has "{t}: create a token that's a copy of this creature, except it has haste. exile that to...
         9 ms     91 attempts  when this creature dies, if it had a revival counter on it, return it to the battlefield with one fewer reviva...
         9 ms     98 attempts  you may reveal this card from your opening hand. if you do, when each opponent casts their first spell of the ...

=== Per-type regex cost at clause starts (top-level ScopeFilling regex only) ===
Sum of per-type regex time 0.5s over 282,345 tries; top 15 types:
       126 ms    7,231 tries     17.4 us/try  len 8,559,276  KeywordWithReminder
        61 ms    7,231 tries      8.5 us/try  len 2,593,219  CardAbilityLine
        37 ms    7,231 tries      5.1 us/try  len 2,394,092  ActivatedAbility
        27 ms    7,231 tries      3.8 us/try  len  433,805  BothSubjectsDoComma
        26 ms    7,231 tries      3.5 us/try  len  427,494  BothSubjectsDo
        23 ms    7,231 tries      3.2 us/try  len  207,734  SubjectDoes
        21 ms    7,231 tries      3.0 us/try  len  283,717  Reveal
        20 ms    7,231 tries      2.7 us/try  len 1,205,115  UnlessPaid
        12 ms    7,231 tries      1.7 us/try  len    5,054  DelayedEffect
        12 ms    7,231 tries      1.6 us/try  len 2,540,692  ActivationRestriction
        12 ms    7,231 tries      1.6 us/try  len 2,435,953  TokenHasAbility
        12 ms    7,231 tries      1.6 us/try  len    2,504  LevelUpRestriction
        11 ms    5,772 tries      1.9 us/try  len  357,400  CostModification
        11 ms    7,231 tries      1.5 us/try  len       78  EndOfCombatEffect
        11 ms    7,231 tries      1.5 us/try  len   15,878  EffectIfCondition
Top 15 types = 87% of per-type time

=== Score sample ===
[    3.56s] MdlScorer.Score (incl. empty-grammar baseline retokenize)
[    0.02s]   of which: baseline retokenize with empty grammar (serial)
[    0.01s]   of which: grammar.ToDefinition()
Score 597,933 bits, coverage 59.2%

=== Evaluate-step simulation ===
[    0.19s] FromDefinition(unchanged) - should reuse every type
  new types emitted: 0
[    0.14s] FromDefinition(+1 trivial glyph)
  new types emitted: 1
[    0.01s] TokenizationChange.MayDiffer over sample lines
  lines that may differ: 0 of 4,805
[    0.00s] Retokenize sample incrementally (+1 glyph)
[    0.39s]   CompareTokenizations (evaluate report, unchanged corpus)
[    0.10s]   FindSteals x1 (evaluate runs it 2-3 times)
[    0.71s]   MdlScorer.Score again (the 'after' score)
Most-referenced dependent glyph: PermanentPhrase (39 direct referrers)
[   25.60s] FromDefinition(PermanentPhrase gets a new marker)
  new types emitted (re-emitted referrer closure): 161
[    4.72s] TokenizationChange.MayDiffer over sample lines
  lines that may differ: 3,829 of 4,805
[   16.70s] Retokenize sample incrementally (tweak)
```

### Baseline, concurrency and per-line memory (`tok-baseline.txt`)

```
Sample: every 10th card. Cores: 16

=== Grammar build ===
[   33.48s] GlyphGrammar.Default
[    2.41s] Compile top-level regexes

=== Regex concurrency micro-test (same Regex object from 16 threads) ===
  KeywordWithReminder    len 8,559,276: serial      34 ms,       0.0 KB/call   |  16 threads    1322 ms,    1099.0 KB/call
  ActivatedAbility       len 2,394,092: serial       6 ms,       0.0 KB/call   |  16 threads    1290 ms,     933.8 KB/call
  CreateToken            len   573,333: serial       1 ms,       0.0 KB/call   |  16 threads      12 ms,      58.3 KB/call
  EndOfCombatEffect      len        78: serial       2 ms,       0.0 KB/call   |  16 threads       1 ms,       0.1 KB/call

=== Corpus ===
[   55.44s] Tokenize sample A (parallel)
  2,623 docs, 4,805 lines; allocated 498.2 GB, gen2 GCs 55; retained delta 2773 MB = 577.1 KB/line
[   44.76s] Tokenize sample B (parallel) - delta is per-line cost only
  2,623 docs, 4,741 lines; allocated 532.7 GB, gen2 GCs 52; retained delta 1205 MB = 254.2 KB/line
[   31.44s] Tokenize sample A lines serially (warm)
```

### Prototype with the lock, concurrency and per-line memory (`tok-spike.txt`)

```
Sample: every 10th card. Cores: 16

=== Grammar build ===
[   31.11s] GlyphGrammar.Default
[    0.92s] Compile top-level regexes

=== Regex concurrency micro-test (same Regex object from 16 threads) ===
  KeywordWithReminder    len 8,559,276: serial      36 ms,       0.0 KB/call   |  16 threads    1082 ms,     871.6 KB/call
  ActivatedAbility       len 2,394,092: serial       4 ms,       0.0 KB/call   |  16 threads     991 ms,     933.8 KB/call
  CreateToken            len   573,333: serial       1 ms,       0.0 KB/call   |  16 threads      14 ms,      36.1 KB/call
  EndOfCombatEffect      len        78: serial       2 ms,       0.0 KB/call   |  16 threads       1 ms,       0.1 KB/call

=== Corpus ===
[    1.88s] Tokenize sample A (parallel)
  2,623 docs, 4,805 lines; allocated 7.7 GB, gen2 GCs 3; retained delta 1956 MB = 407.1 KB/line
[    0.64s] Tokenize sample B (parallel) - delta is per-line cost only
  2,623 docs, 4,741 lines; allocated 3.2 GB, gen2 GCs 3; retained delta 99 MB = 20.9 KB/line
[    2.40s] Tokenize sample A lines serially (warm)
```
