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
    const activePaths = splitPaths(hoveredPathsString);
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
 * Removes all highlight and lowlight attributes and classes from elements within a card.
 */
function clearAllTreatments(card) {
    // Remove attributes from spans and property cards
    card.querySelectorAll('[highlight-active]').forEach(el => el.removeAttribute('highlight-active'));
    card.querySelectorAll('[lowlight-active]').forEach(el => el.removeAttribute('lowlight-active'));
    // Remove the highlight class from the regex container
    card.querySelector('.formatted-regex-fira')?.classList.remove('highlight-active');
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
/**
 * Determines whether two data-path values should be treated as related for highlighting
 * purposes: paths are segment-delimited by '_', and one is related to the other if they're
 * equal, or one is an ancestor (or descendant) of the other along that segment chain.
 * e.g. "A_B_C" is related to "A_B_C", "A_B", and "A_B_C_Draw-draws".
 */
function arePathsRelated(a, b) {
    return a === b || a.startsWith(b + '_') || b.startsWith(a + '_');
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
    const regexSpans = card.querySelectorAll('pre.formatted-regex-fira code span');
    regexSpans.forEach(span => {
        const spanPaths = (span.dataset.paths ?? span.dataset.path ?? '').split(' ');
        const isMatch = spanPaths.some(p => p && activePaths.some(ap => arePathsRelated(p, ap)));
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
    // --- Type Tree Section ---
    const treeBoxes = card.querySelectorAll('.type-tree-container [data-path], .type-tree-container [data-paths]');
    treeBoxes.forEach(box => {
        const boxPaths = (box.dataset.paths ?? box.dataset.path ?? '').split(' ');
        const isMatch = boxPaths.some(p => p && activePaths.some(ap => arePathsRelated(p, ap)));
        if (isMatch) {
            box.setAttribute('highlight-active', '');
        }
        else {
            box.removeAttribute('highlight-active');
        }
        if (isAnyHighlighted && !isMatch) {
            box.setAttribute('lowlight-active', '');
        }
        else {
            box.removeAttribute('lowlight-active');
        }
    });
}
//# sourceMappingURL=type-regex.js.map