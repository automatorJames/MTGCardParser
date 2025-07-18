// wwwroot/js/regexEditorInterop.js

// Global state variables for the editor instance
let editorDotNetReference = null;
let editorElement = null;

// --- Public Functions (Callable from Blazor) ---

function initializeEditor(_dotNetReference, _editorElement) {
    editorDotNetReference = _dotNetReference;
    editorElement = _editorElement;

    if (editorElement) {
        editorElement.addEventListener('input', onEditorInput);
        editorElement.focus();

        document.addEventListener('mousedown', onDropdownMouseDown);
        // *** FIX: Add back the global key listener for Escape. ***
        document.addEventListener('keydown', onGlobalKeyDown);
    }
}

function disposeEditor() {
    if (editorElement) {
        editorElement.removeEventListener('input', onEditorInput);
    }
    document.removeEventListener('mousedown', onDropdownMouseDown);
    // *** FIX: Remove the global listener on cleanup. ***
    document.removeEventListener('keydown', onGlobalKeyDown);
    editorDotNetReference = null;
    editorElement = null;
}

function getEditorRawText() {
    if (!editorElement) return '';
    let rawText = '';
    editorElement.childNodes.forEach(node => {
        rawText += getRawTextFromNode(node);
    });
    // Strip the zero-width spaces from the final output
    return rawText.replace(/\u200B/g, '');
}

function scrollToAutocompleteItem(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ block: 'nearest', inline: 'nearest' });
    }
}

function insertPillIntoEditor(displayText, rawText, color) {
    const caretInfo = getCaretPositionInfo();
    if (!caretInfo || !caretInfo.selection || !caretInfo.range) return;

    const { range, currentWord } = caretInfo;

    if (currentWord && currentWord.startsWith('@')) {
        range.setStart(range.startContainer, range.startOffset - currentWord.length);
        range.deleteContents();
    }

    performInsertion(range, displayText, rawText, color);
}

// --- Internal Helper Functions & Event Handlers ---

// *** FIX: New handler for the global keydown event. ***
function onGlobalKeyDown(event) {
    if (event.key === 'Escape' && editorDotNetReference) {
        // Stop the event from bubbling up further.
        event.stopPropagation();
        // Let the C# component handle the logic.
        editorDotNetReference.invokeMethodAsync('HandleGlobalEscape');
    }
}

function onDropdownMouseDown(event) {
    const item = event.target.closest('.autocomplete-item');
    if (!item) return;

    event.preventDefault();

    const displayText = item.dataset.displayText;
    const rawText = item.dataset.rawText;
    const color = item.dataset.color;

    const caretInfo = getCaretPositionInfo();
    if (!caretInfo) return;

    const { range, currentWord } = caretInfo;

    if (currentWord && currentWord.startsWith('@')) {
        range.setStart(range.startContainer, range.startOffset - currentWord.length);
        range.deleteContents();
    }

    performInsertion(range, displayText, rawText, color);

    if (editorDotNetReference) {
        editorDotNetReference.invokeMethodAsync('HideDropdown');
    }
}

function performInsertion(range, displayText, rawText, color) {
    const pillEl = createPillElement(displayText, rawText, color);
    range.insertNode(pillEl);

    const zeroWidthSpace = document.createTextNode('\u200B');
    range.setStartAfter(pillEl);
    range.collapse(true);
    range.insertNode(zeroWidthSpace);
    range.setStartAfter(zeroWidthSpace);
    range.collapse(true);

    const selection = window.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);

    onEditorInput();
}


function onEditorInput(event) {
    if (!editorDotNetReference) return;
    const rawText = getEditorRawText();
    const caretInfo = getCaretPositionInfo();
    const currentWord = (caretInfo && caretInfo.currentWord) ? caretInfo.currentWord : '';
    editorDotNetReference.invokeMethodAsync('UpdateFromJavaScript', rawText, currentWord);
}

function getRawTextFromNode(node) {
    if (node.nodeType === Node.TEXT_NODE) {
        return node.textContent;
    }
    if (node.nodeType === Node.ELEMENT_NODE && node.classList.contains('pill-wrapper')) {
        return node.dataset.rawText || '';
    }
    let text = '';
    if (node.childNodes) {
        for (const child of node.childNodes) {
            text += getRawTextFromNode(child);
        }
    }
    return text;
}

function createPillElement(displayText, rawText, color) {
    const pillWrapper = document.createElement('span');
    pillWrapper.className = 'pill-wrapper';
    pillWrapper.contentEditable = 'false';
    pillWrapper.dataset.rawText = rawText;
    pillWrapper.style.display = 'inline-block';
    const pill = document.createElement('span');
    pill.style.backgroundColor = color;
    pill.style.color = 'white';
    pill.style.padding = '2px 8px';
    pill.style.borderRadius = '12px';
    pill.style.margin = '0 2px';
    pill.style.userSelect = 'none';
    pill.textContent = displayText;
    pillWrapper.appendChild(pill);
    return pillWrapper;
}

function getCaretPositionInfo() {
    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0 || !editorElement.contains(selection.anchorNode)) {
        return null;
    }
    const range = selection.getRangeAt(0);
    const preCaretRange = range.cloneRange();
    preCaretRange.selectNodeContents(editorElement);
    preCaretRange.setEnd(range.startContainer, range.startOffset);
    const text = preCaretRange.toString();
    const words = text.split(/[\s\u00A0\u200B]+/);
    const currentWord = words[words.length - 1];
    return {
        selection,
        range,
        currentWord
    };
}