
using BeanScene.Data;
using BeanScene.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


[Authorize(Roles = "Manager,Staff")]
public class TablesController : Controller
{
    private readonly ApplicationDbContext _context;

    public TablesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TABLES
    public async Task<IActionResult> Index()
    {
        var tables = await _context.Tables
            .Include(t => t.Area)
            .ToListAsync();

        return View(tables);
    }

    // GET: TABLES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var table = await _context.Tables
            .Include(t => t.Area)
            .FirstOrDefaultAsync(m => m.TableId == id);
        if (table == null)
        {
            return NotFound();
        }

        return View(table);
    }

    // GET: TABLES/Create
    public IActionResult Create()
    {
        ViewData["AreaId"] = new SelectList(_context.Areas, "AreaId", "AreaName");

        return View();
    }

    // POST: TABLES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    // POST: TABLES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TableId,TableCode,Capacity,AreaId")] Table table)
    {
        if (ModelState.IsValid)
        {
            _context.Add(table);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["AreaId"] = new SelectList(_context.Areas, "AreaId", "AreaName", table.AreaId);

        return View(table);
    }

    // GET: TABLES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var table = await _context.Tables.FindAsync(id);
        if (table == null)
        {
            return NotFound();
        }
        ViewBag.Areas = _context.Areas.ToList();
        return View(table);
    }

    // POST: TABLES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("TableId,TableCode,Capacity,AreaId")] Table table)
    {
        if (id != table.TableId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(table);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TableExists(table.TableId))
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
        ViewBag.Areas = _context.Areas.ToList();
        return View(table);
    }

    // GET: TABLES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var table = await _context.Tables
            .FirstOrDefaultAsync(m => m.TableId == id);
        if (table == null)
        {
            return NotFound();
        }

        return View(table);
    }

    // POST: TABLES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table != null)
        {
            _context.Tables.Remove(table);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TableExists(int? tableid)
    {
        return _context.Tables.Any(e => e.TableId == tableid);
    }
}
