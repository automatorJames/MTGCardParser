// wwwroot/js/regexEditorInterop.js

let editorDotNetReference = null;
let editorElement = null;
let isInternallyChanging = false;

function initializeEditor(_dotNetReference, _editorElement) {
    editorDotNetReference = _dotNetReference;
    editorElement = _editorElement;
    if (editorElement) {
        editorElement.addEventListener('input', onEditorInput);
        editorElement.addEventListener('keydown', onEditorKeyDown);
        editorElement.addEventListener('blur', onEditorBlur);
        document.addEventListener('mousedown', onDropdownMouseDown);
        document.addEventListener('keydown', onGlobalKeyDown);
        editorElement.focus();
    }
}

function disposeEditor() {
    if (editorElement) {
        editorElement.removeEventListener('input', onEditorInput);
        editorElement.removeEventListener('keydown', onEditorKeyDown);
        editorElement.removeEventListener('blur', onEditorBlur);
    }
    document.removeEventListener('mousedown', onDropdownMouseDown);
    document.removeEventListener('keydown', onGlobalKeyDown);
    editorDotNetReference = null;
    editorElement = null;
}

function getEditorRawText() {
    return editorElement ? editorElement.textContent : '';
}

function scrollToAutocompleteItem(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ block: 'nearest' });
    }
}

// FIX 2: This function now correctly inserts at the caret.
function commitToken(textToReplace, fullTokenText) {
    const { range } = getCaretPositionInfo() || {};
    if (!range) return;

    // Precisely select the trigger text by moving the start of the range backward.
    range.setStart(range.startContainer, range.startOffset - textToReplace.length);

    // Replace the selection with the full token text.
    range.deleteContents();
    range.insertNode(document.createTextNode(fullTokenText));

    const newCursorPos = range.startOffset + fullTokenText.length;
    highlightAndRestoreCursor(editorElement.textContent, newCursorPos);
}

function onGlobalKeyDown(event) {
    if (event.key === 'Escape' && editorDotNetReference) {
        event.stopPropagation();
        editorDotNetReference.invokeMethodAsync('HandleGlobalEscape');
    }
}

function onEditorKeyDown(event) {
    const dropdown = document.getElementById('autocomplete-dropdown-list');
    const isDropdownVisible = dropdown && dropdown.offsetParent !== null;

    if (isDropdownVisible) {
        // When dropdown is visible, we let the C# component handle nav keys.
        // We just prevent the browser's default action for them.
        const navKeys = ['Enter', 'Tab', 'ArrowUp', 'ArrowDown'];
        if (navKeys.includes(event.key)) {
            event.preventDefault();
        }
        return;
    }

    // On "commit" keys like space or enter, trigger a re-highlight.
    if (event.key === ' ' || event.key === 'Enter') {
        setTimeout(() => highlightAndRestoreCursor(editorElement.textContent, getCaretCharacterOffsetWithin(editorElement)), 0);
    }

    // FIX 3: When dropdown is not visible, check for atomic deletion.
    handleAtomicDeletion(event);
}

function onDropdownMouseDown(event) {
    const item = event.target.closest('.autocomplete-item');
    if (!item) return;
    event.preventDefault();
    commitToken(item.dataset.textToReplace, item.dataset.fullTokenText);
    if (editorDotNetReference) {
        editorDotNetReference.invokeMethodAsync('HideDropdown');
    }
}

// FIX 2: This is now a lightweight notification to Blazor.
function onEditorInput() {
    if (isInternallyChanging) return;
    const { currentWord } = getCaretPositionInfo() || {};
    editorDotNetReference.invokeMethodAsync('UpdateFromJavaScript', editorElement.textContent, currentWord || '');
}

function onEditorBlur() {
    // When the user clicks away, apply final formatting.
    highlightAndRestoreCursor(editorElement.textContent, -1);
}

// FIX 3: This function now correctly handles deletion from anywhere inside a token.
function handleAtomicDeletion(event) {
    const { selection, range } = getCaretPositionInfo() || {};
    if (!selection || !selection.isCollapsed) return;

    const tokenSpan = range.startContainer.parentElement;
    if (!tokenSpan || !tokenSpan.classList.contains('token-style')) {
        return;
    }

    if (event.key === 'Backspace' || event.key === 'Delete') {
        event.preventDefault();
        const text = tokenSpan.textContent;
        const fullText = getEditorRawText();
        const tokenStartPos = fullText.lastIndexOf(text, getCaretCharacterOffsetWithin(editorElement));

        const newText = fullText.substring(0, tokenStartPos) + fullText.substring(tokenStartPos + text.length);
        highlightAndRestoreCursor(newText, tokenStartPos);
    }
}

function highlightAndRestoreCursor(text, cursorPos) {
    if (isInternallyChanging) return;
    isInternallyChanging = true;

    editorElement.innerHTML = '';

    const tokenRegex = /(@\w+)/g;
    let lastIndex = 0;
    let match;

    while ((match = tokenRegex.exec(text)) !== null) {
        if (match.index > lastIndex) {
            editorElement.appendChild(document.createTextNode(text.substring(lastIndex, match.index)));
        }
        const span = document.createElement('span');
        span.className = 'token-style';
        span.textContent = match[0];
        editorElement.appendChild(span);
        lastIndex = match.index + match[0].length;
    }

    if (lastIndex < text.length) {
        editorElement.appendChild(document.createTextNode(text.substring(lastIndex)));
    }

    if (cursorPos >= 0) {
        const result = findNodeAndOffset(editorElement, cursorPos);
        if (result && result.node) {
            const range = document.createRange();
            const selection = window.getSelection();
            range.setStart(result.node, result.offset);
            range.collapse(true);
            selection.removeAllRanges();
            selection.addRange(range);
        }
    }

    isInternallyChanging = false;
    onEditorInput();
}


function getCaretPositionInfo() {
    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0) return null;
    const range = selection.getRangeAt(0);
    const pos = getCaretCharacterOffsetWithin(editorElement);
    const textBeforeCaret = editorElement.textContent.substring(0, pos);
    const words = textBeforeCaret.split(/[\s\u00A0]+/);
    return { selection, range, currentWord: words[words.length - 1] };
}

function getCaretCharacterOffsetWithin(element) {
    let caretOffset = 0;
    const selection = window.getSelection();
    if (selection.rangeCount > 0) {
        const range = selection.getRangeAt(0);
        const preCaretRange = range.cloneRange();
        preCaretRange.selectNodeContents(element);
        preCaretRange.setEnd(range.endContainer, range.endOffset);
        caretOffset = preCaretRange.toString().length;
    }
    return caretOffset;
}

function findNodeAndOffset(element, charOffset) {
    const walker = document.createTreeWalker(element, NodeFilter.SHOW_TEXT);
    let cumulativeOffset = 0;
    let node;
    while (node = walker.nextNode()) {
        const nodeLength = node.length;
        if (cumulativeOffset + nodeLength >= charOffset) {
            return { node, offset: charOffset - cumulativeOffset };
        }
        cumulativeOffset += nodeLength;
    }
    const allTextNodes = Array.from(element.childNodes).filter(n => n.nodeType === Node.TEXT_NODE);
    const lastNode = allTextNodes[allTextNodes.length - 1] || element;
    return { node: lastNode, offset: lastNode.textContent?.length || 0 };
}