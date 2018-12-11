using DataLayer;
using System;
using System.Web.Mvc;

namespace DOCMS.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();       
            return View();
        }

        public JsonResult ValidateLogin(string username, string password)
        {
            try
            {
                var result = DBOperations<User>.GetSpecific(new User() { UserEmail = username.Trim().ToLower(), Opmode = 2 }, Constant.usp_User);
                if (result != null && !string.IsNullOrWhiteSpace(result.UserPassword))
                {
                    if (result.UserPassword == SecurityController.Encrypt(password))
                    {
                        GlobalSettings.oUserData = result;
                        try
                        {
                            var permissions = DBOperations<UserPermission>.GetAllOrByRange(new UserPermission() { UserID = result.ID }, Constant.usp_UserPermission);
                            GlobalSettings.oUserPermission = permissions;
                            if (result.RoleID != (long)Role.BuiltinAdmin && (permissions == null || permissions.Count <= default(int)))
                                throw new Exception("Permission not found for any module");
                        }
                        catch (Exception cex)
                        {
                            return Json(new { Success = default(int), Message = cex.Message }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { Success = 1, Message = "Login successful" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        throw new Exception("Invalid credential");
                }
                else
                    throw new Exception("User does not exists in our system");
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}