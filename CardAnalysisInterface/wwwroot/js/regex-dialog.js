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
        editorElement.addEventListener('keydown', onEditorKeyDown); // For smart backspace and event prevention
        editorElement.focus();

        document.addEventListener('mousedown', onDropdownMouseDown);
        document.addEventListener('keydown', onGlobalKeyDown);

        ensureInitialAnchor();
    }
}

function disposeEditor() {
    if (editorElement) {
        editorElement.removeEventListener('input', onEditorInput);
        editorElement.removeEventListener('keydown', onEditorKeyDown);
    }
    document.removeEventListener('mousedown', onDropdownMouseDown);
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
        // *** FIX: Only prevent default for keys that navigate or select from the dropdown. ***
        if (event.key === 'Enter' || event.key === 'Tab' || event.key === 'ArrowUp' || event.key === 'ArrowDown') {
            event.preventDefault();
        }
    }

    if (event.key !== 'Backspace') return;

    const selection = window.getSelection();
    if (!selection.isCollapsed) return;

    const range = selection.getRangeAt(0);
    const node = range.startContainer;
    const offset = range.startOffset;

    if (node.nodeType === Node.TEXT_NODE && offset > 0 && node.nodeValue.substring(offset - 1, offset) === '\u200B') {
        let previousElement = node.previousSibling;
        if (offset === 1 && node.nodeValue.length === 1) {
            previousElement = node.previousElementSibling;
        }

        if (previousElement && previousElement.classList && previousElement.classList.contains('pill-wrapper')) {
            event.preventDefault();
            previousElement.remove();
            onEditorInput();
            return;
        }
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
    ensureInitialAnchor();
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

function ensureInitialAnchor() {
    if (editorElement && (editorElement.childNodes.length === 0 || editorElement.firstChild.nodeValue !== '\u200B')) {
        const zeroWidthSpace = document.createTextNode('\u200B');
        editorElement.insertBefore(zeroWidthSpace, editorElement.firstChild);
    }
}