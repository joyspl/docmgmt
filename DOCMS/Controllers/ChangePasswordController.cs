using DataLayer;
using System;
using System.Web.Mvc;

namespace DOCMS.Controllers
{
    public class ChangePasswordController : Controller
    {
        public ActionResult Index() // Load view page
        {
            return View();
        }



        public JsonResult UpdatePassword(string password)  // Call web api to update password and return result
        {
            try
            {
                string encPassword = SecurityController.Encrypt(password); // encrypt password (code written in Encrypt function of inside SecurityController class. This controller is written inside utility_class folder )

                long userId = GlobalSettings.oUserData.ID; // get user session id (code written in GlobalSetting page inside inside utility_class folder)

                var result = DBOperations<ChangePassword>.GetSpecific(new ChangePassword() { UserPassword = encPassword, ID = userId }, Constant.usp_Password);

// Call a store procedure ‘ChangePassword’  and pass parameters

                if (result != null)
                {
                   
                       
                        return Json(new { Success = 1, Message = "Password updated successfully" }, JsonRequestBehavior.AllowGet);
                  
                    
                }
                else
                    throw new Exception("Something went wrong.Please try again");
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
