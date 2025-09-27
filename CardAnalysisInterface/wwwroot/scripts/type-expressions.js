// /wwwroot/js/typeExpressions.js

function initializeTypeExpressionsHover() {
    const container = document.querySelector('.type-card-container');
    if (!container) return;

    container.addEventListener('mouseover', handleMouseOver);
    container.addEventListener('mouseout', handleMouseOut);
}

function handleMouseOver(event) {
    const target = event.target.closest('[data-path]');
    const card = event.target.closest('.type-card');
    if (!card) return;

    const hoveredPath = (target && target.dataset.path) ? target.dataset.path : null;
    const currentActivePath = card.dataset.activePath;

    if (hoveredPath === currentActivePath) {
        return;
    }

    if (hoveredPath) {
        card.dataset.activePath = hoveredPath;
    } else {
        delete card.dataset.activePath;
    }

    const activePaths = getActivePaths(hoveredPath);
    applyTreatments(card, activePaths);
}

function handleMouseOut(event) {
    const card = event.target.closest('.type-card');

    if (card && !card.contains(event.relatedTarget)) {
        clearAllTreatments(card);
        delete card.dataset.activePath;
    }
}

function clearAllTreatments(card) {
    card.querySelectorAll('[highlight-active]').forEach(el => el.removeAttribute('highlight-active'));
    card.querySelectorAll('[lowlight-active]').forEach(el => el.removeAttribute('lowlight-active'));
}

function getActivePaths(hoveredPath) {
    const paths = new Set();
    if (hoveredPath) {
        paths.add(hoveredPath);
        const parts = hoveredPath.split('.');
        for (let i = parts.length - 1; i > 0; i--) {
            paths.add(parts.slice(0, i).join('.'));
        }
    }
    return paths;
}

function applyTreatments(card, activePaths) {
    let isAnyHighlighted = activePaths.size > 0;

    // --- Regex Section ---
    const regexSpans = card.querySelectorAll('pre.formatted-regex-fira code span');
    regexSpans.forEach(span => {
        const spanPath = span.dataset.path;
        if (spanPath && activePaths.has(spanPath)) {
            span.setAttribute('highlight-active', '');
        } else {
            span.removeAttribute('highlight-active');
        }
    });

    if (isAnyHighlighted) {
        regexSpans.forEach(span => {
            if (!span.hasAttribute('highlight-active') && span.dataset.lowlight !== 'None') {
                span.setAttribute('lowlight-active', '');
            } else {
                span.removeAttribute('lowlight-active');
            }
        });
    } else {
        card.querySelectorAll('[lowlight-active]').forEach(el => el.removeAttribute('lowlight-active'));
    }

    // --- Properties Section ---
    const allPropCards = card.querySelectorAll('.property-capture-card');

    // Pass 1: Handle highlights on all property elements
    card.querySelectorAll('.properties-container [data-path]').forEach(el => {
        if (activePaths.has(el.dataset.path)) {
            el.setAttribute('highlight-active', '');
        } else {
            el.removeAttribute('highlight-active');
        }
    });

    // Pass 2: Handle lowlights if any highlight is active
    if (isAnyHighlighted) {
        allPropCards.forEach(propCard => {
            const hasHighlightedChild = propCard.querySelector('[highlight-active]');
            if (!propCard.hasAttribute('highlight-active') && !hasHighlightedChild) {
                propCard.setAttribute('lowlight-active', '');
            } else {
                propCard.removeAttribute('lowlight-active');

                // If a card is active, check its children for sibling lowlighting
                const highlightedCanonical = propCard.querySelector('.canonical-representation[highlight-active]');
                if (highlightedCanonical) {
                    const allCanonicalsInCard = propCard.querySelectorAll('.canonical-representation');
                    allCanonicalsInCard.forEach(c => {
                        if (!c.hasAttribute('highlight-active')) {
                            c.setAttribute('lowlight-active', '');
                        } else {
                            c.removeAttribute('lowlight-active');
                        }
                    });
                } else {
                    // If the card is highlighted but no specific child is, remove all child lowlights
                    propCard.querySelectorAll('.canonical-representation[lowlight-active]').forEach(c => c.removeAttribute('lowlight-active'));
                }
            }
        });
    } else {
        // If no highlights are active at all, just clear everything
        allPropCards.forEach(propCard => {
            propCard.removeAttribute('lowlight-active');
            propCard.querySelectorAll('.canonical-representation').forEach(c => c.removeAttribute('lowlight-active'));
        });
    }
}