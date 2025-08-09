using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RezervasyonApp.Data;
using RezervasyonApp.Models;

namespace RezervasyonApp.Controllers
{
    public class ShiftsController : Controller
    {
        private readonly AppDbContext _context;

        public ShiftsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Shifts
        public async Task<IActionResult> Index()
        {
            var list = await _context.Shifts
                .Include(s => s.Driver)
                .Include(s => s.Vehicle)
                .OrderBy(s => s.DepartureTime)
                .ToListAsync(); // CHANGED: direkt list al
            return View(list);
        }

        // GET: Shifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var shift = await _context.Shifts
                .Include(s => s.Driver)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shift == null) return NotFound();

            return View(shift);
        }

        // GET: Shifts/Create
        public async Task<IActionResult> Create()
        {
            // CHANGED: Sadece aktif şoförler + alfabetik
            ViewData["DriverId"] = new SelectList(
                await _context.Drivers.Where(d => d.IsActive).OrderBy(d => d.FullName).ToListAsync(),
                "Id", "FullName"
            );

            ViewData["VehicleId"] = new SelectList(
                await _context.Vehicles.OrderBy(v => v.Plate).ToListAsync(),
                "Id", "Plate"
            );

            return View();
        }

        // POST: Shifts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DriverId,VehicleId,DepartureTime")] Shift shift)
        {
            // NEW: Araç bu şoföre ait mi?
            bool match = await _context.Vehicles
                .AnyAsync(v => v.Id == shift.VehicleId && v.DriverId == shift.DriverId);
            if (!match)
                ModelState.AddModelError("", "Seçilen araç bu şoföre ait değil.");

            // NEW: Çakışma kontrolleri (aynı saatte aynı şoför/araç)
            bool driverClash = await _context.Shifts
                .AnyAsync(s => s.DriverId == shift.DriverId && s.DepartureTime == shift.DepartureTime);
            if (driverClash)
                ModelState.AddModelError("", "Bu şoföre aynı saatte başka bir sefer atanmış.");

            bool vehicleClash = await _context.Shifts
                .AnyAsync(s => s.VehicleId == shift.VehicleId && s.DepartureTime == shift.DepartureTime);
            if (vehicleClash)
                ModelState.AddModelError("", "Bu araca aynı saatte başka bir sefer atanmış.");

            if (!ModelState.IsValid)
            {
                // CHANGED: dropdown’ları tekrar doldur
                ViewData["DriverId"] = new SelectList(
                    await _context.Drivers.Where(d => d.IsActive).OrderBy(d => d.FullName).ToListAsync(),
                    "Id", "FullName", shift.DriverId);
                ViewData["VehicleId"] = new SelectList(
                    await _context.Vehicles.OrderBy(v => v.Plate).ToListAsync(),
                    "Id", "Plate", shift.VehicleId);
                return View(shift);
            }

            _context.Add(shift);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Shift eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Shifts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null) return NotFound();

            // CHANGED: Mevcut şoför inaktifse bile listede kalsın
            var drivers = await _context.Drivers
                .Where(d => d.IsActive || d.Id == shift.DriverId)
                .OrderBy(d => d.FullName)
                .ToListAsync();

            ViewData["DriverId"] = new SelectList(drivers, "Id", "FullName", shift.DriverId);

            ViewData["VehicleId"] = new SelectList(
                await _context.Vehicles.OrderBy(v => v.Plate).ToListAsync(),
                "Id", "Plate", shift.VehicleId);

            return View(shift);
        }

        // POST: Shifts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DriverId,VehicleId,DepartureTime")] Shift shift)
        {
            if (id != shift.Id) return NotFound();

            // NEW: Araç gerçekten bu şoföre ait mi?
            bool match = await _context.Vehicles
                .AnyAsync(v => v.Id == shift.VehicleId && v.DriverId == shift.DriverId);
            if (!match)
                ModelState.AddModelError("", "Seçilen araç bu şoföre ait değil.");

            // NEW: Çakışma kontrolleri (kendi kaydı hariç)
            bool driverClash = await _context.Shifts
                .AnyAsync(s => s.Id != shift.Id && s.DriverId == shift.DriverId && s.DepartureTime == shift.DepartureTime);
            if (driverClash)
                ModelState.AddModelError("", "Bu şoföre aynı saatte başka bir sefer atanmış.");

            bool vehicleClash = await _context.Shifts
                .AnyAsync(s => s.Id != shift.Id && s.VehicleId == shift.VehicleId && s.DepartureTime == shift.DepartureTime);
            if (vehicleClash)
                ModelState.AddModelError("", "Bu araca aynı saatte başka bir sefer atanmış.");

            if (!ModelState.IsValid)
            {
                var drivers = await _context.Drivers
                    .Where(d => d.IsActive || d.Id == shift.DriverId)
                    .OrderBy(d => d.FullName)
                    .ToListAsync();

                ViewData["DriverId"] = new SelectList(drivers, "Id", "FullName", shift.DriverId);
                ViewData["VehicleId"] = new SelectList(
                    await _context.Vehicles.OrderBy(v => v.Plate).ToListAsync(),
                    "Id", "Plate", shift.VehicleId);

                return View(shift);
            }

            try
            {
                _context.Update(shift);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Shift güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Shifts.AnyAsync(e => e.Id == shift.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Shifts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var shift = await _context.Shifts
                .Include(s => s.Driver)
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shift == null) return NotFound();

            return View(shift);
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift != null)
                _context.Shifts.Remove(shift);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Shift silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
