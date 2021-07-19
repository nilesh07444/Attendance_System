using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using AttendanceSystem.Models;

namespace AttendanceSystem.Helper
{
    public class PDFGeneratePageEventHelper : PdfPageEventHelper
    {
        PdfContentByte cb;
        PdfContentByte cb1;
        PdfTemplate template;
        BaseFont bf = null;
        PdfTemplate template1;
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            cb = writer.DirectContent;
            cb1 = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
            template1 = cb.CreateTemplate(300, 50);
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN.ToString() + " of ";
            float len = bf.GetWidthPoint(text, 8);

            iTextSharp.text.Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(document.LeftMargin, pageSize.GetBottom(document.BottomMargin));
            cb.ShowText(text);

            cb.EndText();

            cb.AddTemplate(template, document.LeftMargin + len, pageSize.GetBottom(document.BottomMargin));

            cb.SetRGBColorFill(100, 100, 100);

            cb1.BeginText();
            cb1.SetFontAndSize(bf, 8);
            cb1.SetTextMatrix(document.LeftMargin + 310, pageSize.GetBottom(document.BottomMargin));
            //cb1.ShowText(clsAdminSession.FirmName);

            cb1.EndText();
            cb1.AddTemplate(template1, document.LeftMargin + 260, pageSize.GetBottom(document.BottomMargin));
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber));
            template.EndText();
        }
    }
}