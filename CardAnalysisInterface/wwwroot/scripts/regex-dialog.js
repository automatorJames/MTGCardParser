let editorDotNetReference = null;
let editorElement = null;
let isInternallyChanging = false;
let typeColors = {};

function initializeEditor(_dotNetReference, _editorElement, _colors) {
    editorDotNetReference = _dotNetReference;
    editorElement = _editorElement;
    typeColors = _colors || {};
    if (editorElement) {
        editorElement.addEventListener('beforeinput', onBeforeInput);
        editorElement.addEventListener('input', onEditorInput);
        editorElement.addEventListener('keydown', onEditorKeyDown);
        editorElement.addEventListener('mousedown', onEditorMouseDown);
        editorElement.addEventListener('contextmenu', onPillContextMenu);
        document.addEventListener('mousedown', onDropdownMouseDown);
        document.addEventListener('keydown', onGlobalKeyDown);

        setTimeout(() => editorElement.focus(), 10);
    }
}

function disposeEditor() {
    if (editorElement) {
        editorElement.removeEventListener('beforeinput', onBeforeInput);
        editorElement.removeEventListener('input', onEditorInput);
        editorElement.removeEventListener('keydown', onEditorKeyDown);
        editorElement.removeEventListener('mousedown', onEditorMouseDown);
        editorElement.removeEventListener('contextmenu', onPillContextMenu);
    }
    document.removeEventListener('mousedown', onDropdownMouseDown);
    document.removeEventListener('keydown', onGlobalKeyDown);
    editorDotNetReference = null;
    editorElement = null;
}

function onPillContextMenu(e) {
    const token = e.target.closest('.token-style');
    if (token) {
        e.preventDefault();
        const typeName = token.getAttribute('data-type-name');
        const pillIndex = parseInt(token.getAttribute('data-pill-index')); // Refinement 1
        if (editorDotNetReference) {
            editorDotNetReference.invokeMethodAsync('OpenPillMenu', typeName, pillIndex, e.clientX, e.clientY);
        }
    }
}

function onEditorMouseDown(e) {
    const token = e.target.closest('.token-style');
    if (token) {
        e.preventDefault();
        e.stopPropagation();
        clearTokenHighlights();
        setTokenHighlight(token, true);
        const range = document.createRange();
        range.selectNode(token);
        const selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(range);
        editorElement.focus();
    } else {
        clearTokenHighlights();
    }
}

function onBeforeInput(event) {
    if (!event.inputType.startsWith('delete') && event.inputType !== 'insertText') return;
    const highlighted = editorElement.querySelector('.token-selected');
    if (highlighted && (event.inputType === 'deleteContentBackward' || event.inputType === 'deleteContentForward')) {
        event.preventDefault();
        const pos = getOffsetOfNode(editorElement, highlighted);
        highlighted.remove();
        highlightAndRestoreCursor(editorElement.textContent, pos);
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
        const firstToken = Array.from(tokensToDelete)[0];
        const pos = getOffsetOfNode(editorElement, firstToken);
        tokensToDelete.forEach(t => t.remove());
        highlightAndRestoreCursor(editorElement.textContent, pos);
    }
}

function onEditorKeyDown(event) {
    const dropdown = document.getElementById('autocomplete-dropdown-list');
    if (dropdown && dropdown.offsetParent !== null) {
        if (['Enter', 'Tab', 'ArrowUp', 'ArrowDown'].includes(event.key)) event.preventDefault();
        return;
    }
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
                event.preventDefault();
                if (!targetToken.classList.contains('token-selected')) {
                    clearTokenHighlights();
                    setTokenHighlight(targetToken, true);
                    const newRange = document.createRange();
                    newRange.selectNode(targetToken);
                    selection.removeAllRanges();
                    selection.addRange(newRange);
                } else {
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
    const pos = getCaretCharacterOffsetWithin(editorElement);
    const fullText = editorElement.textContent;
    const textToInsert = fullTokenText + '\u00A0';
    const newText = fullText.substring(0, pos - textToReplace.length) + textToInsert + fullText.substring(pos);
    highlightAndRestoreCursor(newText, pos - textToReplace.length + textToInsert.length);
    setTimeout(() => editorElement.focus(), 0);
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
    const tokenRegex = /(@(\w+)[?*+]?)/g;
    let lastIndex = 0;
    let pillCounter = 0; // Refinement 1
    let match;
    while ((match = tokenRegex.exec(text)) !== null) {
        if (match.index > lastIndex) {
            editorElement.appendChild(document.createTextNode(text.substring(lastIndex, match.index)));
        }
        const fullMatchText = match[1];
        const typeName = match[2];
        const colors = typeColors[typeName] || { normal: '#4A5568', highlight: '#718096' };
        const span = document.createElement('span');
        span.className = 'token-style';
        span.contentEditable = 'false';
        span.style.backgroundColor = colors.normal;
        span.setAttribute('data-type-name', typeName);
        span.setAttribute('data-pill-index', pillCounter++); // Refinement 1
        span.innerHTML = `<span style="display:none">@</span><span>${fullMatchText.substring(1)}</span>`;
        editorElement.appendChild(span);
        lastIndex = match.index + match[0].length;
    }
    if (lastIndex < text.length) {
        editorElement.appendChild(document.createTextNode(text.substring(lastIndex)));
    }
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
            } catch (e) { }
        }
    }
    isInternallyChanging = false;
}

function getCaretPositionInfo() {
    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0) return null;
    const pos = getCaretCharacterOffsetWithin(editorElement);
    const textBeforeCaret = editorElement.textContent.substring(0, pos);
    const words = textBeforeCaret.split(/[\s\u00A0]+/);
    return { currentWord: words[words.length - 1] };
}

function getCaretCharacterOffsetWithin(element) {
    const selection = window.getSelection();
    if (selection.rangeCount === 0) return 0;
    const range = selection.getRangeAt(0);
    const preCaretRange = range.cloneRange();
    preCaretRange.selectNodeContents(element);
    preCaretRange.setEnd(range.startContainer, range.startOffset);
    return preCaretRange.toString().length;
}

function getOffsetOfNode(root, node) {
    const range = document.createRange();
    range.selectNodeContents(root);
    range.setEndBefore(node);
    return range.toString().length;
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
    if (editorDotNetReference) {
        editorDotNetReference.invokeMethodAsync('SelectSuggestionFromJS', typeName);
    }
}

function onGlobalKeyDown(event) {
    if (event.key === 'Escape' && editorDotNetReference) {
        editorDotNetReference.invokeMethodAsync('HandleGlobalEscape');
    }
}