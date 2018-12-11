using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace DOCMS.Controllers
{
    [SessionAuthorize]
    public class OrganizationController : Controller
    {
        public ActionResult Index()
        {
            List<Organization> lst = new List<Organization>();
            try
            {
                lst = DBOperations<Organization>.GetAllOrByRange(new Organization(), Constant.usp_Organization);
            }
            catch (Exception)
            {

            }
            return View(lst);
        }

        public ActionResult New(string id = "")
        {
            long orgid = default(long);
            var obj = new Organization();
            try
            {
                if (!string.IsNullOrEmpty(id))
                    long.TryParse(Encryption64.DecryptQueryString(id), out orgid);

                if (orgid > default(long))
                {
                    obj = DBOperations<Organization>.GetSpecific(new Organization() { OrgID = orgid, Opmode = 1 }, Constant.usp_Organization);
                }
            }
            catch (Exception)
            {

            }
            return View("New", obj);
        }

        [HttpPost]
        public JsonResult SaveData(Organization postData)
        {
            long orgid = default(long);
            try
            {
                if (postData == null)
                    throw new Exception("No data found for insert");

                long.TryParse(Encryption64.DecryptQueryString(postData.EncryptedOrgID), out orgid);
                postData.OrgID = orgid;
                postData.Opmode = orgid > default(int) ? 3 : 2;
                postData.CreatedBy = postData.ModifiedBy = GlobalSettings.oUserData.ID;
                var result = DBOperations<Organization>.DMLOperation(postData, Constant.usp_Organization);

                if (result > default(int))
                    return Json(new { Success = 1, Message = string.Format("Data have been {0} successfully", orgid > default(long) ? "updated" : "saved") }, JsonRequestBehavior.AllowGet);
                else
                    throw new Exception("Operation has been failed to execute");
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}