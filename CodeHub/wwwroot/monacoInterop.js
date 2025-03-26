window.monacoInterop = {
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
                formatOnType: true,
                wordWrap: "on",
            });

            if (defaultCode) {
                window.monacoInterop.editorInstance.setValue(defaultCode);
            }


            //window.monacoInterop.editorInstance.onKeyDown((event) => {
            //    const { keyCode, ctrlKey, metaKey } = event;

            //    // KeyCode 67 = 'C' | KeyCode 86 = 'V'
            //    if ((keyCode === 67 || keyCode === 86) && (ctrlKey || metaKey)) {
            //        event.preventDefault();
            //        console.log(`Blocked ${ctrlKey ? 'Ctrl' : 'Cmd'} + ${String.fromCharCode(keyCode)}`);
            //    }
            //});

            };
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
