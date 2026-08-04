/**
 * Initializes the hover-highlighting functionality for the type expressions page.
 * It attaches mouseover and mouseout listeners to the main container.
 */
function initializeTypeExpressionsHover(): void {
    const container = document.querySelector<HTMLElement>('.type-card-container');
    if (!container) return;

    container.addEventListener('mouseover', handleMouseOver);
    container.addEventListener('mouseout', handleMouseOut);
}

/**
 * Handles the mouseover event to apply highlighting and lowlighting treatments.
 * It determines the active data paths from the hovered element and updates the card's state.
 * If the mouse moves to a neutral element without a data-path, it clears existing highlights.
 */
function handleMouseOver(event: MouseEvent): void {
    const target = (event.target as HTMLElement).closest('[data-path], [data-paths]');
    const card = (event.target as HTMLElement).closest<HTMLElement>('.type-card');
    if (!card) return;

    // If there's no target, we've moved to a neutral space.
    // Clear any active highlights for this card and exit.
    if (!target) {
        if (card.dataset.activePath) {
            clearAllTreatments(card);
            delete card.dataset.activePath;
        }
        return;
    }

    const htmlTarget = target as HTMLElement;
    const hoveredPathsString = htmlTarget.dataset.paths ?? htmlTarget.dataset.path ?? null;
    const currentActivePath = card.dataset.activePath;

    // If we're still hovering over the same active path, do nothing.
    if (hoveredPathsString === currentActivePath) {
        return;
    }

    // Update the card's active path state.
    if (hoveredPathsString) {
        card.dataset.activePath = hoveredPathsString;
    } else {
        delete card.dataset.activePath;
    }

    const activePaths = getAllActivePaths(hoveredPathsString);
    applyTreatments(card, activePaths);
}

/**
 * Handles the mouseout event to clear all treatments when the mouse leaves a card.
 */
function handleMouseOut(event: MouseEvent): void {
    const card = (event.target as HTMLElement).closest<HTMLElement>('.type-card');
    const relatedTarget = event.relatedTarget as Node;

    if (card && !card.contains(relatedTarget)) {
        clearAllTreatments(card);
        delete card.dataset.activePath;
    }
}

/**
 * Removes all highlight and lowlight attributes and classes from elements within a card.
 */
function clearAllTreatments(card: HTMLElement): void {
    // Remove attributes from spans and property cards
    card.querySelectorAll('[highlight-active]').forEach(el => el.removeAttribute('highlight-active'));
    card.querySelectorAll('[lowlight-active]').forEach(el => el.removeAttribute('lowlight-active'));

    // Remove the highlight class from the regex container
    card.querySelector('.formatted-regex-fira')?.classList.remove('highlight-active');
}

/**
 * Generates a Set of all relevant paths from a given string.
 * This includes the paths themselves and all their parent segments.
 * e.g., "a.b.c a.d" -> new Set("a.b.c", "a.b", "a", "a.d")
 * @param pathsString A single path or multiple paths separated by spaces.
 * @returns A Set containing all active paths.
 */
function getAllActivePaths(pathsString: string | null): Set<string> {
    const paths = new Set<string>();
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
function applyTreatments(card: HTMLElement, activePaths: Set<string>): void {
    const isAnyHighlighted = activePaths.size > 0;

    // --- Regex Container Section ---
    // Toggle the "highlight-active" CLASS on the pre element
    const regexContainer = card.querySelector('.formatted-regex-fira');
    if (regexContainer) {
        if (isAnyHighlighted) {
            regexContainer.classList.add('highlight-active');
        } else {
            regexContainer.classList.remove('highlight-active');
        }
    }

    // --- Regex Spans Section ---
    const regexSpans = card.querySelectorAll<HTMLElement>('pre.formatted-regex-fira code span');
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

    // --- Type Tree Section ---
    const treeBoxes = card.querySelectorAll<HTMLElement>('.type-tree-container [data-path], .type-tree-container [data-paths]');
    treeBoxes.forEach(box => {
        const boxPaths = (box.dataset.paths ?? box.dataset.path ?? '').split(' ');
        const isMatch = boxPaths.some(p => p && activePaths.has(p));

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