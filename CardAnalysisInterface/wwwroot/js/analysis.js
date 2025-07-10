// --- CLICK TO COPY LOGIC ---
// This part remains unchanged and works fine as it is.
document.addEventListener('DOMContentLoaded', () => {
    document.body.addEventListener('click', (event) => {
        let target = event.target.closest('pre, td');
        if (!target) return;
        if (document.body.classList.contains('page-variable-capture') && !target.classList.contains('full-original-text')) return;
        let textToCopy = (target.tagName === 'TD' && target.hasAttribute('data-original-text')) ? target.getAttribute('data-original-text') : target.innerText.trim();
        if (textToCopy) {
            if (!event.shiftKey) textToCopy = textToCopy.toLowerCase();
            navigator.clipboard.writeText(textToCopy).then(() => showCopyFeedback(event.clientX, event.clientY)).catch(err => console.error('Failed to copy text: ', err));
        }
    });

    let feedbackDiv = null, feedbackTimeout = null;
    function showCopyFeedback(x, y) {
        if (!feedbackDiv) {
            feedbackDiv = document.createElement('div');
            feedbackDiv.className = 'copy-feedback';
            feedbackDiv.textContent = 'Copied';
            document.body.appendChild(feedbackDiv);
        }
        clearTimeout(feedbackTimeout);
        feedbackDiv.style.left = `${x + 15}px`;
        feedbackDiv.style.top = `${y + 15}px`;
        feedbackDiv.style.opacity = '1';
        feedbackTimeout = setTimeout(() => { feedbackDiv.style.opacity = '0'; }, 1200);
    }
});


// --- VARIABLE CAPTURE PAGE HOVER LOGIC ---

// These variables will hold our event handlers so they can be removed later,
// which is crucial for preventing memory leaks in a Single Page Application (SPA) like Blazor.
let mouseoverHandler;
let mouseleaveHandler;

/**
 * Initializes the hover-highlighting logic for the card capture page.
 * This function should be called once after the component has rendered for the first time.
 */
function initCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (!mainContent) {
        console.error("#main-content not found. Hover highlighting will not work.");
        return;
    }

    // --- MOUSEOVER HANDLER ---
    // This function executes every time the mouse moves over a new element inside '#main-content'.
    mouseoverHandler = (event) => {
        // 1. ADOPT A 'CLEAN SLATE' APPROACH
        // On every single mouse movement, we first remove ALL highlight classes from the
        // entire document. This makes the logic stateless and far more robust than trying
        // to track what was hovered previously. It prevents leftover highlights.
        document.querySelectorAll('.highlight-active, .property-highlight, .prop-highlight-inline').forEach(el => {
            el.classList.remove('highlight-active', 'property-highlight', 'prop-highlight-inline');
        });


        // 2. IDENTIFY THE INTERACTIVE TARGET
        // The element that fired the event (event.target) might be a plain text node or a
        // sub-element inside our span. `closest()` walks up the DOM tree to find the
        // nearest parent element that has one of our data attributes.
        const target = event.target.closest('[data-capture-ids], [data-capture-id]');

        // If the mouse isn't over an interactive element, we have nothing to do.
        if (!target) return;


        // 3. EXTRACT DATA FROM THE TARGET
        // Some elements have 'data-capture-ids' (for nested tokens that might need to highlight parents)
        // while others have a single 'data-capture-id'. The '||' provides a fallback.
        const captureIdsAttr = target.dataset.captureIds || target.dataset.captureId;

        // This is a list of all IDs that should be highlighted. It's an array because a nested
        // element might need to highlight itself AND its parent (see explanation below).
        const currentIds = captureIdsAttr.split(' ').filter(Boolean);

        // Get the specific property name if the user is hovering a property span/row.
        const propertyName = target.dataset.propertyName || null;


        // 4. APPLY GENERAL HIGHLIGHTS
        // Loop through every ID associated with the hovered element.
        if (currentIds.length > 0) {
            currentIds.forEach(id => {
                // --- THE FIX ---
                // The 'id' variable is wrapped in quotes (`"${id}"`) inside the selector string.
                // This is critical because IDs contain spaces (e.g., "Ankh of Mishra-466-0-7").
                // Without quotes, the browser sees the space and breaks the selector.
                // [data-capture-id=Ankh of Mishra] fails.
                // [data-capture-id="Ankh of Mishra"] works.

                // Highlight the underlined text spans. The `~=` selector correctly matches
                // an ID within a space-separated list in the `data-capture-ids` attribute.
                document.querySelectorAll(`.nested-underline[data-capture-ids~="${id}"]`).forEach(el => el.classList.add('highlight-active'));

                // Highlight the corresponding details block and its header.
                const detailBlock = document.querySelector(`.effect-details-block[data-capture-id="${id}"]`);
                if (detailBlock) detailBlock.classList.add('highlight-active');

                const detailHeader = document.querySelector(`h5[data-capture-id="${id}"]`);
                if (detailHeader) detailHeader.classList.add('highlight-active');
            });
        }


        // 5. APPLY PROPERTY-SPECIFIC HIGHLIGHTS
        // We only want to highlight property rows that belong to the *most specific* token being hovered.
        // The most specific ID is always the last one in our list.
        const primaryId = currentIds.length > 0 ? currentIds[currentIds.length - 1] : null;

        if (primaryId && propertyName) {
            // Also apply THE FIX here by quoting the variables in the selectors.

            // Highlight the specific property text within the card text (the overline).
            document.querySelectorAll(`.prop-capture[data-capture-ids~="${primaryId}"][data-property-name="${propertyName}"]`).forEach(el => {
                el.classList.add('prop-highlight-inline');
            });

            // Highlight the corresponding property row in the details table.
            document.querySelectorAll(`[data-capture-id="${primaryId}"][data-property-name="${propertyName}"]`).forEach(el => {
                el.classList.add('property-highlight');
            });
        }
    };

    // --- MOUSELEAVE HANDLER ---
    // This executes once when the mouse leaves the entire '#main-content' container.
    mouseleaveHandler = () => {
        // Its only job is to clean up and remove all highlights.
        document.querySelectorAll('.highlight-active, .property-highlight, .prop-highlight-inline').forEach(el => {
            el.classList.remove('highlight-active', 'property-highlight', 'prop-highlight-inline');
        });
    };

    // Attach the event listeners to the main container.
    mainContent.addEventListener('mouseover', mouseoverHandler);
    mainContent.addEventListener('mouseleave', mouseleaveHandler);
}

/**
 * Cleans up event listeners to prevent memory leaks when the Blazor component is disposed.
 */
function disposeCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (mainContent && mouseoverHandler && mouseleaveHandler) {
        mainContent.removeEventListener('mouseover', mouseoverHandler);
        mainContent.removeEventListener('mouseleave', mouseleaveHandler);
    }
}