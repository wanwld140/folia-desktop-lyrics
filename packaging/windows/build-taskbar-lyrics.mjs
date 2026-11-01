import { execFileSync } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

// Builds the pipe-owned WPF helper alongside Folia's other Windows resources (.NET 10 SDK).
const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../..');
export function buildTaskbarLyrics({ arch = 'x64' } = {}) {
    if (process.platform !== 'win32') return;
    execFileSync(process.env.FOLIA_DOTNET || 'dotnet', [
        'publish', path.join(root, 'electron/taskbarLyrics/native/FoliaTaskbarLyrics.csproj'),
        '-c', 'Release', '-r', `win-${arch}`, '--self-contained', 'true',
        '-p:PublishSingleFile=true', '-p:IncludeNativeLibrariesForSelfExtract=true',
        '-p:DebugType=None', '-o', path.join(root, 'build'),
    ], { stdio: 'inherit', cwd: root });
}
if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
    buildTaskbarLyrics({ arch: process.env.npm_config_arch === 'arm64' ? 'arm64' : 'x64' });
}
