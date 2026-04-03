using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace InvoiceGenerator
{
    public partial class Form1 : Form
    {
        readonly String connectionstring = ConfigurationManager.ConnectionStrings["InvoiceGenerator"].ConnectionString;

        public Form1()
        {
            InitializeComponent();

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF files (*.pdf)|*.pdf";
            sfd.FileName = "Invoice_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Document doc = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                doc.Open();

                iTextSharp.text.Font companyFont = FontFactory.GetFont("Garamond", 35, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.ITALIC);
                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.COURIER, 16, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE);
                iTextSharp.text.Font subtitleFont = FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font boldFont = FontFactory.GetFont("Verdana", 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = FontFactory.GetFont("Verdana", 10);
                iTextSharp.text.Font declarationFont = FontFactory.GetFont("Verdana", 12, iTextSharp.text.Font.UNDERLINE);

                Paragraph originalText = new Paragraph("ORIGINAL FOR BUYER", headerFont);//verdana 10 courier new 
                originalText.Alignment = Element.ALIGN_RIGHT;
                doc.Add(originalText);
                doc.Add(new Paragraph("\n"));

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
                leftCell.PaddingTop = -4f;

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

                string invoiceText = lblID.Text;
                int invoiceNo = Convert.ToInt32(invoiceText.Split('/')[2]);
                DataRow master = null;

                using (SqlConnection con = new SqlConnection(connectionstring))
                using (SqlCommand cmd = new SqlCommand("SP_GetInvoiceMaster", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) { MessageBox.Show("Invoice not found!"); return; }
                    master = dt.Rows[0];
                }

                AddRow("Invoice No : ", "FY2526/CNT/" + master["InvoiceNo"].ToString(), boldFont);
                AddRow("Invoice Date : ", Convert.ToDateTime(master["InvoiceDate"]).ToString("dd.MM.yyyy"), normalFont);
                AddRow("Date of Supply : ", Convert.ToDateTime(master["DateOfSupply"]).ToString("dd.MM.yyyy"), normalFont);
                AddRow("Purchase Order No : ", master["PurchaseOrderNo"].ToString(), boldFont);
                AddRow("Purchase Order Dt : ", Convert.ToDateTime(master["PurchaseOrderDt"]).ToString("dd-MMM-yy"), normalFont);
                AddRow("DC No : ", master["DCNo"].ToString(), boldFont);
                AddRow("DC Date : ", Convert.ToDateTime(master["DCDate"]).ToString("dd.MM.yyyy"), normalFont);
                AddRow("Vehicle No : ", master["VehicleNo"].ToString(), normalFont);


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

                string invoiceTo = master["InvoiceTo"].ToString();
                string shippingTo = master["ShippingTo"].ToString();

                DataRow invoiceToData = null;
                DataRow shippingToData = null;

                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();

                    // Invoice To
                    using (SqlCommand cmd = new SqlCommand("SP_GetCompanyDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompanyName", invoiceTo);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                            invoiceToData = dt.Rows[0];
                    }

                    // Shipping To
                    using (SqlCommand cmd = new SqlCommand("SP_GetCompanyDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompanyName", shippingTo);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                            shippingToData = dt.Rows[0];
                    }
                }

                PdfPCell leftCell3 = new PdfPCell();
                leftCell3.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                 | iTextSharp.text.Rectangle.RIGHT_BORDER
                                 | iTextSharp.text.Rectangle.TOP_BORDER;
                leftCell3.BorderWidth = 0.7f;
                leftCell3.PaddingTop = -4f;

                Paragraph inv2Para = new Paragraph(invoiceToData["CompanyName"].ToString(), boldFont);
                inv2Para.Alignment = Element.ALIGN_CENTER;
                inv2Para.SpacingAfter = -4f;
                leftCell3.AddElement(inv2Para);

                Phrase addressPhrase3 = new Phrase();
                addressPhrase3.Add(new Chunk("Address : ", boldFont));
                addressPhrase3.Add(new Chunk(invoiceToData["Address"].ToString(), normalFont));
                Paragraph addressPara3 = new Paragraph(addressPhrase3);
                addressPara3.SpacingAfter = 6f;
                leftCell3.AddElement(addressPara3);

                Phrase scPhrase = new Phrase();
                scPhrase.Add(new Chunk("State Code : ", boldFont));
                scPhrase.Add(new Chunk(invoiceToData["StateCode"].ToString() + "    ", normalFont));
                scPhrase.Add(new Chunk("State : ", boldFont));
                scPhrase.Add(new Chunk(invoiceToData["State"].ToString(), normalFont));
                Paragraph scPara = new Paragraph(scPhrase);
                scPara.SpacingAfter = -4f;
                leftCell3.AddElement(scPara);

                Phrase GSTINPhrase = new Phrase();
                GSTINPhrase.Add(new Chunk("GSTIN : ", boldFont));
                GSTINPhrase.Add(new Chunk(invoiceToData["GSTIN"].ToString(), boldFont));
                Paragraph gstinPara = new Paragraph(GSTINPhrase);
                gstinPara.SpacingAfter = -4f;
                leftCell3.AddElement(gstinPara);

                Phrase ptPhrase = new Phrase();
                ptPhrase.Add(new Chunk("Payment Term : ", boldFont));
                ptPhrase.Add(new Chunk(invoiceToData["PaymentTerm"].ToString(), boldFont));
                leftCell3.AddElement(new Paragraph(ptPhrase));

                mainTable.AddCell(leftCell3);

                PdfPCell rightCell3 = new PdfPCell();
                rightCell3.Border = iTextSharp.text.Rectangle.LEFT_BORDER
                                  | iTextSharp.text.Rectangle.RIGHT_BORDER
                                  | iTextSharp.text.Rectangle.TOP_BORDER;
                rightCell3.BorderWidth = 0.7f;
                rightCell3.PaddingTop = -4f;

                Paragraph ship2Para = new Paragraph(shippingToData["CompanyName"].ToString(), boldFont);
                ship2Para.Alignment = Element.ALIGN_CENTER;
                ship2Para.SpacingAfter = -4f;
                rightCell3.AddElement(ship2Para);

                Phrase addressPhrase4 = new Phrase();
                addressPhrase4.Add(new Chunk("Address : ", boldFont));
                addressPhrase4.Add(new Chunk(shippingToData["Address"].ToString(), normalFont));
                Paragraph addressPara4 = new Paragraph(addressPhrase4);
                addressPara4.SpacingAfter = 6f;
                rightCell3.AddElement(addressPara4);

                Phrase scPhrase4 = new Phrase();
                scPhrase4.Add(new Chunk("State Code : ", boldFont));
                scPhrase4.Add(new Chunk(shippingToData["StateCode"].ToString() + "    ", normalFont));
                scPhrase4.Add(new Chunk("State : ", boldFont));
                scPhrase4.Add(new Chunk(shippingToData["State"].ToString(), normalFont));
                Paragraph scPara4 = new Paragraph(scPhrase4);
                scPara4.SpacingAfter = -4f;
                rightCell3.AddElement(scPara4);

                Phrase GSTINPhrase4 = new Phrase();
                GSTINPhrase4.Add(new Chunk("GSTIN : ", boldFont));
                GSTINPhrase4.Add(new Chunk(shippingToData["GSTIN"].ToString(), boldFont));
                rightCell3.AddElement(new Paragraph(GSTINPhrase4));

                mainTable.AddCell(rightCell3);


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
                        cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);  //"tbxpdf.Text"

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
                decimal cgst = Convert.ToDecimal(tbxCGST.Text);
                decimal sgst = Convert.ToDecimal(tbxSGST.Text);
                decimal igst = Convert.ToDecimal(tbxIGST.Text);
                decimal total = Convert.ToDecimal(tbxTotalValue.Text);

                PdfPTable finalTable = new PdfPTable(4);
                finalTable.WidthPercentage = 100;
                finalTable.SetWidths(new float[] { 50f, 20f, 15f, 15f });

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
                //declarationCell.AddElement(new Paragraph("Transport Mode : BY ROAD", boldFont));
                declarationCell.AddElement(new Paragraph("Transport Mode : " + cbTransportMode.Text, boldFont));
                declarationCell.AddElement(new Paragraph("Transporter Name : CodNow Technologies", boldFont));
                declarationCell.AddElement(new Paragraph("Remark : " + tbxRemark.Text, boldFont));
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
                authCell.PaddingTop = -4;
                authCell.FixedHeight = 90f;

                Paragraph forPara = new Paragraph("For , CodNow Technologies", boldFont);
                forPara.Alignment = Element.ALIGN_RIGHT;
                authCell.AddElement(forPara);

                authCell.AddElement(new Paragraph(" "));

                // static signature image
                string signPath = @"D:\Dot Net\WinForms\InvoiceGenerator\Signature.png";

                iTextSharp.text.Image signImg = iTextSharp.text.Image.GetInstance(signPath);
                signImg.ScaleToFit(120f, 40f);
                signImg.Alignment = Element.ALIGN_MIDDLE;

                authCell.AddElement(signImg);

                Paragraph authPara = new Paragraph("Authorised Signatory", normalFont);
                authPara.Alignment = Element.ALIGN_RIGHT;

                authCell.AddElement(authPara);
                signatureTable.AddCell(authCell);
                doc.Add(signatureTable);
                doc.Close();
                MessageBox.Show("Invoice PDF generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
        private void Form1_Load_1(object sender, EventArgs e)
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            LoadInvoiceNo();
            LoadCompanies();
            LoadItems();
            CalculateTotal();
            CalculateGST();//load company name
            LoadTransportModes();
            dgvItems.AllowUserToAddRows = false;
            dgvItems.RowHeadersVisible = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvItems.Columns.Clear();

            dgvItems.Columns.Add("SrNo", "Sr No");

            DataGridViewComboBoxColumn cmbItem = new DataGridViewComboBoxColumn();
            cmbItem.Name = "ItemDescription";
            cmbItem.HeaderText = "Item Description";
            cmbItem.DataSource = itemTable;
            cmbItem.DisplayMember = "ItemName";
            cmbItem.ValueMember = "ItemName";
            dgvItems.Columns.Add(cmbItem);

            dgvItems.Columns.Add("HSN", "HSN Code");
            dgvItems.Columns.Add("Rate", "Rate");
            dgvItems.Columns.Add("Qty", "Qty");
            dgvItems.Columns.Add("Amount", "Amount");

            dgvItems.Columns["HSN"].ReadOnly = true;
            dgvItems.Columns["Rate"].ReadOnly = true;
            dgvItems.Columns["Amount"].ReadOnly = true;

            dgvItems.CellEndEdit += dgvItems_CellEndEdit_1;
            dgvItems.RowsAdded += dgvItems_RowsAdded;

            dgvItems.Rows.Add();
        }
        private void dgvItems_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                dgvItems.Rows[i].Cells["SrNo"].Value = i + 1;
            }

        }
        private void dgvItems_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvItems.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            // When Item is selected
            if (dgvItems.Columns[e.ColumnIndex].Name == "ItemDescription")
            {
                string itemName = Convert.ToString(row.Cells["ItemDescription"].Value);

                DataRow[] rows = itemTable.Select($"ItemName = '{itemName}'");

                if (rows.Length > 0)
                {
                    row.Cells["HSN"].Value = rows[0]["HSNCode"];
                    row.Cells["Rate"].Value = rows[0]["Rate"];
                }
            }

            // When Qty is entered
            if (dgvItems.Columns[e.ColumnIndex].Name == "Qty")
            {
                decimal qty = 0;
                decimal.TryParse(Convert.ToString(row.Cells["Qty"].Value), out qty);

                decimal rate = 0;
                decimal.TryParse(Convert.ToString(row.Cells["Rate"].Value), out rate);

                decimal amount = qty * rate;

                row.Cells["Amount"].Value = amount;

                // 🔹 Add new row automatically
                if (e.RowIndex == dgvItems.Rows.Count - 1 && qty > 0)
                {
                    dgvItems.Rows.Add();

                    dgvItems.CurrentCell =
                        dgvItems.Rows[dgvItems.Rows.Count - 1].Cells["ItemDescription"];
                }

                CalculateTotal();
            }
        }
        private void CalculateTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (!row.IsNewRow && row.Cells["Amount"].Value != null)
                {
                    decimal value;
                    decimal.TryParse(row.Cells["Amount"].Value.ToString(), out value);
                    total += value;
                }
            }

            txtTaxableValue.Text = total.ToString("0.00");

            CalculateGST();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            //string dcSeq = "FY2526/CNT/DC";

            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();

                // ---------------- SAVE MASTER ----------------
                SqlCommand cmd = new SqlCommand("SP_InsertInvoiceMaster", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@InvoiceDate", dtID.Value.Date);
                cmd.Parameters.AddWithValue("@DateOfSupply", DTds.Value.Date);
                cmd.Parameters.AddWithValue("@PurchaseOrderNo", "MUM/1141/17587/" + tbxPON.Text);
                cmd.Parameters.AddWithValue("@PurchaseOrderDt", dtPOD.Value.Date);
                cmd.Parameters.AddWithValue("@DCNo", "FY2526/CNT/DC" + tbxDC.Text);
                cmd.Parameters.AddWithValue("@DCDate", dtDC.Value.Date);
                cmd.Parameters.AddWithValue("@VehicleNo", tbxVN.Text.Trim());
                cmd.Parameters.AddWithValue("@InvoiceTo", cbIT.Text);
                cmd.Parameters.AddWithValue("@ShippingTo", cbST.Text);
                cmd.Parameters.AddWithValue("@TaxableValue", txtTaxableValue.Text);
                cmd.Parameters.AddWithValue("@CGST", tbxCGST.Text);
                cmd.Parameters.AddWithValue("@SGST", tbxSGST.Text);
                cmd.Parameters.AddWithValue("@IGST", tbxIGST.Text);
                cmd.Parameters.AddWithValue("@TotalValue", tbxTotalValue.Text);
                cmd.Parameters.AddWithValue("@TransportMode", cbTransportMode.Text);
                cmd.Parameters.AddWithValue("@Remark", tbxRemark.Text);
                object result = cmd.ExecuteScalar();

                MessageBox.Show($"Master saved! ID: FY2526/CNT/{result}");

                // ---------------- SAVE ITEMS ----------------
                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells["ItemDescription"].Value == null)
                        continue;
                    SqlCommand cmdItem = new SqlCommand("SP_InsertInvoiceItem", con);
                    cmdItem.CommandType = CommandType.StoredProcedure;

                    cmdItem.Parameters.AddWithValue("@InvoiceNo", result);  //lblID.Text
                    cmdItem.Parameters.AddWithValue("@SrNo", row.Cells["SrNo"].Value);
                    cmdItem.Parameters.AddWithValue("@ItemDescription", row.Cells["ItemDescription"].Value);
                    cmdItem.Parameters.AddWithValue("@HSNCode", row.Cells["HSN"].Value);
                    cmdItem.Parameters.AddWithValue("@Rate", row.Cells["Rate"].Value);
                    cmdItem.Parameters.AddWithValue("@Qty", row.Cells["Qty"].Value);
                    cmdItem.Parameters.AddWithValue("@Amount", row.Cells["Amount"].Value);

                    cmdItem.ExecuteNonQuery();
                }
            }
        }
        private DataTable itemTable;
        private void LoadItems()
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            using (SqlCommand cmd = new SqlCommand("SP_GetItems", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                itemTable = new DataTable();
                da.Fill(itemTable);
            }
        }
        private void LoadInvoiceNo()
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(InvoiceNo),0) + 1 FROM InvoiceMaster", con))
            {
                con.Open();
                int nextInvoiceNo = Convert.ToInt32(cmd.ExecuteScalar());
                //string invoiceSeq = "FY2526/CNT/";  // e.g. FY2526/CNT/261
                lblID.Text = "FY2526/CNT/" + nextInvoiceNo.ToString();
            }
        }
        private void LoadCompanies()
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            using (SqlCommand cmd = new SqlCommand("SP_GetCompanies", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Invoice To
                cbIT.DataSource = dt.Copy();
                cbIT.DisplayMember = "CompanyName";
                cbIT.ValueMember = "CompanyName";

                // Shipping To
                cbST.DataSource = dt.Copy();
                cbST.DisplayMember = "CompanyName";
                cbST.ValueMember = "CompanyName";
            }
        }
        private void CalculateGST()
        {
            decimal taxableValue = 0;
            decimal.TryParse(txtTaxableValue.Text, out taxableValue);

            int stateCode = 0;
            int.TryParse(lblITSC.Text, out stateCode);   // StateCode label

            decimal cgst = 0;
            decimal sgst = 0;
            decimal igst = 0;

            if (stateCode == 27)
            {
                cgst = taxableValue * 0.09m;
                sgst = taxableValue * 0.09m;
            }
            else
            {
                igst = taxableValue * 0.18m;
            }

            tbxCGST.Text = cgst.ToString("0.00");
            tbxSGST.Text = sgst.ToString("0.00");
            tbxIGST.Text = igst.ToString("0.00");

            decimal total = taxableValue + cgst + sgst + igst;

            tbxTotalValue.Text = total.ToString("0.00");
            // Convert total into words
            lblTotalValueWord.Text = ConvertToWords(total);
        }
        private void cbIT_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cbIT.SelectedIndex == -1)
                return;

            using (SqlConnection con = new SqlConnection(connectionstring))
            using (SqlCommand cmd = new SqlCommand("SP_GetCompanyDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Correct parameter name
                cmd.Parameters.AddWithValue("@CompanyName", cbIT.Text);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblITAdd.Text = dr["Address"].ToString();
                    lblITSC.Text = dr["StateCode"].ToString();
                    lblITS.Text = dr["State"].ToString();
                    lblITGSTIN.Text = dr["GSTIN"].ToString();
                    lblITPT.Text = dr["PaymentTerm"].ToString();
                }

                con.Close();

            }
            CalculateGST();
        }

        private void cbST_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbST.SelectedIndex == -1)
                return;

            using (SqlConnection con = new SqlConnection(connectionstring))
            using (SqlCommand cmd = new SqlCommand("SP_GetCompanyDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CompanyName", cbST.Text);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblSTAdd.Text = dr["Address"].ToString();
                    lblSTSC.Text = dr["StateCode"].ToString();
                    lblSTS.Text = dr["State"].ToString();
                    lblSTGSTIN.Text = dr["GSTIN"].ToString();
                }
            }
            CalculateGST();
        }
        private void LoadTransportModes()
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            using (SqlCommand cmd = new SqlCommand("SP_GetTransportModes", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbTransportMode.DataSource = dt;
                cbTransportMode.DisplayMember = "ModeName";
                cbTransportMode.ValueMember = "ModeName";
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

