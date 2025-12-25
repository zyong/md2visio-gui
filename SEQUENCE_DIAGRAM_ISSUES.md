# 时序图功能当前问题分析

## 🔍 问题诊断

根据测试输出，发现了以下问题：

### 问题 1: 解析器文本合并错误

**症状**：
```
[5] Processing: SeSttText - 'AliceBob:'
```

**预期应该是**：
```
SeSttText - 'Alice'
SeSttArrow - '->>'
SeSttText - 'Bob'
SeSttColon - ':'
SeSttText - 'Hello Bob, how are you?'
```

**实际情况**：
- `Alice` 和 `Bob:` 被合并成了 `AliceBob:`
- 消息文本被分割成了多个token：`'Hello'`, `'Bob,'`, `'how'`, `'are'`, `'you?'`

### 问题 2: 空格处理不正确

解析器在遇到空格时：
1. 应该将空格作为分隔符，分隔不同的token
2. 但实际上可能跳过了空格处理逻辑

### 问题 3: 冒号未被正确识别为 SeSttColon

**症状**：
- 没有看到任何 `SeSttColon` 状态
- 冒号被包含在了文本中（如 `'AliceBob:'`, `'BobAlice:'`）

##  根本原因

从解析器代码来看（`SeSttChar.cs`），应该能正确识别：
- 空格 → 触发 `SeSttWordFlag`
- 冒号 `:` → 触发 `SeSttColon`
- 箭头 `-` → 触发 `SeSttArrow`

但实际运行时这些规则没有生效，可能的原因：

1. **解析器状态机没有正确初始化**
   - `sequenceDiagram` 关键字后应该进入 `SeSttChar` 状态
   - 但可能进入了错误的状态

2. **SttFigureType 或 TypeMap 配置问题**
   - 可能 `sequenceDiagram` 没有正确映射到 `SeSttChar`

3. **Buffer 累积问题**
   - 解析器可能在某个状态下持续累积字符而没有触发正确的转换

## 🔧 建议的修复方案

### 方案 1: 检查 TypeMap 配置

确保 `sequenceDiagram` 正确映射到序列图的字符处理器：

```csharp
// TypeMap.cs
{ "sequenceDiagram", typeof(SeSttChar) },  // 应该是这样
```

### 方案 2: 修改 SeSttKeyword 处理

`SeSttKeyword.NextState()` 在处理 `sequenceDiagram` 时应该返回 `SeSttChar`：

```csharp
if (Buffer == "sequenceDiagram")
{
    Save(Buffer).ClearBuffer().SlideSpaces();
    return Forward<SeSttChar>();  // 这行很关键！
}
```

### 方案 3: 简化测试文件

创建一个最简单的测试用例：

```markdown
\`\`\`mermaid
sequenceDiagram
    Alice->>Bob:Hello
\`\`\`
```

注意：
- 不使用 `participant` 关键字
- 消息文本不包含空格
- 这样可以隔离问题

## 📋 验证步骤

1. 添加解析器状态转换的详细日志
2. 检查每次状态转换时的 Buffer 内容
3. 确认 `SeSttChar` 是否被正确调用
4. 确认空格、冒号、箭头的触发条件

## 🎯 下一步行动

由于这是解析器层面的问题，需要：

1. **检查 SttMermaidStart 如何启动序列图解析**
2. **验证 SeSttKeyword 的 NextState 实现**
3. **确认状态转换链是否完整**

目前 Builder 层的逻辑（`SeBuilder.cs`）基本正确，主要问题在于：
- 解析器没有正确分割输入文本
- 状态机没有按预期工作

## 🔍 需要检查的文件

1. `md2visio/mermaid/@cmn/SttMermaidStart.cs` - 启动逻辑
2. `md2visio/mermaid/@cmn/TypeMap.cs` - 类型映射
3. `md2visio/mermaid/sequence/SeSttKeyword.cs` - 关键字处理
4. `md2visio/mermaid/sequence/_internal/SeSttChar.cs` - 字符处理

## 💡 临时解决方案

在修复解析器之前，可以尝试：

1. **使用更简单的语法**（如果解析器支持）
2. **检查其他已工作的图表类型**（如 graph、journey）的解析逻辑作为参考
3. **逐步简化测试用例**，找出解析器能处理的最小子集

---

**结论**：时序图的 Builder、Drawer 和数据结构都已正确实现，但解析器层面存在严重问题，导致无法正确解析 Mermaid 语法。
