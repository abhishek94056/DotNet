using iTextSharp.text;
using iTextSharp.text.pdf;
using InvoiceGenerator.Models;

namespace InvoiceGenerator.Services
{
    public class PdfService
    {
        private readonly IWebHostEnvironment _env;

        public PdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GenerateInvoicePdf(
            InvoiceMaster master,
            List<InvoiceItem> items,
            CompanyDetails invoiceToData,
            CompanyDetails shippingToData)
        {
            using var ms = new MemoryStream();

            Document doc = new Document(PageSize.A4, 20, 20, 20, 20);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            // ── Fonts (WinForms प्रमाणेच) ──
            iTextSharp.text.Font companyFont = FontFactory.GetFont("Garamond", 35, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font headerFont = FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.ITALIC);
            iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.COURIER, 16, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE);
            iTextSharp.text.Font subtitleFont = FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font boldFont = FontFactory.GetFont("Verdana", 10, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font normalFont = FontFactory.GetFont("Verdana", 10);
            iTextSharp.text.Font declarationFont = FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.UNDERLINE);

            // ── ORIGINAL FOR BUYER ──
            Paragraph originalText = new Paragraph("ORIGINAL FOR BUYER", headerFont);
            originalText.Alignment = Element.ALIGN_RIGHT;
            doc.Add(originalText);
            doc.Add(new Paragraph("\n"));

            // ══════════════════════════════════
            // HEADER TABLE: Logo | CompanyName | TAX INVOICE
            // ══════════════════════════════════
            PdfPTable headerTable = new PdfPTable(2);
            headerTable.WidthPercentage = 100;
            headerTable.SetWidths(new float[] { 0.15f, 0.85f });

