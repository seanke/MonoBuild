## Getting Started

This project is a simple example of how to use the Engine.
```csharp
var groupFile = new GroupFile(new FileInfo("DUKE3D.GRP"));
var map = new MapFile(new FileInfo("E1L1.MAP"), groupFile);
```