using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InvoiceGenerator
{
    public partial class Form1 : Form
    {
        readonly String connectionstring = ConfigurationManager.ConnectionStrings["InvoiceGenerator"].ConnectionString;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGeneratePDF_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF files (*.pdf)|*.pdf";
            sfd.FileName = "Invoice_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Document doc = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                doc.Open();

                // ===================== FONTS =====================
                iTextSharp.text.Font companyFont = FontFactory.GetFont("Garamond", 35, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.ITALIC);
                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.COURIER, 16, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE);    
                iTextSharp.text.Font subtitleFont = FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.BOLD);  
                iTextSharp.text.Font boldFont = FontFactory.GetFont("Verdana", 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = FontFactory.GetFont("Verdana", 10);      
                iTextSharp.text.Font declarationFont = FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.UNDERLINE);

                // ===================== "ORIGINAL FOR BUYER" =====================
                Paragraph originalText = new Paragraph("ORIGINAL FOR BUYER", headerFont);//verdana 10 courier new 
                originalText.Alignment = Element.ALIGN_RIGHT;
                doc.Add(originalText);
                doc.Add(new Paragraph("\n"));


                // ===================== HEADER TABLE =====================
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 0.15f, 0.85f });

                // --- LOGO CELL ---
                string logoPath = @"D:\Dot Net\WinForms\InvoiceGenerator\Logo.png";

                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleToFit(70f, 80f);

                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.HorizontalAlignment = Element.ALIGN_CENTER;
                logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                logoCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                     | iTextSharp.text.Rectangle.TOP_BORDER;
                logoCell.BorderWidth = 0.7f;
                logoCell.Padding = 0;
                headerTable.AddCell(logoCell);

                // --- COMPANY NAME IMAGE CELL ---
                string companyNamePath = @"D:\Dot Net\WinForms\InvoiceGenerator\Company Name.png"; // ← your PNG path
                iTextSharp.text.Image companyNameImg = iTextSharp.text.Image.GetInstance(companyNamePath);
                companyNameImg.ScaleToFit(1000f, 55f);  // ← adjust width/height to fit nicely

                PdfPCell companyCell = new PdfPCell(companyNameImg);
                companyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                companyCell.VerticalAlignment = Element.ALIGN_MIDDLE;  // ← changed to MIDDLE
                companyCell.Border = iTextSharp.text.Rectangle.RIGHT_BORDER
                                        | iTextSharp.text.Rectangle.TOP_BORDER;
                companyCell.BorderWidth = 0.7f;
                companyCell.Padding = 0;
                headerTable.AddCell(companyCell);

                // --- TAX INVOICE CELL (spans both columns) ---
                PdfPCell titleCell = new PdfPCell(new Phrase("TAX INVOICE", titleFont));
                titleCell.Colspan = 2;  // ← spans full width
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

                // ===================== MAIN 2-COLUMN TABLE =====================
                PdfPTable mainTable = new PdfPTable(2);
                mainTable.WidthPercentage = 100;
                mainTable.SetWidths(new float[] { 1f, 1f });
                mainTable.SpacingBefore = 0f;
                mainTable.SpacingAfter = 0f;

                // ---------- LEFT CELL 1 (Company Info) ----------
                PdfPCell leftCell = new PdfPCell();
                leftCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                | iTextSharp.text.Rectangle.TOP_BORDER
                                | iTextSharp.text.Rectangle.RIGHT_BORDER;
                leftCell.BorderWidth = 0.7f;
                leftCell.PaddingTop =  - 4f;

                Paragraph companyPara = new Paragraph("M/S CodNow Technologies", subtitleFont);
                companyPara.Alignment = Element.ALIGN_CENTER;
                companyPara.SpacingAfter = -4f;        // ← added
                leftCell.AddElement(companyPara);

                Phrase addressPhrase = new Phrase();
                addressPhrase.Add(new Chunk("Address : ", boldFont));
                addressPhrase.Add(new Chunk("Plot No.32, Gut No.83, Renuka Nagar, Opp. Vinayak Park, Deolai", normalFont));
                Paragraph addressPara = new Paragraph(addressPhrase);
                addressPara.SpacingAfter = -4f;        // ← added
                leftCell.AddElement(addressPara);

                Phrase addressPhrase2 = new Phrase();
                addressPhrase2.Add(new Chunk("Road, Chhatrapati, Sambhajinagar - 431 001", normalFont));
                Paragraph addressPara2 = new Paragraph(addressPhrase2);
                addressPara2.SpacingAfter = -4f;        // ← added
                leftCell.AddElement(addressPara2);

                Phrase phonePhrase = new Phrase();
                phonePhrase.Add(new Chunk("Phone : ", boldFont));
                phonePhrase.Add(new Chunk("+91 9923 8888 91 / +91 7972 3815 80", normalFont));
                Paragraph phonePara = new Paragraph(phonePhrase);
                phonePara.SpacingAfter = -4f;          // ← added
                leftCell.AddElement(phonePara);

                Phrase emailPhrase = new Phrase();
                emailPhrase.Add(new Chunk("E-mail : ", boldFont));
                emailPhrase.Add(new Chunk("info@codnowtechnologies.com", normalFont));
                Paragraph emailPara = new Paragraph(emailPhrase);
                emailPara.SpacingAfter = -4f;          // ← added
                leftCell.AddElement(emailPara);

                Phrase websitePhrase = new Phrase();
                websitePhrase.Add(new Chunk("Website : ", boldFont));
                websitePhrase.Add(new Chunk("www.codnowtechnologies.com", normalFont));
                Paragraph websitePara = new Paragraph(websitePhrase);
                websitePara.SpacingAfter = -4f;        // ← added
                leftCell.AddElement(websitePara);

                Phrase statePhrase = new Phrase();
                statePhrase.Add(new Chunk("State Code : ", boldFont));
                statePhrase.Add(new Chunk("27 ,  ", normalFont));
                statePhrase.Add(new Chunk("State : ", boldFont));
                statePhrase.Add(new Chunk("Maharashtra ", normalFont));
                Paragraph statePara = new Paragraph(statePhrase);
                statePara.SpacingAfter = -4f;          // ← added
                leftCell.AddElement(statePara);

                Phrase gstPhrase = new Phrase();
                gstPhrase.Add(new Chunk("GSTIN : ", boldFont));
                gstPhrase.Add(new Chunk("27FAEPS7663Q1ZK ,  ", boldFont));
                gstPhrase.Add(new Chunk("PAN : ", boldFont));
                gstPhrase.Add(new Chunk("FAEPS7663Q", boldFont));
                leftCell.AddElement(new Paragraph(gstPhrase)); // ← no spacing on last item

                mainTable.AddCell(leftCell);

                // ---------- RIGHT CELL 1 (Invoice Details) ----------
                PdfPCell rightCell = new PdfPCell();
                rightCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                 | iTextSharp.text.Rectangle.RIGHT_BORDER;
                rightCell.BorderWidth = 0.7f;
                rightCell.Padding = 0;
                rightCell.PaddingTop = 0;

                // Nested 2-column table for label : value alignment
                PdfPTable rightInnerTable = new PdfPTable(2);
                rightInnerTable.WidthPercentage = 100;
                rightInnerTable.SetWidths(new float[] { 1.2f, 1f }); // label col : value col
                rightInnerTable.HorizontalAlignment = Element.ALIGN_CENTER;

                // Helper to add label+value row
                void AddRow(string label, string value, iTextSharp.text.Font valFont)
                {
                    PdfPCell lc = new PdfPCell(new Phrase(label, boldFont));
                    lc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    lc.HorizontalAlignment = Element.ALIGN_RIGHT;  // label right-aligned
                    lc.Padding = 2;
                    rightInnerTable.AddCell(lc);

                    PdfPCell vc = new PdfPCell(new Phrase(value, valFont));
                    vc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    vc.HorizontalAlignment = Element.ALIGN_LEFT;   // value left-aligned
                    vc.Padding = 2;
                    rightInnerTable.AddCell(vc);
                }

                AddRow("Invoice No : ", "FY2526/CNT/261", boldFont);
                AddRow("Invoice Date : ", "02.02.2026", normalFont);
                AddRow("Date of Supply : ", "02.02.2026", normalFont);
                AddRow("Purchase Order No : ", "MUM/1141/17587/895", boldFont);
                AddRow("Purchase Order Dt : ", "05-Dec-25", normalFont);
                AddRow("DC No : ", "FY2526/CNT/DC046", boldFont);
                AddRow("DC Date : ", "27.12.2025", normalFont);
                AddRow("Vehicle No : ", "MH20FP9876", normalFont);

                rightCell.AddElement(rightInnerTable);
                mainTable.AddCell(rightCell);

                // ---------- LEFT CELL 2 (Invoice To Header) ----------
                PdfPCell leftCell2 = new PdfPCell();
                leftCell2.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                      | iTextSharp.text.Rectangle.RIGHT_BORDER
                                      | iTextSharp.text.Rectangle.TOP_BORDER;
                leftCell2.BorderWidth = 0.7f;
                leftCell2.PaddingBottom = 4;
                leftCell2.PaddingTop = -7;

                Paragraph invPara = new Paragraph("Invoice To", subtitleFont);
                invPara.Alignment = Element.ALIGN_CENTER;
                leftCell2.AddElement(invPara);

                mainTable.AddCell(leftCell2);

                // ---------- RIGHT CELL 2 (Shipping To Header) ----------
                PdfPCell rightCell2 = new PdfPCell();
                rightCell2.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                       | iTextSharp.text.Rectangle.RIGHT_BORDER
                                       | iTextSharp.text.Rectangle.TOP_BORDER;
                rightCell2.BorderWidth = 0.7f;
                rightCell2.PaddingBottom = 4;
                rightCell2.PaddingTop = -7;

                Paragraph shipPara = new Paragraph("Shipping To", subtitleFont);
                shipPara.Alignment = Element.ALIGN_CENTER;
                rightCell2.AddElement(shipPara);

                mainTable.AddCell(rightCell2);
                
                // ---------- LEFT CELL 3 (Invoice To Details) ----------
                PdfPCell leftCell3 = new PdfPCell();
                leftCell3.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                 | iTextSharp.text.Rectangle.RIGHT_BORDER
                                 | iTextSharp.text.Rectangle.TOP_BORDER;
                leftCell3.BorderWidth = 0.7f;
                leftCell3.PaddingTop = -4f;

                Paragraph inv2Para = new Paragraph("Procam Logistics Private Limited", boldFont);
                inv2Para.Alignment = Element.ALIGN_CENTER;
                inv2Para.SpacingAfter = -4f;           // ← added
                leftCell3.AddElement(inv2Para);

                Phrase addressPhrase3 = new Phrase();
                addressPhrase3.Add(new Chunk("Address : ", boldFont));
                addressPhrase3.Add(new Chunk("Waluj MIDC, Chh. Sambhajinagar - 431 136", normalFont));
                Paragraph addressPara3 = new Paragraph(addressPhrase3);
                addressPara3.SpacingAfter = 6f;       // ← added
                leftCell3.AddElement(addressPara3);

                Phrase scPhrase = new Phrase();
                scPhrase.Add(new Chunk("State Code : ", boldFont));
                scPhrase.Add(new Chunk("27                            ", normalFont));
                scPhrase.Add(new Chunk("State : ", boldFont));
                scPhrase.Add(new Chunk("Maharashtra", normalFont));
                Paragraph scPara = new Paragraph(scPhrase);
                scPara.SpacingAfter = -4f;             // ← added
                leftCell3.AddElement(scPara);

                Phrase GSTINPhrase = new Phrase();
                GSTINPhrase.Add(new Chunk("GSTIN : ", boldFont));
                GSTINPhrase.Add(new Chunk("27AAFCP6574J1ZW", boldFont));
                Paragraph gstinPara = new Paragraph(GSTINPhrase);
                gstinPara.SpacingAfter = -4f;          // ← added
                leftCell3.AddElement(gstinPara);

                Phrase ptPhrase = new Phrase();
                ptPhrase.Add(new Chunk("Payment Term : ", boldFont));
                ptPhrase.Add(new Chunk("AS PER PO", boldFont));
                leftCell3.AddElement(new Paragraph(ptPhrase)); // ← no spacing on last item

                mainTable.AddCell(leftCell3);

                // ---------- RIGHT CELL 3 (Shipping To Details) ----------
                PdfPCell rightCell3 = new PdfPCell();
                rightCell3.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                  | iTextSharp.text.Rectangle.RIGHT_BORDER
                                  | iTextSharp.text.Rectangle.TOP_BORDER;
                rightCell3.BorderWidth = 0.7f;
                rightCell3.PaddingTop = -4f;

                Paragraph ship2Para = new Paragraph("Procam Logistics Private Limited", boldFont);
                ship2Para.Alignment = Element.ALIGN_CENTER;
                ship2Para.SpacingAfter = -4f;          // ← added
                rightCell3.AddElement(ship2Para);

                Phrase addressPhrase4 = new Phrase();
                addressPhrase4.Add(new Chunk("Address : ", boldFont));
                addressPhrase4.Add(new Chunk("Waluj MIDC, Chh. Sambhajinagar - 431 136", normalFont));
                Paragraph addressPara4 = new Paragraph(addressPhrase4);
                addressPara4.SpacingAfter = 6f;       // ← added
                rightCell3.AddElement(addressPara4);

                Phrase scPhrase4 = new Phrase();
                scPhrase4.Add(new Chunk("State Code : ", boldFont));
                scPhrase4.Add(new Chunk("27                            ", normalFont));
                scPhrase4.Add(new Chunk("State : ", boldFont));
                scPhrase4.Add(new Chunk("Maharashtra", normalFont));
                Paragraph scPara4 = new Paragraph(scPhrase4);
                scPara4.SpacingAfter = -4f;            // ← added
                rightCell3.AddElement(scPara4);

                Phrase GSTINPhrase4 = new Phrase();
                GSTINPhrase4.Add(new Chunk("GSTIN : ", boldFont));
                GSTINPhrase4.Add(new Chunk("27AAFCP6574J1ZW", boldFont));
                rightCell3.AddElement(new Paragraph(GSTINPhrase4)); // ← no spacing on last item

                mainTable.AddCell(rightCell3);
                //// ---------- BLANK BOTTOM ROW (closes mainTable box) ----------
                PdfPCell blankLeft = new PdfPCell(new Phrase(" "));
                blankLeft.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                      | iTextSharp.text.Rectangle.RIGHT_BORDER
                                      | iTextSharp.text.Rectangle.TOP_BORDER
                                      | iTextSharp.text.Rectangle.BOTTOM_BORDER;
                blankLeft.BorderWidth = 0.7f;
                blankLeft.FixedHeight = 4f;
                mainTable.AddCell(blankLeft);

                PdfPCell blankRight = new PdfPCell(new Phrase(" "));
                blankRight.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                       | iTextSharp.text.Rectangle.RIGHT_BORDER
                                       | iTextSharp.text.Rectangle.TOP_BORDER
                                       | iTextSharp.text.Rectangle.BOTTOM_BORDER;
                blankRight.BorderWidth = 0.7f;
                blankRight.FixedHeight = 4f;
                mainTable.AddCell(blankRight);

                doc.Add(mainTable);

                // ===================== ITEMS DATA TABLE =====================
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                table.SetWidths(new float[] { 5f, 25f, 15f, 15f, 10f, 15f });

                // Default cell style — full box border on every cell
                table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.DefaultCell.Border = iTextSharp.text.Rectangle.BOX;
                table.DefaultCell.BorderWidth = 0.7f;

                // Helper to build a header cell
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

                table.AddCell(MakeHeaderCell("Sr No"));
                table.AddCell(MakeHeaderCell("Item Description"));
                table.AddCell(MakeHeaderCell("HSN Code"));
                table.AddCell(MakeHeaderCell("Rate"));
                table.AddCell(MakeHeaderCell("Qty/Nos"));
                table.AddCell(MakeHeaderCell("Amount"));

                // Helper to build a data cell
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

                decimal taxableValue = 0;

                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetInvoiceItems", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@InvoiceNo", tbxpdf.Text);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            table.AddCell(MakeDataCell(reader["SrNo"].ToString(), normalFont));
                            table.AddCell(MakeDataCell(reader["ItemDescription"].ToString(), normalFont));
                            table.AddCell(MakeDataCell(reader["HSNCode"].ToString(), normalFont));
                            table.AddCell(MakeDataCell(Convert.ToDecimal(reader["Rate"]).ToString("0.00"), normalFont));
                            table.AddCell(MakeDataCell(reader["Qty"].ToString(), normalFont));

                            decimal amount = Convert.ToDecimal(reader["Amount"]);
                            table.AddCell(MakeDataCell(amount.ToString("0.00"), normalFont));

                            taxableValue += amount;
                        }
                        reader.Close();
                    }
                }

                doc.Add(table);

                // ===================== TOTALS + BANK DETAILS TABLE =====================
                decimal cgst = taxableValue * 0.09m;
                decimal sgst = taxableValue * 0.09m;
                decimal igst = cgst + sgst;
                decimal total = taxableValue + cgst + sgst;

                PdfPTable finalTable = new PdfPTable(4);
                finalTable.WidthPercentage = 100;
                finalTable.SetWidths(new float[] { 50f,20f, 15f, 15f });

                
                // --- Bank Details Cell ---
                PdfPCell bankCell = new PdfPCell();
                bankCell.Padding = 2;
                bankCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                bankCell.BorderWidth = 0.7f;

                // Nested 2-column table for label : value alignment
                PdfPTable bankInnerTable = new PdfPTable(2);
                bankInnerTable.WidthPercentage = 100;
                bankInnerTable.PaddingTop = 0;
                bankInnerTable.SetWidths(new float[] { 1f, 1f }); 

                void AddBankRow(string label, string value)
                {
                    PdfPCell lc = new PdfPCell(new Phrase(label, normalFont));
                    lc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    lc.HorizontalAlignment = Element.ALIGN_RIGHT;
                    lc.Padding = 2;
                    bankInnerTable.AddCell(lc);

                    PdfPCell vc = new PdfPCell(new Phrase(value, normalFont));
                    vc.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    vc.HorizontalAlignment = Element.ALIGN_LEFT; 
                    vc.Padding = 2;
                    bankInnerTable.AddCell(vc);
                }

                bankCell.AddElement(new Paragraph("NEFT / RTGS BANK DETAILS:", subtitleFont));

                AddBankRow("BANK NAME  :  ", "IDBI BANK");
                AddBankRow("A/C NAME  :  ", "CODNOW TECHNOLOGIES");
                AddBankRow("A/C NUMBER  :  ", "0076102000048736");
                AddBankRow("IFSC CODE  :  ", "IBKL0000076");

                bankCell.AddElement(bankInnerTable);
                finalTable.AddCell(bankCell);

                // --- Bank Details Cell ---
                PdfPCell SpaceCell = new PdfPCell();
                SpaceCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                finalTable.AddCell(SpaceCell);

                // --- Labels Cell (nested table for per-row borders like value cell) ---
                PdfPTable labelInnerTable = new PdfPTable(1);
                labelInnerTable.WidthPercentage = 100;
                labelInnerTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                string[] labelTexts = {
                        "Taxable Value",
                        "CGST @ 9%",
                        "SGST @ 9%",
                        "IGST @ 18%",
                        "Total Value"
                    };

                foreach (string text in labelTexts)
                {
                    PdfPCell lc = new PdfPCell(new Phrase(text, boldFont));
                    lc.HorizontalAlignment = Element.ALIGN_RIGHT;
                    lc.Border = iTextSharp.text.Rectangle.BOX;   // Same as value cell
                    lc.BorderWidth = 0.7f;
                    lc.Padding = 3;
                    labelInnerTable.AddCell(lc);
                }

                PdfPCell labelCell = new PdfPCell(labelInnerTable);
                labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                labelCell.Padding = 0;

                finalTable.AddCell(labelCell);

                // --- Values Cell (nested table for per-row borders) ---
                PdfPTable valueInnerTable = new PdfPTable(1);
                valueInnerTable.WidthPercentage = 100;
                valueInnerTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                string[] valueTexts = {
                    taxableValue.ToString("0.00"),
                    cgst.ToString("0.00"),
                    sgst.ToString("0.00"),
                    igst.ToString("0.00"),
                    total.ToString("0.00")
                };

                foreach (string val in valueTexts)
                {
                    PdfPCell vc = new PdfPCell(new Phrase(val, boldFont));
                    vc.HorizontalAlignment = Element.ALIGN_CENTER;
                    vc.Border = iTextSharp.text.Rectangle.BOX;
                    vc.BorderWidth = 0.7f;
                    vc.Padding = 3;
                    valueInnerTable.AddCell(vc);
                }

                PdfPCell valueCell = new PdfPCell(valueInnerTable);
                valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                valueCell.Padding = 0;

                finalTable.AddCell(valueCell);
                doc.Add(finalTable);

                // ===================== DECLARATION TABLE =====================
                PdfPTable declarationTable = new PdfPTable(1);
                declarationTable.WidthPercentage = 100;

                PdfPCell declarationCell = new PdfPCell();
                declarationCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                            | iTextSharp.text.Rectangle.RIGHT_BORDER
                                            | iTextSharp.text.Rectangle.BOTTOM_BORDER;
                declarationCell.BorderWidth = 0.7f;
                //declarationCell.Padding = 5;
                declarationCell.PaddingTop = 0;
                


                declarationCell.AddElement(new Paragraph("E. & O.E.", normalFont) { Alignment = Element.ALIGN_RIGHT });
                declarationCell.AddElement(new Paragraph("In Words : " + ConvertToWords(total), boldFont));
                declarationCell.AddElement(new Paragraph("Transport Mode : BY ROAD", boldFont));
                declarationCell.AddElement(new Paragraph("Transporter Name : CodNow Technologies", boldFont));
                declarationCell.AddElement(new Paragraph("Remark : E-Waybill No: 2021 3110 7878 & Date : 02/02/2026", boldFont));
                declarationCell.AddElement(new Paragraph("Declaration", declarationFont));
                declarationCell.AddElement(new Paragraph(
                    "I/We here certify that my/our Reg.certi under the GST ACT 2017 is in force & dated on which the sale of goods " +
                    "specified in this tax invoice is made by me/us and transaction of sale covered by this tax invoice has been " +
                    "effected us and it shall be accounted for in the turnover of sale while filling of return and the due tax, " +
                    "if any payable on the sale has been paid or shall be paid.",
                    normalFont));

                declarationTable.AddCell(declarationCell);
                doc.Add(declarationTable);

                // ===================== SIGNATURE TABLE =====================
                PdfPTable signatureTable = new PdfPTable(2);
                signatureTable.WidthPercentage = 100;
                signatureTable.SetWidths(new float[] { 1f, 1f });

                PdfPCell sealCell = new PdfPCell();
                sealCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                     | iTextSharp.text.Rectangle.RIGHT_BORDER
                                     | iTextSharp.text.Rectangle.TOP_BORDER
                                     | iTextSharp.text.Rectangle.BOTTOM_BORDER;
                sealCell.BorderWidth = 0.7f;
                //sealCell.Padding = 5;
                sealCell.PaddingTop = -4;
                sealCell.FixedHeight = 66f;
                sealCell.AddElement(new Paragraph("Customer's Seal & Signature", normalFont));
                signatureTable.AddCell(sealCell);

                PdfPCell authCell = new PdfPCell();
                authCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                     | iTextSharp.text.Rectangle.RIGHT_BORDER
                                     | iTextSharp.text.Rectangle.TOP_BORDER
                                     | iTextSharp.text.Rectangle.BOTTOM_BORDER;
                authCell.BorderWidth = 0.7f;
                //authCell.Padding = 5;
                authCell.PaddingTop = -4;
                authCell.FixedHeight = 66f;
                Paragraph forPara = new Paragraph("For , CodNow Technologies", boldFont);
                forPara.Alignment = Element.ALIGN_RIGHT;
                authCell.AddElement(forPara);
                authCell.AddElement(new Paragraph("\n\n"));

                Paragraph authPara = new Paragraph("Authorised Signatory", normalFont);
                authPara.Alignment = Element.ALIGN_RIGHT;
                authCell.AddElement(authPara);
                signatureTable.AddCell(authCell);

                doc.Add(signatureTable);

                doc.Close();

                MessageBox.Show("Invoice PDF generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private string ConvertToWords(decimal amount)
        {
            long rupees = (long)Math.Floor(amount);
            int paise = (int)Math.Round((amount - rupees) * 100);

            string[] ones = { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE",
                               "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN",
                               "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
            string[] tens = { "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

            string NumberToWords(long n)
            {
                if (n == 0) return "";
                if (n < 20) return ones[n] + " ";
                if (n < 100) return tens[n / 10] + " " + ones[n % 10] + " ";
                if (n < 1000) return ones[n / 100] + " HUNDRED " + NumberToWords(n % 100);
                if (n < 100000) return NumberToWords(n / 1000) + "THOUSAND " + NumberToWords(n % 1000);
                if (n < 10000000) return NumberToWords(n / 100000) + "LAKH " + NumberToWords(n % 100000);
                return NumberToWords(n / 10000000) + "CRORE " + NumberToWords(n % 10000000);
            }

            string result = "RUPEES " + NumberToWords(rupees).Trim();
            if (paise > 0)
                result += " AND " + NumberToWords(paise).Trim() + " PAISE";
            result += " ONLY";
            return result;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            decimal rate = Convert.ToDecimal(tbxRate.Text);
            int qty = Convert.ToInt32(tbxQty.Text);
            decimal amount = rate * qty;

            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_InsertInvoiceItem", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@InvoiceNo", tbxIN.Text);
                    cmd.Parameters.AddWithValue("@SrNo", Convert.ToInt32(tbxSN.Text));
                    cmd.Parameters.AddWithValue("@ItemDescription", tbxID.Text);
                    cmd.Parameters.AddWithValue("@HSNCode", tbxHC.Text);
                    cmd.Parameters.AddWithValue("@Rate", rate);
                    cmd.Parameters.AddWithValue("@Qty", qty);
                    cmd.Parameters.AddWithValue("@Amount", amount);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            MessageBox.Show("Item Saved Successfully!");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in tabPage1.Controls)
            {
                if (ctrl is TextBox)
                {
                    ((TextBox)ctrl).Clear();
                }
            }

            tbxIN.Focus();
        }
    }
}