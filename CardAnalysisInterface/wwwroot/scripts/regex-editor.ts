interface TypeColors {
    [key: string]: string;
}

interface TemplateFragment {
    text: string;
    id: string | null;
    isPill: boolean;
    typeName: string | null;
    methodName: string | null;
    args: string[] | null;
}

interface DotNetReference {
    invokeMethodAsync(methodName: string, ...args: any[]): Promise<void>;
}

class RegexEditor {
    private dotNetRef: DotNetReference | null = null;
    private el: HTMLElement | null = null;
    private typeColors: TypeColors = {};
    private isInternallyChanging = false;

    public initialize(ref: DotNetReference, element: HTMLElement, colors: TypeColors) {
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

    public dispose() {
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

    public focusClassNameInput(selector: string) {
        const el = document.querySelector(selector) as HTMLInputElement;
        if (el) {
            el.focus();
            const val = el.value;
            el.value = '';
            el.value = val;
        }
    }

    public scrollToAutocompleteItem(elementId: string) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollIntoView({ block: 'nearest' });
        }
    }

    public syncPills(fragments: TemplateFragment[], cursorPos: number) {
        if (!this.el) return;

        this.isInternallyChanging = true;
        this.el.innerHTML = '';

        for (const fragment of fragments) {
            if (fragment.isPill) {
                this.el.appendChild(this.createPillElement(fragment));
            } else {
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

    public insertPill(textToReplace: string, typeName: string | null, methodName: string | null, args: string[] | null) {
        if (!this.el || !this.dotNetRef) return;

        const selection = window.getSelection();
        if (!selection?.rangeCount) return;

        const range = selection.getRangeAt(0);
        const node = range.startContainer;

        if (node.nodeType === Node.TEXT_NODE) {
            const content = node.textContent || "";
            const offset = range.startOffset;
            const startOfWord = content.lastIndexOf(textToReplace, offset - 1);

            if (startOfWord !== -1) {
                const newPill = this.createPillElement({
                    text: typeName ? typeName : (methodName ? `@${methodName}` : ""),
                    id: null,
                    isPill: true,
                    typeName: typeName,
                    methodName: methodName,
                    args: args
                });

                range.setStart(node, startOfWord);
                range.setEnd(node, offset);
                range.deleteContents();
                range.insertNode(newPill);

                range.setStartAfter(newPill);
                range.collapse(true);
                selection.removeAllRanges();
                selection.addRange(range);
            }
        }

        this.onInput();
    }

    private getCurrentFragments(): TemplateFragment[] {
        if (!this.el) return [];
        const raw: TemplateFragment[] = [];

        this.el.childNodes.forEach(node => {
            if (node.nodeType === Node.TEXT_NODE) {
                const text = (node.textContent || "").replace(/\u00A0/g, ' ');
                raw.push({ text: text, id: null, isPill: false, typeName: null, methodName: null, args: null });
            } else if (node.nodeType === Node.ELEMENT_NODE && (node as HTMLElement).classList.contains('token-style')) {
                const element = node as HTMLElement;
                const argsRaw = element.getAttribute('data-args');
                raw.push({
                    text: element.textContent || "",
                    id: element.getAttribute('data-snippet-id'),
                    isPill: true,
                    typeName: element.getAttribute('data-type-name'),
                    methodName: element.getAttribute('data-method-name'),
                    args: argsRaw ? JSON.parse(argsRaw) : null
                });
            }
        });

        const normalized: TemplateFragment[] = [];
        for (const frag of raw) {
            if (!frag.isPill && frag.text.length === 0) continue;

            const last = normalized[normalized.length - 1];
            if (last && !last.isPill && !frag.isPill) {
                last.text += frag.text;
            } else {
                normalized.push(frag);
            }
        }
        return normalized;
    }

    private createPillElement(frag: TemplateFragment): HTMLElement {
        const displayLabel = frag.text;
        const colorKey = frag.typeName || frag.methodName || "";
        const color = this.typeColors[colorKey] || '#4A5568';

        const span = document.createElement('span');
        span.className = 'token-style';
        span.contentEditable = 'false';
        span.style.backgroundColor = color;

        if (frag.typeName) span.setAttribute('data-type-name', frag.typeName);
        if (frag.methodName) span.setAttribute('data-method-name', frag.methodName);
        if (frag.args) span.setAttribute('data-args', JSON.stringify(frag.args));

        span.setAttribute('data-snippet-id', frag.id || `pill-${Math.random().toString(36).slice(2, 11)}`);
        span.textContent = displayLabel;
        return span;
    }

    private onInput = () => {
        if (this.isInternallyChanging || !this.el || !this.dotNetRef) return;
        const info = this.getCaretPositionInfo();
        const fragments = this.getCurrentFragments();
        this.dotNetRef.invokeMethodAsync('NotifyContentChanged', fragments, info.currentWord, -1);
    };

    private onBeforeInput = (e: InputEvent) => {
        if (!this.el) return;
        const selection = window.getSelection();
        if (!selection?.rangeCount) return;

        const range = selection.getRangeAt(0);
        const highlighted = this.el.querySelector('.token-selected') as HTMLElement;

        if (highlighted && e.inputType.startsWith('delete')) {
            e.preventDefault();
            highlighted.remove();
            this.onInput();
            return;
        }

        const allTokens = Array.from(this.el.querySelectorAll('.token-style')) as HTMLElement[];
        let tokensToDelete = false;
        allTokens.forEach(token => {
            const tokenRange = document.createRange();
            tokenRange.selectNode(token);
            const intersect = !(range.compareBoundaryPoints(Range.START_TO_END, tokenRange) >= 0 ||
                range.compareBoundaryPoints(Range.END_TO_START, tokenRange) <= 0);
            if (intersect) {
                token.remove();
                tokensToDelete = true;
            }
        });

        if (tokensToDelete) {
            e.preventDefault();
            this.onInput();
        }
    };

    private onKeyDown = (e: KeyboardEvent) => {
        const dropdown = document.getElementById('autocomplete-dropdown-list');
        if (dropdown && dropdown.offsetParent !== null) {
            if (['Enter', 'Tab', 'ArrowUp', 'ArrowDown'].includes(e.key)) e.preventDefault();
            return;
        }

        if (e.key === 'ArrowRight' || e.key === 'ArrowLeft') {
            this.handlePillNavigation(e);
        } else {
            this.clearTokenHighlights();
        }
    };

    private handlePillNavigation(e: KeyboardEvent) {
        const selection = window.getSelection();
        if (!selection?.rangeCount || !this.el) return;
        const range = selection.getRangeAt(0);
        if (!range.collapsed) return;

        const isRight = e.key === 'ArrowRight';
        const container = range.startContainer;
        const offset = range.startOffset;

        let target: ChildNode | null = null;
        if (isRight) {
            if (container.nodeType === Node.TEXT_NODE && offset === (container as Text).length) target = container.nextSibling;
            else if (container.nodeType === Node.ELEMENT_NODE) target = container.childNodes[offset];
        } else {
            if (container.nodeType === Node.TEXT_NODE && offset === 0) target = container.previousSibling;
            else if (container.nodeType === Node.ELEMENT_NODE && offset > 0) target = container.childNodes[offset - 1];
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
            } else {
                this.setTokenHighlight(target, false);
                const newRange = document.createRange();
                isRight ? newRange.setStartAfter(target) : newRange.setStartBefore(target);
                newRange.collapse(true);
                selection.removeAllRanges();
                selection.addRange(newRange);
            }
        }
    }

    private onMouseDown = (e: MouseEvent) => {
        const token = (e.target as HTMLElement).closest('.token-style') as HTMLElement;
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
        } else {
            this.clearTokenHighlights();
        }
    };

    private onContextMenu = (e: MouseEvent) => {
        const token = (e.target as HTMLElement).closest('.token-style') as HTMLElement;
        if (token && this.dotNetRef) {
            e.preventDefault();
            const typeName = token.getAttribute('data-type-name') || "";
            const snippetId = token.getAttribute('data-snippet-id') || "";
            this.dotNetRef.invokeMethodAsync('OpenPillMenu', typeName, snippetId, e.clientX, e.clientY);
        }
    };

    private onGlobalMouseDown = (e: MouseEvent) => {
        const item = (e.target as HTMLElement).closest('.autocomplete-item');
        if (!item) return;
        e.preventDefault();
        const typeName = item.querySelector('.type-name')?.textContent?.trim();
        if (typeName) this.dotNetRef?.invokeMethodAsync('SelectSuggestionFromJS', typeName);
    };

    private onGlobalKeyDown = (e: KeyboardEvent) => {
        if (e.key === 'Escape') this.dotNetRef?.invokeMethodAsync('HandleGlobalEscape');
    };

    private setTokenHighlight(token: HTMLElement, isHighlighted: boolean) {
        if (isHighlighted) {
            token.classList.add('token-selected');
        } else {
            token.classList.remove('token-selected');
        }
    }

    private clearTokenHighlights() {
        this.el?.querySelectorAll('.token-selected').forEach(t => this.setTokenHighlight(t as HTMLElement, false));
    }

    public getCaretOffset(): number {
        const selection = window.getSelection();
        if (!selection?.rangeCount || !this.el) return 0;
        const range = selection.getRangeAt(0);

        const preCaretRange = range.cloneRange();
        preCaretRange.selectNodeContents(this.el);
        try {
            preCaretRange.setEnd(range.startContainer, range.startOffset);
        } catch (e) {
            return 0;
        }

        return preCaretRange.cloneContents().textContent?.length || 0;
    }

    private restoreCursor(charOffset: number) {
        if (!this.el) return;
        this.el.focus();

        const selection = window.getSelection();
        if (!selection) return;

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
                } else {
                    if (charOffset <= cumulativeOffset) {
                        range.setStartBefore(node);
                    } else {
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
                    range.setStart(lastNode, (lastNode as Text).length);
                } else {
                    range.setStartAfter(lastNode);
                }
                range.collapse(true);
            }
        }

        try {
            selection.removeAllRanges();
            selection.addRange(range);
        } catch (err) {
            console.warn("Failed to restore selection:", err);
        }
    }

    private getCaretPositionInfo() {
        const selection = window.getSelection();
        const node = selection?.anchorNode;
        if (!node || node.nodeType !== Node.TEXT_NODE) return { currentWord: "" };

        const textUpToCaret = node.textContent?.substring(0, selection!.anchorOffset) || "";
        const words = textUpToCaret.split(/[\s\u00A0]+/);
        return { currentWord: words[words.length - 1] };
    }
}

(window as any).regexEditor = new RegexEditor();