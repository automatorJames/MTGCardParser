class RegexEditor {
    dotNetRef = null;
    el = null;
    typeColors = {};
    isInternallyChanging = false;
    initialize(ref, element, colors) {
        this.dotNetRef = ref;
        this.el = element;
        this.typeColors = colors;
        this.el.addEventListener('beforeinput', this.onBeforeInput);
        this.el.addEventListener('input', this.onInput);
        this.el.addEventListener('keydown', this.onKeyDown);
        this.el.addEventListener('mousedown', this.onMouseDown);
        this.el.addEventListener('contextmenu', this.onContextMenu);
        document.addEventListener('mousedown', this.onGlobalMouseDown);
        document.addEventListener('keydown', this.onGlobalKeyDown);
        setTimeout(() => this.el?.focus(), 10);
    }
    dispose() {
        if (this.el) {
            this.el.removeEventListener('beforeinput', this.onBeforeInput);
            this.el.removeEventListener('input', this.onInput);
            this.el.removeEventListener('keydown', this.onKeyDown);
            this.el.removeEventListener('mousedown', this.onMouseDown);
            this.el.removeEventListener('contextmenu', this.onContextMenu);
        }
        document.removeEventListener('mousedown', this.onGlobalMouseDown);
        document.removeEventListener('keydown', this.onGlobalKeyDown);
        this.dotNetRef = null;
        this.el = null;
    }
    focusClassNameInput(selector) {
        const el = document.querySelector(selector);
        if (el) {
            el.focus();
            const val = el.value;
            el.value = '';
            el.value = val;
        }
    }
    scrollToAutocompleteItem(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollIntoView({ block: 'nearest' });
        }
    }
    syncPills(text, cursorPos, metadata) {
        if (!this.el)
            return;
        // Block input events while we are reconstructing the DOM
        this.isInternallyChanging = true;
        this.el.innerHTML = '';
        const tokenRegex = /(@[\w<>]+(\([^)]*\))?)/g;
        let lastIndex = 0;
        let match;
        const metaQueue = [...metadata];
        while ((match = tokenRegex.exec(text)) !== null) {
            if (match.index > lastIndex) {
                this.el.appendChild(document.createTextNode(text.substring(lastIndex, match.index)));
            }
            const fullMatchText = match[0];
            const metaIdx = metaQueue.findIndex(m => m.typeName === fullMatchText);
            if (metaIdx !== -1) {
                const meta = metaQueue.splice(metaIdx, 1)[0];
                this.el.appendChild(this.createPillElement(fullMatchText, meta.id));
            }
            else {
                this.el.appendChild(document.createTextNode(fullMatchText));
            }
            lastIndex = match.index + fullMatchText.length;
        }
        if (lastIndex < text.length) {
            this.el.appendChild(document.createTextNode(text.substring(lastIndex)));
        }
        // Ensure there is a trailing text node if the last item is a pill
        if (this.el.childNodes.length === 0 || this.el.lastChild?.nodeType !== Node.TEXT_NODE) {
            this.el.appendChild(document.createTextNode(''));
        }
        if (cursorPos >= 0) {
            // Delay slightly to ensure DOM is painted and focusable
            requestAnimationFrame(() => this.restoreCursor(cursorPos));
        }
        this.isInternallyChanging = false;
    }
    insertPill(textToReplace, fullTokenText) {
        if (!this.el || !this.dotNetRef)
            return;
        const pos = this.getCaretOffset();
        const fullText = this.el.textContent || "";
        // We add a non-breaking space after the pill so the user isn't stuck "inside" the pill logic
        const textToInsert = fullTokenText + '\u00A0';
        const head = fullText.substring(0, pos - textToReplace.length);
        const tail = fullText.substring(pos);
        const newText = head + textToInsert + tail;
        // Calculate exactly where the cursor should be: Start of match + length of new token + 1 for the space
        const newCaretPos = head.length + textToInsert.length;
        this.isInternallyChanging = true;
        this.el.textContent = newText;
        // Pass the new text AND the intended caret position to Blazor
        this.dotNetRef.invokeMethodAsync('NotifyContentChanged', newText, "", newCaretPos);
        this.isInternallyChanging = false;
        // Ensure focus remains on the editor
        this.el.focus();
    }
    createPillElement(text, id) {
        const baseTypeName = text.match(/<([^>]+)>/)?.[1] || text.substring(1);
        const colors = this.typeColors[baseTypeName] || { normal: '#4A5568', highlight: '#718096' };
        const span = document.createElement('span');
        span.className = 'token-style';
        span.contentEditable = 'false';
        span.style.backgroundColor = colors.normal;
        span.setAttribute('data-type-name', text);
        span.setAttribute('data-snippet-id', id);
        // The hidden @ allows the textContent property to still see the @ symbol for regex parsing
        span.innerHTML = `<span style="display:none">@</span><span>${text.substring(1)}</span>`;
        return span;
    }
    onInput = () => {
        if (this.isInternallyChanging || !this.el || !this.dotNetRef)
            return;
        const info = this.getCaretPositionInfo();
        this.dotNetRef.invokeMethodAsync('NotifyContentChanged', this.el.textContent, info.currentWord, -1);
    };
    onBeforeInput = (e) => {
        if (!this.el)
            return;
        const selection = window.getSelection();
        if (!selection?.rangeCount)
            return;
        const range = selection.getRangeAt(0);
        const tokensToDelete = new Set();
        const allTokens = Array.from(this.el.querySelectorAll('.token-style'));
        const highlighted = this.el.querySelector('.token-selected');
        if (highlighted && e.inputType.startsWith('delete')) {
            e.preventDefault();
            highlighted.remove();
            this.onInput();
            return;
        }
        allTokens.forEach(token => {
            const tokenRange = document.createRange();
            tokenRange.selectNode(token);
            const intersect = !(range.compareBoundaryPoints(Range.START_TO_END, tokenRange) >= 0 ||
                range.compareBoundaryPoints(Range.END_TO_START, tokenRange) <= 0);
            if (intersect)
                tokensToDelete.add(token);
        });
        if (tokensToDelete.size > 0) {
            e.preventDefault();
            tokensToDelete.forEach(t => t.remove());
            this.onInput();
        }
    };
    onKeyDown = (e) => {
        const dropdown = document.getElementById('autocomplete-dropdown-list');
        if (dropdown && dropdown.offsetParent !== null) {
            if (['Enter', 'Tab', 'ArrowUp', 'ArrowDown'].includes(e.key))
                e.preventDefault();
            return;
        }
        if (e.key === 'ArrowRight' || e.key === 'ArrowLeft') {
            this.handlePillNavigation(e);
        }
        else {
            this.clearTokenHighlights();
        }
    };
    handlePillNavigation(e) {
        const selection = window.getSelection();
        if (!selection?.rangeCount || !this.el)
            return;
        const range = selection.getRangeAt(0);
        if (!range.collapsed)
            return;
        const isRight = e.key === 'ArrowRight';
        const container = range.startContainer;
        const offset = range.startOffset;
        let target = null;
        if (isRight) {
            if (container.nodeType === Node.TEXT_NODE && offset === container.length)
                target = container.nextSibling;
            else if (container.nodeType === Node.ELEMENT_NODE)
                target = container.childNodes[offset];
        }
        else {
            if (container.nodeType === Node.TEXT_NODE && offset === 0)
                target = container.previousSibling;
            else if (container.nodeType === Node.ELEMENT_NODE && offset > 0)
                target = container.childNodes[offset - 1];
        }
        if (target instanceof HTMLElement && target.classList.contains('token-style')) {
            e.preventDefault();
            if (!target.classList.contains('token-selected')) {
                this.clearTokenHighlights();
                this.setTokenHighlight(target, true);
                const newRange = document.createRange();
                newRange.selectNode(target);
                selection.removeAllRanges();
                selection.addRange(newRange);
            }
            else {
                this.setTokenHighlight(target, false);
                const newRange = document.createRange();
                isRight ? newRange.setStartAfter(target) : newRange.setStartBefore(target);
                newRange.collapse(true);
                selection.removeAllRanges();
                selection.addRange(newRange);
            }
        }
    }
    onMouseDown = (e) => {
        const token = e.target.closest('.token-style');
        if (token) {
            e.preventDefault();
            e.stopPropagation();
            this.clearTokenHighlights();
            this.setTokenHighlight(token, true);
            const range = document.createRange();
            range.selectNode(token);
            const selection = window.getSelection();
            selection?.removeAllRanges();
            selection?.addRange(range);
            this.el?.focus();
        }
        else {
            this.clearTokenHighlights();
        }
    };
    onContextMenu = (e) => {
        const token = e.target.closest('.token-style');
        if (token && this.dotNetRef) {
            e.preventDefault();
            const typeName = token.getAttribute('data-type-name') || "";
            const snippetId = token.getAttribute('data-snippet-id') || "";
            this.dotNetRef.invokeMethodAsync('OpenPillMenu', typeName, snippetId, e.clientX, e.clientY);
        }
    };
    onGlobalMouseDown = (e) => {
        const item = e.target.closest('.autocomplete-item');
        if (!item)
            return;
        e.preventDefault();
        const typeName = item.querySelector('.type-name')?.textContent?.trim();
        if (typeName)
            this.dotNetRef?.invokeMethodAsync('SelectSuggestionFromJS', typeName);
    };
    onGlobalKeyDown = (e) => {
        if (e.key === 'Escape')
            this.dotNetRef?.invokeMethodAsync('HandleGlobalEscape');
    };
    setTokenHighlight(token, isHighlighted) {
        const typeName = token.getAttribute('data-type-name') || "";
        const baseTypeName = typeName.match(/<([^>]+)>/)?.[1] || typeName.replace('@', '');
        const colors = this.typeColors[baseTypeName] || { normal: '#4A5568', highlight: '#718096' };
        if (isHighlighted) {
            token.classList.add('token-selected');
            token.style.backgroundColor = colors.highlight;
        }
        else {
            token.classList.remove('token-selected');
            token.style.backgroundColor = colors.normal;
        }
    }
    clearTokenHighlights() {
        this.el?.querySelectorAll('.token-selected').forEach(t => this.setTokenHighlight(t, false));
    }
    getCaretOffset() {
        const selection = window.getSelection();
        if (!selection?.rangeCount || !this.el)
            return 0;
        const range = selection.getRangeAt(0);
        const preCaretRange = range.cloneRange();
        preCaretRange.selectNodeContents(this.el);
        try {
            preCaretRange.setEnd(range.startContainer, range.startOffset);
        }
        catch (e) {
            return 0;
        }
        // cloneContents() creates a fragment; textContent on a fragment 
        // includes hidden nodes, unlike range.toString()
        return preCaretRange.cloneContents().textContent?.length || 0;
    }
    restoreCursor(charOffset) {
        if (!this.el)
            return;
        // 1. Force focus to the element first
        this.el.focus();
        const selection = window.getSelection();
        if (!selection)
            return;
        const range = document.createRange();
        let cumulativeOffset = 0;
        let found = false;
        // Iterate only through top-level nodes (Pills and Text nodes)
        const childNodes = Array.from(this.el.childNodes);
        for (const node of childNodes) {
            const nodeText = node.textContent || "";
            const len = nodeText.length;
            if (cumulativeOffset + len >= charOffset) {
                if (node.nodeType === Node.TEXT_NODE) {
                    // If it's a text node, we can place the caret inside safely
                    range.setStart(node, charOffset - cumulativeOffset);
                }
                else {
                    // If it's a Pill, place the caret before or after it, never INSIDE.
                    if (charOffset <= cumulativeOffset) {
                        range.setStartBefore(node);
                    }
                    else {
                        range.setStartAfter(node);
                    }
                }
                range.collapse(true);
                found = true;
                break;
            }
            cumulativeOffset += len;
        }
        // 2. Fallback: If offset is at the very end
        if (!found) {
            const lastNode = this.el.lastChild;
            if (lastNode) {
                if (lastNode.nodeType === Node.TEXT_NODE) {
                    range.setStart(lastNode, lastNode.length);
                }
                else {
                    range.setStartAfter(lastNode);
                }
                range.collapse(true);
            }
        }
        try {
            selection.removeAllRanges();
            selection.addRange(range);
        }
        catch (err) {
            console.warn("Failed to restore selection:", err);
        }
    }
    getCaretPositionInfo() {
        const selection = window.getSelection();
        const node = selection?.anchorNode;
        if (!node || node.nodeType !== Node.TEXT_NODE)
            return { currentWord: "" };
        const textUpToCaret = node.textContent?.substring(0, selection.anchorOffset) || "";
        const words = textUpToCaret.split(/[\s\u00A0]+/);
        return { currentWord: words[words.length - 1] };
    }
}
window.regexEditor = new RegexEditor();
//# sourceMappingURL=regex-editor.js.map