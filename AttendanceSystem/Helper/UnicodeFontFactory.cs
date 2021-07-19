using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.Helper
{
    public class UnicodeFontFactory : FontFactoryImp
    {
        private static readonly string fontpath = System.Web.HttpContext.Current.Server.MapPath("~/fonts/");
        private readonly BaseFont _baseFont;

        public UnicodeFontFactory()
        {
            _baseFont = BaseFont.CreateFont(fontpath + "ARIALUNI.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }

        public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color,
          bool cached)
        {
            return new Font(_baseFont, size, style, color);
        }
    }
}