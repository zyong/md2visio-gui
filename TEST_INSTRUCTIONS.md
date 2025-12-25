# 时序图测试说明

## 请按照以下步骤测试

### 1. 打开命令提示符

在项目根目录 `d:\Develop\md2visio-gui` 打开命令提示符

### 2. 运行测试命令

```cmd
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I test_sequence.md /O test_sequence_output.vsdx /Y
```

### 3. 查看输出

程序会输出详细的调试信息，包括：
- 解析的状态总数
- 每个状态的类型和内容
- 关键字处理详情
- 文本/消息处理详情
- 参与者和消息的添加情况
- Visio 文件的生成情况

### 4. 检查结果

运行完成后，检查：
- 是否生成了 `test_sequence_output.vsdx` 文件
- 文件大小是否正常（不应该是0字节）
- 能否在 Visio 中正常打开

### 5. 如果失败

请将**完整的命令行输出**复制给我，包括：
- 所有调试信息
- 任何错误消息
- 最后的状态（成功/失败）

## 预期的输出示例

正常情况下应该看到类似这样的输出：

```
Input file: test_sequence.md
Output path: test_sequence_output.vsdx
Creating SynContext...
Running SttMermaidStart...
Parsed states: 26
Creating FigureBuilderFactory...
Building figure...
Building sequence diagram
Total states in context: 26
[1] Processing: SttMermaidStart - '```'
  -> Found MermaidStart, continuing...
[2] Processing: SttFinishFlag - '\n'
  -> Found FinishFlag (line end), continuing...
[3] Processing: SeSttKeyword - 'sequenceDiagram'
  -> Found Keyword, building...
  BuildKeyword: 'sequenceDiagram'
    ! Unknown keyword: 'sequenceDiagram'
[4] Processing: SttFinishFlag - '\n'
  -> Found FinishFlag (line end), continuing...
[5] Processing: SeSttKeyword - 'participant'
  -> Found Keyword, building...
  BuildKeyword: 'participant'
    Found 'participant' keyword
    After skip participant: SeSttText - 'Alice'
    Participant name: 'Alice'
    Next token: '\n'
    ✓ Adding participant: name='Alice', alias='Alice'
...
```

## 常见问题

### 问题1: 没有任何输出
- 检查 exe 文件路径是否正确
- 确保在正确的目录下运行命令

### 问题2: 提示文件不存在
- 确保 `test_sequence.md` 在当前目录
- 使用绝对路径试试

### 问题3: 生成了文件但打不开
- 检查是否安装了 Microsoft Visio
- 查看文件大小是否为 0
- 查看调试输出中是否有错误

---

**如果您看到了输出，请将完整的输出内容复制给我！**
