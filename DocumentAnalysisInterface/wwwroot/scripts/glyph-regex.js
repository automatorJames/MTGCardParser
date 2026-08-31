let hoverConfig;
/**
 * Initializes the hover-highlighting functionality for the type expressions page.
 * It attaches mouseover and mouseout listeners to the main container.
 * @param config Timing/opacity knobs, sourced from DocumentAnalysisInterface.HoverTreatmentConfig.
 */
function initializeTypeExpressionsHover(config) {
    hoverConfig = config;
    document.documentElement.style.setProperty('--hover-fade-duration', `${hoverConfig.fadeDurationMs}ms`);
    document.documentElement.style.setProperty('--lowlight-opacity', `${hoverConfig.lowlightOpacity}`);
    const container = document.querySelector('.type-card-container');
    if (!container)
        return;
    container.addEventListener('mouseover', handleMouseOver);
    container.addEventListener('mouseout', handleMouseOut);
}
/** Per-card debounce/commit bookkeeping, so rapid mouseover churn doesn't spam treatment changes. */
const pendingKeyByCard = new WeakMap();
const debounceTimerByCard = new WeakMap();
/**
 * Handles the mouseover event. Rather than committing a new highlight state immediately, this
 * schedules the commit after {@link HoverTreatmentConfig.debounceMs}, re-debouncing on every
 * subsequent mouseover — so a hover state only "lands" once the cursor actually rests on it,
 * and the flicker of rapidly crossing many spans while traveling never gets rendered at all.
 */
function handleMouseOver(event) {
    const card = event.target.closest('.type-card');
    if (!card)
        return;
    const target = event.target.closest('[data-path], [data-paths]');
    const htmlTarget = target;
    const hoveredPathsString = htmlTarget?.dataset.paths ?? htmlTarget?.dataset.path ?? null;
    const currentKey = pendingKeyByCard.has(card) ? pendingKeyByCard.get(card) : (card.dataset.activePath ?? null);
    if (hoveredPathsString === currentKey) {
        return;
    }
    pendingKeyByCard.set(card, hoveredPathsString);
    const existingTimer = debounceTimerByCard.get(card);
    if (existingTimer) {
        window.clearTimeout(existingTimer);
    }
    const timerId = window.setTimeout(() => {
        debounceTimerByCard.delete(card);
        commitHoverState(card, hoveredPathsString);
    }, hoverConfig.debounceMs);
    debounceTimerByCard.set(card, timerId);
}
/**
 * Handles the mouseout event to clear all treatments when the mouse leaves a card. This happens
 * immediately (no debounce) — the cursor has definitively left, so there's nothing left to
 * debounce against, and lingering here is exactly the "too loose" feel we're avoiding.
 */
function handleMouseOut(event) {
    const card = event.target.closest('.type-card');
    const relatedTarget = event.relatedTarget;
    if (card && !card.contains(relatedTarget)) {
        const existingTimer = debounceTimerByCard.get(card);
        if (existingTimer) {
            window.clearTimeout(existingTimer);
            debounceTimerByCard.delete(card);
        }
        pendingKeyByCard.delete(card);
        commitHoverState(card, null);
    }
}
/** Applies the given hovered-paths state (or clears it, for null) as the card's committed state. */
function commitHoverState(card, hoveredPathsString) {
    if (hoveredPathsString) {
        card.dataset.activePath = hoveredPathsString;
    }
    else {
        delete card.dataset.activePath;
    }
    const activePaths = splitPaths(hoveredPathsString);
    applyTreatments(card, activePaths);
}
/**
 * Applies (or clears) the fractional dim treatment used for path-ancestor elements.
 */
function setDimTreatment(element, isDimmed) {
    element.toggleAttribute('dim-active', isDimmed);
    element.style.opacity = isDimmed ? String(hoverConfig.pathAncestorDimOpacity) : '';
}
/**
 * Briefly "pops" an element's brightness above its resting highlighted brightness before easing
 * back down — applied only to the exact-match hover target, so it visually reads as *the*
 * target even while ancestor/lowlight elements around it are still mid-fade. Implemented as a
 * `filter: brightness()` Web Animation layered on top of the element's color/border transition
 * (driven separately by CSS via `--hover-fade-duration`), so it works the same way for a plain
 * text span and a multi-property tree box alike, without needing a bespoke "overshoot color".
 */
function popHoverTarget(element) {
    const peakOffset = hoverConfig.overshootDurationMs / (hoverConfig.overshootDurationMs + hoverConfig.overshootSettleDurationMs);
    element.animate([
        { filter: 'brightness(1)', offset: 0 },
        { filter: `brightness(${1 + hoverConfig.overshootBrightnessBoost})`, offset: peakOffset },
        { filter: 'brightness(1)', offset: 1 },
    ], {
        duration: hoverConfig.overshootDurationMs + hoverConfig.overshootSettleDurationMs,
        easing: 'ease-out',
        fill: 'none',
    });
}
/**
 * Splits a space-separated data-path/data-paths value into its individual paths.
 * @param pathsString A single path or multiple paths separated by spaces.
 * @returns The individual paths, with empty entries filtered out.
 */