            // Logo
            string logoPath = Path.Combine(_env.WebRootPath, "images", "Logo.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleToFit(70f, 80f);
            PdfPCell logoCell = new PdfPCell(logo);
            logoCell.HorizontalAlignment = Element.ALIGN_CENTER;
            logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            logoCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
            logoCell.BorderWidth = 0.7f;
            logoCell.Padding = 0;
            headerTable.AddCell(logoCell);

            // Company Name image
            string companyNamePath = Path.Combine(_env.WebRootPath, "images", "Company Name.png");
            iTextSharp.text.Image companyNameImg = iTextSharp.text.Image.GetInstance(companyNamePath);
            companyNameImg.ScaleToFit(1000f, 55f);
            PdfPCell companyCell = new PdfPCell(companyNameImg);
            companyCell.HorizontalAlignment = Element.ALIGN_CENTER;
            companyCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            companyCell.Border = iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER;
            companyCell.BorderWidth = 0.7f;
            companyCell.Padding = 0;
            headerTable.AddCell(companyCell);

            // TAX INVOICE (colspan 2)
            PdfPCell titleCell = new PdfPCell(new Phrase("TAX INVOICE", titleFont));
            titleCell.Colspan = 2;
            titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
            titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            titleCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                             | iTextSharp.text.Rectangle.RIGHT_BORDER
                             | iTextSharp.text.Rectangle.TOP_BORDER
                             | iTextSharp.text.Rectangle.BOTTOM_BORDER;
            titleCell.BorderWidth = 0.7f;
            titleCell.PaddingTop = 0;
            titleCell.PaddingBottom = 8;
            headerTable.AddCell(titleCell);

            doc.Add(headerTable);

            // ══════════════════════════════════
            // MAIN TABLE
            // ══════════════════════════════════
            PdfPTable mainTable = new PdfPTable(2);
            mainTable.WidthPercentage = 100;
            mainTable.SetWidths(new float[] { 1f, 1f });
            mainTable.SpacingBefore = 0f;
            mainTable.SpacingAfter = 0f;

            // ── Left Cell 1: Company Info ──
            PdfPCell leftCell = new PdfPCell();
            leftCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                            | iTextSharp.text.Rectangle.TOP_BORDER
                            | iTextSharp.text.Rectangle.RIGHT_BORDER;
            leftCell.BorderWidth = 0.7f;
            leftCell.PaddingTop = -4f;

            Paragraph companyPara = new Paragraph("M/S CodNow Technologies", subtitleFont);
            companyPara.Alignment = Element.ALIGN_CENTER;
            companyPara.SpacingAfter = -4f;
            leftCell.AddElement(companyPara);

            // Address line 1
            Phrase ap1 = new Phrase();
            ap1.Add(new Chunk("Address : ", boldFont));
            ap1.Add(new Chunk("Plot No.32, Gut No.83, Renuka Nagar, Opp. Vinayak Park, Deolai", normalFont));
            Paragraph addrPara1 = new Paragraph(ap1); addrPara1.SpacingAfter = -4f;
            leftCell.AddElement(addrPara1);

            Phrase ap2 = new Phrase();
            ap2.Add(new Chunk("Road, Chhatrapati, Sambhajinagar - 431 001", normalFont));
            Paragraph addrPara2 = new Paragraph(ap2); addrPara2.SpacingAfter = -4f;
            leftCell.AddElement(addrPara2);

            Phrase pp = new Phrase();
            pp.Add(new Chunk("Phone : ", boldFont));
            pp.Add(new Chunk("+91 9923 8888 91 / +91 7972 3815 80", normalFont));
            Paragraph phonePara = new Paragraph(pp); phonePara.SpacingAfter = -4f;
            leftCell.AddElement(phonePara);

            Phrase ep = new Phrase();
            ep.Add(new Chunk("E-mail : ", boldFont));
            ep.Add(new Chunk("info@codnowtechnologies.com", normalFont));
            Paragraph emailPara = new Paragraph(ep); emailPara.SpacingAfter = -4f;
            leftCell.AddElement(emailPara);

            Phrase wp = new Phrase();
            wp.Add(new Chunk("Website : ", boldFont));
            wp.Add(new Chunk("www.codnowtechnologies.com", normalFont));
            Paragraph webPara = new Paragraph(wp); webPara.SpacingAfter = -4f;
            leftCell.AddElement(webPara);

            Phrase sp = new Phrase();
            sp.Add(new Chunk("State Code : ", boldFont));
            sp.Add(new Chunk("27 ,  ", normalFont));
            sp.Add(new Chunk("State : ", boldFont));
            sp.Add(new Chunk("Maharashtra ", normalFont));
            Paragraph statePara = new Paragraph(sp); statePara.SpacingAfter = -4f;
            leftCell.AddElement(statePara);

            Phrase gp = new Phrase();
            gp.Add(new Chunk("GSTIN : ", boldFont));
            gp.Add(new Chunk("27FAEPS7663Q1ZK ,  ", boldFont));
            gp.Add(new Chunk("PAN : ", boldFont));
            gp.Add(new Chunk("FAEPS7663Q", boldFont));
            leftCell.AddElement(new Paragraph(gp));

            mainTable.AddCell(leftCell);

            // ── Right Cell 1: Invoice Details ──
            PdfPCell rightCell = new PdfPCell();
            rightCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                             | iTextSharp.text.Rectangle.RIGHT_BORDER;
            rightCell.BorderWidth = 0.7f;
            rightCell.Padding = 0;

            PdfPTable rightInnerTable = new PdfPTable(2);
            rightInnerTable.WidthPercentage = 100;
            rightInnerTable.SetWidths(new float[] { 1.2f, 1f });
            rightInnerTable.HorizontalAlignment = Element.ALIGN_CENTER;

            void AddRow(string label, string value, iTextSharp.text.Font valFont)
            {
                PdfPCell lc = new PdfPCell(new Phrase(label, boldFont));
                lc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                lc.HorizontalAlignment = Element.ALIGN_RIGHT;
                lc.Padding = 2;
                rightInnerTable.AddCell(lc);

                PdfPCell vc = new PdfPCell(new Phrase(value, valFont));
                vc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                vc.HorizontalAlignment = Element.ALIGN_LEFT;
                vc.Padding = 2;
                rightInnerTable.AddCell(vc);
            }

            AddRow("Invoice No : ", "FY2526/CNT/" + master.InvoiceNo, boldFont);
            AddRow("Invoice Date : ", master.InvoiceDate.ToString("dd.MM.yyyy"), normalFont);
            AddRow("Date of Supply : ", master.DateOfSupply.ToString("dd.MM.yyyy"), normalFont);
            AddRow("Purchase Order No : ", master.PurchaseOrderNo, boldFont);
            AddRow("Purchase Order Dt : ", master.PurchaseOrderDt.ToString("dd-MMM-yy"), normalFont);
            //AddRow("DC No : ", master.DCNo, boldFont);
            //AddRow("DC Date : ", master.DCDate.ToString("dd.MM.yyyy"), normalFont);
            AddRow("Vehicle No : ", master.VehicleNo, normalFont);
            AddRow("ASN No : ", master.ASNNo, boldFont);

            rightCell.AddElement(rightInnerTable);
            mainTable.AddCell(rightCell);

            // ── Invoice To / Shipping To Headers ──
            PdfPCell leftCell2 = new PdfPCell();
            leftCell2.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                             | iTextSharp.text.Rectangle.RIGHT_BORDER
                             | iTextSharp.text.Rectangle.TOP_BORDER;
            leftCell2.BorderWidth = 0.7f;
            leftCell2.PaddingBottom = 4; leftCell2.PaddingTop = -7;
            Paragraph invPara = new Paragraph("Invoice To", subtitleFont);
            invPara.Alignment = Element.ALIGN_CENTER;
            leftCell2.AddElement(invPara);
            mainTable.AddCell(leftCell2);

            PdfPCell rightCell2 = new PdfPCell();
            rightCell2.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                              | iTextSharp.text.Rectangle.RIGHT_BORDER
                              | iTextSharp.text.Rectangle.TOP_BORDER;
            rightCell2.BorderWidth = 0.7f;
            rightCell2.PaddingBottom = 4; rightCell2.PaddingTop = -7;
            Paragraph shipPara = new Paragraph("Shipping To", subtitleFont);
            shipPara.Alignment = Element.ALIGN_CENTER;
            rightCell2.AddElement(shipPara);
            mainTable.AddCell(rightCell2);

            // ── Invoice To Details ──
            PdfPCell leftCell3 = new PdfPCell();
            leftCell3.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                             | iTextSharp.text.Rectangle.RIGHT_BORDER
                             | iTextSharp.text.Rectangle.TOP_BORDER;
            leftCell3.BorderWidth = 0.7f; leftCell3.PaddingTop = -4f;

            Paragraph inv2Para = new Paragraph(invoiceToData.CompanyName, boldFont);
            inv2Para.Alignment = Element.ALIGN_CENTER; inv2Para.SpacingAfter = -4f;
            leftCell3.AddElement(inv2Para);

            Phrase ap3 = new Phrase();
            ap3.Add(new Chunk("Address : ", boldFont));
            ap3.Add(new Chunk(invoiceToData.Address, normalFont));
            Paragraph ap3p = new Paragraph(ap3); ap3p.SpacingAfter = 6f;
            leftCell3.AddElement(ap3p);

            Phrase sc3 = new Phrase();
            sc3.Add(new Chunk("State Code : ", boldFont));
            sc3.Add(new Chunk(invoiceToData.StateCode + "    ", normalFont));
            sc3.Add(new Chunk("State : ", boldFont));
            sc3.Add(new Chunk(invoiceToData.State, normalFont));
            Paragraph sc3p = new Paragraph(sc3); sc3p.SpacingAfter = -4f;
            leftCell3.AddElement(sc3p);

            Phrase gt3 = new Phrase();
            gt3.Add(new Chunk("GSTIN : ", boldFont));
            gt3.Add(new Chunk(invoiceToData.GSTIN, boldFont));
            Paragraph gt3p = new Paragraph(gt3); gt3p.SpacingAfter = -4f;
            leftCell3.AddElement(gt3p);

            Phrase pt3 = new Phrase();
            pt3.Add(new Chunk("Payment Term : ", boldFont));
            pt3.Add(new Chunk(invoiceToData.PaymentTerm, boldFont));
            leftCell3.AddElement(new Paragraph(pt3));
            mainTable.AddCell(leftCell3);

            // ── Shipping To Details ──
            PdfPCell rightCell3 = new PdfPCell();
            rightCell3.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                              | iTextSharp.text.Rectangle.RIGHT_BORDER
                              | iTextSharp.text.Rectangle.TOP_BORDER;
            rightCell3.BorderWidth = 0.7f; rightCell3.PaddingTop = -4f;

            Paragraph ship2Para = new Paragraph(shippingToData.CompanyName, boldFont);
            ship2Para.Alignment = Element.ALIGN_CENTER; ship2Para.SpacingAfter = -4f;
            rightCell3.AddElement(ship2Para);

            Phrase ap4 = new Phrase();
            ap4.Add(new Chunk("Address : ", boldFont));
            ap4.Add(new Chunk(shippingToData.Address, normalFont));
            Paragraph ap4p = new Paragraph(ap4); ap4p.SpacingAfter = 6f;
            rightCell3.AddElement(ap4p);

            Phrase sc4 = new Phrase();
            sc4.Add(new Chunk("State Code : ", boldFont));
            sc4.Add(new Chunk(shippingToData.StateCode + "    ", normalFont));
            sc4.Add(new Chunk("State : ", boldFont));
            sc4.Add(new Chunk(shippingToData.State, normalFont));
            Paragraph sc4p = new Paragraph(sc4); sc4p.SpacingAfter = -4f;
            rightCell3.AddElement(sc4p);

            Phrase gt4 = new Phrase();
            gt4.Add(new Chunk("GSTIN : ", boldFont));
            gt4.Add(new Chunk(shippingToData.GSTIN, boldFont));
            rightCell3.AddElement(new Paragraph(gt4));
            mainTable.AddCell(rightCell3);

            doc.Add(mainTable);

            // ══════════════════════════════════
            // ITEMS TABLE
            // ══════════════════════════════════
            PdfPTable table = new PdfPTable(6);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SetWidths(new float[] { 5f, 25f, 15f, 15f, 10f, 15f });

            PdfPCell MakeHeaderCell(string text)
            {
                PdfPCell c = new PdfPCell(new Phrase(text, boldFont));
                c.HorizontalAlignment = Element.ALIGN_CENTER;
                c.VerticalAlignment = Element.ALIGN_MIDDLE;
                c.Border = iTextSharp.text.Rectangle.BOX;
                c.BorderWidth = 0.7f;
                c.Padding = 4;
                return c;
            }

            PdfPCell MakeDataCell(string text, iTextSharp.text.Font font, int align = Element.ALIGN_CENTER)
            {
                PdfPCell c = new PdfPCell(new Phrase(text, font));
                c.HorizontalAlignment = align;
                c.VerticalAlignment = Element.ALIGN_MIDDLE;
                c.Border = iTextSharp.text.Rectangle.BOX;
                c.BorderWidth = 0.7f;
                c.Padding = 4;
                return c;
            }

            table.AddCell(MakeHeaderCell("Sr No"));
            table.AddCell(MakeHeaderCell("Item Description"));
            table.AddCell(MakeHeaderCell("HSN Code"));
            table.AddCell(MakeHeaderCell("Rate"));
            table.AddCell(MakeHeaderCell("Qty/Nos"));
            table.AddCell(MakeHeaderCell("Amount"));

            decimal taxableValue = 0;
            foreach (var item in items)
            {
                table.AddCell(MakeDataCell(item.SrNo.ToString(), normalFont));
                table.AddCell(MakeDataCell(item.ItemDescription, normalFont));
                table.AddCell(MakeDataCell(item.HSNCode, normalFont));
                table.AddCell(MakeDataCell(item.Rate.ToString("0.00"), normalFont));
                table.AddCell(MakeDataCell(item.Qty.ToString(), normalFont));
                table.AddCell(MakeDataCell(item.Amount.ToString("0.00"), normalFont));
                taxableValue += item.Amount;
            }
            doc.Add(table);

            // ══════════════════════════════════
            // BANK DETAILS + TOTALS TABLE
            // ══════════════════════════════════
            PdfPTable finalTable = new PdfPTable(4);
            finalTable.WidthPercentage = 100;
            finalTable.SetWidths(new float[] { 50f, 20f, 15f, 15f });

            PdfPCell bankCell = new PdfPCell();
            bankCell.Padding = 2;
            bankCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
            bankCell.BorderWidth = 0.7f;

            PdfPTable bankInnerTable = new PdfPTable(2);
            bankInnerTable.WidthPercentage = 100;
            bankInnerTable.SetWidths(new float[] { 1f, 1f });

            void AddBankRow(string label, string value)
            {
                PdfPCell lc = new PdfPCell(new Phrase(label, normalFont));
                lc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                lc.HorizontalAlignment = Element.ALIGN_RIGHT; lc.Padding = 2;
                bankInnerTable.AddCell(lc);

                PdfPCell vc = new PdfPCell(new Phrase(value, normalFont));
                vc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                vc.HorizontalAlignment = Element.ALIGN_LEFT; vc.Padding = 2;
                bankInnerTable.AddCell(vc);
            }

            bankCell.AddElement(new Paragraph("NEFT / RTGS BANK DETAILS:", subtitleFont));
            AddBankRow("BANK NAME  :  ", "IDBI BANK");
            AddBankRow("A/C NAME  :  ", "CODNOW TECHNOLOGIES");
            AddBankRow("A/C NUMBER  :  ", "0076102000048736");
            AddBankRow("IFSC CODE  :  ", "IBKL0000076");
            bankCell.AddElement(bankInnerTable);
            finalTable.AddCell(bankCell);

            PdfPCell spaceCell = new PdfPCell();
            spaceCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            finalTable.AddCell(spaceCell);

            // Labels column
            PdfPTable labelInnerTable = new PdfPTable(1);
            labelInnerTable.WidthPercentage = 100;
            foreach (string lbl in new[] { "Taxable Value", "CGST @ 9%", "SGST @ 9%", "IGST @ 18%", "Total Value" })
            {
                PdfPCell lc = new PdfPCell(new Phrase(lbl, boldFont));
                lc.HorizontalAlignment = Element.ALIGN_RIGHT;
                lc.Border = iTextSharp.text.Rectangle.BOX;
                lc.BorderWidth = 0.7f; lc.Padding = 3;
                labelInnerTable.AddCell(lc);
            }
            PdfPCell labelCell = new PdfPCell(labelInnerTable);
            labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            labelCell.Padding = 0;
            finalTable.AddCell(labelCell);

            // Values column
            PdfPTable valueInnerTable = new PdfPTable(1);
            valueInnerTable.WidthPercentage = 100;
            foreach (string val in new[] {
                taxableValue.ToString("0.00"),
                master.CGST.ToString("0.00"),
                master.SGST.ToString("0.00"),
                master.IGST.ToString("0.00"),
                master.TotalValue.ToString("0.00") })
            {
                PdfPCell vc = new PdfPCell(new Phrase(val, boldFont));
                vc.HorizontalAlignment = Element.ALIGN_CENTER;
                vc.Border = iTextSharp.text.Rectangle.BOX;
                vc.BorderWidth = 0.7f; vc.Padding = 3;
                valueInnerTable.AddCell(vc);
            }
            PdfPCell valueCell = new PdfPCell(valueInnerTable);
            valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            valueCell.Padding = 0;
            finalTable.AddCell(valueCell);

            doc.Add(finalTable);

            // ══════════════════════════════════
            // DECLARATION
            // ══════════════════════════════════
            PdfPTable declarationTable = new PdfPTable(1);
            declarationTable.WidthPercentage = 100;

            PdfPCell declarationCell = new PdfPCell();
            declarationCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                   | iTextSharp.text.Rectangle.RIGHT_BORDER
                                   | iTextSharp.text.Rectangle.BOTTOM_BORDER;
            declarationCell.BorderWidth = 0.7f;
            declarationCell.PaddingTop = 0;

            declarationCell.AddElement(new Paragraph("E. & O.E.", normalFont) { Alignment = Element.ALIGN_RIGHT });
            declarationCell.AddElement(new Paragraph("In Words : " + GstHelper.ConvertToWords(master.TotalValue), boldFont));
            declarationCell.AddElement(new Paragraph("Transport Mode : " + master.TransportMode, boldFont));
            declarationCell.AddElement(new Paragraph("Transporter Name : CodNow Technologies", boldFont));
            declarationCell.AddElement(new Paragraph("Remark : " + master.Remark, boldFont));
            declarationCell.AddElement(new Paragraph("Declaration", declarationFont));
            declarationCell.AddElement(new Paragraph(
                "I/We here certify that my/our Reg.certi under the GST ACT 2017 is in force & " +
                "dated on which the sale of goods specified in this tax invoice is made by me/us " +
                "and transaction of sale covered by this tax invoice has been effected us and it " +
                "shall be accounted for in the turnover of sale while filling of return and the due " +
                "tax, if any payable on the sale has been paid or shall be paid.", normalFont));

            declarationTable.AddCell(declarationCell);
            doc.Add(declarationTable);

            // ══════════════════════════════════
            // SIGNATURE
            // ══════════════════════════════════
            PdfPTable signatureTable = new PdfPTable(2);
            signatureTable.WidthPercentage = 100;
            signatureTable.SetWidths(new float[] { 1f, 1f });

            PdfPCell sealCell2 = new PdfPCell();
            sealCell2.Border = iTextSharp.text.Rectangle.BOX;
            sealCell2.BorderWidth = 0.7f;
            sealCell2.PaddingTop = -4;
            sealCell2.FixedHeight = 66f;
            sealCell2.AddElement(new Paragraph("Customer's Seal & Signature", normalFont));
            signatureTable.AddCell(sealCell2);

            PdfPCell authCell2 = new PdfPCell();
            authCell2.Border = iTextSharp.text.Rectangle.BOX;
            authCell2.BorderWidth = 0.7f;
            authCell2.PaddingTop = -4;
            authCell2.FixedHeight = 90f;

            Paragraph forPara = new Paragraph("For , CodNow Technologies", boldFont);
            forPara.Alignment = Element.ALIGN_RIGHT;
            authCell2.AddElement(forPara);
            authCell2.AddElement(new Paragraph(" "));

            string signPath = Path.Combine(_env.WebRootPath, "images", "Signature.png");
            iTextSharp.text.Image signImg = iTextSharp.text.Image.GetInstance(signPath);
            signImg.ScaleToFit(120f, 40f);
            signImg.Alignment = Element.ALIGN_MIDDLE;
            authCell2.AddElement(signImg);

            Paragraph authPara = new Paragraph("Authorised Signatory", normalFont);
            authPara.Alignment = Element.ALIGN_RIGHT;
            authCell2.AddElement(authPara);

            signatureTable.AddCell(authCell2);
            doc.Add(signatureTable);

            doc.Close();
            return ms.ToArray();
        }

       
    }
}