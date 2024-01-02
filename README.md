# Maa Star Remote

## 简介

使用 DotNet Core 6 Web 应用实现的 **简易** Maa 远程控制服务器。项目中仅提供了 Web UI 前端，但功能提供了可匿名访问的端点。可使用其他应用调用。

项目完成度仅**够用**。

---

## 安装

- 前往[**Release**](https://github.com/star-whisper9/MaaStarRemote/releases)下载最新的 Release 包
- 确保安装了 DotNet 6 运行环境
- 使用终端启动**MaaStarRemote.exe**
- 自行暴露端口部署或使用其他应用调用
- _按需修改_ `appsettings.json`

---

## 端点

所有端点均要求 Https 访问。

#### 更新任务列表

```
api/Core/addTasks
```

功能：清空对应用户的任务列表，新增新的任务列表

类型：**HttpPost**

接受：**Application/Json**

Json 示例：

```
{
    "user":"string",
    "device":"string",
    "tasks":[
        {"order":0,"task":"string"},
        ...
        ]
}
```

返回值：**Http 200**；数据库未找到用户/设备时**Http 403**

---

#### 获取截图任务回报信息

```
api/Core/getReport
```

功能：获取某天的截图任务的回报信息

类型：**HttpPost**

接受：**Application/Json**

Json 示例：

```
{
    "user":"string",
    "device":"string",
    "time":"date or time inclued date info"
}
```

返回值：**Http 200**与一个列表，包含所有截图任务的回报信息；数据库未找到用户/设备时**Http 403**；数据库未找到对应用户的截图任务回报时**Http 404**

返回列表示例：

```
{
    [
        "status":"SUCCESS",
        "payload":"data:image/png;base64,..."
    ],
    [
        "status":"FAILED",
        "payload":""
    ],
    ...
}
```
