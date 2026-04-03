// Controllers/TransportController.cs
using Microsoft.AspNetCore.Mvc;
using InvoiceGenerator.Models;
using InvoiceGenerator.Interfaces;

namespace InvoiceGenerator.Controllers
{
    public class TransportController : Controller
    {
        private readonly ITransportService _svc;

        public TransportController(ITransportService svc) => _svc = svc;

        // GET: /Transport
        public IActionResult TransportView()
        {
            var list = _svc.GetAll();
            return View(list);
        }

        // GET: fetch one record for Edit modal
        [HttpGet]
        public IActionResult GetById(int Id)
        {
            var mode = _svc.GetById(Id);
            if (mode == null) return NotFound();
            return Json(mode);
        }

        // POST: Save (Insert or Update)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(TransportModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });

            if (model.Id == 0)
                _svc.Insert(model);
            else
                _svc.Update(model);

            return Json(new { success = true });
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int Id)
        {
            _svc.Delete(Id);
            return Json(new { success = true });
        }
    }
}