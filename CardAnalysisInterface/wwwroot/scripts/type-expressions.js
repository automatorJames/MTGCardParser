/**
 * Initializes the hover-highlighting functionality for the type expressions page.
 * It attaches mouseover and mouseout listeners to the main container.
 */
function initializeTypeExpressionsHover() {
    const container = document.querySelector('.type-card-container');
    if (!container)
        return;
    container.addEventListener('mouseover', handleMouseOver);
    container.addEventListener('mouseout', handleMouseOut);
}
/**
 * Handles the mouseover event to apply highlighting and lowlighting treatments.
 * It determines the active data paths from the hovered element and updates the card's state.
 * If the mouse moves to a neutral element without a data-path, it clears existing highlights.
 */
function handleMouseOver(event) {
    const target = event.target.closest('[data-path], [data-paths]');
    const card = event.target.closest('.type-card');
    if (!card)
        return;
    // If there's no target, we've moved to a neutral space.
    // Clear any active highlights for this card and exit.
    if (!target) {
        if (card.dataset.activePath) {
            clearAllTreatments(card);
            delete card.dataset.activePath;
        }
        return;
    }
    const htmlTarget = target;
    const hoveredPathsString = htmlTarget.dataset.paths ?? htmlTarget.dataset.path ?? null;
    const currentActivePath = card.dataset.activePath;
    // If we're still hovering over the same active path, do nothing.
    if (hoveredPathsString === currentActivePath) {
        return;
    }
    // Update the card's active path state.
    if (hoveredPathsString) {
        card.dataset.activePath = hoveredPathsString;
    }
    else {
        delete card.dataset.activePath;
    }
    const activePaths = getAllActivePaths(hoveredPathsString);
    applyTreatments(card, activePaths);
}
/**
 * Handles the mouseout event to clear all treatments when the mouse leaves a card.
 */
function handleMouseOut(event) {
    const card = event.target.closest('.type-card');
    const relatedTarget = event.relatedTarget;
    if (card && !card.contains(relatedTarget)) {
        clearAllTreatments(card);
        delete card.dataset.activePath;
    }
}
/**
 * Removes all highlight and lowlight attributes from elements within a card.
 */
function clearAllTreatments(card) {
    card.querySelectorAll('[highlight-active]').forEach(el => el.removeAttribute('highlight-active'));
    card.querySelectorAll('[lowlight-active]').forEach(el => el.removeAttribute('lowlight-active'));
}
/**
 * Generates a Set of all relevant paths from a given string.
 * This includes the paths themselves and all their parent segments.
 * e.g., "a.b.c a.d" -> new Set("a.b.c", "a.b", "a", "a.d")
 * @param pathsString A single path or multiple paths separated by spaces.
 * @returns A Set containing all active paths.
 */
function getAllActivePaths(pathsString) {
    const paths = new Set();
    if (!pathsString) {
        return paths;
    }
    const individualPaths = pathsString.split(' ').filter(p => p); // Filter out empty strings
    for (const path of individualPaths) {
        paths.add(path);
        const parts = path.split('.');
        for (let i = parts.length - 1; i > 0; i--) {
            paths.add(parts.slice(0, i).join('.'));
        }
    }
    return paths;
}
/**
 * Applies or removes 'highlight-active' and 'lowlight-active' attributes
 * to all relevant elements within a card based on the set of active paths.
 * @param card The parent .type-card element.
 * @param activePaths A Set of paths that should be highlighted.
 */
function applyTreatments(card, activePaths) {
    const isAnyHighlighted = activePaths.size > 0;
    // --- Regex Section ---
    const regexSpans = card.querySelectorAll('pre.formatted-regex-fira code span');
    regexSpans.forEach(span => {
        const spanPaths = (span.dataset.paths ?? span.dataset.path ?? '').split(' ');
        const isMatch = spanPaths.some(p => p && activePaths.has(p));
        if (isMatch) {
            span.setAttribute('highlight-active', '');
        }
        else {
            span.removeAttribute('highlight-active');
        }
    });
    if (isAnyHighlighted) {
        regexSpans.forEach(span => {
            if (!span.hasAttribute('highlight-active') && span.dataset.lowlight !== 'None') {
                span.setAttribute('lowlight-active', '');
            }
            else {
                span.removeAttribute('lowlight-active');
            }
        });
    }
    else {
        card.querySelectorAll('[lowlight-active]').forEach(el => el.removeAttribute('lowlight-active'));
    }
    // --- Properties Section ---
    const allPropCards = card.querySelectorAll('.property-capture-card');
    const allPropElements = card.querySelectorAll('.properties-container [data-path], .properties-container [data-paths]');
    // Pass 1: Handle highlights on all property elements
    allPropElements.forEach(el => {
        const elPaths = (el.dataset.paths ?? el.dataset.path ?? '').split(' ');
        const isMatch = elPaths.some(p => p && activePaths.has(p));
        if (isMatch) {
            el.setAttribute('highlight-active', '');
        }
        else {
            el.removeAttribute('highlight-active');
        }
    });
    // Pass 2: Handle lowlights based on the results of the highlight pass
    if (isAnyHighlighted) {
        allPropCards.forEach(propCard => {
            const hasHighlightedChild = propCard.querySelector('[highlight-active]');
            if (!propCard.hasAttribute('highlight-active') && !hasHighlightedChild) {
                propCard.setAttribute('lowlight-active', '');
            }
            else {
                propCard.removeAttribute('lowlight-active');
                const highlightedCanonical = propCard.querySelector('.canonical-representation[highlight-active]');
                if (highlightedCanonical) {
                    const allCanonicalsInCard = propCard.querySelectorAll('.canonical-representation');
                    allCanonicalsInCard.forEach(c => {
                        if (!c.hasAttribute('highlight-active')) {
                            c.setAttribute('lowlight-active', '');
                        }
                        else {
                            c.removeAttribute('lowlight-active');
                        }
                    });
                }
                else {
                    propCard.querySelectorAll('.canonical-representation[lowlight-active]').forEach(c => c.removeAttribute('lowlight-active'));
                }
            }
        });
    }
    else {
        allPropCards.forEach(propCard => {
            propCard.removeAttribute('lowlight-active');
            propCard.querySelectorAll('.canonical-representation').forEach(c => c.removeAttribute('lowlight-active'));
        });
    }
}
//# sourceMappingURL=type-expressions.js.map