function splitPaths(pathsString) {
    if (!pathsString) {
        return [];
    }
    return pathsString.split(' ').filter(p => p);
}
function classifyPathRelation(candidate, active) {
    if (candidate === active)
        return 'exact';
    if (candidate.startsWith(active + '_'))
        return 'descendant';
    if (active.startsWith(candidate + '_'))
        return 'ancestor';
    return 'none';
}
/**
 * The strongest relation between any of an element's paths and any of the active (hovered)
 * paths, where strength is exact > descendant > ancestor > none.
 */
function bestPathRelation(candidatePaths, activePaths) {
    let best = 'none';
    for (const p of candidatePaths) {
        if (!p)
            continue;
        for (const ap of activePaths) {
            const relation = classifyPathRelation(p, ap);
            if (relation === 'exact')
                return 'exact';
            if (relation === 'descendant')
                best = 'descendant';
            else if (relation === 'ancestor' && best === 'none')
                best = 'ancestor';
        }
    }
    return best;
}
/**
 * Applies or removes 'highlight-active' and 'lowlight-active' attributes
 * to all relevant elements within a card based on the set of active paths.
 * @param card The parent .type-card element.
 * @param activePaths The paths (of the hovered element) that should be highlighted.
 */
function applyTreatments(card, activePaths) {
    const isAnyHighlighted = activePaths.length > 0;
    // --- Regex Container Section ---
    // Toggle the "highlight-active" CLASS on the pre element
    const regexContainer = card.querySelector('.formatted-regex-fira');
    if (regexContainer) {
        if (isAnyHighlighted) {
            regexContainer.classList.add('highlight-active');
        }
        else {
            regexContainer.classList.remove('highlight-active');
        }
    }
    // --- Regex Spans Section ---
    // A span whose path is an ancestor of the hovered path (its whole FQN matches only the
    // *beginning* of the target's) gets fractionally dimmed via pathAncestorDimOpacity rather
    // than highlighted or fully lowlit. That leaves the true (exact-match) target as the only
    // fully-highlighted span, while ancestor spans read as "still relevant" without competing
    // with it for attention. The exact-match target additionally gets a brief brightness pop.
    const regexSpans = card.querySelectorAll('pre.formatted-regex-fira code span');
    regexSpans.forEach(span => {
        const spanPaths = (span.dataset.paths ?? span.dataset.path ?? '').split(' ');
        const relation = bestPathRelation(spanPaths, activePaths);
        span.toggleAttribute('highlight-active', relation === 'exact' || relation === 'descendant');
        setDimTreatment(span, relation === 'ancestor');
        if (isAnyHighlighted && relation === 'none' && span.dataset.lowlight !== 'None') {
            span.setAttribute('lowlight-active', '');
        }
        else {
            span.removeAttribute('lowlight-active');
        }
        if (relation === 'exact') {
            popHoverTarget(span);
        }
    });
    // --- Type Tree Section ---
    const treeBoxes = card.querySelectorAll('.type-tree-container [data-path], .type-tree-container [data-paths]');
    treeBoxes.forEach(box => {
        const boxPaths = (box.dataset.paths ?? box.dataset.path ?? '').split(' ');
        const relation = bestPathRelation(boxPaths, activePaths);
        box.toggleAttribute('highlight-active', relation === 'exact' || relation === 'descendant');
        box.toggleAttribute('lowlight-active', isAnyHighlighted && relation === 'none');
        setDimTreatment(box, relation === 'ancestor');
        if (relation === 'exact') {
            popHoverTarget(box);
        }
    });
    // --- Matches Table Section ---
    // Every span in a match's content participates, including the grey surrounding-context spans
    // (which carry no data-path of their own, so they can only ever read as "unrelated" and dim
    // along with everything else) - same treatment as the regex column's own non-interactive spans.
    const matchSpans = card.querySelectorAll('.matches-table .match-content span');
    matchSpans.forEach(span => {
        const spanPaths = (span.dataset.paths ?? span.dataset.path ?? '').split(' ');
        const relation = bestPathRelation(spanPaths, activePaths);
        span.toggleAttribute('highlight-active', relation === 'exact' || relation === 'descendant');
        setDimTreatment(span, relation === 'ancestor');
        span.toggleAttribute('lowlight-active', isAnyHighlighted && relation === 'none');
        if (relation === 'exact') {
            popHoverTarget(span);
        }
    });
    // --- C# Class Section ---
    // Same treatment as the regex column's spans - a span with no data-path at all (neutral C#
    // syntax, a Nibs-array literal) never carries highlight-active/lowlight-active either way, so
    // it just reads as plain, unhighlighted text regardless of what else is hovered.
    const classSpans = card.querySelectorAll('.csharp-class-view [data-path], .csharp-class-view [data-paths]');
    classSpans.forEach(span => {
        const spanPaths = (span.dataset.paths ?? span.dataset.path ?? '').split(' ');
        const relation = bestPathRelation(spanPaths, activePaths);
        span.toggleAttribute('highlight-active', relation === 'exact' || relation === 'descendant');
        setDimTreatment(span, relation === 'ancestor');
        span.toggleAttribute('lowlight-active', isAnyHighlighted && relation === 'none');
        if (relation === 'exact') {
            popHoverTarget(span);
        }
    });
}
//# sourceMappingURL=glyph-regex.js.map