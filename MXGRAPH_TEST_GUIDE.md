# mxGraph to Visio Conversion Test Guide

## The Problem

The batch file `test_mxgraph.bat` encounters encoding issues when run from PowerShell. Windows batch files with UTF-8 encoding and special characters can be misinterpreted by PowerShell's execution environment.

## Solution: Use PowerShell Directly

### ⭐ Option 1: Built-in Test Command (Easiest & Most Reliable)

```powershell
.\test_mxgraph_builtin.ps1
```

**Best for:** Quick testing of the mxGraph implementation.

This uses the built-in `testmx` command that's already in the console app:
- No CSC compiler needed
- Uses hardcoded test data
- Creates output at `d:\temp\test.vsdx`
- Automatically opens in Visio

### Option 2: Test with test_mxgraph.md File

```powershell
.\test_mxgraph_run.ps1
```

**Best for:** Testing with the actual test_mxgraph.md file.

This creates a temporary .NET project and runs the conversion:
- No CSC compiler needed
- Uses `test_mxgraph.md` as input
- Creates output at `test_mxgraph_output.vsdx`
- Cleans up temporary files automatically

### Option 3: Run Batch File from CMD

If you must use the batch file, run it from a proper CMD prompt (not PowerShell):

1. Open Command Prompt (cmd.exe)
2. Navigate to the project directory:
   ```cmd
   cd d:\Develop\md2visio-gui
   ```
3. Run the batch file:
   ```cmd
   test_mxgraph.bat
   ```

**Note:** Requires CSC compiler in PATH.

### Option 4: Use the Original PowerShell Script

```powershell
.\convert_mxgraph.ps1
```

**Note:** Requires CSC compiler in PATH.

## Expected Output

After successful conversion, you should see a file named `test_mxgraph_output.vsdx` containing:

1. **8 Shapes:**
   - 开始访问网站 (ellipse, blue)
   - 浏览首页内容 (rounded rectangle, yellow)
   - 寻找需要的信息 (rounded rectangle, yellow)
   - 是否找到需要的内容？ (diamond, pink)
   - 继续浏览其他页面 (rounded rectangle, yellow)
   - 执行目标行动 (rounded rectangle, green)
   - 离开网站 (ellipse, blue)
   - Plus labels "是" and "否"

2. **7 Connectors:**
   - Orthogonal-style arrows connecting the shapes

3. **Correct Colors:**
   - Blue: #dae8fc
   - Yellow: #fff2cc
   - Pink: #f8cecc
   - Green: #d5e8d4

## Troubleshooting

### CSC Not Found

If you get "CSC compilation failed", you need the C# compiler in your PATH.

**Fix:**
1. Find the Developer Command Prompt for Visual Studio
2. Run the test from there, OR
3. Add CSC to your PATH:
   ```
   C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn
   ```

### Visio Not Installed

The conversion requires Microsoft Visio to be installed on your system.

### Build Failed

Ensure you have .NET 8 SDK installed:
```powershell
dotnet --version
```

Should show 8.0.x or higher.

## Technical Details

### Why the Batch File Has Issues

1. **UTF-8 BOM**: Batch files with UTF-8 encoding can confuse CMD/PowerShell
2. **Special Characters**: Characters like `(`, `)`, `{`, `}` need careful escaping
3. **PowerShell Context**: PowerShell interprets batch files differently than CMD

### The Fix

The `test_mxgraph_simple.ps1` script:
- Uses PowerShell's native here-strings for multi-line code
- Handles UTF-8 natively
- Uses proper path resolution
- Has better error handling

## Files

- `test_mxgraph_builtin.ps1` ⭐ - **Recommended**: Uses built-in testmx command (no CSC needed)
- `test_mxgraph_run.ps1` - Tests with test_mxgraph.md file (no CSC needed)
- `test_mxgraph.bat` - Original batch file (use from CMD only, requires CSC)
- `test_mxgraph_simple.ps1` - PowerShell test with CSC compilation (requires CSC)
- `convert_mxgraph.ps1` - Original working PowerShell script (requires CSC)
- `test_mxgraph.md` - Test data (mxGraph XML)
- `MXGRAPH_TEST_GUIDE.md` - This file

## Quick Start (TL;DR)

Just run this command:

```powershell
.\test_mxgraph_builtin.ps1
```

Or if you want to test with test_mxgraph.md:

```powershell
.\test_mxgraph_run.ps1
```

Both work without needing CSC compiler!
