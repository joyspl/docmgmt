using System;
using System.Text;
using System.Web;

namespace DataLayer
{
    public static class ExtensionUtil
    {
        public static string UrlDecodeIso8859(this string strvar)
        {
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            return HttpUtility.UrlDecode(strvar, enc);
        }
    }
}
