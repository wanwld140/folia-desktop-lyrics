# Folia Desktop Lyrics · 非官方二次开发

本项目是基于 [chthollyphile/folia-major](https://github.com/chthollyphile/folia-major) 的**非官方二次开发版本**，由 [wanwld140](https://github.com/wanwld140) 维护。原作者为 chthollyphile，原项目贡献者名单保留在 [CONTRIBUTORS.md](CONTRIBUTORS.md)。本仓库与原项目官方版本独立。

[English](README.en.md) · [原项目](https://github.com/chthollyphile/folia-major) · [许可证](LICENSE)

## 本版新增与调整

- **完整动画歌词壁纸**：将 Folia 的完整歌词动画画面显示在 Windows 桌面图标后面。
- **任务栏歌词**：显示当前句和下一句，长句滚动，提供封面及上一首、播放/暂停、下一首控制。
- **任务栏与托盘控制**：任务栏歌词的“…”/右键菜单和系统托盘均提供“退出壁纸模式”；点击托盘、从歌词卡片打开播放器或再次启动程序也可恢复普通窗口。
- **Windows 显示修复**：调整桌面画面的渲染与绘制路径，处理黑屏及闪烁；移除常驻恢复小窗，并补齐托盘图标资源。

任务栏歌词同步 Folia 自身的播放状态与歌词。此版本主要针对 Windows 验证；其他平台沿用上游实现。

## 使用

在“设置 → 桌面”中开启“任务栏歌词”或“壁纸模式”。壁纸模式下通过任务栏歌词“…”/右键菜单或系统托盘菜单退出，也可使用快捷键 **Ctrl+Alt+Shift+F12** 恢复普通窗口。

详细说明：[任务栏歌词](electron/taskbarLyrics/README.md) · [壁纸模式](docs/wallpaper-mode.md)。本仓库当前提供源码，未附带安装包。

## 本地运行与构建

需要 Node.js 24+。Windows 任务栏歌词 helper 还需要 .NET 10 SDK；它打包为独立程序，最终用户无需安装 .NET。

```powershell
npm ci
npm run build:taskbar-lyrics
npm run dev:electron
```

```powershell
npm run typecheck
npm run test:unit
npm run build:electron -- --win --publish never
```

首次打包会按上游构建脚本下载模型、音频运行时并构建所需原生组件，需要网络与对应工具链。构建方式和其他平台部署说明参见 [上游 README](https://github.com/chthollyphile/folia-major#readme)。

## 来源与许可

- 基于 Folia 上游提交 `d695db52a313abe2077ce4c2e5894cc4a82190f1`，二创修改日期：2026-11-01。
- 保留上游完整 [GNU AGPL v3 许可证](LICENSE)、原作者及贡献者归属。第三方组件按各自许可使用，见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)。
- 任务栏组件整合自提供的 TaskbarMusic V1 实现，并保留其所参考的 AgentSpeed MIT 版权声明；该声明不代表 TaskbarMusic 全部代码均采用 MIT 许可。
- 上游另有学习、非商业用途及在线内容免责声明，原文见 [上游法律说明](https://github.com/chthollyphile/folia-major#法律与免责声明)。此处保留其来源，不将该说明解释为已解决与许可证文本之间的差异。

音乐流媒体、歌词和专辑封面的版权归各自权利人所有，请通过合法渠道使用相关内容。
