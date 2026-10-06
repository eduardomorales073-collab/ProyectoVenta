const path = require('path');
const fs = require('fs');
const os = require('os');
const http = require('http');

const USER_DATA_PATH = path.join(os.homedir(), 'AppData', 'Roaming', 'SistemaVentas');

if (!fs.existsSync(USER_DATA_PATH)) {
    fs.mkdirSync(USER_DATA_PATH, { recursive: true });
}

process.env.ELECTRON_USER_DATA = USER_DATA_PATH;

const { app, BrowserWindow, ipcMain, shell } = require('electron');
const { spawn } = require('child_process');

const isDev = !app.isPackaged;
const BACKEND_PORT = 5000;
const FRONTEND_PORT = 4200;
const LOCAL_SERVER_PORT = 4321;

let mainWindow = null;
let backendProcess = null;
let localServer = null;

app.setPath('userData', USER_DATA_PATH);

app.commandLine.appendSwitch('disable-http-cache');
app.commandLine.appendSwitch('disable-gpu-shader-disk-cache');
app.commandLine.appendSwitch('disk-cache-size', '1');

function startLocalServer() {
    if (isDev) return;

    const browserPath = path.join(__dirname, 'browser');
    console.log('[PROD] Browser path:', browserPath);

    localServer = http.createServer((req, res) => {
        let urlPath = req.url.split('?')[0].split('#')[0];
        console.log('[PROD] Request:', urlPath);
        
        let filePath = path.join(browserPath, urlPath === '/' ? 'index.html' : urlPath);
        
        if (!fs.existsSync(filePath) || fs.statSync(filePath).isDirectory()) {
            filePath = path.join(browserPath, 'index.html');
        }

        const ext = path.extname(filePath).toLowerCase();
        const contentTypes = {
            '.html': 'text/html',
            '.js': 'application/javascript',
            '.css': 'text/css',
            '.json': 'application/json',
            '.png': 'image/png',
            '.jpg': 'image/jpeg',
            '.svg': 'image/svg+xml',
            '.ico': 'image/x-icon',
            '.woff': 'font/woff',
            '.woff2': 'font/woff2',
            '.ttf': 'font/ttf'
        };
        const contentType = contentTypes[ext] || 'application/octet-stream';

        fs.readFile(filePath, (err, data) => {
            if (err) {
                res.writeHead(404);
                res.end('Not found');
                return;
            }
            res.writeHead(200, { 'Content-Type': contentType });
            res.end(data);
        });
    });

    localServer.listen(LOCAL_SERVER_PORT, '127.0.0.1', () => {
        console.log('[PROD] Servidor local en http://127.0.0.1:' + LOCAL_SERVER_PORT);
    });
}

function stopLocalServer() {
    if (localServer) {
        localServer.close();
        localServer = null;
    }
}

function startBackend() {
    if (isDev) return;
    const backendPath = path.join(process.resourcesPath, 'backend', 'Venta.exe');
    console.log('[PROD] Buscando backend en:', backendPath);
    if (!fs.existsSync(backendPath)) {
        console.error('No se encontro el backend');
        return;
    }
    console.log('[PROD] Arrancando backend en puerto', BACKEND_PORT);
    backendProcess = spawn(backendPath, [], {
        env: { ...process.env, ASPNETCORE_URLS: `http://localhost:${BACKEND_PORT}`, ASPNETCORE_ENVIRONMENT: 'Production' },
        stdio: 'ignore',
        windowsHide: true
    });
}

function stopBackend() {
    if (backendProcess) { backendProcess.kill(); backendProcess = null; }
}

function createWindow() {
    console.log('[PROD] Creando ventana...');
    mainWindow = new BrowserWindow({
        width: 1400, height: 900, minWidth: 1024, minHeight: 700, show: true,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            nodeIntegration: false, contextIsolation: true, sandbox: false,
            webSecurity: true, partition: 'persist:main'
        },
        backgroundColor: '#f1f5f9'
    });

    if (isDev) {
        mainWindow.loadURL(`http://localhost:${FRONTEND_PORT}`);
        mainWindow.webContents.openDevTools();
    } else {
        const url = `http://127.0.0.1:${LOCAL_SERVER_PORT}/`;
        console.log('[PROD] Cargando desde:', url);
        mainWindow.loadURL(url).catch(err => {
            console.error('Error al cargar:', err);
        });
        setTimeout(() => {
            if (mainWindow && !mainWindow.isDestroyed()) {
                mainWindow.webContents.openDevTools();
            }
        }, 3000);
    }

    mainWindow.once('ready-to-show', () => { mainWindow.show(); });
    mainWindow.on('closed', () => { mainWindow = null; });
    mainWindow.webContents.setWindowOpenHandler(({ url }) => {
        shell.openExternal(url);
        return { action: 'deny' };
    });
    mainWindow.webContents.on('did-fail-load', (event, errorCode, errorDescription) => {
        console.error('Error al cargar:', errorCode, errorDescription);
    });
    mainWindow.webContents.on('did-finish-load', () => {
        console.log('Contenido cargado');
    });
}

app.whenReady().then(() => {
    console.log('=== SISTEMA VENTAS DESKTOP ===');
    console.log('Modo:', isDev ? 'DESARROLLO' : 'PRODUCCION');
    console.log('ResourcesPath:', process.resourcesPath);

    startLocalServer();
    startBackend();
    setTimeout(createWindow, 1500);
});

app.on('window-all-closed', () => { stopBackend(); stopLocalServer(); if (process.platform !== 'darwin') app.quit(); });
app.on('before-quit', () => { stopBackend(); stopLocalServer(); });

ipcMain.handle('get-app-version', () => app.getVersion());
ipcMain.handle('get-user-data-path', () => USER_DATA_PATH);
ipcMain.handle('get-backend-url', () => isDev ? 'http://localhost:5000' : `http://localhost:${BACKEND_PORT}`);

app.on('web-contents-created', (event, contents) => {
    contents.on('will-navigate', (event, url) => {
        if (!url.startsWith('http://localhost') && !url.startsWith('http://127.0.0.1') && !url.startsWith('file://')) {
            event.preventDefault();
        }
    });
});