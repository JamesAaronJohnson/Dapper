# Dapper
Dapper is a data Appending Library for reading and writing data at the end of files. 
This allows the storing of data in images, videos, music or other formats that support this method.

## Overview
Here's a quick overview of how to use Dapper to append some fake save data, read it back, and then write it back to a file.
```cs
void Example ()
{
    string path = @"folder\some_file.png";
    byte[] sourceData = File.ReadAllBytes (path);
    
    Save save = new Save ();
    string json = save.ToJSON ();
    
    byte[] appendedData = Dapper.AppendAll (sourceData, json, Encoding.UTF8);
    
    if (Dapper.HasAppendedData (appendedData))
    {
        string appendedJSON = Dapper.Read (appendedData, Encoding.UTF8);
        Console.WriteLine (appendedJSON);
    }
    
    File.WriteAllBytes (path, appendedData);
}
```

## Getting Started

To get started, clone or download the repo and grab the Dapper.cs file.

Or, right click and save the raw: [Dapper.cs](https://raw.githubusercontent.com/Chi-Time/Dapper/master/Dapper/Dapper.cs?token=ACT4OTBKCMGK6EQ4WSEHRYDB3JKWO) file here.

After adding Dapper to your project, check out the [Wiki](https://github.com/Chi-Time/Dapper/wiki/Dapper-Example-Usages) for the documentation and some more in-depth examples.

# Licence
Dapper is licensed under the Apache License, Version 2.0. See
[LICENSE](https://github.com/moby/moby/blob/master/LICENSE) for the full
license text.
