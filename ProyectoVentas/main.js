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

// ✅ Credenciales de JasperReports (cambiar si son otras)
const JASPER_USER = 'jasperadmin';
const JASPER_PASSWORD = 'jasperadmin';

let mainWindow = null;
let backendProcess = null;
let localServer = null;

app.setPath('userData', USER_DATA_PATH);

// ============ INTERCEPTAR PETICIONES A JASPERREPORTS ============
// ✅ Añadir Basic Auth automáticamente a todas las peticiones al servidor Jasper
function setupJasperAuth() {
    session.defaultSession.webRequest.onBeforeSendHeaders(
        { urls: ['http://localhost:8080/*', 'http://127.0.0.1:8080/*'] },
        (details, callback) => {
            const credentials = Buffer.from(`${JASPER_USER}:${JASPER_PASSWORD}`).toString('base64');
            details.requestHeaders['Authorization'] = `Basic ${credentials}`;
            callback({ requestHeaders: details.requestHeaders });
        }
    );

    console.log('[JASPER] Interceptor de autenticacion activado');
}

// ============ SERVIDOR HTTP LOCAL PARA ANGULAR ============
function startLocalServer() {
    if (isDev) return;

    const browserPath = path.join(__dirname, 'browser');

    localServer = http.createServer((req, res) => {
        let urlPath = req.url.split('?')[0].split('#')[0];
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
        show: true,
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
        mainWindow.show();
    });

    mainWindow.on('closed', () => {
        mainWindow = null;
    });

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

// ============ CICLO DE VIDA ============
app.whenReady().then(() => {
    console.log('=== SISTEMA VENTAS DESKTOP ===');
    console.log('Modo:', isDev ? 'DESARROLLO' : 'PRODUCCION');

    // ✅ Activar interceptor de Jasper ANTES de crear ventana
    setupJasperAuth();

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