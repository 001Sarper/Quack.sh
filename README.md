# 🦆 Quack.sh
> *quack quack, you're in!*

A lightweight SSH client built with Avalonia UI and xterm.js — designed for Homelab use.

## Features

- 🖥️ **Multi-tab SSH sessions** — connect to multiple servers simultaneously
- 🎨 **Light & Dark theme** — Catppuccin Latte / Mocha
- 🔤 **Customizable font size**
- 📋 **Host management** — save, edit and delete connections
- ⚡ **xterm.js terminal** — full terminal emulation with JetBrains Mono / Cascadia Code

## Built With

- [Avalonia UI](https://avaloniaui.net/) 11.3 — cross-platform UI framework
- [SSH.NET](https://github.com/sshnet/SSH.NET) — SSH connectivity
- [xterm.js](https://xtermjs.org/) — terminal emulator
- [CefGlue / WebViewControl](https://github.com/OutSystems/WebView) — Chromium WebView for Avalonia
- .NET 8

## Getting Started

### Prerequisites
- .NET 8 SDK
- Windows / Linux / macOS

### Build
```bash
git clone https://github.com/001Sarper/Quack.sh.git
cd Quack.sh
dotnet build
dotnet run
```

## Roadmap

- [ ] Encrpytion of Connection Data
- [ ] SSH key authentication
- [ ] Session logging

## License

MIT — see [LICENSE](LICENSE)
