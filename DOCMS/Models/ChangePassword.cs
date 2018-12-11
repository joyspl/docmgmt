using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

public class ChangePassword : ModelBase
{
    // defined variables used in changePassword controller

    public long ID { get; set; }
    public string UserPassword { get; set; }
}
