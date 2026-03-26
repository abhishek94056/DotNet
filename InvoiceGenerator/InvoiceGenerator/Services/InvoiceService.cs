// Services/InvoiceService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class InvoiceService
    {
        private readonly string _conn;

        public InvoiceService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── Next Invoice No ──────────────────────────────────────────
        public int GetNextInvoiceNo()
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SELECT ISNULL(MAX(InvoiceNo), 0) + 1 FROM InvoiceMaster", con);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ── Companies dropdown ───────────────────────────────────────
        public List<CompanyDetails> GetCompanies()
        {
            var list = new List<CompanyDetails>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetCompanies", con)
            { CommandType = CommandType.StoredProcedure };
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(new CompanyDetails { CompanyName = dr["CompanyName"].ToString()! });
            return list;
        }

        // ── Single company details ───────────────────────────────────
        public CompanyDetails? GetCompanyDetails(string name)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetCompanyDetails", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CompanyName", name);
            con.Open();
            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;
            return new CompanyDetails
            {
                CompanyName = dr["CompanyName"].ToString()!,
                Address = dr["Address"].ToString()!,
                StateCode = dr["StateCode"].ToString()!,
                State = dr["State"].ToString()!,
                GSTIN = dr["GSTIN"].ToString()!,
                PaymentTerm = dr["PaymentTerm"].ToString()!
            };
        }

        // ── All items (for dropdown) ─────────────────────────────────
        public List<InvoiceItem> GetAllItems()
        {
            var list = new List<InvoiceItem>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetItems", con)
            { CommandType = CommandType.StoredProcedure };
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(new InvoiceItem
                {
                    ItemDescription = dr["ItemName"].ToString()!,
                    HSNCode = dr["HSNCode"].ToString()!,
                    Rate = Convert.ToDecimal(dr["Rate"])
                });
            return list;
        }

        // ── Single item by name ──────────────────────────────────────
        public InvoiceItem? GetItemByName(string name)
            => GetAllItems().FirstOrDefault(i =>
                i.ItemDescription.Equals(name, StringComparison.OrdinalIgnoreCase));

        // ── Transport modes ──────────────────────────────────────────
        public List<string> GetTransportModes()
        {
            var list = new List<string>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetTransportModes", con)
            { CommandType = CommandType.StoredProcedure };
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(dr["ModeName"].ToString()!);
            return list;
        }

        // ── Save invoice + items, return new InvoiceNo ───────────────
        public int SaveInvoice(InvoiceMaster master, List<InvoiceItem> items)
        {
            using var con = new SqlConnection(_conn);
            con.Open();

            // Master
            var cmd = new SqlCommand("SP_InsertInvoiceMaster", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@InvoiceDate", master.InvoiceDate);
            cmd.Parameters.AddWithValue("@DateOfSupply", master.DateOfSupply);
            cmd.Parameters.AddWithValue("@PurchaseOrderNo", master.PurchaseOrderNo);
            cmd.Parameters.AddWithValue("@PurchaseOrderDt", master.PurchaseOrderDt);
            //cmd.Parameters.AddWithValue("@DCNo", master.DCNo);
            //cmd.Parameters.AddWithValue("@DCDate", master.DCDate);
            cmd.Parameters.AddWithValue("@VehicleNo", master.VehicleNo);
            cmd.Parameters.AddWithValue("@ASNNo", master.ASNNo);
            cmd.Parameters.AddWithValue("@InvoiceTo", master.InvoiceTo);
            cmd.Parameters.AddWithValue("@ShippingTo", master.ShippingTo);
            cmd.Parameters.AddWithValue("@TaxableValue", master.TaxableValue);
            cmd.Parameters.AddWithValue("@CGST", master.CGST);
            cmd.Parameters.AddWithValue("@SGST", master.SGST);
            cmd.Parameters.AddWithValue("@IGST", master.IGST);
            cmd.Parameters.AddWithValue("@TotalValue", master.TotalValue);
            cmd.Parameters.AddWithValue("@TransportMode", master.TransportMode);
            cmd.Parameters.AddWithValue("@Remark", master.Remark ?? "");

            int invoiceNo = Convert.ToInt32(cmd.ExecuteScalar());

            // Items
            foreach (var item in items)
            {
                var ci = new SqlCommand("SP_InsertInvoiceItem", con)
                { CommandType = CommandType.StoredProcedure };

                ci.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                ci.Parameters.AddWithValue("@SrNo", item.SrNo);
                ci.Parameters.AddWithValue("@ItemDescription", item.ItemDescription);
                ci.Parameters.AddWithValue("@HSNCode", item.HSNCode);
                ci.Parameters.AddWithValue("@Rate", item.Rate);
                ci.Parameters.AddWithValue("@Qty", item.Qty);
                ci.Parameters.AddWithValue("@Amount", item.Amount);

                ci.ExecuteNonQuery();
            }

            return invoiceNo;
        }

        // ── Get invoice + items for PDF ──────────────────────────────
        public (InvoiceMaster? master, List<InvoiceItem> items) GetInvoiceForPdf(int invoiceNo)
        {
            InvoiceMaster? master = null;

            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand("SP_GetInvoiceMaster", con)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                con.Open();
                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0) return (null, new List<InvoiceItem>());

                var r = dt.Rows[0];
                master = new InvoiceMaster
                {
                    InvoiceNo = Convert.ToInt32(r["InvoiceNo"]),
                    InvoiceDate = Convert.ToDateTime(r["InvoiceDate"]),
                    DateOfSupply = Convert.ToDateTime(r["DateOfSupply"]),
                    PurchaseOrderNo = r["PurchaseOrderNo"].ToString()!,
                    PurchaseOrderDt = Convert.ToDateTime(r["PurchaseOrderDt"]),
                    //DCNo = r["DCNo"].ToString()!,
                    //DCDate = Convert.ToDateTime(r["DCDate"]),
                    VehicleNo = r["VehicleNo"].ToString()!,
                    ASNNo = r["ASNNo"].ToString()!,
                    InvoiceTo = r["InvoiceTo"].ToString()!,
                    ShippingTo = r["ShippingTo"].ToString()!,
                    TaxableValue = Convert.ToDecimal(r["TaxableValue"]),
                    CGST = Convert.ToDecimal(r["CGST"]),
                    SGST = Convert.ToDecimal(r["SGST"]),
                    IGST = Convert.ToDecimal(r["IGST"]),
                    TotalValue = Convert.ToDecimal(r["TotalValue"]),
                    TransportMode = r["TransportMode"].ToString()!,
                    Remark = r["Remark"].ToString()!
                };
            }

            var items = new List<InvoiceItem>();
            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand("SP_GetInvoiceItems", con)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                con.Open();
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    items.Add(new InvoiceItem
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        ItemDescription = dr["ItemDescription"].ToString()!,
                        HSNCode = dr["HSNCode"].ToString()!,
                        Rate = Convert.ToDecimal(dr["Rate"]),
                        Qty = Convert.ToDecimal(dr["Qty"]),
                        Amount = Convert.ToDecimal(dr["Amount"])
                    });
            }

            return (master, items);
        }
    }
}