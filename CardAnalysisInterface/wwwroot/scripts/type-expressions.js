function initTypeCardHover() {
    const cards = document.querySelectorAll('.type-card');

    cards.forEach(card => {
        // Find all elements with a data-path attribute within the current card.
        const pathElements = card.querySelectorAll('[data-path]');

        // A handler to remove highlights from all related elements within the card.
        const clearHighlights = () => {
            pathElements.forEach(el => {
                el.classList.remove('highlight-strong', 'highlight-soft');
            });
        };

        // Attach a mouseover listener to every element with a data-path.
        pathElements.forEach(el => {
            el.addEventListener('mouseover', (event) => {
                // Stop the event from bubbling up to parent elements with data-paths.
                event.stopPropagation();

                // Clear any existing highlights first.
                clearHighlights();

                const hoveredElement = event.currentTarget;
                const hoveredPath = hoveredElement.dataset.path;

                if (!hoveredPath) return;

                // Iterate through all path elements in the card to check for relationships.
                pathElements.forEach(otherEl => {
                    const otherPath = otherEl.dataset.path;
                    if (!otherPath) return;

                    // Apply 'highlight-strong' for an exact path match.
                    if (otherPath === hoveredPath) {
                        otherEl.classList.add('highlight-strong');
                    }
                    // Apply 'highlight-soft' if one path is an ancestor of the other.
                    else if (otherPath.startsWith(hoveredPath + '.') || hoveredPath.startsWith(otherPath + '.')) {
                        otherEl.classList.add('highlight-soft');
                    }
                });
            });
        });

        // When the mouse leaves the boundary of the card, clear all highlights.
        card.addEventListener('mouseleave', clearHighlights);
    });
}