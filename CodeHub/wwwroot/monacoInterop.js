﻿window.monacoInterop = {
    editorInstance: null,

    initialize: function (elementId, language, defaultCode) {
        if (window.monacoInterop.editorInstance) {
            window.monacoInterop.editorInstance.dispose();
        }

        require.config({ paths: { vs: 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.43.0/min/vs' } });
        require(['vs/editor/editor.main'], function () {
            window.monacoInterop.editorInstance = monaco.editor.create(document.getElementById(elementId), {
                language: language,
                theme: 'vs-dark',
                automaticLayout: true,
                quickSuggestions: { other: true, comments: false, strings: true },
                smoothScrolling: true,
                mouseWheelZoom: true,
                contextmenu: false,
                wordWrap: 'on',
                suggest: { snippetsPreventQuickSuggestions: false },
                scrollBeyondLastLine: false
            });

            if (defaultCode) {
                window.monacoInterop.editorInstance.setValue(defaultCode);
            }

            window.monacoInterop.disableCopyPaste();
        });
    },

    disableCopyPaste: function () {
        const editor = window.monacoInterop.editorInstance;
        editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyC, () => null);
        editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyV, () => null);
    },

    setTheme: (theme) => {
        if (window.monacoInterop.editorInstance) {
            monaco.editor.setTheme(theme);
        } else {
            console.error("Monaco Editor instance not found!");
        }
    },

    setLanguage: (language) => {
        if (window.monacoInterop.editorInstance) {
            monaco.editor.setModelLanguage(window.monacoInterop.editorInstance.getModel(), language);
        } else {
            console.error("Monaco Editor instance not found!");
        }
    },

    setValue: (code) => {
        if (window.monacoInterop.editorInstance) {
            window.monacoInterop.editorInstance.setValue(code);
        } else {
            console.error("Monaco Editor instance not found!");
        }
    },

    getValue: function () {
        return window.monacoInterop.editorInstance ? window.monacoInterop.editorInstance.getValue() : "";
    },
};