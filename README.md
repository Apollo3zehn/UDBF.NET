# UDBF.NET

[![GitHub Actions](https://github.com/Apollo3zehn/UDBF.NET/actions/workflows/build-and-publish.yml/badge.svg)](https://github.com/Apollo3zehn/UDBF.NET/actions) [![NuGet](https://img.shields.io/nuget/vpre/UDBF.NET.svg?label=Nuget)](https://www.nuget.org/packages/UDBF.NET)

UDBF.NET is a .NET Standard library that provides a reader for Gantner files stored in the UDBF (Universal Data-Bin-File) format (.dat).

Since no official specification exists, errors may occur. Please file an issue if you encounter problems.

The following code shows how to use read all data from a file:

```cs
var filePath = "testdata.dat";

using (var udbf = new UDBFFile(filePath))
{
    Console.WriteLine($"File '{filePath}' contains {udbf.Variables.Count} variables.");

    (var timestamps, var dataset) = udbf.ReadAll();
    Console.WriteLine($"The value of the first timestamp is '{timestamps[0].ToString()}'");

    foreach (var data in dataset)
    {
        Console.WriteLine($"Variable '{data.Variable.Name}' is of type {data.Variable.DataType}.");
        Console.WriteLine($"    The first value is {data.Buffer[0]}.");
    }
}
```

And here is a version to read only a specific variable:

```cs
var filePath = "testdata.dat";

using (var udbf = new UDBF(filePath))
{
    Console.WriteLine($"File '{filePath}' contains {udbf.Variables.Count} variables.");

    var variable = udbf.Variables[0];
    (var timestamps, var data) = udbf.Read<float>(variable);

    Console.WriteLine($"Variable '{data.Variable.Name}' is of type {data.Variable.DataType}.");
    Console.WriteLine($"    The first value is {data.Buffer[0]}.");
}
```

If you prefer working with raw buffers (`byte[]`) instead, just pass `byte` as generic parameter into the read method:

```cs
(var timestamps, var data) = udbf.Read<byte>(variable);
```

## See also

Without an official format specification, this implementation is based on http://www.famosforum.de/index.php?attachment/508-udbf-107-pdf/.
