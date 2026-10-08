// ============ DESHABILITAR CACHÉ (ANTES DE TODO) ============
const path = require('path');
const fs = require('fs');
const os = require('os');
const http = require('http');

const USER_DATA_PATH = path.join(os.homedir(), 'AppData', 'Roaming', 'SistemaVentas');

if (!fs.existsSync(USER_DATA_PATH)) {
    fs.mkdirSync(USER_DATA_PATH, { recursive: true });
}

process.env.ELECTRON_USER_DATA = USER_DATA_PATH;

const { app, BrowserWindow, ipcMain, shell, session } = require('electron');
const { spawn } = require('child_process');

const isDev = !app.isPackaged;
const BACKEND_PORT = 5000;
const FRONTEND_PORT = 4200;
const LOCAL_SERVER_PORT = 4321;

let mainWindow = null;
let backendProcess = null;
let localServer = null;

app.setPath('userData', USER_DATA_PATH);

// ============ SERVIDOR HTTP LOCAL PARA ANGULAR ============
function startLocalServer() {
    if (isDev) return;

    const browserPath = path.join(__dirname, 'browser');

    localServer = http.createServer((req, res) => {
        // ✅ Quitar query string
        let urlPath = req.url.split('?')[0];
        urlPath = decodeURIComponent(urlPath);

        // ✅ Normalizar el path
        if (urlPath === '/' || urlPath === '') {
            urlPath = '/index.html';
        }

        // ✅ Construir la ruta del archivo
        let filePath = path.join(browserPath, urlPath);

        // ✅ SPA fallback: si NO existe el archivo, servir index.html
        if (!fs.existsSync(filePath) || fs.statSync(filePath).isDirectory()) {
            console.log(`[Server] SPA fallback: ${urlPath} → index.html`);
            filePath = path.join(browserPath, 'index.html');
        }

        // ✅ Content-Type según extensión
        const ext = path.extname(filePath).toLowerCase();
        const contentTypes = {
            '.html': 'text/html; charset=utf-8',
            '.js': 'application/javascript; charset=utf-8',
            '.mjs': 'application/javascript; charset=utf-8',
            '.css': 'text/css; charset=utf-8',
            '.json': 'application/json; charset=utf-8',
            '.png': 'image/png',
            '.jpg': 'image/jpeg',
            '.jpeg': 'image/jpeg',
            '.svg': 'image/svg+xml',
            '.ico': 'image/x-icon',
            '.woff': 'font/woff',
            '.woff2': 'font/woff2',
            '.ttf': 'font/ttf',
            '.map': 'application/json'
        };
        const contentType = contentTypes[ext] || 'application/octet-stream';

        // ✅ Leer y enviar el archivo
        fs.readFile(filePath, (err, data) => {
            if (err) {
                console.error(`[Server] Error: ${filePath} -`, err.message);
                res.writeHead(404, { 'Content-Type': 'text/plain' });
                res.end('Not found');
                return;
            }

            res.writeHead(200, {
                'Content-Type': contentType,
                'Cache-Control': 'no-cache, no-store, must-revalidate',
                'Pragma': 'no-cache',
                'Expires': '0'
            });
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

// ============ ARRANCAR BACKEND ============
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
        env: {
            ...process.env,
            ASPNETCORE_URLS: `http://localhost:${BACKEND_PORT}`,
            ASPNETCORE_ENVIRONMENT: 'Production'
        },
        stdio: 'ignore',
        windowsHide: true
    });
}

function stopBackend() {
    if (backendProcess) {
        backendProcess.kill();
        backendProcess = null;
    }
}

// ============ CREAR VENTANA ============
function createWindow() {
    console.log('[PROD] Creando ventana...');

    mainWindow = new BrowserWindow({
        width: 1400,
        height: 900,
        minWidth: 1024,
        minHeight: 700,
        show: false,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            nodeIntegration: false,
            contextIsolation: true,
            sandbox: false,
            webSecurity: true,
            partition: 'persist:main'
        },
        backgroundColor: '#f1f5f9'
    });

    if (isDev) {
        mainWindow.loadURL(`http://localhost:${FRONTEND_PORT}`);
        mainWindow.webContents.openDevTools();
    } else {
        // ✅ Cargar SIEMPRE la raíz (con base href="/" los recursos se piden correctamente)
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

    mainWindow.once('ready-to-show', () => {
        console.log('[PROD] Ventana lista para mostrar');
        mainWindow.show();
    });

    mainWindow.on('closed', () => {
        mainWindow = null;
    });

    mainWindow.webContents.setWindowOpenHandler(({ url }) => {
        shell.openExternal(url);
        return { action: 'deny' };
    });

    // ✅ Interceptar F5/Ctrl+R
    mainWindow.webContents.on('before-input-event', (event, input) => {
        const isF5 = input.key === 'F5';
        const isCtrlR = input.control && (input.key === 'r' || input.key === 'R');

        if (isF5 || isCtrlR) {
            event.preventDefault();
            const currentUrl = mainWindow.webContents.getURL();
            console.log('[F5] Recargando:', currentUrl);
            // ✅ Recargar la URL actual (el servidor tiene SPA fallback)
            mainWindow.reload();
        }
    });

    mainWindow.webContents.on('did-fail-load', (event, errorCode, errorDescription) => {
        console.error('[ERROR] Fallo al cargar:', errorCode, errorDescription);
    });

    mainWindow.webContents.on('did-finish-load', () => {
        console.log('[OK] Contenido cargado correctamente');
    });
}

// ============ CICLO DE VIDA ============
app.whenReady().then(() => {
    console.log('=== SISTEMA VENTAS DESKTOP ===');
    console.log('Modo:', isDev ? 'DESARROLLO' : 'PRODUCCION');

    startLocalServer();
    startBackend();

    setTimeout(createWindow, 1500);

    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) {
            createWindow();
        }
    });
});

app.on('window-all-closed', () => {
    stopBackend();
    stopLocalServer();
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('before-quit', () => {
    stopBackend();
    stopLocalServer();
});

// ============ IPC ============
ipcMain.handle('get-app-version', () => app.getVersion());
ipcMain.handle('get-user-data-path', () => USER_DATA_PATH);
ipcMain.handle('get-backend-url', () => isDev ? 'http://localhost:5000' : `http://localhost:${BACKEND_PORT}`);

// ============ SEGURIDAD ============
app.on('web-contents-created', (event, contents) => {
    contents.on('will-navigate', (event, url) => {
        if (!url.startsWith('http://localhost') &&
            !url.startsWith('http://127.0.0.1') &&
            !url.startsWith('file://')) {
            event.preventDefault();
        }
    });
});