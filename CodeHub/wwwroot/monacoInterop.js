window.monacoInterop = {
    editorInstance: null,

    initialize: function (elementId, language) {
        require.config({ paths: { vs: 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.43.0/min/vs' } });
        require(['vs/editor/editor.main'], function () {
            window.monacoInterop.editorInstance = monaco.editor.create(document.getElementById(elementId), {
                language: language,
                theme: 'vs-dark',
                automaticLayout: true,
                quickSuggestions: { other: true, comments: false, strings: true },
                smoothScrolling: true
            });

            window.monacoInterop.editorInstance.setValue("//sem napíšte svoj kód");
        });
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

    getValue: function () {
        return window.monacoInterop.editorInstance ? window.monacoInterop.editorInstance.getValue() : "";
    }
};
