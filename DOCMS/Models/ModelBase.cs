using System;
using System.ComponentModel;

public class ModelBase
{
    protected DateTime? _CreatedOn = null;
    protected DateTime? _ModifiedOn = null;
    protected DateTime? _DocDate = null;
}

public enum Role
{
    [Description("Built-in Administrator")]
    BuiltinAdmin = 1,
    [Description("Admin")]
    Admin = 2
}

public enum Dropdown
{
    [Description("Project")]
    Project = 0,
    [Description("Department")]
    Department = 1,
    [Description("Project Sub-Type")]
    SubType = 2,
    [Description("Sub Location")]
    SubLocation = 3,
    [Description("Location")]
    Location = 4,
    [Description("Financial Year")]
    FinYear = 5,
    [Description("Organization")]
    Organization = 6
}

public enum TaggingType
{
    ScanOnly = 1,
    ScanAndOCR = 2
}