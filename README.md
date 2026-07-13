# MacroWidgets

**方寸之间** — 适用于 Windows 平台的桌面小组件

> 将实用信息凝练于方寸之间，置于桌面随时 glance。时钟、天气、日历、提醒事项——轻量、透明、不干扰，让你的桌面兼具美观与效率。基于 [Avalonia UI](https://avaloniaui.net/) 11 构建，MVVM 架构。

## 功能特性

- **桌面小组件** — 时钟、天气、日历、提醒事项，每个小组件独立窗口，不遮挡桌面操作
- **自由拖动** — Win32 原生拖动，零抖动
- **系统托盘** — 托盘图标常驻，左键打开小组件画廊，右键菜单快捷操作
- **小组件画廊** — 浏览可用小组件，一键添加到桌面
- **持久化** — 小组件位置和布局自动保存，重启后恢复
- **设置窗口** — 左侧导航 + 右侧内容的 Windows 11 风格设置面板
- **提醒事项** — 支持添加、删除、勾选完成
- **设计系统** — 统一 Design Token，支持后续主题扩展

## 技术栈

| 技术 | 版本 | 用途 |
|---|---|---|
| .NET | 9.0 | 运行时 |
| Avalonia UI | 11.2.7 | 跨平台 UI 框架 |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM 基础设施 |
| Win32 API | — | 窗口置底、拖动、系统托盘 |

## 项目结构

```
MacWidget/
├── Program.cs                        # 入口点 + 单实例锁
├── App.axaml / App.axaml.cs          # 应用程序 + 托盘 + 生命周期
├── Theme/
│   └── MacWidgetTheme.xaml           # Design Token 系统
├── Assets/
│   └── tray_icon.ico                 # 托盘图标
├── Models/
│   ├── ReminderItem.cs                # 提醒事项模型
│   └── WeatherData.cs                 # 天气数据模型
├── ViewModels/Widgets/
│   ├── ClockViewModel.cs
│   ├── CalendarViewModel.cs
│   ├── WeatherViewModel.cs
│   ├── RemindersViewModel.cs
│   └── WidgetGalleryViewModel.cs
├── Views/
│   ├── WidgetWindow.cs               # 小组件独立窗口（置底 + 拖动 + 右键菜单）
│   ├── Widgets/
│   │   ├── ClockWidget.axaml
│   │   ├── CalendarWidget.axaml
│   │   ├── WeatherWidget.axaml
│   │   └── RemindersWidget.axaml
│   ├── Gallery/
│   │   └── WidgetGalleryWindow.axaml # 小组件画廊
│   └── Settings/
│       ├── SettingsWindow.axaml      # 设置主窗口（导航栏 + 内容区）
│       └── Pages/
│           ├── GeneralSettingsPage.axaml
│           ├── AppearanceSettingsPage.axaml
│           ├── NotificationSettingsPage.axaml
│           └── AdvancedSettingsPage.axaml
├── Services/
│   ├── WidgetManager.cs              # 小组件注册表 + 工厂
│   └── WidgetPersistenceService.cs   # JSON 配置持久化
└── Native/
    └── Win32WorkerW.cs               # Win32 P/Invoke（窗口管理）
```

## 构建与运行

### 前提条件

- .NET 9.0 SDK
- Windows 操作系统

### 构建

```bash
git clone https://github.com/your-username/MacWidget.git
cd MacWidget
dotnet build
dotnet run
```

## 使用方式

1. **启动应用** — 运行后 4 个默认小组件出现在桌面
2. **拖动小组件** — 左键按住拖动到任意位置
3. **移除小组件** — 右键点击小组件 → "移除此小组件"
4. **添加小组件** — 点击托盘图标或右键 → "打开小组件画廊" → 点击卡片添加
5. **打开设置** — 托盘右键 → "设置"
6. **退出应用** — 托盘右键 → "退出"

## 配置文件

配置保存在 `%AppData%\MacWidget\` 下：

| 文件 | 说明 |
|---|---|
| `config.json` | 小组件布局（类型、位置、尺寸） |
| `settings.json` | 应用设置（城市、主题、通知等） |

## 设计系统

项目使用统一的 Design Token 系统（`Theme/MacWidgetTheme.xaml`），涵盖：

- **Surface 颜色** — 小组件背景、卡片、输入框、分隔线等
- **Text 颜色** — 主文本、次文本、三级文本、禁用文本
- **Accent 颜色** — 9 种强调色（蓝/紫/粉/红/绿/橙/黄/青/青绿）
- **Typography** — 8 级字号（Display → Micro）
- **Radius** — 6 级圆角
- **Spacing** — 7 级间距

所有 UI 组件通过 `DynamicResource` 引用 Token，支持后续主题市场扩展。

## 后续规划

- [ ] 主题市场（用户上传/下载自定义主题）
- [ ] 天气 API 对接（已预留 `WeatherApiKey` 字段）
- [ ] 开机自启设置生效
- [ ] 提醒事项数据持久化
- [ ] 更多小组件（便签、系统监控、音乐等）
- [ ] 自动更新检查

## 许可证

MIT License
