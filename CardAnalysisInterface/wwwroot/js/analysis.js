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

// By adding 'export', we make this function callable from the Blazor component.
function initCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (!mainContent) {
        console.error("#main-content not found. Hover will not work.");
        return;
    }

    let lastCaptureId = null;
    let lastPropertyName = null;

    // Define the handler functions and assign them to our module-scoped variables.
    mouseoverHandler = (event) => {
        const target = event.target.closest('[data-capture-id]');
        const captureId = target ? target.dataset.captureId : null;
        const propertyName = target ? target.dataset.propertyName : null;

        if (captureId !== lastCaptureId) {
            if (lastCaptureId) {
                document.querySelectorAll(`[data-capture-id='${lastCaptureId}']`).forEach(el => el.classList.remove('highlight-active'));
            }
            if (captureId) {
                const span = document.querySelector(`.nested-underline[data-capture-id='${captureId}']`);
                const block = document.querySelector(`.effect-details-block[data-capture-id='${captureId}']`);
                if (span && block) {
                    const mainColor = span.style.getPropertyValue('--underline-color');
                    span.classList.add('highlight-active');
                    block.classList.add('highlight-active');
                    block.style.setProperty('--highlight-color', mainColor);
                }
            }
        }

        if (propertyName !== lastPropertyName || captureId !== lastCaptureId) {
            if (lastCaptureId) {
                const oldBlock = document.querySelector(`.effect-details-block[data-capture-id='${lastCaptureId}']`);
                if (oldBlock) {
                    oldBlock.querySelectorAll('.property-highlight').forEach(el => el.classList.remove('property-highlight'));
                    document.querySelectorAll('.prop-highlight-inline').forEach(el => el.classList.remove('prop-highlight-inline'));
                }
            }
            if (captureId && propertyName) {
                const newBlock = document.querySelector(`.effect-details-block[data-capture-id='${captureId}']`);
                const propSpanInText = document.querySelector(`.prop-capture[data-capture-id='${captureId}'][data-property-name='${propertyName}']`);

                if (newBlock && propSpanInText) {
                    const propColor = propSpanInText.style.getPropertyValue('--prop-color');
                    newBlock.querySelectorAll(`[data-property-name='${propertyName}']`).forEach(el => {
                        el.classList.add('property-highlight');
                        el.style.setProperty('--highlight-color', propColor);
                    });
                    propSpanInText.classList.add('prop-highlight-inline');
                }
            }
        }
        lastCaptureId = captureId;
        lastPropertyName = propertyName;
    };

    mouseleaveHandler = () => {
        if (lastCaptureId) {
            document.querySelectorAll('.highlight-active').forEach(el => el.classList.remove('highlight-active'));
            const block = document.querySelector(`.effect-details-block[data-capture-id='${lastCaptureId}']`);
            if (block) {
                block.querySelectorAll('.property-highlight').forEach(el => el.classList.remove('property-highlight'));
                document.querySelectorAll('.prop-highlight-inline').forEach(el => el.classList.remove('prop-highlight-inline'));
            }
        }
        lastCaptureId = null;
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