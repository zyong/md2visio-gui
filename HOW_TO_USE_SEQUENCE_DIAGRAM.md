# 如何使用时序图转换功能

## ✅ 功能已实现

md2visio **已经支持 Mermaid 时序图 (sequenceDiagram)** 转换为 Visio 格式！核心代码已经完成，包括：

- ✅ 时序图解析器 ([md2visio/mermaid/sequence](md2visio/mermaid/sequence/))
- ✅ AST 数据结构 ([md2visio/struc/sequence](md2visio/struc/sequence/))
- ✅ Visio 绘制引擎 ([md2visio/vsdx/VBuilderSe.cs](md2visio/vsdx/VBuilderSe.cs), [md2visio/vsdx/VDrawerSe.cs](md2visio/vsdx/VDrawerSe.cs))
- ✅ 配置文件支持 ([md2visio/default/sequence.yaml](md2visio/default/sequence.yaml))
- ✅ 测试文件 ([md2visio/test/sequence.md](md2visio/test/sequence.md))

## 📝 准备测试文件

创建一个 `.md` 文件，包含 Mermaid 时序图代码块：

```markdown
## 时序图测试

\`\`\`mermaid
sequenceDiagram
    participant Alice
    participant Bob
    Alice->>Bob: Hello Bob, how are you?
    Bob-->>Alice: I am good thanks!
    Alice->>Bob: Nice to hear!
\`\`\`
```

## 🚀 使用方法

### 方法一：使用 GUI 版本（**推荐**）

这是最简单的方法！

1. 运行 GUI 程序：
   ```bash
   md2visio.GUI\bin\Debug\net8.0-windows\win-x64\md2visio.GUI.exe
   ```

2. 在窗口中：
   - 点击"选择文件"按钮，或直接拖拽 `.md` 文件到窗口
   - 选择输出目录
   - 点击"开始转换"按钮

3. 转换完成后，Visio 文件会生成在指定的输出目录中

### 方法二：使用命令行版本

#### Windows 批处理文件（最简单）

我已经为您创建了测试脚本 [test_sequence.bat](test_sequence.bat)，直接双击运行即可！

或者手动运行：

```batch
# 进入项目根目录
cd d:\Develop\md2visio-gui

# 运行批处理文件
test_sequence.bat
```

#### 直接使用 exe 文件

```batch
# 基本用法
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I "输入文件.md" /O "输出文件.vsdx" /Y

# 显示 Visio 窗口（查看绘制过程）
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I "test_sequence.md" /O "output.vsdx" /Y /V

# 启用调试输出
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I "test_sequence.md" /O "output.vsdx" /Y /D
```

#### ⚠️ 使用 dotnet run 的注意事项

**不推荐**使用 `dotnet run` 运行命令行版本，因为它会错误地解析 Windows 路径参数。例如：
- `/I` 会被解析为驱动器 `I:/`
- `/O` 会被解析为驱动器 `O:/`

如果一定要使用 `dotnet run`，请使用以下方式：

```bash
# 错误示例（不要这样做）
dotnet run --project md2visio/md2visio.csproj --configuration ConsoleRelease -- /I test.md /O output.vsdx

# 请改用直接运行 exe 或使用 GUI 版本
```

## 📊 命令行参数说明

| 参数 | 说明 | 必需 | 示例 |
|------|------|------|------|
| `/I` | 指定输入的 Markdown 文件路径 | 是 | `/I "test_sequence.md"` |
| `/O` | 指定输出的 Visio 文件路径或目录 | 是 | `/O "output.vsdx"` |
| `/Y` | 静默覆盖已存在的输出文件 | 否 | `/Y` |
| `/V` | 显示 Visio 窗口（查看绘制过程）| 否 | `/V` |
| `/D` | 启用调试输出 | 否 | `/D` |

## 🎯 完整示例

### 示例 1：基本时序图

**输入文件** (`simple_sequence.md`):
```markdown
\`\`\`mermaid
sequenceDiagram
    Alice->>Bob: Hello
    Bob-->>Alice: Hi
\`\`\`
```

**命令**:
```batch
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I "simple_sequence.md" /O "simple_output.vsdx" /Y
```

### 示例 2：带参与者定义的时序图

**输入文件** (`advanced_sequence.md`):
```markdown
\`\`\`mermaid
sequenceDiagram
    participant A as Alice
    participant B as Bob
    participant C as Charlie

    A->>B: Request data
    B->>C: Forward request
    C-->>B: Return data
    B-->>A: Send response
\`\`\`
```

**命令**:
```batch
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I "advanced_sequence.md" /O "advanced_output.vsdx" /Y /V
```

## 🔍 故障排查

### 问题：命令运行后没有生成文件

**可能原因和解决方案**：

1. **未安装 Microsoft Visio**
   - 确保电脑上已安装 Visio 桌面版
   - 尝试手动打开 Visio 确认可以正常运行

2. **输入文件路径错误**
   - 检查文件路径是否正确
   - 使用绝对路径或确保相对路径正确
   - 路径中有空格时使用引号包裹

3. **输出路径无写入权限**
   - 确保输出目录存在
   - 检查是否有写入权限
   - 尝试输出到其他目录

4. **Markdown 格式错误**
   - 确保使用 ````mermaid` 代码块包裹
   - 检查时序图语法是否正确
   - 参考 [md2visio/test/sequence.md](md2visio/test/sequence.md) 的格式

### 问题：程序显示帮助信息后退出

这说明参数解析失败。请检查：
- 确保使用 `/I` 和 `/O` (大小写不敏感)
- 确保每个参数后都跟着对应的值
- 如果使用 `dotnet run`，请改用直接运行 exe

### 问题：Visio 进程残留

如果转换失败导致 Visio 进程残留：
```batch
# 结束所有 Visio 进程
taskkill /F /IM VISIO.EXE
```

## 📚 更多信息

- 详细的语法指南：[docs/SEQUENCE_DIAGRAM_GUIDE.md](docs/SEQUENCE_DIAGRAM_GUIDE.md)
- Mermaid 官方文档：https://mermaid.js.org/syntax/sequenceDiagram.html
- 项目主 README：[README.md](README.md)

## 🤝 反馈

如果遇到问题或有改进建议，欢迎：
- 提交 [GitHub Issue](https://github.com/konbakuyomu/md2visio-gui/issues)
- 查看项目文档获取更多帮助

---

**更新时间**: 2025-12-25
