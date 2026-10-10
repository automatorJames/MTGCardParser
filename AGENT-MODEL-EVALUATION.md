# Grammar agent: model evaluation and run guidance

What one head-to-head comparison of models in the Grammar agent role taught us, kept so later runs (and later model
choices) start from it. The comparison: Haiku, Sonnet and Opus, all at medium effort, each in its own scratch
workspace copied from the same MTGGlyphs grammar, given the same instructions, guidance and settings (a 10-step
round, 12 bits per word covered), on 2026-10-10.

## Measure the right things

The app's headline numbers - total bits and coverage - **don't separate a deep grammar from a shallow one**, and
in this comparison they ranked the models the wrong way round. A frame with `[AllowUnmatched]` slots counts its own
literal words as covered while holding the rest unresolved, so a round that adds frames and a round that models text
through can post the same coverage gain. Judge a round by:

| Metric | Why |
|---|---|
| **Fully covered lines** | A line is done only when every word is inside a match and nothing is held. The truest measure of progress toward the goal. |
| **Words held unresolved** | Text placed inside a frame but not modeled - deferred work. A round that raises it a lot has mostly built scaffolding. |
| Share of gained words on lines now fully covered | Separates "finished these lines" from "nudged many lines". |
| Coverage and bits | Still the score, and still what the step rules gate on - just not enough alone. |
| Tool calls, wall-clock time, tokens | The cost side. The chat pane's stats give tokens; use them when comparing models for cost. |

The overview, `evaluate`, `apply` and the check-in now report fully covered lines and held words, so later
comparisons can read them straight from the tools.

## The comparison

Corpus: 439 cards, 631 lines, 8,648 words. Starting point: 90,845 bits, 47.90% coverage, 367 fully covered lines,
1,540 held words.

| | Haiku | Sonnet | Opus |
|---|---|---|---|
| Steps applied (of 10) | 1 | 9 | 10 |
| Bits | −101 | **−914** | +28 |
| Words covered | 0 | **+330** | +310 |
| Of those, on lines now fully covered | – | 80 (24%) | **280 (90%)** |
| Newly fully covered lines | 0 | 8 | **22** |
| Words held unresolved | ±0 | **+814** | −95 |
| Lines with nothing covered (127 at start) | 127 | **78** | 115 |
| Tool calls / wall-clock | ~29 / ~2 min | ~45 / ~2.5 min | ~70 / ~10 min |

By headline numbers Sonnet won outright. By fully covered lines Opus did nearly three times as well.

### Opus: deep, compositional, compliant (8.5/10)

- Worked **bottom-up**: every step added a reusable building block (`CharacteristicOf`, `NumberOf`, `EqualTo`,
  `CountersOn`, `ThisPeriod`, `ArithmeticAmount`, `RemovalVerb`, `ObjectPossessive`) or widened an existing
  glyph to use them (`Permanents` taking `PermanentKind`, which fixed "swamps" and "islands" everywhere).
- Treated the per-word cap as a design signal: when a draft came in at 14.4 bits per word, it found a shared form
  (merging "its controller" into `ThatCardsController` through the shared possessive) instead of forcing it through.
- Reduced held text, because its glyphs resolved text that existing frames had been holding.
- Ran exactly to the check-in, and wrote one precise, actionable journal entry.
- Semantic slips worth reviewing in its output: an alternative typed more broadly than its meaning (`Cost`'s exile
  alternative accepts any `DestroyTarget`, so "destroy …" parses as a cost), a back-reference widened to "the
  creature", untyped dynamic slots in `ThenSequence`.
- Cost: the slowest and most expensive per step - about 4× Sonnet's wall-clock, with large source payloads per call.

### Sonnet: fast, top-down, scaffold-heavy (6.5/10)

- Worked **top-down**: found the single most valuable move of the whole comparison - a general "when/whenever …, …"
  trigger frame and an "if …, …" frame, which together saved 1,090 bits because existing effect glyphs began
  resolving inside lines that had been entirely unmatched. Opus never found this.
- But 7 of its 9 steps were frames with held slots: 76% of its gained words sit on lines still partly uncovered, and
  about 25 lines gained a single word ("whenever").
- Left design debt: frames that overlap existing families (`ConditionalEffect` beside `IfOutcome`; `EnchantedStatic`
  taking `CardType` where `EnchantedPermanent` takes `PermanentKind`; a generic `TriggeredAbility` beside the
  specific trigger glyphs), and a hard-coded literal where a vocabulary already existed (`DefinedPowerToughness`).
