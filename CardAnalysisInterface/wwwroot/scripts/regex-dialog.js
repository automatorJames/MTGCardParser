let editorDotNetReference = null;
let editorElement = null;
let isInternallyChanging = false;
let typeColors = {}; // Now stores { Normal, Highlight }

function initializeEditor(_dotNetReference, _editorElement, _colors) {
    editorDotNetReference = _dotNetReference;
    editorElement = _editorElement;
    typeColors = _colors || {};
    if (editorElement) {
        editorElement.addEventListener('beforeinput', onBeforeInput);
        editorElement.addEventListener('input', onEditorInput);
        editorElement.addEventListener('keydown', onEditorKeyDown);
        editorElement.addEventListener('mousedown', onEditorMouseDown);
        document.addEventListener('mousedown', onDropdownMouseDown);
        document.addEventListener('keydown', onGlobalKeyDown);
        editorElement.focus();
    }
}

function disposeEditor() {
    if (editorElement) {
        editorElement.removeEventListener('beforeinput', onBeforeInput);
        editorElement.removeEventListener('input', onEditorInput);
        editorElement.removeEventListener('keydown', onEditorKeyDown);
        editorElement.removeEventListener('mousedown', onEditorMouseDown);
    }
    document.removeEventListener('mousedown', onDropdownMouseDown);
    document.removeEventListener('keydown', onGlobalKeyDown);
    editorDotNetReference = null;
    editorElement = null;
}

function onEditorMouseDown(e) {
    // Requirement 4: Mouse clicks on tokens highlight them
    const token = e.target.closest('.token-style');
    if (token) {
        e.preventDefault();
        e.stopPropagation();
        clearTokenHighlights();
        setTokenHighlight(token, true);
        editorElement.focus();
    } else {
        clearTokenHighlights();
    }
}

function onBeforeInput(event) {
    if (!event.inputType.startsWith('delete') && event.inputType !== 'insertText') return;

    const highlighted = editorElement.querySelector('.token-selected');

    // Requirement 4: Delete/Backspace removes highlighted token
    if (highlighted && (event.inputType === 'deleteContentBackward' || event.inputType === 'deleteContentForward')) {
        event.preventDefault();
        highlighted.remove();
        onEditorInput();
        highlightAndRestoreCursor(editorElement.textContent, getCaretCharacterOffsetWithin(editorElement));
        return;
    }

    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0) return;

    const range = selection.getRangeAt(0);
    const tokensToDelete = new Set();
    const allTokens = Array.from(editorElement.querySelectorAll('.token-style'));

    for (const token of allTokens) {
        const tokenRange = document.createRange();
        tokenRange.selectNode(token);
        const intersect = !(range.compareBoundaryPoints(Range.START_TO_END, tokenRange) >= 0 ||
            range.compareBoundaryPoints(Range.END_TO_START, tokenRange) <= 0);
        if (intersect) tokensToDelete.add(token);
    }

    if (range.collapsed) {
        const container = range.startContainer;
        const offset = range.startOffset;
        let adj = null;

        if (event.inputType === 'deleteContentBackward') {
            if (container.nodeType === Node.TEXT_NODE && offset === 0) adj = container.previousSibling;
            else if (container.nodeType === Node.ELEMENT_NODE && offset > 0) adj = container.childNodes[offset - 1];
        } else if (event.inputType === 'deleteContentForward') {
            if (container.nodeType === Node.TEXT_NODE && offset === container.textContent.length) adj = container.nextSibling;
            else if (container.nodeType === Node.ELEMENT_NODE && offset < container.childNodes.length) adj = container.childNodes[offset];
        }

        if (adj && adj.classList && adj.classList.contains('token-style')) tokensToDelete.add(adj);
    }

    if (tokensToDelete.size > 0) {
        event.preventDefault();
        tokensToDelete.forEach(t => t.remove());
        onEditorInput();
        // Re-render to ensure text flow is correct after removal
        highlightAndRestoreCursor(editorElement.textContent, getCaretCharacterOffsetWithin(editorElement));
    }
}

function onEditorKeyDown(event) {
    const dropdown = document.getElementById('autocomplete-dropdown-list');
    if (dropdown && dropdown.offsetParent !== null) {
        if (['Enter', 'Tab', 'ArrowUp', 'ArrowDown'].includes(event.key)) event.preventDefault();
        return;
    }

    // Requirement 3: Arrow navigation highlighting
    if (event.key === 'ArrowRight' || event.key === 'ArrowLeft') {
        const selection = window.getSelection();
        if (!selection.rangeCount) return;
        const range = selection.getRangeAt(0);

        if (range.collapsed) {
            const isRight = event.key === 'ArrowRight';
            const container = range.startContainer;
            const offset = range.startOffset;

            let targetToken = null;
            if (isRight) {
                if (container.nodeType === Node.TEXT_NODE && offset === container.textContent.length) targetToken = container.nextSibling;
                else if (container.nodeType === Node.ELEMENT_NODE) targetToken = container.childNodes[offset];
            } else {
                if (container.nodeType === Node.TEXT_NODE && offset === 0) targetToken = container.previousSibling;
                else if (container.nodeType === Node.ELEMENT_NODE && offset > 0) targetToken = container.childNodes[offset - 1];
            }

            if (targetToken && targetToken.classList && targetToken.classList.contains('token-style')) {
                if (!targetToken.classList.contains('token-selected')) {
                    event.preventDefault();
                    clearTokenHighlights();
                    setTokenHighlight(targetToken, true);
                } else {
                    event.preventDefault();
                    setTokenHighlight(targetToken, false);
                    const newRange = document.createRange();
                    if (isRight) newRange.setStartAfter(targetToken);
                    else newRange.setStartBefore(targetToken);
                    newRange.collapse(true);
                    selection.removeAllRanges();
                    selection.addRange(newRange);
                }
                return;
            }
        }
    }

    clearTokenHighlights();

    if (event.key === ' ' || event.key === 'Enter') {
        setTimeout(() => highlightAndRestoreCursor(editorElement.textContent, getCaretCharacterOffsetWithin(editorElement)), 0);
    }
}

