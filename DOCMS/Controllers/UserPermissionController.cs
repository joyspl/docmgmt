using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace DOCMS.Controllers
{
    [SessionAuthorize]
    public class UserPermissionController : Controller
    {
        public ActionResult Index()
        {
            List<UserPermissionMaster> lst = new List<UserPermissionMaster>();
            try
            {
                lst = DBOperations<UserPermissionMaster>.GetAllOrByRange(new UserPermissionMaster() {Opmode=3 }, Constant.usp_UserPermissionMaster);
            }
            catch (Exception)
            {

            }
            return View(lst);
        }

        public ActionResult New(string id = "")
        {
            long permissionid = default(long);
            var obj = new UserPermissionMaster();
            try
            {
                if (!string.IsNullOrEmpty(id))
                    long.TryParse(Encryption64.DecryptQueryString(id), out permissionid);

                if (permissionid > default(long))
                {
                    obj = DBOperations<UserPermissionMaster>.GetSpecific(new UserPermissionMaster() { AccessID = permissionid, Opmode = 4 }, Constant.usp_UserPermissionMaster);
                }
            }
            catch (Exception)
            {

            }
            return View("New", obj);
        }

        [HttpGet]
        public ActionResult GetAllUser()
        {

            List<User> lst = new List<User>();
            try
            {

                lst = DBOperations<User>.GetAllOrByRange(new User() { Opmode = 3 }, Constant.usp_User);


                if (lst != null && lst.Count() > default(int))
                {

                    return Json(new { Success = 1, Message = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("No user found");
                }
            }
            catch (Exception ex)
            {
                
                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpGet]
        public ActionResult GetAllOrg()
        {

            List<Organization> lst = new List<Organization>();
            try
            {

                lst = DBOperations<Organization>.GetAllOrByRange(new Organization() { Opmode = 5 }, Constant.usp_Organization);


                if (lst != null && lst.Count() > default(int))
                {

                    return Json(new { Success = 1, Message = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("No organisation found");
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpGet]
        public ActionResult GetLocationOrg(long orgid)
        {

            List<Location> lst = new List<Location>();
            try
            {

                lst = DBOperations<Location>.GetAllOrByRange(new Location() { Opmode = 0,OrgID= orgid }, Constant.usp_Location);


                if (lst != null && lst.Count() > default(int))
                {

                    return Json(new { Success = 1, Message = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("No location found");
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult GetSubLocation(long locationid)
        {

            List<SubLocation> lst = new List<SubLocation>();
            try
            {

                lst = DBOperations<SubLocation>.GetAllOrByRange(new SubLocation() { Opmode = 0, LocationID = locationid }, Constant.usp_SubLocation);


                if (lst != null && lst.Count() > default(int))
                {

                    return Json(new { Success = 1, Message = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("No sub location found");
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }
        public ActionResult GetDepartment(long sublocid)
        {

            List<Department> lst = new List<Department>();
            try
            {

                lst = DBOperations<Department>.GetAllOrByRange(new Department() { Opmode = 0, SubLocationID = sublocid }, Constant.usp_Department);


                if (lst != null && lst.Count() > default(int))
                {

                    return Json(new { Success = 1, Message = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("No department found");
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }
        public ActionResult GetProject(long deptid)
        {

            List<Project> lst = new List<Project>();
            try
            {

                lst = DBOperations<Project>.GetAllOrByRange(new Project() { Opmode = 0, DeptID = deptid }, Constant.usp_Project);


                if (lst != null && lst.Count() > default(int))
                {

                    return Json(new { Success = 1, Message = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("No project found");
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult GetSubType(long projectid)
        {

            List<SubType> lst = new List<SubType>();
            try
            {

                lst = DBOperations<SubType>.GetAllOrByRange(new SubType() { Opmode = 0, ProjectID = projectid }, Constant.usp_SubType);


                if (lst != null && lst.Count() > default(int))
                {

                    return Json(new { Success = 1, Message = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("No subtype found");
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult SaveData(UserPermissionMaster postData)
        {
            long accessid = default(long);
            try
            {
                if (postData == null)
                    throw new Exception("No data found for insert");

                long.TryParse(Encryption64.DecryptQueryString(postData.EncryptedAccessID), out accessid);
                postData.AccessID = accessid;
                postData.Opmode = accessid > default(int) ? 5 : 0;

                UserPermissionMaster obj = new UserPermissionMaster();
                obj.UserID = postData.UserID;
                obj.OrgID = postData.OrgID;
                obj.LocationID = postData.LocationID;
                obj.SublocationID = postData.SublocationID;
                obj.DeptID = postData.DeptID;
                obj.ProjectID = postData.ProjectID;
                obj.SubTypeID = postData.SubTypeID;
                obj.AccessID = accessid;
                
                if (postData.Opmode==0) //add
                {
                    obj.Opmode = 1;
                }
                else
                {
                    obj.Opmode = 6; // edit
                }

              //  var countid = default(long);
                if(postData.Opmode==0) // add
                {
                    var objcount = new UserPermissionMaster();

                    objcount = DBOperations<UserPermissionMaster>.GetSpecific(obj, Constant.usp_UserPermissionMaster);

                    if (objcount.AccessID > default(long))
                    {
                        throw new Exception("Permission already exist");
                    }
                    else
                    {
                        var result = DBOperations<UserPermissionMaster>.DMLOperation(postData, Constant.usp_UserPermissionMaster);

                        if (result > default(int))
                            return Json(new { Success = 1, Message = string.Format("Data have been {0} successfully", accessid > default(long) ? "updated" : "saved") }, JsonRequestBehavior.AllowGet);
                        else
                            throw new Exception("Operation has been failed to execute");
                    }
                }
                else // edit
                {
                    var objcount = new UserPermissionMaster();

                    objcount = DBOperations<UserPermissionMaster>.GetSpecific(obj, Constant.usp_UserPermissionMaster);

                    if (objcount.AccessID > default(long))
                    {
                        throw new Exception("Permission already exist");
                    }
                    else
                    {
                        var result = DBOperations<UserPermissionMaster>.DMLOperation(postData, Constant.usp_UserPermissionMaster);

                        if (result > default(int))
                            return Json(new { Success = 1, Message = string.Format("Data have been {0} successfully", accessid > default(long) ? "updated" : "saved") }, JsonRequestBehavior.AllowGet);
                        else
                            throw new Exception("Operation has been failed to execute");
                    }
                }


                
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}