# UDBF.NET

[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/apollo3zehn/udbf.net?svg=true)](https://ci.appveyor.com/project/Apollo3zehn/udbf-net) [![NuGet](https://img.shields.io/nuget/vpre/UDBF.NET.svg?label=Nuget)](https://www.nuget.org/packages/UDBF.NET)

UDBF.NET is a .NET Standard library that provides a reader for Gantner files stored in the UDBF (Universal Data-Bin-File) format (.dat).

Since no official specification exists, errors may occur. Please file an issue if you encounter problems.

The following code shows how to use read all data from a file:

```cs
var filePath = "testdata.dat";

using (var udbf = new UDBF(filePath))
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

## Advanced features

As long as the number of bytes of the generic type (here: ```float```) matches the number of bytes of the variable type (```variable.DataType```), you can provide any numeric type. For example, instead of interpreting the data as ```float```, you can also do the following:

```cs
(var timestamps, var data) = udbf.Read<uint>(variable);
```

This works since both, ```float``` and ```uint32``` have a length of 4 bytes. Of course, interpreting actual ```float``` data as ```uint32``` will result in meaningless numbers, but this feature may be useful to reinterpret other types like ```int32``` vs. ```uint32```.

## See also

Without an official format specification, this implementation is based on http://www.famosforum.de/index.php?attachment/508-udbf-107-pdf/.
