# The grammar loop's speed after the performance commits: what the first Release session showed, and what to do next

**Date:** 2026-10-21
**Reviewed:** commits `972b8b0` (engine fixes, Phases 0 to 3.1 of PERFORMANCE-PLAN.md), `a4d011a` (scoring re-matches with plain groups) and `939fb87` (equivalent types in incremental re-tokenization); the agent session of 15:21 to 15:47 today on the `source` workspace (Release build of 15:13, i.e. all three commits); and a replay of that session's slowest step on each build.
**Machine:** 32 logical cores, 64 GB RAM (the plan's measurements quote 16 cores and 128 GB; the figures below are from this machine).

---

## Summary

1. **The session's time went where the plan said it would, at a scale the plan didn't foresee.** Of the session's 26 minutes, 20 were spent inside tool calls: `evaluate` 958 s over 8 calls, `tokenize` 221 s over 12 calls (217 s of it in the three calls that carried a draft glyph), everything else under a minute. The 16-minute step was one dependent glyph (`ShortNameObject`, a new alternative of `PermanentPhrase`) drafted three times with `tokenize` and three times with `evaluate`: six full rebuilds of the 227 types that inline `PermanentPhrase`, three re-tokenizations of 42,000 of the corpus's 47,865 lines, three scores.

2. **The engine commits are a real and large improvement, and none of them is the regression.** Replaying the same step on the pre-optimization code (`577c857`) builds the session's grammar in 237 s with a 27.5 GB heap; the current code builds it in 30 s with 6.7 GB, and re-tokenizes the step's 42,000 lines in 31 s. The baseline replay did not get through its first full tokenization before the machine ran out of memory. The equivalence commit (`939fb87`) measured neutral on this step, for a reason given below.

3. **What changed is the grammar.** Since the plan's baseline the grammar's regex text has grown from 105 M to 680 M characters (991 types), and its largest glyph from 8.6 M to 38.5 M characters with 69,505 named groups. `Cost` alone, inlined in 21 glyphs, now accounts for 254 M characters, 37% of the whole. Every cost in the engine that the plan measured as "seconds" is proportional to that text: building graphs, compiling regexes, matching (a `Match` allocates arrays sized by the group count), scoring's pattern regexes, and the memory each rebuilt generation pins. The plan's acceptance targets were set against a grammar a sixth of this size.

4. **The app ran the step 1.5 to 2.3 times slower than the harness does on the same machine, most likely for memory.** The harness evaluates the step in about 111 s (build 26 s, compile the rebuilt regexes 32 s, re-tokenize 31 s, score 22 s); the app reported 157 s, 261 s and 229 s, its grammar builds slowing from 33 s at the session's start to 86 s by its end while a fresh harness build takes 26 to 30 s. One evaluate takes the harness's heap from 21 GB to 43 GB; the workbench keeps several such generations alive, the machine has 64 GB, the page file peaked at 15 GB today, and the baseline replay was killed by the system for low memory. Not proven, but nothing else in the code accounts for a build growing 2.6 times over a session.

5. **The plan.** Bound memory and make the app's own cost line show it (days); then take the plan's Phase 4, a compositional matcher that stops inlining, as the one change that makes shared-glyph edits cheap regardless of grammar size (one to two weeks); in parallel, have the loop see regex size and refactor the worst inliners (`Cost` first) as ordinary agent steps, which shrinks every number above by most of an order of magnitude on the current engine. Details in order below.

---

## 1. The session, from its log

Source: the agent's Claude Code session transcript (`367c4a2e…jsonl` under the `Glyphotype\agent` project folder in `~/.claude/projects`), tool-call start and result timestamps, and the `Evaluating took …` line each evaluate returns.

### Where the time went

| Tool | Calls | Time | Notes |
|---|---|---|---|
| `evaluate` | 8 | 958 s | three of them 157 to 261 s |
| `tokenize` | 12 | 221 s | 217 s in the three calls with a `source` (each builds the grammar) |
| `search_lines` | 10 | 32 s | 31 s in one call, waiting on the re-score after the first `apply` |
| everything else | 29 | 6 s | |
| **Tools in all** | | **1,217 s** of the session's 1,559 s | the model's own thinking and API round trips are the rest |

### The 16-minute step (15:28:15 to 15:43:56, 941 s)

| Time | Call | Wall | Build | Types rebuilt | Lines re-tokenized | Tokenize | Score |
|---|---|---|---|---|---|---|---|
| 15:28:15 | `tokenize` with draft 1 | 82 s | all of it | 227 | | | |
| 15:29:49 | `tokenize` with draft 2 | 70 s | all of it | 227 | | | |
| 15:31:09 | `tokenize` with draft 3 | 65 s | all of it | 227 | | | |
| 15:32:22 | `evaluate` draft 3 | 157 s | 0.5 s (reused) | 227 | 42,242 of 47,865 | 120 s | 37 s |
| 15:35:22 | `evaluate` draft 4 | 261 s | 81 s | 227 | 42,135 | 156 s | 25 s |
| 15:40:00 | `evaluate` draft 5 | 229 s | 86 s | 227 | 42,135 | 115 s | 28 s |
| 15:43:56 | `apply` | 1 s | | | | | |

864 s of the 941 s is tool time: 385 s building the same 227 types six times over, 391 s re-tokenizing, 90 s scoring. The agent was iterating on `ShortNameObject`'s pattern (which words it must not match) and each iteration cost a full rebuild of everything that inlines `PermanentPhrase`, because `ShortNameObject` is an alternative of it. `tokenize` is described to the agent as "Instant: nothing is scored", which is true of the scoring and not of the build.

For comparison, the session's other steps: the `CreatureType` vocabulary edit (499 types rebuilt, 13,563 lines) took 118 s the first time and 30 s when re-evaluated with the grammar already built; the leaf-glyph `ManaSpendingRestriction` step (9 types, 103 lines) took 13.7 s, 13 s of it scoring; the `Subject` edit (20 types, 26,851 lines) took 56 s.

---

## 2. The three commits, assessed

### `972b8b0`: the Phase 0 to 3.1 engine work

Replaying the session's grammar on the pre-optimization code and on the current code, same machine, Release, server GC:

| Full corpus unless noted | `577c857` (before) | `a4d011a` | `939fb87` (current) |
|---|---|---|---|
| Build the session's grammar (712 glyphs, 991 types) | 237 s, heap 27.5 GB | 28.6 s, 6.7 GB | 30.1 s, 6.7 GB |
| Tokenize the whole corpus (47,865 lines) | not reached (out of memory at 45 GB, every-3rd-document sample) | 65.9 s | 68.5 s |
| Score, first / again | | 23.2 s / 10.3 s | 23.5 s / 10.3 s |
| Build the step's grammar (144 glyphs, 227 types rebuilt) | | 25.1 s | 26.1 s |
| Compile the 227 rebuilt types' regexes | | 32.4 s | 31.9 s |
| `MayDiffer` over every line, serial / parallel | | 6.4 s / 1.1 s | 3.0 s / 0.5 s |
| Re-tokenize 42,135 lines | | 40.0 s | 31.3 s |
| Score the result, first / again | | 22.4 s / 10.5 s | 22.1 s / 10.3 s |
| Heap at the end | | 43.3 GB | 43.3 GB |

Everything the commit claims holds on this grammar: lazy capture lookup, first-glyph dynamic resolution, one thread per `Regex`, memoized dynamic slots, parallel graph building, shared vocabulary member tables, regexes and pattern text shared by content, the effort line on `evaluate`. The effort line is what made this review possible. No parse changed (the fingerprint and dump checks the commit describes, and the tests).

Two things it leaves, both proportional to regex size and both visible in the replay: a tokenization of the corpus still allocates 100 to 143 GB (a `Match` of a regex with tens of thousands of groups allocates arrays sized by that count on every successful regex match, hydration or not), and compiling the rebuilt types' regexes costs 32 s and about 6 GB per rebuilt generation, which the app pays lazily inside "tokenized … in N s" and keeps for as long as any line holds an instance of those types.

### `a4d011a`: pattern regexes with plain groups

Correct and effective: the warm score is 10 s on the full corpus. The first score after a build still compiles a pattern regex for every rebuilt type with authored patterns, 12 s here. That cost is a design choice (re-matching each token with a third copy of its regex to locate open-ended pattern text), not a bug; see A4 below for the way out.

### `939fb87`: equivalent types

The idea is sound: a glyph rebuilt only because something it inlines changed tokenizes a line the same unless the changed part could match in that line. Measured on this step, it did nothing, and the reasons are structural:

- **It bails out whenever any changed content holds `\b`, `^`, `$`, a lookaround or a backreference** (`TokenizationChange.Normalize` returns null, and `FindEquivalents` then returns no equivalents at all). `ShortNameObject`'s pattern has `\b` and a lookahead; so do `ShortName`, `ShortNameSubject` and most pattern glyphs. All three of the session's big steps went through the old path, every referrer treated as removed and added. The bail-out is over-cautious: the attempts a trace records carry the exact windows the engine matched in, and `Regex.Match(text, index, length)` gives anchors and `\b` the window's edges just as the enclosing match saw them, so contents could be tested per attempt window instead of per line and the normalizer could keep those constructs.
- **When it does apply, its `_changedContents` are unanchored, interpreted `Regex` objects, run from every tokenization thread with no lock** (`MayDiffer`, line 103). The same commit's author found and fixed exactly this pattern in `RegexGraph.TryMatchRegex`: a `Regex` caches one runner, and concurrent callers allocate a fresh one per call. For a changed glyph the size of `PermanentPhrase` (hundreds of kilobytes of content) that is the runner-thrash of the plan's finding 3, at a smaller scale.
- **A broadly matching changed glyph keeps few lines anyway.** `PermanentPhrase` matches "it", "this creature" and so on in most lines, so even with the check working, most lines are re-tokenized.
- **What `evaluate` saves, `apply` pays back**: kept lines hold superseded types, so the workbench's own score of an applied definition runs `Refresh` and re-tokenizes those lines after all. Roughly half the agent's evaluates are followed by an apply.
- It adds a second notion of "same type" (equivalence by name and regex text) beside the emitter's reuse by key, which every reader of `TokenizationChange` now has to hold in mind.

Verdict: neutral on this session, net complexity for a case (a leaf-pattern edit to a narrow dependent) that will be cheap anyway once the structural work below is done. Worth reverting unless the two defects above are fixed, and even then only if a measured step shows it earning its keep.

---

## 3. The root cause: the grammar's regex text grew 6.5 times in a day

The expansion report on the session's grammar (`ConsoleUtility expansion-def`, added with this review):

| | Plan baseline (`577c857`, 762 types) | Session's grammar (991 types) |
|---|---|---|
| Regex text, all types | 104.7 M chars | 680.2 M chars |
| Largest regex | `KeywordWithReminder` 8.56 M chars, 18,271 groups | `KeywordWithReminder` 38.5 M chars, 69,505 groups |
| `UnlessPaid` | 1.2 M | 17.4 M |
| `ActivationRestriction` | 2.5 M | 13.7 M |
| `CardAbilityLine` | 2.6 M | 11.9 M |
| `TokenHasAbility` | 2.4 M | 11.4 M |
| `OptionalPayCost` | under 0.35 M | 17.3 M |
| `CastWithAdditionalCost` | under 0.35 M | 22.9 M |
| `Cost`, per inlining | 1.1 M | 5.2 M |
| `Cost`, across the grammar | 27 M in 13 glyphs | 254 M in 21 glyphs |

`Cost` is the single largest driver: its own regex grew nearly five times (among its additions since the baseline it gained `Mill`, which inlines `ForEach` and the thirteen-way `CountedThing`), and it is inlined up to six times in one glyph (`KeywordWithReminder` nests `CardAbilityLine` three times, which nests `KeywordAbility` twice, which nests `CumulativeUpkeep`, which nests `Cost`). After `Cost` come the `CumulativeUpkeep`/`KeywordAbility`/`CardAbilityLine` chain (over 100 M chars each), a `OneOf` of characteristic phrases inlined in 159 glyphs (97 M), `Permanents` in 79 glyphs (74 M), and the `EffectDuration` family (about 60 M each for three nested one-ofs).

The engine's costs scale with this text, and the step's costs with the share of it that a change re-emits. A change to `PermanentPhrase` re-emits 384 M of the 680 M characters. Build time, compile time, match allocation, pattern-regex compile and per-generation memory all follow. The plan's targets ("evaluate on a shared dependent under 60 s") were reached on the grammar the plan measured, and lost again to growth within the day.

Nothing in the loop pushes back on that growth. The expansion report exists (`list_glyphs sort=size`) but no step reads it, the step rules speak of bits and words only, and `evaluate`'s cost line reports seconds without saying why. The agent that added `Mill` to `Cost` had no way to see it multiply the grammar's text by five.

---

## 4. Why the app is slower than the harness

The harness evaluates the step in about 111 s; the app took 157 s, 261 s and 229 s for the same work, and its grammar builds went 33 s (first evaluate of the session), 81 s, 86 s where the harness builds a fresh grammar in 26 to 30 s every time. Memory is the likeliest cause:

- A rebuilt generation pins about 6 GB of compiled regexes (227 types' anchored and scope-filling regexes, held by their graphs, held through `GlyphTypeCache` by every line holding an instance of those types) plus 2 to 3 GB of documents. The harness went from 21 GB before the step to 43 GB after it.
- The workbench holds up to two recent scores, the latest working score, the committed documents, the last evaluation's before and after, and `ActiveCorpus` holds its current and building analyzers. Three or four generations of this grammar is more than the machine's 64 GB; the page file's peak use today was 15 GB, and the baseline replay was stopped by the system for low memory at a 45 GB working set.
- Server GC over a 40 to 60 GB heap: the basis build alone triggered 23 gen-2 collections in the harness.

Not proven from the log, because the app doesn't record its memory. The first item of the plan is to make it.

---

## 5. Plan

### Phase A: this week, engine and app, no parse changes

1. **Make memory visible and bounded.** Add the process's heap and working set to `evaluate`'s cost line and the agent pane's stopwatches. Drop `Documents` from a recent score when it's evicted and from `_lastEvaluated` once applied or superseded; keep the committed documents and the latest working score only. Let a grammar generation release its compiled regexes when nothing but the cache holds them: move the `Regex` objects out of each `BuiltRegex`'s `Lazy` fields into the by-text cache alone, bounded by total pattern length, so a superseded generation's regexes go even while its lines live. Expected: app timings match the harness's (about half), and a session no longer slows as it goes.
2. **Build without the churn.** Building 144 graphs allocates 58 GB to produce 384 M characters: the brick list, `string.Concat`, the `EscapedSpace` replace, the pattern text and the typed-group scan each copy or walk the text. Emit the text once during the walk, computing pattern text and typed groups in the same pass. Profile the 11 to 12 s `GlyphGrammar` constructor spends with every graph already built (validation walks and the closed generic types' graphs are the candidates). Expected: build 26 s to about 10 s, and far fewer gen-2 collections.
3. **Draft `tokenize` without the whole build.** A `tokenize` or `explain_mismatch` with a draft builds the whole grammar (65 to 82 s here) to tokenize five lines. Build graphs on first use by the tokenizer instead of eagerly in `GlyphGrammar.Build`, skipping validation for re-emitted types whose definitions didn't change, with start characters derived from a type's first nib rather than its graph. Until then, say in the tool's description that a draft costs a grammar build, and report the build time in its output, so the agent batches its drafts.
4. **Score what changed.** Memoize each line's encoding choices (contexts named, symbols chosen, fixed bits) on the `ProcessedLine`, so a score of a step that kept 46,000 lines re-encodes only the rest and merges counts. Locate open-ended pattern text from the capture traces (the authored pattern nodes are known, and the traces record where every group matched) instead of re-matching each token against a third regex, which removes the pattern-regex compile (12 s per rebuilt generation) and `FindPatternCaptures` entirely. Expected: 22 s to a few seconds for a shared-glyph step, about a second for a leaf step.
5. **Measure lock contention** in parallel tokenization with `dotnet-counters` (the per-`Regex` lock now serializes the eight largest regexes across 32 threads). If it shows, keep a small pool of `Regex` copies for the largest patterns only.
6. **Decide `939fb87`**: fix its two defects (test contents per attempt window with `Match(text, index, length)`, which also lets the normalizer keep `\b`, anchors and lookarounds; run the content regexes under the same one-thread-per-`Regex` rule) and measure it on a replayed step, or revert it.

### Phase B: one to two weeks, the structural change

**The plan's Phase 4: a compositional matcher.** Composite glyphs match their children by the children's own graphs, memoized by (node, position); only leaves (literal text, patterns, vocabularies) are regexes. Regex text becomes linear in the grammar (a few megabytes rather than 680 M), group counts and `Match` allocations shrink with it, a rebuilt generation holds kilobytes, a change to `PermanentPhrase` re-emits 227 types in milliseconds, and dynamic slots and one-of alternatives become ordinary choice points that can backtrack (closing the plan's Phase 2.5 gap). The `CaptureTrace` tree, the formatted regex display (built from the node graph) and the tests are unchanged; the dump diff is the safety net. This is the only change that makes a shared-dependent edit cheap whatever the grammar does next. Everything in Phase A still pays: A1 and A4 are needed regardless, A2 and A3 become moot.

### Phase C: now, the grammar and the loop

1. **Let the loop see size.** Add the grammar's regex text and its change to `evaluate`'s cost line ("regex text 680 M chars, +42 M; Cost inlined in 21 glyphs"), and make a step that grows it by more than a few percent a rule violation like a word steal: something the agent must justify. Put the expansion report's top entries in `overview`.
2. **Refactor the worst inliners as agent steps, person-directed.** `Cost` as a `DynamicGlyph` slot with an `ICost` marker in its 21 users (254 M chars); the `CumulativeUpkeep`/`KeywordAbility`/`CardAbilityLine` chain; the characteristic-phrase one-of inlined in 159 glyphs; `Permanents` (79 users); the `EffectDuration` family. Dynamic slot resolution is memoized per text now, so the match-time cost is small. Each is an ordinary evaluated step; together they should take the regex text well under 100 M characters, and every Phase A number with it, on the engine as it is.
3. **Guide the agent.** Say that `tokenize` with a draft rebuilds the grammar and costs what `evaluate`'s build does; that a dependent inlined widely is expensive to iterate on, so a draft pattern is better tried as a throwaway top-level glyph against sample lines first; and that `list_glyphs sort=size` is where to look before adding to `Cost`, `Effect`, `PermanentPhrase` or `Subject`.

### Phase D: operating the app until A1 lands

Restart the app between sessions, and watch its working set; at this grammar size a session of a dozen shared-glyph steps exceeds 64 GB.

---

## Appendix: the replay harness

`ConsoleUtility replay <history.json> <step> [every] [working.json]` replays one step of a workspace's history as the workbench evaluates it: builds the step's "before" definition and tokenizes and scores the corpus with it, then builds the "after" (the next step's "before", or the working definition for the last step), compiles the rebuilt types' regexes, runs `TokenizationChange` and `MayDiffer`, re-tokenizes, scores, and refreshes, timing each part, counting allocations and gen-2 collections, and reporting the heap after each. `ConsoleUtility expansion-def <definition.json> [count]` prints the expansion report for a grammar built from a definition. Both are what the figures above were taken with; the full outputs are in the session's temp folder (`replay/*.txt`).
