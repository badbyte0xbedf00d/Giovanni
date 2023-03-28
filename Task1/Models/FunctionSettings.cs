using System;

namespace Giovanni.Task1.Models;

public class FunctionSettings
{
    public Uri PublicApiFetchUri { get; set; }
    public string BlobContainerName { get; set; }
    public string LogTableName { get; set; }
}