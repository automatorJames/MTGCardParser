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

        setTimeout(() => {
            if (editorElement) editorElement.focus();
        }, 10);
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
        const dataSnippetId = token.getAttribute('data-snippet-id');
        if (editorDotNetReference) {
            editorDotNetReference.invokeMethodAsync('OpenPillMenu', typeName, dataSnippetId, e.clientX, e.clientY);
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
        onEditorInput();
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
                if (!targetToken.classList.contains('token-selected')) {
                    event.preventDefault();
                    clearTokenHighlights();
                    setTokenHighlight(targetToken, true);

                    const newRange = document.createRange();
                    newRange.selectNode(targetToken);
                    selection.removeAllRanges();
                    selection.addRange(newRange);
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
}

function clearTokenHighlights() {
    if (editorElement) {
        editorElement.querySelectorAll('.token-selected').forEach(t => setTokenHighlight(t, false));
    }
}

function setTokenHighlight(token, isHighlighted) {
    const typeName = token.getAttribute('data-type-name');
    const baseTypeMatch = typeName.match(/<([^>]+)>/) || [null, typeName.replace('@', '')];
    const colors = typeColors[baseTypeMatch[1]] || { normal: '#4A5568', highlight: '#718096' };

    if (isHighlighted) {
        token.classList.add('token-selected');
        token.style.backgroundColor = colors.highlight;
    } else {
        token.classList.remove('token-selected');
        token.style.backgroundColor = colors.normal;
    }
}

function commitToken(textToReplace, fullTokenText) {
    const pos = getCaretCharacterOffsetWithin(editorElement);
    const fullText = editorElement.textContent;

    const textToInsert = fullTokenText + '\u00A0';
    const newText = fullText.substring(0, pos - textToReplace.length) + textToInsert + fullText.substring(pos);

    // We force a high-level update so Blazor knows the dropdown is closed 
    // and can safely call highlightAndRestoreCursor with the new IDs
    isInternallyChanging = false;
    editorElement.textContent = newText;
    onEditorInput();

    setTimeout(() => {
        if (editorElement) editorElement.focus();
    }, 0);
}

function onEditorInput() {
    if (isInternallyChanging || !editorElement) return;
    const { currentWord } = getCaretPositionInfo() || {};
    editorDotNetReference.invokeMethodAsync('UpdateFromJavaScript', editorElement.textContent, currentWord || '');
}

function highlightAndRestoreCursor(text, cursorPos, snippetMetadata) {
    if (isInternallyChanging || !editorElement) return;
    isInternallyChanging = true;

    editorElement.innerHTML = '';
    const tokenRegex = /(@[\w<>]+(\([^)]*\))?)/g;
    let lastIndex = 0;
    let match;

    let metaQueue = snippetMetadata ? [...snippetMetadata] : [];

    while ((match = tokenRegex.exec(text)) !== null) {
        if (match.index > lastIndex) {
            editorElement.appendChild(document.createTextNode(text.substring(lastIndex, match.index)));
        }

        const fullMatchText = match[0];
        let dataSnippetId = "";

        // Only convert to a pill if we have valid metadata (ID) from the backend
        const metaIdx = metaQueue.findIndex(m => m.typeName === fullMatchText);
        if (metaIdx !== -1) {
            dataSnippetId = metaQueue[metaIdx].id;
            metaQueue.splice(metaIdx, 1);

            const baseTypeMatch = fullMatchText.match(/<([^>]+)>/) || [null, fullMatchText.substring(1)];
            const colors = typeColors[baseTypeMatch[1]] || { normal: '#4A5568', highlight: '#718096' };

            const span = document.createElement('span');
            span.className = 'token-style';
            span.contentEditable = 'false';
            span.style.backgroundColor = colors.normal;
            span.setAttribute('data-type-name', fullMatchText);
            span.setAttribute('data-snippet-id', dataSnippetId);
            span.innerHTML = `<span style="display:none">@</span><span>${fullMatchText.substring(1)}</span>`;
            editorElement.appendChild(span);
        } else {
            // Keep as plain text if no ID yet (allows typing filter)
            editorElement.appendChild(document.createTextNode(fullMatchText));
        }

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

    const node = selection.anchorNode;
    if (!node || node.nodeType !== Node.TEXT_NODE) {
        return { currentWord: "" };
    }

    const offset = selection.anchorOffset;
    const textUpToCaret = node.textContent.substring(0, offset);
    const words = textUpToCaret.split(/[\s\u00A0]+/);
    const lastWord = words[words.length - 1];

    return { currentWord: lastWord };
}

function getCaretCharacterOffsetWithin(element) {
    const selection = window.getSelection();
    if (!selection || selection.rangeCount === 0) return 0;
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