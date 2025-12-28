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
    public class SalesOrderItemsController : Controller
    {
        private readonly WMSDbContext _context;

        public SalesOrderItemsController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: SalesOrderItems
        public async Task<IActionResult> Index()
        {
            var wMSDbContext = _context.SalesOrderItems.Include(s => s.Product).Include(s => s.SalesOrder);
            return View(await wMSDbContext.ToListAsync());
        }

        // GET: SalesOrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderItem = await _context.SalesOrderItems
                .Include(s => s.Product)
                .Include(s => s.SalesOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesOrderItem == null)
            {
                return NotFound();
            }

            return View(salesOrderItem);
        }

        // GET: SalesOrderItems/Create
        public IActionResult Create(int? salesOrderId)
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad");
            ViewBag.SalesOrderId = salesOrderId; // Çantaya koyduk
            return View();
        }

        // POST: SalesOrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SalesOrderId,ProductId,Quantity,UnitPrice")] SalesOrderItem salesOrderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesOrderItem);
                await _context.SaveChangesAsync();

                // SÜPER: İş bitince ana listeye değil, o siparişin detayına dönüyoruz
                return RedirectToAction("Details", "SalesOrders", new { id = salesOrderItem.SalesOrderId });
            }

            // Hata olursa ürün listesini tekrar doldur
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad", salesOrderItem.ProductId);      
            ViewBag.SalesOrderId = salesOrderItem.SalesOrderId;

            return View(salesOrderItem);
        }

        // GET: SalesOrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderItem = await _context.SalesOrderItems.FindAsync(id);
            if (salesOrderItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad", salesOrderItem.ProductId);
            ViewData["SalesOrderId"] = new SelectList(_context.SalesOrders, "Id", "Id", salesOrderItem.SalesOrderId);
            return View(salesOrderItem);
        }

        // POST: SalesOrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SalesOrderId,ProductId,Quantity,UnitPrice")] SalesOrderItem salesOrderItem)
        {
            if (id != salesOrderItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesOrderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderItemExists(salesOrderItem.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad", salesOrderItem.ProductId);
            ViewData["SalesOrderId"] = new SelectList(_context.SalesOrders, "Id", "Id", salesOrderItem.SalesOrderId);
            return View(salesOrderItem);
        }

        // GET: SalesOrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrderItem = await _context.SalesOrderItems
                .Include(s => s.Product)
                .Include(s => s.SalesOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesOrderItem == null)
            {
                return NotFound();
            }

            return View(salesOrderItem);
        }

        // POST: SalesOrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesOrderItem = await _context.SalesOrderItems.FindAsync(id);
            if (salesOrderItem != null)
            {
                _context.SalesOrderItems.Remove(salesOrderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrderItemExists(int id)
        {
            return _context.SalesOrderItems.Any(e => e.Id == id);
        }
    }
}
