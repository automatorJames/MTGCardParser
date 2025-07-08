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

// These variables will hold our event handlers. They are defined in the module's
// top-level scope so both init and dispose can access them.
let mouseoverHandler;
let mouseleaveHandler;

function initCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (!mainContent) {
        console.error("#main-content not found. Hover will not work.");
        return;
    }

    let lastCaptureIds = [];
    let lastPropertyName = null;

    // Define the handler functions and assign them to our module-scoped variables.
    mouseoverHandler = (event) => {
        const target = event.target.closest('[data-capture-ids], [data-capture-id]');

        const captureIdsAttr = target ? (target.dataset.captureIds || target.dataset.captureId) : '';
        const currentIds = captureIdsAttr.split(' ').filter(Boolean);
        const propertyName = target ? target.dataset.propertyName : null;

        if (currentIds.join(' ') !== lastCaptureIds.join(' ')) {
            document.querySelectorAll('.highlight-active').forEach(el => el.classList.remove('highlight-active'));

            if (currentIds.length > 0) {
                const rootId = currentIds[0];
                const rootSpan = document.querySelector(`.nested-underline[data-capture-ids~='${rootId}']`);
                const mainColor = rootSpan ? rootSpan.style.getPropertyValue('--underline-color') : 'transparent';

                currentIds.forEach(id => {
                    document.querySelectorAll(`.nested-underline[data-capture-ids~='${id}']`).forEach(el => el.classList.add('highlight-active'));

                    const detailBlock = document.querySelector(`.effect-details-block[data-capture-id='${id}']`);
                    if (detailBlock) {
                        detailBlock.classList.add('highlight-active');
                        detailBlock.style.setProperty('--highlight-color', mainColor);
                    }

                    const detailHeader = document.querySelector(`h5[data-capture-id='${id}']`);
                    if (detailHeader) {
                        detailHeader.classList.add('highlight-active');
                        detailHeader.style.setProperty('--highlight-color', mainColor);
                    }
                });
            }
        }

        const primaryId = currentIds.length > 0 ? currentIds[currentIds.length - 1] : null;
        const lastPrimaryId = lastCaptureIds.length > 0 ? lastCaptureIds[lastCaptureIds.length - 1] : null;

        if (propertyName !== lastPropertyName || primaryId !== lastPrimaryId) {
            document.querySelectorAll('.property-highlight, .prop-highlight-inline').forEach(el => {
                el.classList.remove('property-highlight', 'prop-highlight-inline');
            });

            if (primaryId && propertyName) {
                const propSpanInText = document.querySelector(`.prop-capture[data-capture-ids~='${primaryId}'][data-property-name='${propertyName}']`);
                if (propSpanInText) {
                    const propColor = propSpanInText.style.getPropertyValue('--prop-color');
                    propSpanInText.classList.add('prop-highlight-inline');

                    document.querySelectorAll(`[data-capture-id='${primaryId}'][data-property-name='${propertyName}']`).forEach(el => {
                        el.classList.add('property-highlight');
                        el.style.setProperty('--highlight-color', propColor);
                    });
                }
            }
        }

        lastCaptureIds = currentIds;
        lastPropertyName = propertyName;
    };

    mouseleaveHandler = () => {
        document.querySelectorAll('.highlight-active, .property-highlight, .prop-highlight-inline').forEach(el => {
            el.classList.remove('highlight-active', 'property-highlight', 'prop-highlight-inline');
        });
        lastCaptureIds = [];
        lastPropertyName = null;
    };

    // Attach the event listeners.
    mainContent.addEventListener('mouseover', mouseoverHandler);
    mainContent.addEventListener('mouseleave', mouseleaveHandler);
}

// Also export the cleanup function.
function disposeCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    // Check that the handlers were actually created before trying to remove them.
    if (mainContent && mouseoverHandler && mouseleaveHandler) {
        mainContent.removeEventListener('mouseover', mouseoverHandler);
        mainContent.removeEventListener('mouseleave', mouseleaveHandler);
    }
}