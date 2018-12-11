using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

public class User : ModelBase
{
    public long ID { get; set; }
    public string FullName { get; set; }
    public string UserEmail { get; set; }
    public string UserPassword { get; set; }
    public long RoleID { get; set; }
    public string RoleName { get; set; }
    public int IsActive { get; set; }
    public long SubTypeID { get; set; }
    public string SubTypeName { get; set; }
    public long ProjectID { get; set; }
    public string ProjectName { get; set; }
    public long DeptID { get; set; }
    public string DeptName { get; set; }
    public long SubLocationID { get; set; }
    public string SubLocationName { get; set; }
    public long LocationID { get; set; }
    public string LocationName { get; set; }
    public long OrgID { get; set; }
    public string OrganizationName { get; set; }
    public string OrgShortName { get; set; }
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

public class UserPermission
{
    public long AccessID { get; set; }
    public long UserID { get; set; }
    public long OrgID { get; set; }
    public long LocationID { get; set; }
    public long SublocationID { get; set; }
    public long DeptID { get; set; }
    public long ProjectID { get; set; }
    public long SubTypeID { get; set; }
    public int HasFullPermission { get; set; }

    public int Opmode { get; set; }
}