// echo-tree-event-handler.ts
//
// All hover treatment for a echo tree card. Every visual change here is expressed as a class the
// stylesheet transitions (see wwwroot/css/echo-tree.css) rather than as a per-frame opacity
// animation, so this module only decides *which* elements are in which state.
//
// There are two distinct highlight axes, driven by the two key strips around the tree:
//
//   documents - hovering a document chip, a node, or a captured sub-span. Whole node/connector
//               groups fade out except those the active documents pass through.
//   glyph     - hovering a Glyph type chip. Only *outlines* fade, because node text gets its own
//               treatment: everything not captured by the hovered Glyph type dims, and everything
//               that is captured by it saturates.
import { createGradientStops } from "./echo-tree-svg-drawer.js";
const RESET_STATE = {
    mode: 'none', activeKeys: new Set(), glyphType: null,
    emphasis: null, anchorHover: false, showStats: false
};
let lastHoveredCard = null;
const GRADIENT_TRANSITION_RATIO = 0.1;
/**
 * Collects the elements hover treatment touches. Called once per render (the tree is rebuilt from
 * scratch on every resize), so hovering never has to re-query the DOM.
 */
export function indexCardHoverTargets(card) {
    const svg = card.querySelector('svg');
    const textSpans = svg ? Array.from(svg.querySelectorAll('.node-text-content')) : [];
    // Each run remembers its node, so emphasis can span every line a wrapped run was broken across.
    textSpans.forEach(span => span.__nodeGroup = span.closest('.node-group'));
    card.__hoverIndex = {
        keyedGroups: svg ? Array.from(svg.querySelectorAll('.wt-keyed')) : [],
        textSpans,
        documentChips: Array.from(card.querySelectorAll('[data-document-name]')),
        glyphChips: Array.from(card.querySelectorAll('.key-item[data-glyph-type]')),
        statAbove: svg?.querySelector('.tree-stat-above') ?? null,
        statBelow: svg?.querySelector('.tree-stat-below') ?? null
    };
    card.__hoverSignature = undefined;
}
function intersects(candidate, active) {
    for (const key of candidate)
        if (active.has(key))
            return true;
    return false;
}
/**
 * A stable description of a hover state, so re-entering the same target (which fires a fresh
 * mouseover for every child element crossed) doesn't re-apply identical classes.
 */
function signatureOf(state) {
    return [
        state.mode,
        [...state.activeKeys].sort().join(''),
        state.glyphType ?? '',
        state.emphasis ? `${state.emphasis.nodeGroup.id}:${state.emphasis.glyphType}` : '',
        state.anchorHover ? 'a' : '',
        state.showStats ? 's' : ''
    ].join('');
}
/** Whether a text run is one of the pieces of the capture the pointer is resting on. */
function isEmphasized(span, emphasis) {
    return emphasis !== null
        && span.dataset.glyphType === emphasis.glyphType
        && span.__nodeGroup === emphasis.nodeGroup;
}
/**
 * How many visually distinct throughlines the highlighted nodes form. Nodes are slotted into
 * vertical columns, and a column is where throughlines are at their most spread out - so the
 * busiest highlighted column is exactly the number of separate paths the eye can trace. It falls
 * below the document count wherever documents share a node ("overloading"), which is the case
 * worth calling out.
 */
function countThroughlines(index) {
    const perColumn = new Map();
    for (const group of index.keyedGroups) {
        if (group.__column === undefined || !group.classList.contains('wt-highlight'))
            continue;
        perColumn.set(group.__column, (perColumn.get(group.__column) ?? 0) + 1);
    }
    let max = 0;
    for (const count of perColumn.values())
        max = Math.max(max, count);
    return max;
}
const pluralize = (count, noun) => `${count} ${noun}${count === 1 ? '' : 's'}`;
function setStatLabel(label, text) {
    if (!label)
        return;
    if (text)
        label.textContent = text;
    label.classList.toggle('visible', text !== null);
}
/**
 * Rebuilds a highlighted group's saturated gradient from only the documents currently active, so a
 * node shared by four documents shows just the one band that's actually being pointed at.
 */
