using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Project_Orange.Data;
using MVC_Project_Orange.Models;

namespace MVC_Project_Orange.Controllers
{
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Coupons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupons.ToListAsync());
        }

        // GET: Coupons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.CouponID == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // GET: Coupons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coupons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CouponID,Code,ExpiryDate,Status")] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        // GET: Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST: Coupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CouponID,Code,ExpiryDate,Status")] Coupon coupon)
        {
            if (id != coupon.CouponID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coupon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.CouponID))
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
            return View(coupon);
        }

        // GET: Coupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.CouponID == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.CouponID == id);
        }
    }
}
