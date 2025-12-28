using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_bitirme2.Data;
using WMS_bitirme2.Models;

namespace WMS_bitirme2.Controllers
{
    public class PurchaseOrderItemsController : Controller
    {
        private readonly WMSDbContext _context;

        public PurchaseOrderItemsController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrderItems
        public async Task<IActionResult> Index()
        {
            var wMSDbContext = _context.PurchaseOrderItems.Include(p => p.Product).Include(p => p.PurchaseOrder);
            return View(await wMSDbContext.ToListAsync());
        }

        // GET: PurchaseOrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrderItem = await _context.PurchaseOrderItems
                .Include(p => p.Product)
                .Include(p => p.PurchaseOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrderItem == null)
            {
                return NotFound();
            }

            return View(purchaseOrderItem);
        }

        // GET: PurchaseOrderItems/Create
        // DİKKAT: Buraya parametre olarak (int? purchaseOrderId) ekledik 
        public IActionResult Create(int? purchaseOrderId)
        {
            // Ürün listesini gönderiyoruz (Seçmesi için)
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad");

            // DİKKAT: Sipariş listesini (SelectList) göndermiyoruz!
            // Onun yerine, gelen ID'yi direkt çantaya (ViewBag) atıyoruz.
            ViewBag.PurchaseOrderId = purchaseOrderId;

            return View();
        }

        // POST: PurchaseOrderItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PurchaseOrderId,ProductId,Quantity,UnitPrice")] PurchaseOrderItem purchaseOrderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrderItem);
                await _context.SaveChangesAsync();

                // DİKKAT: İş bitince genel listeye değil, geldiğimiz Siparişin detayına dönüyoruz 
                return RedirectToAction("Details", "PurchaseOrders", new { id = purchaseOrderItem.PurchaseOrderId });
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad", purchaseOrderItem.ProductId);

            // Hata olursa ID kaybolmasın diye tekrar gönderiyoruz
            ViewBag.PurchaseOrderId = purchaseOrderItem.PurchaseOrderId;

            return View(purchaseOrderItem);
        }

        // GET: PurchaseOrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrderItem = await _context.PurchaseOrderItems.FindAsync(id);
            if (purchaseOrderItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad", purchaseOrderItem.ProductId);
            ViewData["PurchaseOrderId"] = new SelectList(_context.PurchaseOrders, "Id", "Id", purchaseOrderItem.PurchaseOrderId);
            return View(purchaseOrderItem);
        }

        // POST: PurchaseOrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PurchaseOrderId,ProductId,Quantity,UnitPrice")] PurchaseOrderItem purchaseOrderItem)
        {
            if (id != purchaseOrderItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderItemExists(purchaseOrderItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad", purchaseOrderItem.ProductId);
            ViewData["PurchaseOrderId"] = new SelectList(_context.PurchaseOrders, "Id", "Id", purchaseOrderItem.PurchaseOrderId);
            return View(purchaseOrderItem);
        }

        // GET: PurchaseOrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrderItem = await _context.PurchaseOrderItems
                .Include(p => p.Product)
                .Include(p => p.PurchaseOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrderItem == null)
            {
                return NotFound();
            }

            return View(purchaseOrderItem);
        }

        // POST: PurchaseOrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseOrderItem = await _context.PurchaseOrderItems.FindAsync(id);
            if (purchaseOrderItem != null)
            {
                _context.PurchaseOrderItems.Remove(purchaseOrderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderItemExists(int id)
        {
            return _context.PurchaseOrderItems.Any(e => e.Id == id);
        }
    }
}
