// Client/Scripts/typeExpressions.ts
// Use a Map to store the original, unmodified text content of each <code> element.
const originalRegexContent = new Map();
/**
 * Escapes special HTML characters in a string to prevent them from being
 * interpreted as HTML tags.
 * @param text The plain text to escape.
 * @returns An HTML-safe string.
 */
function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}
/**
 * Highlights a specific span of text within a <pre><code> element.
 * This version correctly handles HTML entities by working with text content.
 * @param preElement The <pre> element containing the regex code.
 * @param start The starting index for the highlight.
 * @param end The ending index for the highlight.
 */
function highlightRegexSpan(preElement, start, end) {
    const codeElement = preElement.querySelector('code');
    if (!codeElement)
        return;
    // If we haven't stored this element's original content, do so now.
    // We store the raw text content, which is safer than innerHTML.
    if (!originalRegexContent.has(codeElement)) {
        originalRegexContent.set(codeElement, codeElement.textContent || '');
    }
    const content = originalRegexContent.get(codeElement);
    // Validate the range.
    if (isNaN(start) || isNaN(end) || start < 0 || end < 0 || start >= end || end > content.length) {
        // If range is invalid, restore original text content to be safe.
        codeElement.textContent = content;
        return;
    }
    const prefix = content.substring(0, start);
    const highlighted = content.substring(start, end);
    const suffix = content.substring(end);
    // Escape each part before injecting it as HTML. This prevents the browser
    // from interpreting characters like '>' as tags, fixing the bug.
    const safePrefix = escapeHtml(prefix);
    const safeHighlighted = escapeHtml(highlighted);
    const safeSuffix = escapeHtml(suffix);
    // Reconstruct the HTML with the highlighted portion wrapped in a <mark> tag.
    codeElement.innerHTML = `${safePrefix}<mark class="regex-highlight">${safeHighlighted}</mark>${safeSuffix}`;
}
/**
 * Removes any highlighting from a <pre><code> element by restoring its original content.
 * @param preElement The <pre> element to clear.
 */
function clearRegexHighlight(preElement) {
    const codeElement = preElement.querySelector('code');
    if (codeElement && originalRegexContent.has(codeElement)) {
        // Restoring textContent is the safest way to revert.
        codeElement.textContent = originalRegexContent.get(codeElement);
    }
}
/**
 * Initializes all hover event listeners for the Type Expressions page.
 * This function is designed to be called from Blazor's OnAfterRenderAsync.
 */
export function initTypeExpressionHover() {
    originalRegexContent.clear();
    const container = document.querySelector('.type-card-container');
    if (!container)
        return;
    let hoveredElement = null;
    container.addEventListener('mouseover', (event) => {
        const target = event.target?.closest('.property-capture-card, .canonical-representation, .capture-variant');
        if (!target)
            return;
        hoveredElement = target;
        const typeCard = hoveredElement.closest('.type-card');
        const preElement = typeCard?.querySelector('pre');
        if (!preElement)
            return;
        const start = parseInt(hoveredElement.dataset.start ?? '-1', 10);
        const end = parseInt(hoveredElement.dataset.end ?? '-1', 10);
        highlightRegexSpan(preElement, start, end);
    });
    container.addEventListener('mouseout', () => {
        if (!hoveredElement)
            return;
        const typeCard = hoveredElement.closest('.type-card');
        const preElement = typeCard?.querySelector('pre');
        if (preElement) {
            clearRegexHighlight(preElement);
        }
        hoveredElement = null;
    });
}
// Expose the Blazor interop function to the global window object.
window.initTypeExpressionHover = initTypeExpressionHover;
//# sourceMappingURL=type-expressions.js.map