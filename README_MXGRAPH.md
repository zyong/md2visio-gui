# mxGraph XML 转 Visio 使用说明

## 功能概述

本工具支持将 mxGraph XML 格式转换为 Microsoft Visio 文件（.vsdx），包含以下特性：

✅ **形状支持**：
- Ellipse（椭圆）- 开始/结束节点
- Rectangle（矩形）- 标准流程
- Rounded Rectangle（圆角矩形）- 子流程
- Diamond（菱形）- 决策节点

✅ **样式支持**：
- fillColor（填充颜色）- 支持 #RRGGBB 格式
- strokeColor（边框颜色）- 支持 #RRGGBB 格式
- 自动连接线生成

✅ **坐标转换**：
- 自动处理 mxGraph 像素坐标到 Visio 英寸坐标
- 自动处理 Y 轴反转（mxGraph 从上到下，Visio 从下到上）

## 命令行操作

### 方法1：使用 PowerShell 脚本（推荐）

```powershell
# 进入项目目录
cd d:\Develop\md2visio-gui

# 运行转换脚本
.\convert_mxgraph.ps1
```

### 方法2：使用批处理文件

```batch
# 进入项目目录
cd d:\Develop\md2visio-gui

# 运行批处理
test_mxgraph.bat
```

### 方法3：使用 C# 代码

```csharp
using md2visio.mxgraph;
using System.IO;

// 读取 XML 文件
string xmlContent = File.ReadAllText("test_mxgraph.md");

// 创建转换器
var converter = new MxGraphConverter();

// 转换为 Visio 文件
converter.ConvertToVisio(xmlContent, "output.vsdx");
```

### 方法4：编程方式

```csharp
using md2visio.mxgraph;

// 直接使用 XML 字符串
string xml = @"
<mxGraphModel>
  <root>
    <mxCell id='2' value='开始' style='ellipse;fillColor=#dae8fc;strokeColor=#6c8ebf;' vertex='1' parent='1'>
      <mxGeometry x='300' y='40' width='120' height='60' as='geometry'/>
    </mxCell>
  </root>
</mxGraphModel>";

var converter = new MxGraphConverter();
converter.ConvertToVisio(xml, "output.vsdx");
```

## XML 格式说明

### 节点（Vertex）格式

```xml
<mxCell id="唯一ID"
        value="节点文本"
        style="形状;属性=值;属性=值;"
        vertex="1"
        parent="1">
  <mxGeometry x="X坐标" y="Y坐标" width="宽度" height="高度" as="geometry"/>
</mxCell>
```

### 连接线（Edge）格式

```xml
<mxCell id="唯一ID"
        style="edgeStyle=orthogonalEdgeStyle;endArrow=classic;"
        edge="1"
        parent="1"
        source="起始节点ID"
        target="目标节点ID">
  <mxGeometry relative="1" as="geometry"/>
</mxCell>
```

### 样式属性

| 属性名 | 说明 | 示例 |
|--------|------|------|
| `fillColor` | 填充颜色 | `#dae8fc` |
| `strokeColor` | 边框颜色 | `#6c8ebf` |
| `ellipse` | 椭圆形状 | 无值 |
| `rhombus` | 菱形形状 | 无值 |
| `rounded` | 圆角矩形 | `rounded=1` |
| `whiteSpace` | 文本换行 | `wrap` |
| `html` | HTML 支持 | `1` |

## 颜色配置示例

### 蓝色系（开始/结束）
```
fillColor=#dae8fc;strokeColor=#6c8ebf;
```
效果：淡蓝色填充，深蓝色边框

### 黄色系（流程步骤）
```
fillColor=#fff2cc;strokeColor=#d6b656;
```
效果：淡黄色填充，金色边框

### 粉色系（决策）
```
fillColor=#f8cecc;strokeColor=#b85450;
```
效果：淡粉色填充，深粉色边框

### 绿色系（成功/完成）
```
fillColor=#d5e8d4;strokeColor=#82b366;
```
效果：淡绿色填充，深绿色边框

## 测试文件

项目包含测试文件 `test_mxgraph.md`，包含一个完整的用户访问网站流程图：

- 8 个节点（椭圆、矩形、菱形）
- 7 条连接线
- 4 种颜色配置
- 文本标签

## 输出结果

转换后的 Visio 文件 (`test_mxgraph_output.vsdx`) 将包含：

1. **形状准确性**：所有形状类型正确转换
2. **颜色还原**：填充和边框颜色与 XML 定义一致
3. **位置精确**：节点位置根据坐标正确放置
4. **自动连接**：连接线自动粘附到形状中心点
5. **文本显示**：所有节点文本正确显示

## 故障排查

### 问题1：编译失败
```
解决方法：
1. 确保安装了 .NET 8.0 SDK
2. 运行：dotnet build md2visio/md2visio.csproj --configuration Release
```

### 问题2：Visio 未安装
```
错误信息：无法创建 COM 对象
解决方法：安装 Microsoft Visio（需要专业版或标准版）
```

### 问题3：坐标位置不对
```
说明：mxGraph 使用像素（96 DPI），Visio 使用英寸
转换公式：inches = pixels / 96.0
Y 轴已自动反转处理
```

### 问题4：颜色不显示
```
检查：
1. XML 中颜色格式是否为 #RRGGBB
2. 确保样式字符串正确解析（分号分隔）
```

## 技术细节

### 坐标系统转换

```
mxGraph:      Visio:
(0,0)------X  Y------
|             |     |
|             |     |
Y             (0,0)-X

转换公式：
visioX = mxGraphX / 96.0
visioY = pageHeight - (mxGraphY / 96.0)
```

### 颜色转换

```
输入: #dae8fc (hex)
解析: R=218, G=232, B=252
输出: RGB(218, 232, 252) (Visio格式)
```

## 扩展开发

如需添加新的形状支持，修改 `MxGraphVBuilder.cs` 中的 `GetMasterName` 方法：

```csharp
private string GetMasterName(string? style)
{
    if (style == null) return "Rectangle";

    if (style.Contains("ellipse")) return "Ellipse";
    if (style.Contains("rhombus")) return "Diamond";
    if (style.Contains("rounded=1")) return "Rounded Rectangle";
    // 添加新形状
    if (style.Contains("triangle")) return "Triangle";

    return "Rectangle";
}
```

## 文件列表

- `md2visio/mxgraph/MxGraphParser.cs` - XML 解析器
- `md2visio/mxgraph/MxStyleParser.cs` - 样式解析器
- `md2visio/mxgraph/MxGraphConverter.cs` - 转换器主类
- `md2visio/mxgraph/MxGraphVBuilder.cs` - Visio 构建器
- `test_mxgraph.md` - 测试 XML 文件
- `convert_mxgraph.ps1` - PowerShell 测试脚本
- `test_mxgraph.bat` - 批处理测试脚本

## 许可证

根据项目主许可证
