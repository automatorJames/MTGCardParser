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
const highlightActiveClass = 'highlight-active';
const dataPathAttribute = '[data-path]';

function initCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (!mainContent) {
        console.error("#main-content not found. Hover highlighting will not work.");
        return;
    }

    mouseoverHandler = (event) => {
        // 1. CLEAN SLATE
        // Always remove all highlights first. This part was correct.
        const elements = document.getElementsByClassName(highlightActiveClass);
        // A while loop is safer for removing classes from a live HTMLCollection.
        while (elements.length > 0) {
            elements[0].classList.remove(highlightActiveClass);
        }

        // 2. HIGHLIGHT LOCAL TEXT SPANS (The new, direct method)
        // This logic appeared correct and was left as is.
        const underlineTarget = event.target.closest('.nested-underline');
        if (underlineTarget) {
            underlineTarget.classList.add(highlightActiveClass);

            // Get all ancestor spans (parent, grandparent, etc) and add highlight-active on them
            let ancestor = underlineTarget.parentElement;
            while (ancestor) {
                if (ancestor.tagName === 'SPAN') {
                    ancestor.classList.add(highlightActiveClass);
                }
                ancestor = ancestor.parentElement;
            }
        }

        const propTarget = event.target.closest('.prop-capture');
        if (propTarget) {
            propTarget.classList.add(highlightActiveClass);
        }

        // 3. HIGHLIGHT THE REMOTE DETAILS TABLE (Corrected Logic)
        // Find the interactive element to get the ID for linking to the table.
        const interactiveTarget = event.target.closest(dataPathAttribute);
        if (!interactiveTarget) return;

        // Get the single ID from the 'data-path' attribute.
        const id = interactiveTarget.dataset.path;
        if (!id) return; // Exit if the element doesn't have the data-path.

        const propertyName = interactiveTarget.dataset.propertyName || null;

        // Highlight corresponding details block and header.
        // No loop is needed since we're only looking for one ID.
        const detailBlock = document.querySelector(`.effect-details-block[data-path="${id}"]`);
        if (detailBlock) {
            detailBlock.classList.add(highlightActiveClass);
        }

        const detailHeader = document.querySelector(`h5[data-path="${id}"]`);
        if (detailHeader) {
            detailHeader.classList.add(highlightActiveClass);
        }

        // Highlight the specific property row in the table, if applicable.
        if (propertyName && detailBlock) {
            // Assume the property row is within the detailBlock and has its own data attribute.
            const propertyRow = detailBlock.querySelector(`[data-property-name="${propertyName}"]`);
            if (propertyRow) {
                propertyRow.classList.add(highlightActiveClass);
            }
        }
    };

    mouseleaveHandler = () => {
        // Using a while loop here is also safer.
        const elements = document.getElementsByClassName(highlightActiveClass);
        while (elements.length > 0) {
            elements[0].classList.remove(highlightActiveClass);
        }
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