function clearTokenHighlights() {
    editorElement.querySelectorAll('.token-selected').forEach(t => setTokenHighlight(t, false));
}

function setTokenHighlight(token, isHighlighted) {
    const typeName = token.getAttribute('data-type-name');
    const colors = typeColors[typeName];
    if (isHighlighted) {
        token.classList.add('token-selected');
        if (colors) token.style.backgroundColor = colors.highlight;
    } else {
        token.classList.remove('token-selected');
        if (colors) token.style.backgroundColor = colors.normal;
    }
}

function commitToken(textToReplace, fullTokenText) {
    // Requirement 2: Ensure focus and proper cursor placement after token commit
    const pos = getCaretCharacterOffsetWithin(editorElement);
    const fullText = editorElement.textContent;

    // Inserting a non-breaking space after the token ensures the cursor has a text node to land in
    const textToInsert = fullTokenText + '\u00A0';
    const newText = fullText.substring(0, pos - textToReplace.length) + textToInsert + fullText.substring(pos);

    highlightAndRestoreCursor(newText, pos - textToReplace.length + textToInsert.length);
    editorElement.focus();
}

function onEditorInput() {
    if (isInternallyChanging) return;
    const { currentWord } = getCaretPositionInfo() || {};
    editorDotNetReference.invokeMethodAsync('UpdateFromJavaScript', editorElement.textContent, currentWord || '');
}

function highlightAndRestoreCursor(text, cursorPos) {
    if (isInternallyChanging) return;
    isInternallyChanging = true;

    editorElement.innerHTML = '';
    const tokenRegex = /(@(\w+))/g;
    let lastIndex = 0;
    let match;

    while ((match = tokenRegex.exec(text)) !== null) {
        if (match.index > lastIndex) {
            editorElement.appendChild(document.createTextNode(text.substring(lastIndex, match.index)));
        }

        const typeName = match[2];
        const colors = typeColors[typeName] || { normal: '#4A5568', highlight: '#718096' };

        const span = document.createElement('span');
        span.className = 'token-style';
        span.contentEditable = 'false';
        span.style.backgroundColor = colors.normal;
        span.setAttribute('data-type-name', typeName);

        // Requirement 5: Hide @ visually but keep it in textContent for downstream regex
        span.innerHTML = `<span style="display:none">@</span><span>${typeName}</span>`;

        editorElement.appendChild(span);
        lastIndex = match.index + match[0].length;
    }

    if (lastIndex < text.length) {
        editorElement.appendChild(document.createTextNode(text.substring(lastIndex)));
    }

    // Ensure we have at least one text node at the end to allow cursor placement
    if (editorElement.childNodes.length === 0 || editorElement.lastChild.nodeType !== Node.TEXT_NODE) {
        editorElement.appendChild(document.createTextNode(''));
    }

    if (cursorPos >= 0) {
        const result = findNodeAndOffset(editorElement, cursorPos);
        if (result && result.node) {
            const range = document.createRange();
            const selection = window.getSelection();
            try {
                range.setStart(result.node, result.offset);
                range.collapse(true);
                selection.removeAllRanges();
                selection.addRange(range);
            } catch (e) { console.error("Cursor restoration failed", e); }
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
    // Fallback to the very last text node
    const lastTextNode = Array.from(element.childNodes).reverse().find(n => n.nodeType === Node.TEXT_NODE);
    return { node: lastTextNode || element, offset: lastTextNode ? lastTextNode.textContent.length : 0 };
}

function scrollToAutocompleteItem(elementId) {
    const element = document.getElementById(elementId);
    if (element) element.scrollIntoView({ block: 'nearest' });
}

function onDropdownMouseDown(event) {
    const item = event.target.closest('.autocomplete-item');
    if (!item) return;
    event.preventDefault();
    const typeName = item.querySelector('.type-name').textContent.trim();
    commitToken(editorDotNetReference.lastAutocompleteFilter || "@", "@" + typeName);
    editorDotNetReference.invokeMethodAsync('HideDropdown');
}

function onGlobalKeyDown(event) {
    if (event.key === 'Escape' && editorDotNetReference) {
        editorDotNetReference.invokeMethodAsync('HandleGlobalEscape');
    }
}