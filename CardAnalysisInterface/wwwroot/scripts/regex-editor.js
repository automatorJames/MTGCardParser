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
    syncPills(fragments, cursorPos) {
        if (!this.el)
            return;
        this.isInternallyChanging = true;
        this.el.innerHTML = '';
        for (const fragment of fragments) {
            if (fragment.isPill) {
                this.el.appendChild(this.createPillElement(fragment.text, fragment.id));
            }
            else {
                this.el.appendChild(document.createTextNode(fragment.text));
            }
        }
        if (this.el.childNodes.length === 0 || this.el.lastChild?.nodeType !== Node.TEXT_NODE) {
            this.el.appendChild(document.createTextNode(''));
        }
        if (cursorPos >= 0) {
            requestAnimationFrame(() => this.restoreCursor(cursorPos));
        }
        this.isInternallyChanging = false;
    }
    insertPill(textToReplace, fullTokenText) {
        if (!this.el || !this.dotNetRef)
            return;
        const pos = this.getCaretOffset();
        const fullText = this.el.textContent || "";
        const textToInsert = fullTokenText;
        const head = fullText.substring(0, pos - textToReplace.length);
        const tail = fullText.substring(pos);
        const newText = head + textToInsert + tail;
        const newCaretPos = head.length + textToInsert.length;
        this.isInternallyChanging = true;
        this.el.textContent = newText;
        this.dotNetRef.invokeMethodAsync('NotifyContentChanged', newText, "", newCaretPos);
        this.isInternallyChanging = false;
        this.el.focus();
    }
    createPillElement(text, id) {
        const baseTypeName = text.match(/<([^>]+)>/)?.[1] || text.substring(1);
        const color = this.typeColors[baseTypeName] || '#4A5568';
        const span = document.createElement('span');
        span.className = 'token-style';
        span.contentEditable = 'false';
        span.style.backgroundColor = color;
        span.setAttribute('data-type-name', text);
        span.setAttribute('data-snippet-id', id);
        // Explicitly set text content to avoid @ symbols being hidden by innerHTML hacks
        span.textContent = text;
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
        if (isHighlighted) {
            token.classList.add('token-selected');
        }
        else {
            token.classList.remove('token-selected');
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
        return preCaretRange.cloneContents().textContent?.length || 0;
    }
    restoreCursor(charOffset) {
        if (!this.el)
            return;
        this.el.focus();
        const selection = window.getSelection();
        if (!selection)
            return;
        const range = document.createRange();
        let cumulativeOffset = 0;
        let found = false;
        const childNodes = Array.from(this.el.childNodes);
        for (const node of childNodes) {
            const nodeText = node.textContent || "";
            const len = nodeText.length;
            if (cumulativeOffset + len >= charOffset) {
                if (node.nodeType === Node.TEXT_NODE) {
                    range.setStart(node, charOffset - cumulativeOffset);
                }
                else {
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