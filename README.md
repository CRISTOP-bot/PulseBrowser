# ⚡ Pulse Browser

> Un navegador web completo, rápido y moderno construido con C# y WinForms.

![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.8-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-14.0-239120?style=flat&logo=csharp)
![Windows](https://img.shields.io/badge/Windows-10|11-0078D6?style=flat&logo=windows)
![License](https://img.shields.io/badge/license-MIT-green?style=flat)
![Repo](https://img.shields.io/github/repo-size/CRISTOP-bot/PulseBrowser?style=flat&label=tamaño)

---

## ✨ Características

| Característica | Descripción |
|---------------|-------------|
| 🗂️ **Pestañas** | Navegación por pestañas con apertura/cierre |
| ⬅️➡️ **Navegación** | Atrás, adelante, recargar, página de inicio |
| 🔖 **Marcadores** | Añade, edita, elimina y organiza favoritos |
| 📜 **Historial** | Historial completo con búsqueda integrada |
| ⚙️ **Configuración** | Página de inicio y motor de búsqueda personalizables |
| 📊 **Barra de progreso** | Indicador de carga en tiempo real |
| 📌 **Barra de marcadores** | Acceso rápido a tus sitios favoritos |
| 🖥️ **Pantalla completa** | Modo inmersivo con F11 |

---

## 🚀 Instalación

### Opción 1: Instalador automático

```powershell
.\install.ps1
```

Esto copia los archivos a `%LOCALAPPDATA%\PulseBrowser`, crea accesos directos en el escritorio y menú inicio, y registra el programa para desinstalación desde `Configuración > Apps`.

### Opción 2: Portátil

```powershell
.\bin\Release\PulseBrowser.exe
```

Ejecuta directamente sin instalación.

### Opción 3: Compilar desde código

```powershell
.\build.ps1
```

Requiere [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) y MSBuild (incluido con Visual Studio Build Tools 2022).

---

## 🖼️ Capturas

```
┌─────────────────────────────────────────────────────────┐
│  📁 🗂️ ⚙️    ┌──────────────────────┐  ☆  ⏱  ⚙️       │
│  ◀ ▶ ⟳ ⌂    │ https://google.com   │ [Go]            │
│               └──────────────────────┘                 │
│  ┌────────────────────────────────────────────────────┐ │
│  │                                                    │ │
│  │              🌐 Página web aquí                    │ │
│  │                                                    │ │
│  └────────────────────────────────────────────────────┘ │
│  🔗 Listo                            ████████░░░ 70%   │
└─────────────────────────────────────────────────────────┘
```

---

## 🧱 Estructura del proyecto

```
PulseBrowser/
├── 📄 Program.cs              # Punto de entrada
├── 📄 MainForm.cs             # Ventana principal (+700 líneas)
├── 📄 BookmarkManager.cs      # Gestor de marcadores (JSON)
├── 📄 BookmarkForm.cs         # Diálogo de administración
├── 📄 HistoryManager.cs       # Gestor de historial (JSON)
├── 📄 HistoryForm.cs          # Visor de historial
├── 📄 SettingsManager.cs      # Configuración persistente
├── 📄 SettingsForm.cs         # Preferencias de usuario
├── 📄 GenerateIcon.cs         # Generador del icono
├── 📦 Pulse.ico               # Icono personalizado
├── 📄 build.ps1               # Script de compilación
└── 📄 install.ps1             # Instalador/desinstalador
```

---

## 🛠️ Tecnologías

- **Lenguaje:** C# 14.0
- **Framework:** .NET Framework 4.8
- **UI:** Windows Forms (WinForms)
- **Motor de renderizado:** Internet Explorer 11 (WebBrowser control)
- **Almacenamiento:** JSON en `%APPDATA%\PulseBrowser\`
- **Compilador:** Roslyn (VS BuildTools 2022)

---

## 📦 Descargar

Descarga la última versión desde [Releases](https://github.com/CRISTOP-bot/PulseBrowser/releases) o clona el repositorio:

```bash
git clone https://github.com/CRISTOP-bot/PulseBrowser.git
```

---

## 📄 Licencia

MIT © 2026 [CRISTOP-bot](https://github.com/CRISTOP-bot)

---

<p align="center">
  <b>Hecho con 💙 y C#</b>
</p>
