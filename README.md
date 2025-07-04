# RemoteMonitoring 本地远程监控系统
本项目是一个基于 Avalonia 跨平台 UI 框架 和 C# 后端 的现代化监控管理系统Demo，适用于Windows主机的实时监控、数据分析与智能运维场景。

PS：当前Demo并没有配置Api服务端，如果需要与部署的Web端进行通信请自行修改Service端代码与Core中相应配置文件来达到目的。

## 功能特性
- 👁️ **主机监控与管理** - 支持上下线客户端上下线实时监控
- ️🤖 **AI智能分析**    - 支持AI数据分析与智能诊断建议
- 🖥️ **远程桌面**      - 支持远程桌面查看
- 🌐 **远程控制**      - 支持远程鼠标键盘控制
- 💻 **远程终端**      - 远程远程终端命令输入与执行
- 😊 **高性能网络通信** - 基于DotNetty实现高并发，低延迟的主机通信

## 技术栈
- 前端 UI：Avalonia（XAML + MVVM）
- 后端服务：C# (.NET 8)
- AI 数据分析：DeepSeek AI（RESTful，Refit 强类型封装）
- 网络通信：DotNetty

## 运行与开发
### 1. 环境要求
- .NET 8.0 SDK
- Avalonia 11+
- Windows运行环境

### 2. 启动方式
1. 配置RemoteMonitoring.Core/Services/Networks 路径中的.json文件
2. 配置RemoteMonitoring.Core/Services/Refits 路径中的.json文件
3. 使用 Visual Studio 或 Rider 打开解决方案，编译并运行 Service/Client/Console 项目即可。

## 使用指南
优先启动 Service 端，启动完成后再启动 Client 端和 Console 端，客户端需要给管理员权限

### 服务端
服务端支持对主机进行管理，信息记录，生成Ai健康检测报告等功能
#### 主页
1. 主页可以查看最近的主机上线下线记录。
2. 点击生成报告后会在一段时间后生成Ai诊断报告，可在最上方的通知中查看，无需在主页等待。

#### 主机列表
1. 主机列表可以通过添加主机来直接添加主机信息(不会进行远程连接，只是添加主机信息)
2. 通过删除选中主机可以来断开远程主机的连接

#### 设置
1. 设置可以进行头像等信息的设置，点击取消/切换其他页面会重置设置信息(当前信息保存在本地，推荐通过Api接口获取配置信息)

### 控制端
控制端支持远程桌面查看，远程控制，远程终端命令行操作，开关机等操作


## 许可证

本项目采用 [MIT License](LICENSE) 开源。

## 联系方式

如有任何问题或建议，请联系：

- 邮箱：LvMaxZz1@outlook.com
- GitHub: [@LvMaxZz1](https://github.com/LvMaxZz1)