function refreshHighlightGradient(card, group, activeKeys) {
    if (!group.__highlightGradient || !card.__data)
        return;
    const keysForGradient = group.__sourceKeys.filter(key => activeKeys.has(key));
    const signature = keysForGradient.join('');
    if (group.__gradientKeys === signature)
        return;
    group.__gradientKeys = signature;
    group.__highlightGradient.innerHTML =
        createGradientStops(keysForGradient, card.__data.documentPalettes, 'sat', GRADIENT_TRANSITION_RATIO);
}
function applyHoverState(card, state) {
    const index = card.__hoverIndex;
    if (!index)
        return;
    const signature = signatureOf(state);
    if (card.__hoverSignature === signature)
        return;
    card.__hoverSignature = signature;
    const { activeKeys, glyphType } = state;
    const hasActiveKeys = activeKeys.size > 0;
    const glyphMode = state.mode === 'glyph';
    card.classList.toggle('highlight-active', hasActiveKeys);
    card.classList.toggle('glyph-hover', glyphMode);
    card.classList.toggle('anchor-hover', state.anchorHover);
    // --- Key strips ---
    for (const chip of index.documentChips) {
        const isHighlighted = hasActiveKeys && activeKeys.has(chip.dataset.documentName || '');
        chip.classList.toggle('highlight', isHighlighted);
        chip.classList.toggle('lowlight', hasActiveKeys && !isHighlighted);
    }
    for (const chip of index.glyphChips) {
        const isHighlighted = glyphType !== null && chip.dataset.glyphType === glyphType;
        chip.classList.toggle('highlight', isHighlighted);
        chip.classList.toggle('lowlight', glyphType !== null && !isHighlighted);
    }
    // --- Node and connector outlines ---
    for (const group of index.keyedGroups) {
        const isHighlighted = hasActiveKeys && intersects(group.__sourceKeysSet, activeKeys);
        group.classList.toggle('wt-highlight', isHighlighted);
        group.classList.toggle('wt-lowlight', hasActiveKeys && !isHighlighted);
        if (isHighlighted)
            refreshHighlightGradient(card, group, activeKeys);
    }
    // --- Node text ---
    for (const span of index.textSpans) {
        const referencesGlyph = glyphMode && span.dataset.glyphType === glyphType;
        span.classList.toggle('glyph-dim', glyphMode && !referencesGlyph);
        span.classList.toggle('glyph-match', referencesGlyph);
        span.classList.toggle('glyph-emphasis', isEmphasized(span, state.emphasis));
    }
    // --- Participation labels ---
    const documentCount = state.showStats ? activeKeys.size : 0;
    const throughlineCount = state.showStats ? countThroughlines(index) : 0;
    setStatLabel(index.statAbove, documentCount > 0 ? pluralize(documentCount, 'document') : null);
    setStatLabel(index.statBelow, documentCount > 0 && throughlineCount > 0 && throughlineCount !== documentCount
        ? pluralize(throughlineCount, 'throughline')
        : null);
}
/**
 * Reads the pointer's target and decides which of the two highlight axes it engages.
 */
function resolveHoverState(card, target) {
    const data = card.__data;
    if (!data)
        return RESET_STATE;
    const subspan = target.closest('.interactive-subspan');
    if (subspan) {
        // A captured run: the node's own document highlighting, plus emphasis on the run itself and
        // on its Glyph type in the lower key strip.
        const nodeGroup = subspan.closest('.node-group');
        const glyphType = subspan.dataset.glyphType ?? null;
        return {
            mode: 'documents',
            activeKeys: nodeGroup?.__sourceKeysSet ?? new Set(),
            glyphType,
            emphasis: nodeGroup && glyphType ? { nodeGroup, glyphType } : null,
            anchorHover: false,
            showStats: true
        };
    }
    const glyphChip = target.closest('.key-item[data-glyph-type]');
    if (glyphChip) {
        const glyph = glyphChip.dataset.glyphType;
        return {
            ...RESET_STATE,
            mode: 'glyph',
            activeKeys: data.glyphTypeDocuments.get(glyph) ?? new Set(),
            glyphType: glyph
        };
    }
    const documentChip = target.closest('[data-document-name]');
    if (documentChip)
        return { ...RESET_STATE, mode: 'documents', activeKeys: new Set([documentChip.dataset.documentName]) };
    const nodeGroup = target.closest('.node-group');
    if (nodeGroup) {
        // The anchor belongs to every document by definition, so hovering it lights the whole tree.
        const isAnchor = nodeGroup.classList.contains('main-anchor-span');
        return {
            mode: 'documents',
            activeKeys: isAnchor ? data.allDocumentsSet : (nodeGroup.__sourceKeysSet ?? new Set()),
            glyphType: null,
            emphasis: null,
            anchorHover: isAnchor,
            showStats: true
        };
    }
    return RESET_STATE;
}
/**
 * Sets up the single, comprehensive global event listener.
 */
export function setupGlobalEventHandlers() {
    if (window.unifiedHighlighterInitialized)
        return;
    window.unifiedHighlighterInitialized = true;
    document.addEventListener('mouseover', (event) => {
        const target = event.target;
        const card = target.closest('.echo-trees-card');
        // Leaving a card (for another card, or for nothing) must clear it explicitly - no further
        // mouseover will ever fire inside it to do so.
        if (lastHoveredCard && lastHoveredCard !== card) {
            applyHoverState(lastHoveredCard, RESET_STATE);
            lastHoveredCard = null;
        }
        if (!card)
            return;
        applyHoverState(card, resolveHoverState(card, target));
        lastHoveredCard = card;
    });
}
//# sourceMappingURL=echo-tree-event-handler.js.map