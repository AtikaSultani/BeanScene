
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeanScene.Models;
using BeanScene.Data;
using Microsoft.AspNetCore.Authorization;


[Authorize(Roles = "Manager,Staff")]

public class SittingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public SittingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: SITTINGS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Sittings.ToListAsync());
    }

    // GET: SITTINGS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sitting = await _context.Sittings
            .FirstOrDefaultAsync(m => m.SittingId == id);
        if (sitting == null)
        {
            return NotFound();
        }

        return View(sitting);
    }

    // GET: SITTINGS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SITTINGS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SittingId,SittingType,StartTime,EndTime,Capacity,Reservations")] Sitting sitting)
    {
        if (ModelState.IsValid)
        {
            _context.Add(sitting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(sitting);
    }

    // GET: SITTINGS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sitting = await _context.Sittings.FindAsync(id);
        if (sitting == null)
        {
            return NotFound();
        }
        return View(sitting);
    }

    // POST: SITTINGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("SittingId,SittingType,StartTime,EndTime,Capacity,Reservations")] Sitting sitting)
    {
        if (id != sitting.SittingId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(sitting);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SittingExists(sitting.SittingId))
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
        return View(sitting);
    }

    // Details Action
   

    // GET: SITTINGS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sitting = await _context.Sittings
            .FirstOrDefaultAsync(m => m.SittingId == id);
        if (sitting == null)
        {
            return NotFound();
        }

        return View(sitting);
    }

    // POST: SITTINGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var sitting = await _context.Sittings.FindAsync(id);
        if (sitting != null)
        {
            _context.Sittings.Remove(sitting);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SittingExists(int? sittingid)
    {
        return _context.Sittings.Any(e => e.SittingId == sittingid);
    }
}
