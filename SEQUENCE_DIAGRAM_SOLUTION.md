# 时序图功能解决方案

## 🔍 问题根源

经过详细分析，发现时序图功能**已经 90% 实现**，但解析器存在严重的 Bug，导致：

1. ✅ **已完成的部分**：
   - 数据结构（`Participant`, `Message`, `Sequence`）
   - Visio 绘制器（`VDrawerSe`, `VBuilderSe`）
   - Builder 逻辑（`SeBuilder`）
   - 配置文件（`sequence.yaml`）
   - TypeMap 注册

2. ❌ **存在问题的部分**：
   - **解析器状态机**：将多个token合并成一个
   - 例如：`Alice ->> Bob: Hello` 被解析成 `'AliceBob:'`, `'Hello'` 等错误的片段

## 🔧 解决方案选项

### 方案 A: 修复现有解析器（推荐）

**优点**：
- 保持代码一致性
- 利用现有架构
- 解决根本问题

**需要做的**：
1. 调试序列图解析器的状态转换逻辑
2. 确保 `SeSttChar` 正确处理空格、冒号、箭头
3. 修复 Buffer 累积问题

**预计工作量**：2-4小时的调试

###方案 B: 使用简化的临时解析器

创建一个简单的正则表达式解析器，专门处理基本的时序图语法：

```csharp
// 简化版解析器（临时方案）
public class SimpleSequenceParser
{
    // participant Alice
    Regex participant = new Regex(@"participant\s+(\w+)(?:\s+as\s+(.+))?");

    // Alice->>Bob: Hello
    Regex message = new Regex(@"(\w+)\s*(--?>>?)\s*(\w+)\s*:\s*(.+)");

    public Sequence Parse(string content)
    {
        var seq = new Sequence();
        var lines = content.Split('\n');

        foreach (var line in lines)
        {
            var partMatch = participant.Match(line);
            if (partMatch.Success)
            {
                string name = partMatch.Groups[1].Value;
                string alias = partMatch.Groups[2].Success ?
                    partMatch.Groups[2].Value : name;
                seq.Participants.Add(new Participant(name, alias));
                continue;
            }

            var msgMatch = message.Match(line);
            if (msgMatch.Success)
            {
                string from = msgMatch.Groups[1].Value;
                string arrow = msgMatch.Groups[2].Value;
                string to = msgMatch.Groups[3].Value;
                string text = msgMatch.Groups[4].Value.Trim();

                // Auto-add participants
                if (!seq.Participants.Any(p => p.Alias == from))
                    seq.Participants.Add(new Participant(from));
                if (!seq.Participants.Any(p => p.Alias == to))
                    seq.Participants.Add(new Participant(to));

                MessageType type = arrow.Contains(">>") ?
                    MessageType.Solid : MessageType.Dashed;
                seq.Messages.Add(new Message(from, to, text, type));
            }
        }

        return seq;
    }
}
```

**优点**：
- 快速实现
- 容易理解和维护
- 能立即工作

**缺点**：
- 不支持复杂语法
- 与现有架构不一致
- 需要额外维护

### 方案 C: 使用 JavaScript Mermaid 解析器（不推荐）

通过 JavaScript Interop 调用官方 Mermaid.js 解析器。

**优点**：
- 100% 兼容 Mermaid 语法
- 官方支持

**缺点**：
- 需要 JavaScript 运行时
- 性能开销
- 部署复杂
- 与项目架构严重不符

## 📊 可用的 C# Mermaid 库

目前 NuGet 上的 Mermaid 相关库都是**生成器**，不是**解析器**：

- [MermaidDotNet](https://github.com/FoggyBalrog/MermaidDotNet) - 生成 Mermaid 代码
- [Cs2Mermaid](https://www.nuget.org/packages/Cs2Mermaid/) - C# 转 Mermaid
- [MermaidNet](https://www.nuget.org/packages/MermaidNet) - 生成库
- [Blazorade.Mermaid](https://www.nuget.org/packages/Blazorade.Mermaid/1.3.0) - Blazor 集成

**结论**：没有现成的 C# Mermaid 解析器可以直接使用。

## 🎯 推荐行动方案

### 立即可行的方案（方案 B）

**步骤**：

1. 创建 `SimpleSequenceParser.cs`（见上面的代码示例）

2. 修改 `SeBuilder.cs`，添加一个临时的解析路径：

```csharp
public override void Build(string outputFile)
{
    // 临时方案：直接从原始文本解析
    string content = File.ReadAllText(iter.Context.InputFile);

    // 提取 mermaid 代码块
    var match = Regex.Match(content, @"```mermaid\s*\n(.*?)```",
        RegexOptions.Singleline);

    if (match.Success)
    {
        string mermaidCode = match.Groups[1].Value;
        var parser = new SimpleSequenceParser();
        sequence = parser.Parse(mermaidCode);
    }

    Output(outputFile);
}
```

3. 测试并验证

**优点**：30分钟内可以完成并测试

### 长期方案（方案 A）

需要深入调试现有解析器，修复状态机逻辑。这需要：

1. 理解整个状态机架构
2. 跟踪每个状态的转换
3. 找出 Buffer 累积的根本原因
4. 修复并测试

**预计时间**：2-4小时

## 💡 我的建议

1. **短期**：使用方案 B（简化解析器）快速实现功能
2. **中期**：逐步完善简化解析器，支持更多语法
3. **长期**：有时间后再修复原始解析器（或保持简化版本）

这样可以：
- ✅ 立即让时序图功能可用
- ✅ 满足基本使用需求
- ✅ 为用户提供价值
- 📝 将复杂的解析器修复作为技术债务记录下来

## 📝 下一步

您希望我：

**A. 实现简化解析器（快速方案）**
我可以立即编写代码，30分钟内让时序图功能工作。

**B. 继续调试现有解析器**
需要更多时间深入分析状态机逻辑。

**C. 两者都做**
先实现简化版让功能可用，然后并行调试原解析器。

请告诉我您的选择！

---

## Sources
- [MermaidDotNet on GitHub](https://github.com/FoggyBalrog/MermaidDotNet)
- [Cs2Mermaid on NuGet](https://www.nuget.org/packages/Cs2Mermaid/)
- [Blazorade.Mermaid on NuGet](https://www.nuget.org/packages/Blazorade.Mermaid/1.3.0)
- [MermaidNet on NuGet](https://www.nuget.org/packages/MermaidNet)
- [MermaidDotNet on NuGet](https://www.nuget.org/packages/MermaidDotNet)
