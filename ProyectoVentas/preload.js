const { contextBridge, ipcRenderer } = require('electron');

// ============ API EXPUESTA A ANGULAR ============
// Todo lo que expongas aquí estará disponible en window.electronAPI
contextBridge.exposeInMainWorld('electronAPI', {
    // Información de la app
    getAppVersion: () => ipcRenderer.invoke('get-app-version'),
    getUserDataPath: () => ipcRenderer.invoke('get-user-data-path'),
    getBackendUrl: () => ipcRenderer.invoke('get-backend-url'),

    // Detectar si está en Electron (útil para el frontend)
    isElectron: true
});

// ============ LOG ============
console.log('[PRELOAD] API de Electron expuesta en window.electronAPI');