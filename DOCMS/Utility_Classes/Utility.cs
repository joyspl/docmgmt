using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.IO;
using System.Globalization;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using SD = System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Text;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Mime;
using System.Web.Routing;

using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Drawing;
/*using System.Net.Http;
using System.Net.Http.Headers;*/
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;

public sealed class Utility
{
    #region [Singleton Implementation]
    private static readonly Utility instance = new Utility();

    static Utility()
    {
    }

    private Utility()
    {
    }

    public static Utility Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion [Singleton Implementation]

    static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string SizeSuffix(long value, int decimalPlaces = 1)
    {
        if (value == 0)
        {
            return "0";
        }

        if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
        if (value < 0) { return "-" + SizeSuffix(-value); }
        if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)Math.Log(value, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[mag]);
    }
    public static string Truncate(string value, int maxChars = 20)
    {
        if (!string.IsNullOrEmpty(value))
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        else
            return string.Empty;
    }

    public static string UrlDecodeIso8859(string strvar)
    {
        Encoding enc = Encoding.GetEncoding("iso-8859-1");
        return HttpUtility.UrlDecode(strvar, enc);
    }

    public static string GetCustomDescription(Enum en)
    {
        FieldInfo fieldInfo = en.GetType().GetField(en.ToString());
        DescriptionAttribute[] descriptionAttribute =
              (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
              typeof(DescriptionAttribute), false);
        return (descriptionAttribute.Length > 0) ? descriptionAttribute[0].Description : null;
    }

    //Get IP address of the visitor method
    public static string GetIpAddress()
    {
        string stringIpAddress;
        stringIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (stringIpAddress == null)
        {
            stringIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        return stringIpAddress;
    }

    //Get Lan Connected IP address
    public static string GetLanIPAddress()
    {
        //Get Host Name
        string stringHostName = Dns.GetHostName();
        //Get Ip Host Entry
        IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
        //Get Ip Address From The Ip Host Entry Address List
        IPAddress[] arrIpAddress = ipHostEntries.AddressList;
        return Convert.ToString(arrIpAddress[(arrIpAddress.Length - 1)]);
    }

    public static string MonthName(int m)
    {
        string res;
        switch (m)
        {
            case 1:
                res = "January";
                break;
            case 2:
                res = "February";
                break;
            case 3:
                res = "March";
                break;
            case 4:
                res = "April";
                break;
            case 5:
                res = "May";
                break;
            case 6:
                res = "June";
                break;
            case 7:
                res = "July";
                break;
            case 8:
                res = "August";
                break;
            case 9:
                res = "September";
                break;
            case 10:
                res = "October";
                break;
            case 11:
                res = "November";
                break;
            case 12:
                res = "December";
                break;
            default:
                res = "";
                break;
        }
        return res;
    }

    //Delete Specified Directory & its files
    public static clsDirectoryDeleteStatus DeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                //Delete all files from the Directory
                foreach (string file in Directory.GetFiles(path))
                {
                    System.IO.File.Delete(file);
                }
                //Delete all child Directories
                foreach (string directory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(directory);
                }
                //Delete a Directory
                Directory.Delete(path);
            }
            return new clsDirectoryDeleteStatus() { status = true, StatusMessage = "Success" };
        }
        catch (Exception excp)
        {
            return new clsDirectoryDeleteStatus() { status = false, StatusMessage = excp.Message };
        }
    }

    public static string GetIP(HttpRequestBase request)
    {
        string ip = request.Headers["X-Forwarded-For"]; // AWS compatibility
        if (string.IsNullOrEmpty(ip))
        {
            ip = request.UserHostAddress;
        }
        return ip;
    }

    public static XmlDocument CreateXml(DataTable dt)
    {
        XmlDocument xDoc = new XmlDocument();

        XmlNode nodeRoot = xDoc.CreateNode(XmlNodeType.Element, "Root", null);
        xDoc.AppendChild(nodeRoot);

        if (dt != null)
        {
            XmlNode nodeRows = xDoc.CreateNode(XmlNodeType.Element, "Rows", null);
            nodeRoot.AppendChild(nodeRows);

            foreach (DataRow row in dt.Rows)
            {
                XmlNode nodeRow = xDoc.CreateNode(XmlNodeType.Element, "Row", null);
                nodeRows.AppendChild(nodeRow);

                foreach (DataColumn col in dt.Columns)
                {
                    XmlNode node = xDoc.CreateNode(XmlNodeType.Element, col.ColumnName, null);
                    node.InnerText = Convert.ToString(row[col.ColumnName]);
                    nodeRow.AppendChild(node);
                }
            }
        }
        return xDoc;
    }

    public static DataTable ToDataTable<T>(List<T> items)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);

        //Get all the properties
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in Props)
        {
            //Setting column names as Property names
            dataTable.Columns.Add(prop.Name);
        }

        foreach (T item in items)
        {
            var values = new object[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                values[i] = Props[i].GetValue(item, null);
            }
            dataTable.Rows.Add(values);
        }
        return dataTable;
    }

    public static int IndexOf<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key)
    {
        int i = 1;
        foreach (var pair in dictionary)
        {
            if (pair.Key.Equals(key))
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    public static void GenerateCSV(string p_strPath, DataTable p_dsSrc, List<string> deleteColumnsList)
    {
        if (deleteColumnsList != null && deleteColumnsList.Count<string>() > 0)
        {
            foreach (string current in deleteColumnsList)
            {
                p_dsSrc.Columns.Remove(current);
            }
            p_dsSrc.AcceptChanges();
        }
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Join(",", from DataColumn x in p_dsSrc.Columns
                                                  select x.ColumnName));
        foreach (DataRow dataRow in p_dsSrc.Rows)
        {
            IEnumerable<string> values = from field in dataRow.ItemArray
                                         select "\"" + field.ToString().Replace("\"", "\"\"") + "\"";
            stringBuilder.AppendLine(string.Join(",", values));
        }
        File.WriteAllText(p_strPath, stringBuilder.ToString(), new UTF8Encoding());
    }

    public static void GenerateExcel2007(string p_strPath, DataTable p_dsSrc, List<string> deleteColumnsList, bool includeChart = false, int columnNumber = 0)
    {
        if (deleteColumnsList != null && deleteColumnsList.Count() > 0)
        {
            foreach (string col in deleteColumnsList)
            {
                p_dsSrc.Columns.Remove(col);
            }
            p_dsSrc.AcceptChanges();
        }

        using (ExcelPackage objExcelPackage = new ExcelPackage())
        {
            //Create the woorkbook
            ExcelWorkbook objWorkbook = objExcelPackage.Workbook;
            //Create the worksheet    
            ExcelWorksheet objWorksheet = objWorkbook.Worksheets.Add(p_dsSrc.TableName);
            //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1    
            objWorksheet.Cells["A1"].LoadFromDataTable(p_dsSrc, true);
            objWorksheet.Cells.Style.Font.SetFromFont(new Font("Calibri", 11));

            //Add autoFilter to all columns
            objWorksheet.Cells[objWorksheet.Dimension.Address].AutoFilter = true;

            //AutoFit All Columns
            objWorksheet.Cells.AutoFitColumns();

            //Format the header
            //var headerCells = objWorksheet.Cells[1, 1, 1, objWorksheet.Dimension.End.Column];
            using (ExcelRange objRange = objWorksheet.Cells[1, 1, 1, objWorksheet.Dimension.End.Column])
            {
                objRange.Style.Font.Bold = true;
                //objRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;    
                //objRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;    
                objRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                objRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFCC66"));
            }

            if (includeChart)
            {
                ExcelWorksheet objWorksheetGraph = objWorkbook.Worksheets.Add(p_dsSrc.TableName + "_Graph");
                var chart = objWorksheetGraph.Drawings.AddChart("Chart", OfficeOpenXml.Drawing.Chart.eChartType.ColumnStacked);
                //objWorksheet.Cells[1, 8, 1, 8]
                var series = chart.Series.Add(objWorksheet.Cells[1, columnNumber], objWorksheet.Cells[1, columnNumber]);

            }

            //Write it back to the client    
            if (File.Exists(p_strPath))
                File.Delete(p_strPath);

            //Create excel file on physical disk    
            FileStream objFileStrm = File.Create(p_strPath);
            objFileStrm.Close();

            //Write content to excel file
            File.WriteAllBytes(p_strPath, objExcelPackage.GetAsByteArray());
        }
    }

    public static HttpResponseMessage FileAsAttachment(string path, string filename)
    {
        string fullFilePath = path + filename;
        if (File.Exists(fullFilePath))
        {

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(fullFilePath, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = filename;
            return result;
        }
        else
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }

    public static byte[] FileAsByte(string path, string filename)
    {
        string fullFilePath = path + filename;
        if (File.Exists(fullFilePath))
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullFilePath);
            return fileBytes;
        }
        else
        {
            return null;
        }
    }

    public static byte[] FileAsByte(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return fileBytes;
        }
        else
        {
            return null;
        }
    }

    private static string GenerateRandomCode()
    {
        Random r = new Random();
        string s = "";
        for (int j = 0; j < 5; j++)
        {
            int i = r.Next(3);
            int ch;
            switch (i)
            {
                case 1:
                    ch = r.Next(0, 9);
                    s = s + ch.ToString();
                    break;
                case 2:
                    ch = r.Next(65, 90);
                    s = s + Convert.ToChar(ch).ToString();
                    break;
                case 3:
                    ch = r.Next(97, 122);
                    s = s + Convert.ToChar(ch).ToString();
                    break;
                default:
                    ch = r.Next(97, 122);
                    s = s + Convert.ToChar(ch).ToString();
                    break;
            }
            r.NextDouble();
            r.Next(100, 1999);
        }
        return s;
    }

    public static string GenerateCaptcha()
    {
        GlobalSettings.CaptchaText = Utility.GenerateRandomCode();
        CaptchaImageGenerator ci = new CaptchaImageGenerator(GlobalSettings.CaptchaText, 300, 75);

        MemoryStream oMemoryStream = new MemoryStream();
        ci.Image.Save(oMemoryStream, System.Drawing.Imaging.ImageFormat.Png);
        //byte[] oBytes = oMemoryStream.GetBuffer();
        byte[] oBytes = oMemoryStream.ToArray();
        string base64Image = "data:image/png;base64, " + System.Convert.ToBase64String(oBytes);

        ci.Dispose();
        oMemoryStream.Close();
        return base64Image;
    }

    public static byte[] Crop(string Img, int Width, int Height, int X, int Y)
    {
        try
        {
            using (SD.Image OriginalImage = SD.Image.FromFile(Img))
            {
                if (Width == 0 && Height == 0)
                {
                    if (OriginalImage.Width <= OriginalImage.Height)
                    {
                        Width = OriginalImage.Width;
                        Height = OriginalImage.Width;
                    }
                    else
                    {
                        Width = OriginalImage.Height;
                        Height = OriginalImage.Height;
                    }
                }

                using (SD.Bitmap bmp = new SD.Bitmap(Width, Height))
                {
                    bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                    using (SD.Graphics Graphic = SD.Graphics.FromImage(bmp))
                    {
                        Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                        Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        Graphic.DrawImage(OriginalImage, new SD.Rectangle(0, 0, Width, Height), X, Y, Width, Height, SD.GraphicsUnit.Pixel);
                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, OriginalImage.RawFormat);
                        return ms.GetBuffer();
                    }
                }
            }
        }
        catch (Exception Ex)
        {
            throw Ex;
        }
    }

    public static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, System.Drawing.Size size)
    {
        return (System.Drawing.Image)(new System.Drawing.Bitmap(imgToResize, size));
    }

    public static List<string> GetDataReaderColumnNames(IDataReader rdr)
    {
        var columnNames = new List<string>();
        for (int i = 0; i < rdr.FieldCount; i++)
            columnNames.Add(rdr.GetName(i));
        return columnNames;
    }

    public static object MagicallyCreateInstance(string classLibraryName, string className)
    {
        try
        {
            Type type = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Contains(classLibraryName))
                {
                    type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
                    break;
                }
            }
            return Activator.CreateInstance(type);
        }
        catch (Exception exc)
        {
            //Logger.Write(exc.Message, System.Diagnostics.TraceEventType.Error);
            throw new Exception("Cannot find the matching class of specified UserControl. Please contact with Administrator/Developer.");
        }
    }

    public static string ImageToBase64String(string imagePath)
    {
        using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath))
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save((Stream)memoryStream, image.RawFormat);
                return "data:image/jpeg;base64," + Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    public static string GenerateMenu(DataRow[] menu, DataTable table, StringBuilder sb)
    {
        sb.AppendLine("[");

        if (menu.Length > 0)
        {
            foreach (DataRow dr in menu)
            {
                string handler = string.IsNullOrEmpty(Convert.ToString(dr["MenuURL"])) ? "javascript:void(0);" : "#/" + Convert.ToString(dr["MenuURL"]);
                string menuText = Convert.ToString(dr["MenuName"]);
                string mID = Convert.ToString(dr["MenuId"]);
                DataRow[] subMenu = table.Select(String.Format("ParentMenuId = '{0}'", dr["MenuId"].ToString()));
                string arrow = (subMenu.Length > 0) ? @"<span class=""fa arrow""></span>" : "";
                string icon = dr["MenuIcon"].ToString();
                var subMenuBuilder = new StringBuilder();
                //string line = String.Format(@"{ display: '{0}', href: '{1}', children: {2}", menuText, handler, GetSubmenu(dr["MenuId"].ToString(), dr["ParentMenuId"].ToString(), subMenu, table).ToString());
                //string line = "{ display: '" + menuText + "', href: '" + handler + "', children: " + GetSubmenu(dr["MenuId"].ToString(), dr["ParentMenuId"].ToString(), subMenu, table).ToString();
                string line = "{ \"display\": \"" + menuText + "\", \"href\": \"" + handler + "\", \"children\": " + GenerateMenu(subMenu, table, subMenuBuilder);
                sb.Append(line);

                /*string pid = dr["MenuId"].ToString();
                string parentId = dr["ParentMenuId"].ToString();

                if (subMenu.Length > 0 && !pid.Equals(parentId))
                {
                    var subMenuBuilder = new StringBuilder();
                    sb.Append(GenerateMenu(subMenu, table, subMenuBuilder));
                }*/

                sb.Append(" }, ");
            }
        }
        sb.Append("]");
        return sb.ToString();
    }

    private static StringBuilder GetSubmenu(string pid, string parentId, DataRow[] subMenu, DataTable table)
    {
        StringBuilder sbSUB = new StringBuilder();
        if (subMenu.Length > 0 && !pid.Equals(parentId))
        {
            sbSUB.Append(GenerateMenu(subMenu, table, sbSUB));
        }
        else
        {
            sbSUB.Append("[]");
        }
        return sbSUB;
    }

    public static string PopulateBody(string Name, string title, string description, System.Web.HttpContextWrapper ctx = null)
    {
        string body = string.Empty;

        string fullFilePath = string.Empty;
        if (HttpContext.Current != null)
            fullFilePath = HttpContext.Current.Server.MapPath("~/EmailTemplates/CommonTemplate.html");
        else
            fullFilePath = ctx.Server.MapPath("~/EmailTemplates/CommonTemplate.html");

        using (StreamReader reader = new StreamReader(fullFilePath))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{Name}", Name);
        body = body.Replace("{Title}", title);
        body = body.Replace("{Description}", description);
        return body;
    }

    public static string PopulateDocketReplyBody(string Name, string DocketNumber, string CompanyName, string DocketStatus, string ProblemDescription, string CustomerName, string CustomerPhone, string CreatedOn, string UpdatedOn, string StatusRemarks, string UpdatedBy, string title = "Reason For Outage", string ServiceType = "&nbsp;", string CaseType = "Complaint", string OutageReason = "&nbsp;")
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates/DocketReply.html")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{Name}", Name);
        body = body.Replace("{Title}", title);
        body = body.Replace("{DocketNumber}", DocketNumber);
        body = body.Replace("{CompanyName}", CompanyName);
        body = body.Replace("{DocketStatus}", DocketStatus);
        body = body.Replace("{ProblemDescription}", ProblemDescription);
        body = body.Replace("{CustomerName}", CustomerName);
        body = body.Replace("{CustomerPhone}", CustomerPhone);
        body = body.Replace("{CreatedOn}", CreatedOn);
        body = body.Replace("{UpdatedOn}", UpdatedOn);
        body = body.Replace("{StatusRemarks}", StatusRemarks);
        body = body.Replace("{UpdatedBy}", UpdatedBy);
        body = body.Replace("{ServiceType}", ServiceType);
        body = body.Replace("{CaseType}", CaseType);
        body = body.Replace("{OutageReason}", OutageReason);
        return body;
    }

    public static string PopulatePage(string message)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates/thankyou.html")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{Message}", message);
        return body;
    }

    public static Page PageResults(long total, int ordinal = -1, int pageSize = 10, int PageNo = 1)
    {
        long start = 1, end = total;
        var maxPage = total % pageSize == 0 ? total / pageSize : total / pageSize + 1;
        var page = -1;

        if (ordinal > 0)
        {
            var previousPage = (ordinal % pageSize == 0) ? (ordinal / pageSize) - 1 : ordinal / pageSize;
            page = previousPage + 1;

            start = previousPage * pageSize + 1;
            end = (start + pageSize - 1) <= total ? (start + pageSize - 1) : total;
        }
        else
        {
            //page = int.Parse(GetHeaderValue("page"));
            page = PageNo;

            if (page <= maxPage)
            {
                start = ((page - 1) * pageSize) + 1;
                end = page * pageSize <= total ? page * pageSize : total;
            }
            else
            {
                start = 0;
                end = 0;
            }
        }

        if (start > 0 && end > 0 && page > 0)
        {
            //if (HttpRequest != null)
            //{
            //    ResponseContext.SetProperties(Request.Properties, WebHeaders.ContentRange, string.Format("{0}-{1}/{2}", start, end, total));
            //    ResponseContext.SetProperties(Request.Properties, WebHeaders.PreviousPage, (start == 1 ? 0 : page - 1).ToString());
            //    ResponseContext.SetProperties(Request.Properties, WebHeaders.NextPage, (end < total ? page + 1 : 0).ToString());
            //}
        }

        return new Page() { Start = start, End = end };
    }

    private static string GetHeaderValue(string headerKey)
    {
        return HttpContext.Current.Request.Headers.AllKeys.Contains(headerKey) ?
            HttpContext.Current.Request.Headers[headerKey] : string.Empty;
    }

    private static bool ContainsHeader(string headerKey)
    {
        return HttpContext.Current.Request.Headers.AllKeys.Contains(headerKey);
    }

    public static void SendHtmlFormattedEmail(string recepientEmail, string subject, string body, bool hasAttachment = false, string FilePath = "", System.Web.HttpContextWrapper ctx = null)
    {
        string fullFilePath = string.Empty;
        if (HttpContext.Current != null)
            fullFilePath = HttpContext.Current.Server.MapPath("~/img/logo.png");
        else
            fullFilePath = ctx.Server.MapPath("~/img/logo.png");

        using (MailMessage mailMessage = new MailMessage())
        {
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
            mailMessage.Subject = subject;

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, MediaTypeNames.Text.Html);
            LinkedResource logo = new LinkedResource(fullFilePath);
            logo.ContentId = "companylogo";
            htmlView.LinkedResources.Add(logo);
            mailMessage.AlternateViews.Add(htmlView);
            mailMessage.Body = body;

            //Attachment checking & creation
            if (hasAttachment && !string.IsNullOrEmpty(FilePath))
            {
                Attachment objAttachment = new Attachment(FilePath);
                mailMessage.Attachments.Add(objAttachment);
            }

            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            //mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, new ContentType("text/plain")));
            mailMessage.To.Add(new MailAddress(recepientEmail));
            SmtpClient smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["Host"];
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;//later added
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];
            NetworkCred.Password = ConfigurationManager.AppSettings["Password"];
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);

            //TLS Addition
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            //Certificate hack if “The remote certificate is invalid according to the validation procedure.” using 587 port in SMTP server
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            smtp.Send(mailMessage);
            /*Object state = mailMessage;
            smtp.SendAsync(mailMessage, state);*/
        }
    }

    public static string PopulateSMSBody(string DocketNumber, string DocketStatus, string CreatedOn)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/SMSTemplates/DocketReply.txt")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{DocketNumber}", DocketNumber);
        body = body.Replace("{DocketStatus}", DocketStatus);
        body = body.Replace("{CreatedOn}", CreatedOn);
        return body;
    }

    public static string PopulatePlainSMSBody(string Name, string Description)
    {
        string str = string.Empty;
        using (StreamReader streamReader = new StreamReader(HttpContext.Current.Server.MapPath("~/SMSTemplates/WelcomeVerificaion.txt")))
            str = streamReader.ReadToEnd();
        return str.Replace("{Name}", Name).Replace("{Description}", Description);
    }

    public static string SendSMS(string sendToPhoneNumber, string messageBody)
    {
        string result = "";
        WebRequest request = null;
        HttpWebResponse response = null;
        string encodedSMSBody = EscapeUriDataStringRfc3986(messageBody);

        try
        {
            //1 = sendToPhoneNumber, 3 = MessageBody, 5 = userid, 7 = password, (0, 2, 4, 6, 8, 9, 10 = &)
            string url = string.Format(ConfigurationManager.AppSettings["SMSGatewayUri"].ToString(), "&", sendToPhoneNumber, "&", encodedSMSBody, "&", ConfigurationManager.AppSettings["SMSGatewayUserID"].ToString(), "&", ConfigurationManager.AppSettings["SMSGatewayPassword"].ToString(), "&", "&", "&");
            request = WebRequest.Create(url);

            //in case u work behind proxy, uncomment the commented code and provide correct details
            /*WebProxy proxy = new WebProxy("http://proxy:80/",true);
            proxy.Credentials = new 
            NetworkCredential("userId","password", "Domain");
            request.Proxy = proxy;*/

            // Send the 'HttpWebRequest' and wait for response.
            response = (HttpWebResponse)request.GetResponse();

            Stream stream = response.GetResponseStream();
            Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader reader = new System.IO.StreamReader(stream, ec);
            result = reader.ReadToEnd();
            reader.Close();
            stream.Close();

            return result;
        }
        catch (Exception exp)
        {
            //return exp.Message;
            throw new Exception(exp.Message);
        }
        finally
        {
            if (response != null)
                response.Close();
        }
    }

    private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };
    internal static string EscapeUriDataStringRfc3986(string value)
    {
        StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));
        for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
        {
            escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
        }
        return escaped.ToString();
    }

    public static List<string> InitializeFontAwesomeIcons()
    {
        return new List<string>()
                {
                    "adjust", "adn", "align-center", "align-justify", "align-left", "align-right", "ambulance",
                    "anchor", "android", "angle-double-down", "angle-double-left", "angle-double-right",
                    "angle-double-up", "angle-down", "angle-left", "angle-right", "angle-up", "apple", "archive",
                    "arrow-circle-down", "arrow-circle-left", "arrow-circle-o-down", "arrow-circle-o-left", "arrow-circle-o-right",
                    "arrow-circle-o-up", "arrow-circle-right", "arrow-circle-up", "arrow-down", "arrow-left", "arrow-right",
                    "arrow-up", "arrows", "arrows-alt", "arrows-h", "arrows-v", "asterisk", "automobile", "backward","ban", "bank",
                    "bar-chart-o", "barcode", "bars", "beer", "behance", "behance-square", "bell", "bell-o", "bitbucket",
                    "bitbucket-square", "bitcoin", "bold", "bolt", "bomb", "book", "bookmark", "bookmark-o", "briefcase", "btc",
                    "bug", "building", "building-o", "bullhorn", "bullseye", "cab", "calendar", "calendar-o", "camera",
                    "camera-retro", "car", "caret-down", "caret-left", "caret-right", "caret-square-o-down", "caret-square-o-left",
                    "caret-square-o-right", "caret-square-o-up", "caret-up", "certificate", "chain", "chain-broken", "check",
                    "check-circle", "check-circle-o", "check-square", "check-square-o", "chevron-circle-down",
                    "chevron-circle-left", "chevron-circle-right", "chevron-circle-up", "chevron-down", "chevron-left",
                    "chevron-right", "chevron-up", "child", "circle", "circle-o", "circle-o-notch", "circle-thin", "clipboard",
                    "clock-o", "cloud", "cloud-download", "cloud-upload", "cny", "code", "code-fork", "codepen", "coffee", "cog",
                    "cogs", "columns", "comment", "comment-o", "comments", "comments-o", "compass", "compress", "copy",
                    "credit-card", "crop", "crosshairs", "css3", "cube", "cubes", "cut", "cutlery", "dashboard", "database",
                    "dedent", "delicious", "desktop", "deviantart", "digg", "dollar", "dot-circle-o", "download", "dribbble",
                    "dropbox", "drupal", "edit", "eject", "ellipsis-h", "ellipsis-v", "empire", "envelope", "envelope-o",
                    "envelope-square", "eraser", "eur", "euro", "exchange", "exclamation", "exclamation-circle",
                    "exclamation-triangle", "expand", "external-link", "external-link-square", "eye", "eye-slash", "facebook",
                    "facebook-square", "fast-backward", "fast-forward", "fax", "female", "fighter-jet", "file", "file-archive-o",
                    "file-audio-o", "file-code-o", "file-excel-o", "file-image-o", "file-movie-o", "file-o", "file-pdf-o",
                    "file-photo-o", "file-picture-o", "file-powerpoint-o", "file-sound-o", "file-text", "file-text-o",
                    "file-video-o", "file-word-o", "file-zip-o", "files-o", "film", "filter", "fire", "fire-extinguisher", "flag",
                    "flag-checkered", "flag-o", "flash", "flask", "flickr", "floppy-o", "folder", "folder-o", "folder-open",
                    "folder-open-o", "font", "forward", "foursquare", "frown-o", "gamepad", "gavel", "gbp", "ge", "gear", "gears",
                    "gift", "git", "git-square", "github", "github-alt", "github-square", "gittip", "glass", "globe", "google",
                    "google-plus", "google-plus-square", "graduation-cap", "group", "h-square", "hacker-news", "hand-o-down",
                    "hand-o-left", "hand-o-right", "hand-o-up", "hdd-o", "header", "headphones", "heart", "heart-o", "history",
                    "home", "hospital-o", "html5", "image", "inbox", "indent", "info", "info-circle", "inr", "instagram",
                    "institution", "italic", "joomla", "jpy", "jsfiddle", "key", "keyboard-o", "krw", "language", "laptop", "leaf",
                    "legal", "lemon-o", "level-down", "level-up", "life-bouy", "life-ring", "life-saver", "lightbulb-o", "link",
                    "linkedin", "linkedin-square", "linux", "list", "list-alt", "list-ol", "list-ul", "location-arrow", "lock",
                    "long-arrow-down", "long-arrow-left", "long-arrow-right", "long-arrow-up", "magic", "magnet", "mail-forward",
                    "mail-reply", "mail-reply-all", "male", "map-marker", "maxcdn", "medkit", "meh-o", "microphone",
                    "microphone-slash", "minus", "minus-circle", "minus-square", "minus-square-o", "mobile", "mobile-phone",
                    "money", "moon-o", "mortar-board", "music", "navicon", "openid", "outdent", "pagelines", "paper-plane",
                    "paper-plane-o", "paperclip", "paragraph", "paste", "pause", "paw", "pencil", "pencil-square", "pencil-square-o",
                    "phone", "phone-square", "photo", "picture-o", "pied-piper", "pied-piper-alt", /*"pied-piper-square", */"pinterest",
                    "pinterest-square", "plane", "play", "play-circle", "play-circle-o", "plus", "plus-circle", "plus-square",
                    "plus-square-o", "power-off", "print", "puzzle-piece", "qq", "qrcode", "question", "question-circle",
                    "quote-left", "quote-right", "ra", "random", "rebel", "recycle", "reddit", "reddit-square", "refresh", "renren",
                    "reorder", "repeat", "reply", "reply-all", "retweet", "rmb", "road", "rocket", "rotate-left", "rotate-right",
                    "rouble", "rss", "rss-square", "rub", "ruble", "rupee", "save", "scissors", "search", "search-minus",
                    "search-plus", "send", "send-o", "share", "share-alt", "share-alt-square", "share-square", "share-square-o",
                    "shield", "shopping-cart", "sign-in", "sign-out", "signal", "sitemap", "skype", "slack", "sliders", "smile-o",
                    "sort", "sort-alpha-asc", "sort-alpha-desc", "sort-amount-asc", "sort-amount-desc", "sort-asc", "sort-desc",
                    "sort-down", "sort-numeric-asc", "sort-numeric-desc", "sort-up", "soundcloud", "space-shuttle", "spinner",
                    "spoon", "spotify", "square", "square-o", "stack-exchange", "stack-overflow", "star", "star-half",
                    "star-half-empty", "star-half-full", "star-half-o", "star-o", "steam", "steam-square", "step-backward",
                    "step-forward", "stethoscope", "stop", "strikethrough", "stumbleupon", "stumbleupon-circle", "subscript",
                    "suitcase", "sun-o", "superscript", "support", "table", "tablet", "tachometer", "tag", "tags", "tasks", "taxi",
                    "tencent-weibo", "terminal", "text-height", "text-width", "th", "th-large", "th-list", "thumb-tack",
                    "thumbs-down", "thumbs-o-down", "thumbs-o-up", "thumbs-up", "ticket", "times", "times-circle", "times-circle-o",
                    "tint", "toggle-down", "toggle-left", "toggle-right", "toggle-up", "trash-o", "tree", "trello", "trophy",
                    "truck", "try", "tumblr", "tumblr-square", "turkish-lira", "twitter", "twitter-square", "umbrella", "underline",
                    "undo", "university", "unlink", "unlock", "unlock-alt", "unsorted", "upload", "usd", "user", "user-md", "users",
                    "video-camera", "vimeo-square", "vine", "vk", "volume-down", "volume-off", "volume-up", "warning", "wechat",
                    "weibo", "weixin", "wheelchair", "windows", "won", "wordpress", "wrench", "xing", "xing-square", "yahoo",
                    "yen", "youtube", "youtube-play", "youtube-square"
                };
    }

    private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        #region Big freaking list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion
    };

    public static string GetMimeType(string extension)
    {
        if (extension == null)
        {
            throw new ArgumentNullException("extension");
        }
        if (!extension.StartsWith("."))
        {
            extension = "." + extension;
        }
        string mime;
        return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
    }
    public class clsEnum
    {
        public enum Month { January = 1, February = 2, March = 3, April = 4, May = 5, June = 6, July = 7, August = 8, September = 9, October = 10, November = 11, December = 12 }

        public enum AlertType { Default = 0, Info = 1, Primary = 2, Success = 3, Warning = 4, Danger = 5 }
    }
}

public class clsDirectoryDeleteStatus
{
    public bool status { get; set; }
    public string StatusMessage { get; set; }
}

public class Page
{
    public long Start;
    public long End;
}

public static class ContentGenerator
{
    private static ConcurrentDictionary<string, string> _cacheForStaticContent = new ConcurrentDictionary<string, string>();

    //adds a version to static contents : js or css
    public static string DatedContent(this System.Web.Mvc.UrlHelper urlHelper, string contentPath)
    {
        string tag = "";
        var datedPath = new StringBuilder(contentPath);
        if (!_cacheForStaticContent.ContainsKey(contentPath))
        {
            tag = getModifiedDate(contentPath);
            _cacheForStaticContent.GetOrAdd(contentPath, tag);
        }
        else
            _cacheForStaticContent.TryGetValue(contentPath, out tag);
        datedPath.AppendFormat("{0}m={1}", contentPath.IndexOf('?') >= 0 ? '&' : '?', tag);
        return urlHelper.Content(datedPath.ToString());
    }

    private static string getModifiedDate(string contentPath)
    {
        return System.IO.File.GetLastWriteTime(System.Web.Hosting.HostingEnvironment.MapPath(contentPath)).ToString("yyyyMMddhhmmss");
    }
}