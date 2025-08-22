# Claw Game (抓娃娃机游戏)

一个基于Unity开发的简单抓娃娃机小游戏

![游戏截图](Game%20Screenshot.png)

## 游戏介绍

这是一个基于Unity开发的简单抓娃娃机小游戏。玩家可以通过控制杆控制机械爪的移动，按下按钮让机械爪抓取娃娃。

## 功能特点

- 可调节的抓取难度设置
- 真实物理引擎模拟抓取过程
- 强力爪机制
- 弹幕系统


## 项目结构

```
Assets/
├── Animation/           # 动画文件
├── Audios/              # 音频资源
├── Materials/           # 材质文件
├── Models/              # 3D模型
├── Prefabs/             # 预制体
├── Scenes/              # 场景文件
├── Scripts/             # 脚本文件
└── Textures/            # 纹理贴图
```

## 主要脚本

- [ClawController.cs](Assets/Scripts/ClawController.cs) - 控制机械爪的移动和抓取逻辑
- [ButtonsController.cs](Assets/Scripts/ButtonsController.cs) - 处理用户输入控制
- [GameManager.cs](Assets/Scripts/GameManager.cs) - 游戏主管理器
- [AudioController.cs](Assets/Scripts/AudioController.cs) - 音频控制
- [DanmakuController.cs](Assets/Scripts/DanmakuController.cs) - 弹幕系统控制
