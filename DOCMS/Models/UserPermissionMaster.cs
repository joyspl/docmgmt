using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class UserPermissionMaster
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
        public string EncryptedAccessID { get; set; }

        public string UserEmail { get; set; }
        public string OrganizationName { get; set; }
        public string LocationName { get; set; }
        public string SubLocationName { get; set; }
        public string DeptName { get; set; }
        public string ProjectName { get; set; }
        public string SubTypeName { get; set; }
    }
