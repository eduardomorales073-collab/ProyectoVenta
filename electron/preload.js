const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
  getAppVersion: () => ipcRenderer.invoke('get-app-version'),
  getUserDataPath: () => ipcRenderer.invoke('get-user-data-path'),
  getBackendUrl: () => ipcRenderer.invoke('get-backend-url'),
  isElectron: true
});

console.log('[PRELOAD] API de Electron expuesta en window.electronAPI');
