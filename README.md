# UCParser

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![C# Version](https://img.shields.io/badge/C%23-.NET%206.0-purple.svg)](https://dotnet.microsoft.com/en-us/)

## 📋 Overview

UCParser is a professional-grade tool designed for reverse engineering Unity-compiled applications. It analyzes Unity application assemblies to extract method and field information, providing valuable insights for developers, security researchers, and software engineers. By leveraging reflection capabilities, UCParser can inspect running Unity processes or analyze assemblies directly from specified paths.

## ✨ Key Features

- **Live Process Analysis**: Connect to running Unity applications to analyze their assemblies in real-time
- **Method and Field Extraction**: Extract comprehensive method signatures, field definitions, and their respective tokens
- **Flexible Filtering**: Search for specific classes or methods using targeted filters
- **Parallel Processing**: Utilize asynchronous operations for improved performance with large assemblies
- **Detailed Logging**: Comprehensive logging system to track extraction progress and any issues
- **Human-Readable Output**: Results are formatted in easily readable text files organized by class

## 🚀 Getting Started

### Prerequisites

- Windows operating system
- .NET 6.0 SDK or later
- Visual Studio 2022 (recommended) or any compatible IDE

### Installation

1. Clone the repository:
   ```
   git clone https://github.com/kimurux/UCParser.git
   ```

2. Open the solution in Visual Studio:
   ```
   cd UCParser
   start UCParser.sln
   ```

3. Build the solution:
   ```
   dotnet build
   ```

### Usage

1. Run the application:
   ```
   dotnet run
   ```

2. Follow the interactive prompts:
   - Enter the name of the Unity process (without .exe extension)
   - Alternatively, provide a manual path to the Managed folder
   - Optionally specify class or method name filters
   - View results in the generated output files /Parsed/ProgramName

## 📊 Example Output

Each class file will contain structured information similar to this:

```
Class: GameManager
Token: 0x02000123

Method:
=======
Method: private void Start(), Token: 0x06001456
Method: public void LoadLevel(string levelName), Token: 0x06001457
Method: public static GameManager GetInstance(), Token: 0x06001458

Fields:
=====
private int _score, Token: 0x04000789
public static GameManager Instance, Token: 0x0400078A
```

## 🔍 Advanced Usage

### Targeted Class Analysis

To analyze a specific class within an assembly:
```
Class name: Player
```

### Method Filtering

To search for specific methods across all classes:
```
Method name: Update
```

### Manual Assembly Path

When automatic process detection fails, specify the path manually:
```
Path: C:\Games\MyGame\MyGame_Data\Managed
```

## 📝 License

Distributed under the MIT License. See `LICENSE` file for more information.

## 📮 Contact

Project Link: [https://github.com/kimurux/UCParser](https://github.com/kimurux/UCParser)

---

<p align="center">
  <i>Made with ❤️ for the reverse engineering community</i>
</p>
