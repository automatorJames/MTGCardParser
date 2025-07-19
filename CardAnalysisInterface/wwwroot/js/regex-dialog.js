// wwwroot/js/regexEditorInterop.js

let editorDotNetReference = null;
let editorElement = null;
let isInternallyChanging = false;

function initializeEditor(_dotNetReference, _editorElement) {
    editorDotNetReference = _dotNetReference;
    editorElement = _editorElement;
    if (editorElement) {
        // MODIFIED: Added 'beforeinput' listener for atomic deletions
        editorElement.addEventListener('beforeinput', onBeforeInput);
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
        // MODIFIED: Remove 'beforeinput' listener
        editorElement.removeEventListener('beforeinput', onBeforeInput);
        editorElement.removeEventListener('input', onEditorInput);
        editorElement.removeEventListener('keydown', onEditorKeyDown);
        editorElement.removeEventListener('blur', onEditorBlur);
    }
    document.removeEventListener('mousedown', onDropdownMouseDown);
    document.removeEventListener('keydown', onGlobalKeyDown);
    editorDotNetReference = null;
    editorElement = null;
}

// NEW: Handles all deletion events to make tokens "atomic".
function onBeforeInput(event) {
    // We only care about deletion events.
    if (!event.inputType.startsWith('delete') && event.inputType !== 'insertText') {
        return;
    }

    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0) return;

    const range = selection.getRangeAt(0);
    const tokensToDelete = new Set();
    const allTokens = Array.from(editorElement.querySelectorAll('.token-style'));

    // --- Main Logic ---
    // 1. Find all tokens that intersect with the current selection range. This handles
    //    highlighted selections that are inside, across, or partially touching tokens.
    for (const token of allTokens) {
        const tokenRange = document.createRange();
        tokenRange.selectNode(token);

        // A selection intersects a token if it does NOT start after the token ends,
        // AND it does NOT end before the token starts.
        const selectionStartsAfterTokenEnds = range.compareBoundaryPoints(Range.START_TO_END, tokenRange) >= 0;
        const selectionEndsBeforeTokenStarts = range.compareBoundaryPoints(Range.END_TO_START, tokenRange) <= 0;

        if (!selectionStartsAfterTokenEnds && !selectionEndsBeforeTokenStarts) {
            tokensToDelete.add(token);
        }
    }

    // 2. If the selection is just a caret (collapsed), check for adjacency.
    //    This handles pressing Backspace right after a token, or Delete right before it.
    if (range.collapsed) {
        const container = range.startContainer;
        const offset = range.startOffset;
        let adjacentNode = null;

        if (event.inputType === 'deleteContentBackward') {
            // Caret is at `|text` -> check node before text node
            if (container.nodeType === Node.TEXT_NODE && offset === 0) {
                adjacentNode = container.previousSibling;
            }
            // Caret is at `...</span>|` -> offset is index of caret position among children
            else if (container.nodeType === Node.ELEMENT_NODE && offset > 0) {
                adjacentNode = container.childNodes[offset - 1];
            }
        } else if (event.inputType === 'deleteContentForward') {
            // Caret is at `text|` -> check node after text node
            if (container.nodeType === Node.TEXT_NODE && offset === container.textContent.length) {
                adjacentNode = container.nextSibling;
            }
            // Caret is at `|<span...`
            else if (container.nodeType === Node.ELEMENT_NODE && offset < container.childNodes.length) {
                adjacentNode = container.childNodes[offset];
            }
        }

        if (adjacentNode && adjacentNode.nodeType === Node.ELEMENT_NODE && adjacentNode.classList.contains('token-style')) {
            tokensToDelete.add(adjacentNode);
        }
    }


    if (tokensToDelete.size === 0) {
        return;
    }

    // If we have tokens to delete, we take over the deletion process.
    event.preventDefault();

    // To place the cursor correctly, find the character position of the
    // first token that's about to be deleted.
    let cursorPosition = -1;
    const sortedTokens = [...tokensToDelete].sort((a, b) => a.compareDocumentPosition(b) & Node.DOCUMENT_POSITION_FOLLOWING ? -1 : 1);

    if (sortedTokens.length > 0) {
        let tempRange = document.createRange();
        tempRange.selectNodeContents(editorElement);
        tempRange.setEnd(sortedTokens[0], 0);
        cursorPosition = tempRange.toString().length;
    }

    // Remove the targeted tokens from the DOM.
    sortedTokens.forEach(t => t.remove());

    // Refresh the editor's state and move the cursor.
    highlightAndRestoreCursor(getEditorRawText(), cursorPosition);
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

function commitToken(textToReplace, fullTokenText) {
    const { range } = getCaretPositionInfo() || {};
    if (!range) return;
    const textToInsert = fullTokenText + '\u00A0';
    range.setStart(range.startContainer, range.startOffset - textToReplace.length);
    range.deleteContents();
    range.insertNode(document.createTextNode(textToInsert));
    const finalCursorPos = range.startOffset + textToInsert.length;
    highlightAndRestoreCursor(editorElement.textContent, finalCursorPos);
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
        const navKeys = ['Enter', 'Tab', 'ArrowUp', 'ArrowDown'];
        if (navKeys.includes(event.key)) {
            event.preventDefault();
        }
        return;
    }

    if (event.key === ' ' || event.key === 'Enter') {
        setTimeout(() => highlightAndRestoreCursor(editorElement.textContent, getCaretCharacterOffsetWithin(editorElement)), 0);
    }
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

function onEditorInput() {
    // The onBeforeInput handles the deletion, so this only needs to fire
    // after a change has been committed to the DOM.
    if (isInternallyChanging) return;
    const { currentWord } = getCaretPositionInfo() || {};
    editorDotNetReference.invokeMethodAsync('UpdateFromJavaScript', editorElement.textContent, currentWord || '');
}

function onEditorBlur() {
    highlightAndRestoreCursor(editorElement.textContent, -1);
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
    // We call this after our internal change to sync Blazor's state.
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
    const walker = document.createTreeWalker(element, NodeFilter.SHOW_TEXT, null, false);
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