- Did real modeling when it modeled (its split-damage glyph matched Opus's).
- Stopped one step short, misreading the status line "step 9 of 10 before checking in" as a reason to pause.
- Recorded a false engine "fact" in the journal (that `Nib.This` before `'s` needs `Joiner.None`) from a
  misdiagnosed failure - journals can carry an agent's misconceptions forward.
- Turned down a step the rules would have allowed (a hollow "the next time … instead" frame at ~1.4 bits per word),
  which was good judgment.

### Haiku: diagnoses, doesn't act (1.5/10)

- Applied one bit-saving refactor (−101 bits, no coverage), then stopped at step 1 of 10: "the remaining targets
  are expensive". It had never evaluated them against the cap, and the brief says expensive text is no reason to
  stop.
- Spent its round on glyphs costing more than they save - the top of `list_glyphs`' then-default sort by net bits -
  rather than on uncovered text.
- Its diagnosis was right: it named the exact fix (`Permanents` needs land types) that Opus later made. Haiku
  can find problems; it doesn't push changes through.

## Insights that generalize

1. **Headline metrics reward scaffolding.** Frames are the cheapest coverage under both the score and the per-word
   step cap, because their literal words count and their held text is priced as unmatched text was. Without
   held-word and fully-covered-line tracking, a frame-heavy model looks best.
2. **The strategies complement each other.** Sonnet's frames and Opus's building blocks barely overlapped: merging
   both (Opus winning conflicts) gave 89,795 bits, 55.24% coverage and 394 fully covered lines - better on every
   metric than either alone. Sonnet's frames hold exactly the text Opus-style building blocks resolve.
3. **Models stop early unless the harness insists.** Two of three stopped before their check-in. The tools' wording
   and the loop itself have to make "keep going" the default.
4. **Default sort orders steer weaker models.** Whatever a list shows first is what a weaker agent works on.
5. **The per-word cap does useful work at 12.** It bound once (Opus at 14.4) and produced a better design; it never
   constrained the frame-heavy run, since frame words are cheap.
6. **Journals need the same skepticism as code.** A wrong diagnosis written as a hint misleads every later session.
   Keep engine facts out of them (the brief says so), and correct the guide when a misconception shows up.

## Recommendations

- **Workhorse: Sonnet medium.** Roughly 4× Opus's coverage per minute and, even counted by fully covered words,
  ahead per minute (≈35 vs ≈28) before accounting for Opus's higher per-token cost against the 5-hour allowance.
  Much of its weakness was a strategy the harness rewarded, which the harness changes below now push against.
- **Opus for consolidation rounds:** merging overlapping frames, filling held slots, generalizing building blocks -
  for example, every few Sonnet rounds, or whenever held words climb.
- **Not Haiku** for this role, at least at medium effort. It might serve as a cheap diagnostician (read-only questions
  about why lines don't parse), not as the step-taker.
- **Watch held words.** When a round raises them more than it covers, the next round should fill slots, not add
  frames.
- **Confirm with tokens.** This was one run per model. Before settling, repeat with the chat pane's token stats
  recorded per round, and compare fully covered lines gained per token.

## What a round costs in tokens

Measured from the app agent's CLI transcripts for the same three rounds. "Weighted" counts each token at its price
relative to plain input: cache reads about 0.1×, cache writes 1.25×, output 5×.

| Run | Requests | Context: start → end | Weighted tokens | Reads / writes / output |
|---|---|---|---|---|
| Haiku | 20 | 6K → 50K | ~192K | |
| Sonnet | 33 | 6K → 75K | ~310K | 49% / 30% / 20% |
| Opus | 68 | 6K → 138K | ~915K | 56% / 19% / 25% |

- **The chief cost is rereading.** Every request rereads the whole conversation, and the agent made nearly one
  request per tool call, adding about 2K tokens of context each time (tool results, plus its own thinking and C#
  drafts). So a round's cost grows with roughly the square of its number of calls. Opus made twice Sonnet's calls,
  each over a longer context.
- **A handoff threshold set as a share of the window never fired:** 20% of a 1M window is 200K tokens, and no round
  got that far. Simulated on the Opus round, handing off at 60K tokens saves about 18%; at 40K, handoffs come so
  often they cost more for Sonnet.
- **`apply` repeated `evaluate`:** the agent sent its C# draft a second time (as output, the dearest kind of token)
  and read the full report again, and both stayed in context.
- **Lookups went one per request** even when they didn't depend on each other.
- Prompt caching worked throughout (almost no uncached input), and the agent's starting context was a lean 6K.


| Problem seen | Change |
|---|---|
| Haiku and Sonnet stopped before the check-in | The chat pane tells an agent that stops with no check-in due to go on (twice a round at most); the brief and each step's status say to keep going until the tools call the check-in |
| Coverage alone made scaffolding look like progress | The score counts fully covered lines and held words; overview, evaluate, apply and the check-in report them, and flag a step that mostly frames text; the guide says to fill held slots before adding frame after frame, and to look for an existing frame first |
| Haiku chased glyphs that cost more than they save | `list_glyphs` sorts by words covered by default |
| Agents found frame targets only by hand-written regexes | `unmatched_openings` groups uncovered text by how it opens, ranked by the uncovered words it holds |
| A false joiner "fact" entered the journal | The guide states that tight punctuation (`'s`, `,`) binds under the default joiner, `{this}` included; the false entry was removed |
| The pane's bits-per-word and max-steps settings reset on every restart | Both are kept between runs and restored at startup; the default cap is 12 bits per word |
| The handoff threshold, as a share of a 1M window, never fired | A round hands off at a number of tokens of context, 60K by default |
| `apply` re-sent the evaluated draft and repeated its report | `apply` with no source makes the change last evaluated, reusing its evaluation, and reports a change applied as evaluated in brief |
| One lookup per request | The chat's system prompt and the guide ask for independent lookups to be made together in one request |

## Workspace notes from this comparison

- The merged grammar was committed to C#. The MTGGlyphs journal holds the agents' still-open problems, updated
  for the merge, including the overlapping frames left to consolidate.
- The three workspaces as the agents left them are backed up at `%LOCALAPPDATA%\Glyphotype\workspaces-backup-20261010`.
