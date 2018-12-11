using DataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DOCMS.Controllers
{
    [SessionAuthorize]
    public class DocumentController : Controller
    {
        public ActionResult GetTotalDocCount(int firstload = 1, int pageNo = 1, string dtfrm = "", string dtto = "", long finyear = 0, long orgid = 0, long deptid = 0, long prjid = 0, long subtypeid = 0, string docno = "", string info1 = "", string info2 = "", string info3 = "", int appstt = 2)
        {
            List<Document> lst = new List<Document>();
            string sortExpression = string.Empty;
            string conditionExpression = string.Empty;
            string FinCondition = string.Empty;
            sortExpression = " d.CreatedOn DESC ";
            DateTime dtFrm = DateTime.Now;
            DateTime dtTo = DateTime.Now;
            long TotalRecords = default(long);
            string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy" };
            try
            {
                IEnumerable<long> orgLst = GlobalSettings.oUserPermission.Select(d => d.OrgID);
                IEnumerable<long> deptLst = GlobalSettings.oUserPermission.Select(d => d.DeptID);
                IEnumerable<long> locationLst = GlobalSettings.oUserPermission.Select(d => d.LocationID);
                IEnumerable<long> sublocationLst = GlobalSettings.oUserPermission.Select(d => d.SublocationID);
                IEnumerable<long> projectLst = GlobalSettings.oUserPermission.Select(d => d.ProjectID);
                IEnumerable<long> subtypeLst = GlobalSettings.oUserPermission.Select(d => d.SubTypeID);

                if (!string.IsNullOrWhiteSpace(dtfrm))
                {
                    DateTime.TryParseExact(dtfrm, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtFrm);
                }
                if (!string.IsNullOrWhiteSpace(dtto))
                {
                    DateTime.TryParseExact(dtto, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtTo);
                }

                if (!string.IsNullOrWhiteSpace(dtfrm) && !string.IsNullOrWhiteSpace(dtto))
                {
                    conditionExpression += string.Format(" AND d.DocDate BETWEEN '{0}' AND '{1}' ", dtFrm.ToString("yyyy-MM-dd"), dtTo.ToString("yyyy-MM-dd"));
                }

                if (finyear > default(long))
                {
                    conditionExpression += string.Format(" AND d.FinYearID = {0} ", finyear);
                }

                if (deptid > default(long))
                {
                    conditionExpression += string.Format(" AND d.DeptID = {0} ", deptid);
                }
                else if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    conditionExpression += string.Format(" AND d.DeptID IN ({0}) ", string.Join(",", deptLst));
                }

                if (prjid > default(long))
                {
                    conditionExpression += string.Format(" AND d.ProjectID = {0} ", prjid);
                }
                else if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    conditionExpression += string.Format(" AND d.ProjectID IN ({0}) ", string.Join(",", projectLst));
                }

                if (subtypeid > default(long))
                {
                    conditionExpression += string.Format(" AND d.SubTypeID = {0} ", subtypeid);
                }
                else if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    conditionExpression += string.Format(" AND d.SubTypeID IN ({0}) ", string.Join(",", subtypeLst));
                }

                if (!string.IsNullOrWhiteSpace(docno))
                {
                    conditionExpression += string.Format(@" AND d.DocNumber LIKE '%{0}%' ESCAPE '\' ", docno.Trim().ToUpper().Replace("[", @"\[").Replace("]", @"\]"));
                }

                if (!string.IsNullOrWhiteSpace(info1))
                {
                    conditionExpression += string.Format(@"AND a.AttachInfo1 LIKE '%{0}%' ESCAPE '\' ", info1.Trim().ToUpper().Replace("[", @"\[").Replace("]", @"\]"));
                }

                if (!string.IsNullOrWhiteSpace(info2))
                {
                    conditionExpression += string.Format(@"AND a.AttachInfo2 LIKE '%{0}%' ESCAPE '\' ", info2.Trim().ToUpper().Replace("[", @"\[").Replace("]", @"\]"));
                }

                if (!string.IsNullOrWhiteSpace(info3))
                {
                    conditionExpression += string.Format(@"AND a.AttachInfo3 LIKE '%{0}%' ESCAPE '\' ", info3.Trim().ToUpper().Replace("[", @"\[").Replace("]", @"\]"));
                }

                /*if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    conditionExpression = string.Format(@" AND d.IsUploadApproved = 1 {0}", conditionExpression);
                }
                else if (GlobalSettings.oUserData.RoleID == (long)Role.BuiltinAdmin)
                {
                    switch(appstt)
                    {
                        case 1:
                            conditionExpression = string.Format(@" AND d.IsUploadApproved = 1 {0}", conditionExpression);
                            break;
                        case 2:
                            conditionExpression = string.Format(@" AND d.IsUploadApproved = 0 {0}", conditionExpression);
                            break;
                        default:
                            break;
                    }
                }*/
                switch (appstt)
                {
                    case 1:
                        conditionExpression = string.Format(@" AND d.IsUploadApproved = 1 {0}", conditionExpression);
                        break;
                    case 2:
                        conditionExpression = string.Format(@" AND d.IsUploadApproved = 0 {0}", conditionExpression);
                        break;
                    default:
                        break;
                }

                var tlst = DBOperations<Document>.GetAllOrByRange(new Document()
                {
                    strSortFields = sortExpression,
                    strCondition = conditionExpression,
                    Opmode = 4
                }, Constant.usp_DocumentManager);
                if (tlst != null && tlst.Count() > default(int) && tlst[0].TotalRecords > default(long))
                {
                    TotalRecords = tlst[0].TotalRecords;
                }
            }
            catch (Exception) { }

            return Json(new { Success = 1, Message = TotalRecords }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int firstload = 1, int pageNo = 1, string dtfrm = "", string dtto = "", long finyear = 0, long orgid = 0, long deptid = 0, long prjid = 0, long subtypeid = 0, string docno = "", string info1 = "", string info2 = "", string info3 = "", int appstt = 2)
        {
            List<Document> lst = new List<Document>();
            string sortExpression = string.Empty;
            string conditionExpression = string.Empty;
            string FinCondition = string.Empty;
            sortExpression = " d.CreatedOn DESC ";
            DateTime dtFrm = DateTime.Now;
            DateTime dtTo = DateTime.Now;
            long TotalRecords = default(long);
            string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy" };
            try
            {
                IEnumerable<long> orgLst = GlobalSettings.oUserPermission.Select(d => d.OrgID);
                IEnumerable<long> deptLst = GlobalSettings.oUserPermission.Select(d => d.DeptID);
                IEnumerable<long> locationLst = GlobalSettings.oUserPermission.Select(d => d.LocationID);
                IEnumerable<long> sublocationLst = GlobalSettings.oUserPermission.Select(d => d.SublocationID);
                IEnumerable<long> projectLst = GlobalSettings.oUserPermission.Select(d => d.ProjectID);
                IEnumerable<long> subtypeLst = GlobalSettings.oUserPermission.Select(d => d.SubTypeID);

                if (!string.IsNullOrWhiteSpace(dtfrm))
                {
                    DateTime.TryParseExact(dtfrm, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtFrm);
                }
                if (!string.IsNullOrWhiteSpace(dtto))
                {
                    DateTime.TryParseExact(dtto, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtTo);
                }

                if (!string.IsNullOrWhiteSpace(dtfrm) && !string.IsNullOrWhiteSpace(dtto))
                {
                    conditionExpression += string.Format(" AND d.DocDate BETWEEN '{0}' AND '{1}' ", dtFrm.ToString("yyyy-MM-dd"), dtTo.ToString("yyyy-MM-dd"));
                }

                if (finyear > default(long))
                {
                    conditionExpression += string.Format(" AND d.FinYearID = {0} ", finyear);
                }

                if (deptid > default(long))
                {
                    conditionExpression += string.Format(" AND d.DeptID = {0} ", deptid);
                }
                else if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    conditionExpression += string.Format(" AND d.DeptID IN ({0}) ", string.Join(",", deptLst));
                }

                if (prjid > default(long))
                {
                    conditionExpression += string.Format(" AND d.ProjectID = {0} ", prjid);
                }
                else if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    conditionExpression += string.Format(" AND d.ProjectID IN ({0}) ", string.Join(",", projectLst));
                }

                if (subtypeid > default(long))
                {
                    conditionExpression += string.Format(" AND d.SubTypeID = {0} ", subtypeid);
                }
                else if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    conditionExpression += string.Format(" AND d.SubTypeID IN ({0}) ", string.Join(",", subtypeLst));
                }

                if (!string.IsNullOrWhiteSpace(docno))
                {
                    conditionExpression += string.Format(@" AND d.DocNumber LIKE '%{0}%' ESCAPE '\' ", docno.Trim().ToUpper().Replace("[", @"\[").Replace("]", @"\]"));
                }

                if (!string.IsNullOrWhiteSpace(info1))
                {
                    conditionExpression += string.Format(@"AND a.AttachInfo1 LIKE '%{0}%' ESCAPE '\' ", info1.Trim().ToUpper().Replace("[", @"\[").Replace("]", @"\]"));
                }

                if (!string.IsNullOrWhiteSpace(info2))
                {
                    conditionExpression += string.Format(@"AND a.AttachInfo2 LIKE '%{0}%' ESCAPE '\' ", info2.Trim().ToUpper().Replace("[", @"\[").Replace("]", @"\]"));
                }

                if (!string.IsNullOrWhiteSpace(info3))
                {
                    conditionExpression += string.Format(@"AND a.AttachInfo3 LIKE '%{0}%' ESCAPE '\' ", info3.Trim().ToUpper().Replace("[", @"\[").Replace("]", @"\]"));
                }

                /*if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    conditionExpression = string.Format(@" AND d.IsUploadApproved = 1 {0}", conditionExpression);
                }
                else if (GlobalSettings.oUserData.RoleID == (long)Role.BuiltinAdmin)
                {
                    switch(appstt)
                    {
                        case 1:
                            conditionExpression = string.Format(@" AND d.IsUploadApproved = 1 {0}", conditionExpression);
                            break;
                        case 2:
                            conditionExpression = string.Format(@" AND d.IsUploadApproved = 0 {0}", conditionExpression);
                            break;
                        default:
                            break;
                    }
                }*/
                switch (appstt)
                {
                    case 1:
                        conditionExpression = string.Format(@" AND d.IsUploadApproved = 1 {0}", conditionExpression);
                        break;
                    case 2:
                        conditionExpression = string.Format(@" AND d.IsUploadApproved = 0 {0}", conditionExpression);
                        break;
                    default:
                        break;
                }

                var tlst = DBOperations<Document>.GetAllOrByRange(new Document()
                {
                    strSortFields = sortExpression,
                    strCondition = conditionExpression,
                    Opmode = 4
                }, Constant.usp_DocumentManager);
                if (tlst != null && tlst.Count() > default(int) && tlst[0].TotalRecords > default(long))
                {
                    TotalRecords = tlst[0].TotalRecords;
                }
                ViewBag.TotalRecords = TotalRecords;
                ViewBag.pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["RecordsPerPage"]);
                var page = Utility.PageResults(Convert.ToInt64(TotalRecords), -1, Convert.ToInt32(ConfigurationManager.AppSettings["RecordsPerPage"]), pageNo);
                if (page.Start > 0 && page.End > 0)
                {
                    lst = DBOperations<Document>.GetAllOrByRange(new Document()
                    {
                        strSortFields = sortExpression,
                        strCondition = conditionExpression,
                        Opmode = 4,
                        RowStartIndex = page.Start,
                        RowEndIndex = page.End
                    }, Constant.usp_DocumentManager);
                }
            }
            catch (Exception) { }
            if (firstload > default(int))
                return View(lst);
            else
                return PartialView("_List", lst);
        }

        [HttpGet]
        public JsonResult SetApproval(string key, int key1)
        {
            long docid = default(long);
            try
            {
                long.TryParse(Encryption64.DecryptQueryString(key), out docid);
                var result = DBOperations<Document>.DMLOperation(new Document() { Opmode = 6, IsUploadApproved = key1, DocID = docid }, Constant.usp_DocumentManager);
                if (result > default(int))
                    return Json(new { Success = 1, Message = "Approval status has been updated for the document" }, JsonRequestBehavior.AllowGet);
                else
                    throw new Exception("Unable to update approval status");
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult New(string id = "")
        {
            long docid = default(long);
            Document obj = new Document();
            var lstDepartments = new List<SelectLists>();
            try
            {
                IEnumerable<long> deptLst = GlobalSettings.oUserPermission.Select(d => d.DeptID);

                if (!string.IsNullOrEmpty(id))
                    long.TryParse(Encryption64.DecryptQueryString(id), out docid);

                if (docid > default(long))
                {
                    obj = DBOperations<Document>.GetSpecific(new Document() { DocID = docid, Opmode = 1 }, Constant.usp_DocumentManager);
                }

                //ViewBag.Projects = new DropdownClient().Getdropdown(Dropdown.Project);
                //ViewBag.Departments = new DropdownClient().Getdropdown(Dropdown.Department);
                //ViewBag.SubTypes = new DropdownClient().Getdropdown(Dropdown.SubType);
                //ViewBag.FinYears = new DropdownClient().Getdropdown(Dropdown.FinYear);

                //lstDepartments.Add(new SelectLists() { Value = 0, Text = "Select Department" });
                lstDepartments.AddRange(new DropdownClient().Getdropdown(Dropdown.Department));
                ViewBag.Departments = (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin ? lstDepartments.Where(w => deptLst.Contains(w.Value) || w.Value == 0).ToList() : lstDepartments);

                ViewBag.FinYears = new DropdownClient().Getdropdown(Dropdown.FinYear);
            }
            catch (Exception) { }
            return View("New", obj);
        }

        public JsonResult LoadDropdownOnDemand(string id, int param)
        {
            List<SelectLists> lst = new List<SelectLists>();
            List<SelectLists> lstfin = new List<SelectLists>();
            lst.Add(new SelectLists() { Value = 0, Text = string.Format("Select {0}", Utility.GetCustomDescription((Dropdown)param)) });
            try
            {
                lst.AddRange(DBOperations<SelectLists>.GetAllOrByRange(new SelectLists() { Opmode = param, Valum = id }, Constant.usp_Dropdown));

                if (GlobalSettings.oUserData.RoleID != (long)Role.BuiltinAdmin)
                {
                    IEnumerable<long> orgLst = GlobalSettings.oUserPermission.Select(d => d.OrgID);
                    IEnumerable<long> deptLst = GlobalSettings.oUserPermission.Select(d => d.DeptID);
                    IEnumerable<long> locationLst = GlobalSettings.oUserPermission.Select(d => d.LocationID);
                    IEnumerable<long> sublocationLst = GlobalSettings.oUserPermission.Select(d => d.SublocationID);
                    IEnumerable<long> projectLst = GlobalSettings.oUserPermission.Select(d => d.ProjectID);
                    IEnumerable<long> subtypeLst = GlobalSettings.oUserPermission.Select(d => d.SubTypeID);

                    switch ((Dropdown)param)
                    {
                        case Dropdown.Organization:
                            lstfin = lst.Where(w => orgLst.Contains(w.Value) || w.Value == 0).ToList();
                            break;
                        case Dropdown.Department:
                            lstfin = lst.Where(w => deptLst.Contains(w.Value) || w.Value == 0).ToList();
                            break;
                        case Dropdown.Location:
                            lstfin = lst.Where(w => locationLst.Contains(w.Value) || w.Value == 0).ToList();
                            break;
                        case Dropdown.SubLocation:
                            lstfin = lst.Where(w => sublocationLst.Contains(w.Value) || w.Value == 0).ToList();
                            break;
                        case Dropdown.Project:
                            lstfin = lst.Where(w => projectLst.Contains(w.Value) || w.Value == 0).ToList();
                            break;
                        case Dropdown.SubType:
                            lstfin = lst.Where(w => subtypeLst.Contains(w.Value) || w.Value == 0).ToList();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    lstfin = lst;
                }
            }
            catch (Exception) { }
            return Json(lstfin, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadAttachmentList(string docid)
        {
            long documentId = default(long);
            List<AttachmentFile> lst = new List<AttachmentFile>();
            try
            {
                long.TryParse(Encryption64.DecryptQueryString(docid), out documentId);
                lst = DBOperations<AttachmentFile>.GetAllOrByRange(new AttachmentFile() { DocID = documentId, Opmode = default(int) }, Constant.usp_Attachment);
            }
            catch (Exception) { }
            return PartialView("_AttachmentList", lst);
        }

        public ActionResult AttachmentDelete(string id, string did)
        {
            long attachid = default(long);
            long docid = default(long);
            int delresult = default(int);
            try
            {
                long.TryParse(Encryption64.DecryptQueryString(id), out attachid);
                long.TryParse(Encryption64.DecryptQueryString(did), out docid);
                var obj = DBOperations<AttachmentFile>.GetSpecific(new AttachmentFile() { AttachmentID = attachid, Opmode = 1 }, Constant.usp_Attachment);
                if (System.IO.File.Exists(string.Format(@"{0}\{1}\{2}{3}", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docid, obj.FileName, obj.FileExtenssion)))
                {
                    System.IO.File.Delete(string.Format(@"{0}\{1}\{2}{3}", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docid, obj.FileName, obj.FileExtenssion));
                    delresult = DBOperations<AttachmentFile>.DMLOperation(new AttachmentFile() { DocID = docid, AttachmentID = attachid, Opmode = 5 }, Constant.usp_Attachment, DMLOperationFlag.Delete);
                }
                if (delresult > default(int))
                    return Json(new { Success = 1, Message = string.Format("Attachement file ({0}{1}) has been deleted successfully", obj.FileName, obj.FileExtenssion) }, JsonRequestBehavior.AllowGet);
                else
                    throw new Exception(string.Format("Unable to delete file ({0}{1})", obj.FileName, obj.FileExtenssion));
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DocumentDelete(string did)
        {
            long docid = default(long);
            int docdelresult = default(int);
            int delresult = default(int);
            List<AttachmentFile> lst = new List<AttachmentFile>();
            StringBuilder sb = new StringBuilder();
            try
            {
                long.TryParse(Encryption64.DecryptQueryString(did), out docid);
                lst = DBOperations<AttachmentFile>.GetAllOrByRange(new AttachmentFile() { DocID = docid, Opmode = default(int) }, Constant.usp_Attachment);
                if (lst != null && lst.Count() > default(int))
                {
                    foreach (var attachmentobj in lst)
                    {
                        try
                        {
                            if (System.IO.File.Exists(string.Format(@"{0}\{1}\{2}{3}", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docid, attachmentobj.FileName, attachmentobj.FileExtenssion)))
                            {
                                System.IO.File.Delete(string.Format(@"{0}\{1}\{2}{3}", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docid, attachmentobj.FileName, attachmentobj.FileExtenssion));
                                delresult = DBOperations<AttachmentFile>.DMLOperation(new AttachmentFile() { DocID = docid, AttachmentID = attachmentobj.AttachmentID, Opmode = 5 }, Constant.usp_Attachment, DMLOperationFlag.Delete);
                            }
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine(ex.Message);
                            continue;
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(sb.ToString()))
                {
                    docdelresult = DBOperations<Document>.DMLOperation(new Document() { DocID = docid, Opmode = 5 }, Constant.usp_DocumentManager, DMLOperationFlag.Delete);
                    if (docdelresult > default(int))
                        return Json(new { Success = 1, Message = "Document has been deleted successfully" }, JsonRequestBehavior.AllowGet);
                    else
                        throw new Exception("Unable to delete document right now. Please try again after some time");
                }
                else
                    throw new Exception(sb.ToString());
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveAttachmentInfo(AttachmentFile postData)
        {
            try
            {
                postData.Opmode = 3;
                var result = DBOperations<AttachmentFile>.DMLOperation(postData, Constant.usp_Attachment, DMLOperationFlag.Update);
                if (result > default(int))
                    return Json(new { Success = 1, Message = "Data saved successfully" });
                else
                    throw new Exception("Unable to update data");
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveDoc(Document postData)
        {
            long docid = default(long);
            string[] formats = { "dd.MM.yyyy", "dd/MM/yyyy", "dd/MM/yy" };
            DateTime docdate = DateTime.Now;
            try
            {
                long.TryParse(Encryption64.DecryptQueryString(postData.DocInfo10), out docid);
                postData.DocID = docid;
                DateTime.TryParseExact(postData.DocInfo9, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out docdate);
                postData.DocDate = docdate;
                postData.Opmode = 3;
                postData.ModifiedBy = GlobalSettings.oUserData.ID;
                var result = DBOperations<Document>.DMLOperation(postData, Constant.usp_DocumentManager, DMLOperationFlag.Update);
                if (result > default(int))
                    return Json(new { Success = 1, Message = "Data saved successfully" });
                else
                    throw new Exception("Unable to update data");
            }
            catch (Exception ex)
            {
                return Json(new { Success = default(int), Message = ex.Message });
            }
        }

        public ActionResult AttachmentPreview(string id, string iseditargument)
        {
            long attachid = default(long);
            int iseditrequest = default(int);
            AttachmentFile obj = new AttachmentFile();
            try
            {
                long.TryParse(Encryption64.DecryptQueryString(id), out attachid);
                obj = DBOperations<AttachmentFile>.GetSpecific(new AttachmentFile() { AttachmentID = attachid, Opmode = 1 }, Constant.usp_Attachment);
                int.TryParse(Encryption64.DecryptQueryString(iseditargument), out iseditrequest);
                Tuple<AttachmentFile, int> tpl = new Tuple<AttachmentFile, int>(obj, iseditrequest);
                return View("AttachmentViewer", tpl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public FileResult DowloadUnsupportedFile(string id)
        {
            long attachid = default(long);
            var obj = new AttachmentFile();
            try
            {
                long.TryParse(Encryption64.DecryptQueryString(id), out attachid);
                obj = DBOperations<AttachmentFile>.GetSpecific(new AttachmentFile() { AttachmentID = attachid, Opmode = 1 }, Constant.usp_Attachment);
                //var stream = new FileStream(Server.MapPath(obj.FilePath), FileMode.Open);
                var stream = new FileStream(obj.FilePath, FileMode.Open);
                return File(stream, MediaTypeNames.Application.Octet, string.Format("{0}{1}", obj.FileName, obj.FileExtenssion));
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveUploadedFile(long deptid, long prjid, long subtype, long finyr, string docno, string id)
        {
            long docIdentity = default(long);
            DateTime dt = DateTime.Now;
            var doc = new Document();
            string[] viewerSupportedFiles = ConfigurationManager.AppSettings["ViewerJSSupportedExtenssions"].Split(',');
            try
            {
                if (GlobalSettings.oUserData != null && GlobalSettings.oUserData.ID > default(long))
                {
                    long.TryParse(Encryption64.DecryptQueryString(id), out docIdentity);

                    if (docIdentity <= default(long))
                    {
                        doc.DeptID = deptid;
                        doc.ProjectID = prjid;
                        doc.SubTypeID = subtype;
                        doc.FinYearID = finyr;
                        doc.DocNumber = !string.IsNullOrWhiteSpace(docno.Trim()) ? docno.Trim().ToUpper() : "DOC-" + dt.ToString("yyyyMMdd") + "_" + dt.ToString("hhmmss");
                        doc.CreatedBy = GlobalSettings.oUserData.ID;
                        doc.TaggingType = (int)TaggingType.ScanOnly;
                        doc.DocDate = dt;
                        doc.Opmode = 2;
                        var docidstr = DBOperations<Document>.InsertWithOutputVariable(doc, Constant.usp_DocumentManager);
                        long.TryParse(docidstr, out docIdentity);
                    }

                    foreach (string fileName in Request.Files)
                    {
                        try
                        {
                            HttpPostedFileBase file = Request.Files[fileName];
                            var contentLength = file.ContentLength;
                            if (file != null && contentLength > 0)
                            {
                                byte[] imgAsBytes = new byte[contentLength];
                                using (BinaryReader theReader = new BinaryReader(file.InputStream))
                                {
                                    imgAsBytes = theReader.ReadBytes(contentLength);
                                }
                                //if (!Directory.Exists(Server.MapPath(string.Format("~/Uploads/{0}/", docIdentity))))
                                //{
                                //    Directory.CreateDirectory(Server.MapPath(string.Format("~/Uploads/{0}/", docIdentity)));
                                //}
                                //if (System.IO.File.Exists(Server.MapPath(string.Format("~/Uploads/{0}/{1}", docIdentity, file.FileName))))
                                //{
                                //    System.IO.File.Delete(Server.MapPath(string.Format("~/Uploads/{0}/{1}", docIdentity, file.FileName)));
                                //}
                                //System.IO.File.WriteAllBytes(Server.MapPath(string.Format("~/Uploads/{0}/{1}", docIdentity, file.FileName)), imgAsBytes);
                                if (!Directory.Exists(string.Format(@"{0}\{1}\", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docIdentity)))
                                {
                                    Directory.CreateDirectory(string.Format(@"{0}\{1}\", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docIdentity));
                                }
                                if (System.IO.File.Exists(string.Format(@"{0}\{1}\{2}", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docIdentity, file.FileName)))
                                {
                                    System.IO.File.Delete(string.Format(@"{0}\{1}\{2}", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docIdentity, file.FileName));
                                }
                                System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docIdentity, file.FileName), imgAsBytes);

                                AttachmentFile a = new AttachmentFile();
                                a.DocID = docIdentity;
                                a.FileSizeBytes = contentLength.ToString();
                                a.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                                a.FileExtenssion = System.IO.Path.GetExtension(file.FileName);
                                //a.FilePath = string.Format("~/Uploads/{0}/{1}", docIdentity, file.FileName); // Set Virtual Path in DB
                                a.FilePath = string.Format(@"{0}\{1}\{2}", ConfigurationManager.AppSettings["DocumentDomainRootDirectory"], docIdentity, file.FileName); // Set Virtual Path in DB
                                a.IsViewerSupported = viewerSupportedFiles.Contains(System.IO.Path.GetExtension(file.FileName)) ? 1 : default(int);
                                a.CreatedOn = dt;
                                a.CreatedBy = GlobalSettings.oUserData.ID;
                                a.Opmode = 2;
                                var result = DBOperations<AttachmentFile>.DMLOperation(a, Constant.usp_Attachment);
                            }
                        }
                        catch (Exception ex)
                        {
                            //continue;
                            throw ex;
                        }
                    }
                    return Json(new { success = true, docid = Encryption64.EncryptQueryString(docIdentity.ToString()) });
                }
                else
                {
                    throw new Exception("Session timed-out. Unable to upload. Please login again");
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, docid = Encryption64.EncryptQueryString(default(long).ToString()) });
            }
        }

        [HttpGet]
        public ActionResult GetAttachmentInfoMap(long subtypeid)
        {
            AttachInfoMap objam = new AttachInfoMap();
            string Info1 = ConfigurationManager.AppSettings["Info1DefaultText"];
            string Info2 = ConfigurationManager.AppSettings["Info2DefaultText"];
            string Info3 = ConfigurationManager.AppSettings["Info3DefaultText"];
            try
            {
                objam = DBOperations<AttachInfoMap>.GetSpecific(new AttachInfoMap() { SubTypeID = subtypeid, Opmode = default(int) }, Constant.usp_AttachmentNameMap);
                if (objam != null && objam.ID > default(long))
                {
                    Info1 = objam.AttachInfo1Name;
                    Info2 = objam.AttachInfo2Name;
                    Info3 = objam.AttachInfo3Name;
                }
            }
            catch { }
            return Json(new { Info1 = string.Format("{0} (Optional)", Info1), Info2 = string.Format("{0} (Optional)", Info2), Info3 = string.Format("{0} (Optional)", Info3), }, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public bool IsSupportedFile(HttpPostedFileBase file)
        {
            //Checks for image type... you could also do filename extension checks and other things
            return ((file != null) &&
                (
                System.Text.RegularExpressions.Regex.IsMatch(file.ContentType, "image/\\S+") ||
                file.ContentType == "application/pdf"
                ) && (file.ContentLength > 0));
        }
    }
}