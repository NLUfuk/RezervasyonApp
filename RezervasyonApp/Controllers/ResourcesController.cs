using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RezervasyonApp.Data;
using RezervasyonApp.Models;

namespace RezervasyonApp.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly AppDbContext _context;

        public ResourcesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Resources
        public async Task<IActionResult> Index()
        {
            // CHANGED: Listeyi ada göre sıralayalım ki UI daha düzgün olsun.
            var list = await _context.Resources
                                     .OrderBy(r => r.Name) // CHANGED
                                     .ToListAsync();
            return View(list);
        }

        // GET: Resources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var resource = await _context.Resources
                                         .FirstOrDefaultAsync(m => m.Id == id);
            if (resource == null) return NotFound();

            return View(resource);
        }

        // GET: Resources/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Resources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Resource resource)
        {
            // NEW: Aynı isimde kaynak var mı? (case-insensitive)
            var nameExists = await _context.Resources
                .AnyAsync(r => r.Name.ToLower() == resource.Name.ToLower());

            if (nameExists)
                ModelState.AddModelError(nameof(Resource.Name), "Bu isimde bir kaynak zaten var.");

            if (!ModelState.IsValid)
                return View(resource);

            _context.Add(resource);
            await _context.SaveChangesAsync();

            // NEW: Başarı mesajı
            TempData["Success"] = "Kaynak oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Resources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();

            return View(resource);
        }

        // POST: Resources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Resource resource)
        {
            if (id != resource.Id) return NotFound();

            // NEW: Aynı isim kontrolü (kendisi hariç)
            var duplicate = await _context.Resources
                .AnyAsync(r => r.Id != resource.Id &&
                               r.Name.ToLower() == resource.Name.ToLower());

            if (duplicate)
                ModelState.AddModelError(nameof(Resource.Name), "Bu isimde başka bir kaynak mevcut.");

            if (!ModelState.IsValid)
                return View(resource);

            try
            {
                _context.Update(resource);
                await _context.SaveChangesAsync();

                // NEW: Başarı mesajı
                TempData["Success"] = "Kaynak güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResourceExists(resource.Id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Resources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var resource = await _context.Resources
                                         .FirstOrDefaultAsync(m => m.Id == id);
            if (resource == null) return NotFound();

            return View(resource);
        }

        // POST: Resources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.Resources.Remove(resource);
                await _context.SaveChangesAsync();

                // NEW: Başarı mesajı
                TempData["Success"] = "Kaynak silindi.";
            }
            catch (DbUpdateException)
            {
                // NEW: Muhtemel FK hatası (ör. bu kaynağa bağlı rezervasyon var)
                TempData["Error"] = "Bu kaynak, mevcut rezervasyonlarda kullanılıyor. Önce ilişkili kayıtları güncelleyin veya silin.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ResourceExists(int id)
        {
            return _context.Resources.Any(e => e.Id == id);
        }
    }
}
