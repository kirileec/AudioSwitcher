# AudioSwitcher

Windows 声音输出设备快速切换工具。

## 功能

- 快速切换预设的声音输出设备
- 全局快捷键 `Win + Alt + Q` 切换设备
- 系统托盘运行，双击托盘图标切换设备
- 支持开机自启动
- 单实例运行，防止重复启动

## 使用方法

1. 首次运行，右键托盘图标 → 设置输出设备，选择两个需要切换的设备
2. 按 `Win + Alt + Q` 或双击托盘图标即可在两个设备间切换
3. 右键托盘图标可设置开机自启动

## 构建

需要 .NET 6.0 SDK

```bash
cd AudioSwitcher
dotnet build -c Release
```

输出文件位于 `bin/Release/net6.0-windows/`

## 技术栈

- .NET 6.0 Windows Forms
- CoreAudio - Windows 音频设备管理
- Newtonsoft.Json - 配置文件解析
