using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

public class Organization : ModelBase
{
    public long OrgID { get; set; }

    public string EncryptedOrgID { get; set; }
    public string OrganizationName { get; set; }
    public string OrgShortName { get; set; }
    public string OrgDescription { get; set; }
    public int IsActive { get; set; }
    public long CreatedBy { get; set; }
    public string CreatedByFullName { get; set; }
    public DateTime CreatedOn
    {
        get
        {
            return this._CreatedOn.HasValue ? this._CreatedOn.Value : DateTime.Now;
        }
        set
        {
            this._CreatedOn = value;
        }
    }
    public long ModifiedBy { get; set; }
    public string ModifiedByFullName { get; set; }
    public DateTime ModifiedOn
    {
        get
        {
            return this._ModifiedOn.HasValue ? this._ModifiedOn.Value : DateTime.Now;
        }
        set
        {
            this._ModifiedOn = value;
        }
    }

    [XmlIgnore, JsonIgnore]
    public int Opmode { get; set; }
}