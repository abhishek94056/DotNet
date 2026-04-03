using InvoiceGenerator.Services;
using InvoiceGenerator.Interfaces;
using InvoiceGenerator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//[Route("Invoice")]    //Attribute Routing
namespace InvoiceGenerator.Controllers
{
    // Controllers/InvoiceController.cs
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

        [HttpPost]
        public IActionResult Save([FromBody] InvoiceViewModel vm)  //AJAX accept JSON data 
        {
            // Compute GST server-side (never trust client)
            var invoiceTo = _svc.GetCompanyDetails(vm.Master.InvoiceTo);
            int stateCode = int.Parse(invoiceTo.StateCode);
            (vm.Master.CGST, vm.Master.SGST, vm.Master.IGST, vm.Master.TotalValue)
                = GstHelper.Calculate(vm.Master.TaxableValue, stateCode);

            int invoiceNo = _svc.SaveInvoice(vm.Master, vm.Items);
            return Json(new { success = true, invoiceNo });
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

        // AJAX endpoint: get item details by name
        [HttpGet]
        public IActionResult GetItemDetails(string itemName)
        {
            var item = _svc.GetAllItems().FirstOrDefault(i => i.ItemDescription == itemName);
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