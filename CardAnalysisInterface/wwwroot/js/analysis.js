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

function initCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (!mainContent) {
        console.error("#main-content not found. Hover highlighting will not work.");
        return;
    }

    mouseoverHandler = (event) => {
        // 1. CLEAN SLATE
        // Always remove all highlights first.
        document.querySelectorAll('.highlight-active, .property-highlight, .prop-highlight-inline').forEach(el => {
            el.classList.remove('highlight-active', 'property-highlight', 'prop-highlight-inline');
        });

        // 2. HIGHLIGHT LOCAL TEXT SPANS (The new, direct method)
        // Find the single closest underline and overline to the cursor and highlight them.
        const underlineTarget = event.target.closest('.nested-underline');
        if (underlineTarget) {
            underlineTarget.classList.add('highlight-active');
        }

        const propTarget = event.target.closest('.prop-capture');
        if (propTarget) {
            propTarget.classList.add('prop-highlight-inline');
        }

        // 3. HIGHLIGHT THE REMOTE DETAILS TABLE (This logic remains the same)
        // Find the interactive element to get the IDs for linking to the table.
        const interactiveTarget = event.target.closest('[data-capture-ids], [data-capture-id]');
        if (!interactiveTarget) return;

        const captureIdsAttr = interactiveTarget.dataset.captureIds || interactiveTarget.dataset.captureId;
        const allIds = captureIdsAttr.split(' ').filter(Boolean);
        const propertyName = interactiveTarget.dataset.propertyName || null;

        // Highlight corresponding details blocks/headers
        allIds.forEach(id => {
            const detailBlock = document.querySelector(`.effect-details-block[data-capture-id="${id}"]`);
            if (detailBlock) detailBlock.classList.add('highlight-active');

            const detailHeader = document.querySelector(`h5[data-capture-id="${id}"]`);
            if (detailHeader) detailHeader.classList.add('highlight-active');
        });

        // Highlight the specific property row in the table
        const primaryId = allIds.length > 0 ? allIds[allIds.length - 1] : null;
        if (primaryId && propertyName) {
            document.querySelectorAll(`[data-capture-id="${primaryId}"][data-property-name="${propertyName}"]`).forEach(el => {
                el.classList.add('property-highlight');
            });
        }
    };

    mouseleaveHandler = () => {
        document.querySelectorAll('.highlight-active, .property-highlight, .prop-highlight-inline').forEach(el => {
            el.classList.remove('highlight-active', 'property-highlight', 'prop-highlight-inline');
        });
    };

    mainContent.addEventListener('mouseover', mouseoverHandler);
    mainContent.addEventListener('mouseleave', mouseleaveHandler);
}

function disposeCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (mainContent && mouseoverHandler && mouseleaveHandler) {
        mainContent.removeEventListener('mouseover', mouseoverHandler);
        mainContent.removeEventListener('mouseleave', mouseleaveHandler);
    }
}