using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Interfaces;
using InvoiceGenerator.Services;
using InvoiceGenerator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//[Route("Invoice")]    //Attribute Routing
namespace InvoiceGenerator.Controllers
{
    // Controllers/InvoiceController.cs
    [RequireLogin]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _svc;
        private readonly PdfService _pdf;

        public InvoiceController(IInvoiceService svc, PdfService pdf)
            => (_svc, _pdf) = (svc, pdf);

        [HttpGet]
        public IActionResult InvoiceView()
        {
            var vm = BuildViewModel();
            return View(vm);
        }

        //[HttpPost]
        //public IActionResult Save([FromBody] InvoiceViewModel vm)  //AJAX accept JSON data 
        //{
        //    // Compute GST server-side (never trust client)
        //    var invoiceTo = _svc.GetCompanyDetails(vm.Master.InvoiceTo);
        //    int stateCode = int.Parse(invoiceTo.StateCode);
        //    (vm.Master.CGST, vm.Master.SGST, vm.Master.IGST, vm.Master.TotalValue)
        //        = GstHelper.Calculate(vm.Master.TaxableValue, stateCode);

        //    int invoiceNo = _svc.SaveInvoice(vm.Master, vm.Items);
        //    return Json(new { success = true, invoiceNo });
        //}

        //[HttpPost]
        //public IActionResult Save([FromBody] InvoiceViewModel vm)
        //{
        //    var invoiceTo = _svc.GetCompanyDetails(vm.Master.InvoiceTo);
        //    int stateCode = int.Parse(invoiceTo!.StateCode);

        //    // Recalculate each item server-side using its own GST%
        //    decimal totalTaxable = 0;
        //    decimal totalGstAmt = 0;

        //    foreach (var item in vm.Items)
        //    {
        //        var (taxable, gstAmt, total) =
        //            GstHelper.CalcItemAmounts(item.Rate, item.Qty, item.GST);
        //        item.TaxableAmount = taxable;
        //        item.GSTAmount = gstAmt;
        //        item.Amount = total;
        //        totalTaxable += taxable;
        //        totalGstAmt += gstAmt;
        //    }

        //    vm.Master.TaxableValue = totalTaxable;
        //    (vm.Master.CGST, vm.Master.SGST, vm.Master.IGST, vm.Master.TotalValue)
        //        = GstHelper.SplitGst(totalTaxable, totalGstAmt, stateCode);

        //    int invoiceNo = _svc.SaveInvoice(vm.Master, vm.Items);
        //    return Json(new { success = true, invoiceNo });
        //}

        [HttpPost]
        public IActionResult Save([FromBody] InvoiceViewModel vm)
        {
            try
            {
                var invoiceTo = _svc.GetCompanyDetails(vm.Master.InvoiceTo);

                if (invoiceTo == null || invoiceTo.StateCode == 0)
                    return Json(new { success = false, message = "Invalid company details" });

                int stateCode = invoiceTo.StateCode;

                decimal totalTaxable = 0;
                decimal totalGstAmt = 0;

                foreach (var item in vm.Items)
                {
                    var (taxable, gstAmt, total) =
                        GstHelper.CalcItemAmounts(item.Rate, item.Qty, item.GST);

                    item.TaxableAmount = taxable;
                    item.GSTAmount = gstAmt;
                    item.Amount = total;

                    totalTaxable += taxable;
                    totalGstAmt += gstAmt;
                }

                vm.Master.TaxableValue = totalTaxable;

                (vm.Master.CGST, vm.Master.SGST, vm.Master.IGST, vm.Master.TotalValue)
                    = GstHelper.SplitGst(totalTaxable, totalGstAmt, stateCode);

                int invoiceNo = _svc.SaveInvoice(vm.Master, vm.Items);

                return Json(new { success = true, invoiceNo });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("Invoice/Pdf/{invoiceNo}")]  //Attribute Routing
        public IActionResult Pdf(int invoiceNo)
        {
            var (master, items) = _svc.GetInvoiceForPdf(invoiceNo);
            var invoiceTo = _svc.GetCompanyDetails(master.InvoiceTo);
            var shippingTo = _svc.GetCompanyDetails(master.ShippingTo);
            byte[] bytes = _pdf.GenerateInvoicePdf(master, items, invoiceTo, shippingTo);
            return File(bytes, "application/pdf",
                $"Invoice_{master.InvoiceNo}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        //// AJAX endpoint: get item details by name
        //[HttpGet]
        //public IActionResult GetItemDetails(string itemName)
        //{
        //    var item = _svc.GetAllItems().FirstOrDefault(i => i.ItemDescription == itemName);
        //    return Json(item);
        //}
        // GET: item details by name — now returns GST too
        [HttpGet]
        public IActionResult GetItemDetails(string itemName)
        {
            var item = _svc.GetAllItems()
                .FirstOrDefault(i => i.ItemDescription == itemName);
            return Json(item);
        }

        // AJAX endpoint: get company state code (for GST calc)
        [HttpGet]
        public IActionResult GetCompanyDetails(string name)
            => Json(_svc.GetCompanyDetails(name));

        private InvoiceViewModel BuildViewModel() => new()  //ViewModel
        {
            NextInvoiceLabel = "FY2526/CNT/" + _svc.GetNextInvoiceNo(),
            Companies = _svc.GetCompanies()
            //----------Dropdown------------------
                .Select(c => new SelectListItem(c.CompanyName, c.CompanyName)).ToList(),
            TransportModes = _svc.GetTransportModes()
                .Select(m => new SelectListItem(m, m)).ToList(),
            ItemsList = _svc.GetAllItems()
                .Select(i => new SelectListItem(i.ItemDescription, i.ItemDescription)).ToList()
        };
    }
}