
using BeanScene.Data;
using BeanScene.Models;
using BeanScene.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Identity.UI.Services;

[Authorize(Roles = "Manager,Staff")]
public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;

    public ReservationsController(
    ApplicationDbContext context,
    IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    // GET: RESERVATIONS
    public async Task<IActionResult> Index(
    string searchString,
    DateTime? fromDate,
    DateTime? toDate)
    {
        var reservations = _context.Reservations
            .Include(r => r.Sitting)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            reservations = reservations.Where(r =>
                r.FirstName.Contains(searchString) ||
                r.LastName.Contains(searchString) ||
                r.Email.Contains(searchString) ||
                r.PhoneNumber.Contains(searchString));
        }

        // From Date filter
        if (fromDate.HasValue)
        {
            reservations = reservations.Where(r =>
                r.ReservationDate >= fromDate.Value);
        }

        // To Date filter
        if (toDate.HasValue)
        {
            reservations = reservations.Where(r =>
                r.ReservationDate <= toDate.Value);
        }

        ViewData["CurrentFilter"] = searchString;
        ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
        ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");

        return View(await reservations.ToListAsync());
    }

    // GET: RESERVATIONS/Details/5





    public async Task<IActionResult> ExportExcel(
      string searchString,
      DateTime? fromDate,
      DateTime? toDate)
    {
        var reservations = _context.Reservations
            .Include(r => r.Sitting)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            reservations = reservations.Where(r =>
                r.FirstName.Contains(searchString) ||
                r.LastName.Contains(searchString) ||
                r.Email.Contains(searchString) ||
                r.PhoneNumber.Contains(searchString));
        }

        // From Date filter
        if (fromDate.HasValue)
        {
            reservations = reservations.Where(r =>
                r.ReservationDate >= fromDate.Value);
        }

        // To Date filter
        if (toDate.HasValue)
        {
            reservations = reservations.Where(r =>
                r.ReservationDate <= toDate.Value);
        }

        var data = await reservations.ToListAsync();

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Reservations");

        // Report Title
        worksheet.Cell(1, 1).Value = "Bean Scene Reservation Report";
        worksheet.Range("A1:I1").Merge();

        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 18;
        worksheet.Row(1).Height = 30;
        worksheet.Cell(1, 1).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        // Report Information
        worksheet.Cell(2, 1).Value =
            $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}";

        if (fromDate.HasValue)
        {
            worksheet.Cell(2, 5).Value =
                $"From: {fromDate.Value:dd/MM/yyyy}";
        }

        if (toDate.HasValue)
        {
            worksheet.Cell(2, 7).Value =
                $"To: {toDate.Value:dd/MM/yyyy}";
        }

        // Headers (Row 4)
        worksheet.Cell(4, 1).Value = "First Name";
        worksheet.Cell(4, 2).Value = "Last Name";
        worksheet.Cell(4, 3).Value = "Email";
        worksheet.Cell(4, 4).Value = "Phone";
        worksheet.Cell(4, 5).Value = "Date";
        worksheet.Cell(4, 6).Value = "Time";
        worksheet.Cell(4, 7).Value = "Guests";
        worksheet.Cell(4, 8).Value = "Status";
        worksheet.Cell(4, 9).Value = "Sitting";

        var headerRange = worksheet.Range("A4:I4");

        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.DarkBrown;
        headerRange.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        // Data starts from Row 5
        int row = 5;

        foreach (var reservation in data)
        {
            worksheet.Cell(row, 1).Value = reservation.FirstName;
            worksheet.Cell(row, 2).Value = reservation.LastName;
            worksheet.Cell(row, 3).Value = reservation.Email;
            worksheet.Cell(row, 4).Value = reservation.PhoneNumber;
            worksheet.Cell(row, 5).Value =
                reservation.ReservationDate?.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 6).Value =
                reservation.StartTime.ToString();
            worksheet.Cell(row, 7).Value =
                reservation.NumberOfGuests;
            worksheet.Cell(row, 8).Value =
                reservation.Status;
            worksheet.Cell(row, 9).Value =
                reservation.Sitting?.SittingType;

            row++;
        }

        // Total Reservations
        worksheet.Cell(row + 1, 1).Value =
            $"Total Reservations: {data.Count}";

        worksheet.Cell(row + 1, 1).Style.Font.Bold = true;

        // Auto Filter
        worksheet.Range(4, 1, row - 1, 9).SetAutoFilter();
        // Auto Size Columns
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var content = stream.ToArray();

        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Reservations_{DateTime.Now:yyyyMMdd}.xlsx");
    }
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Sitting)
            .Include(r => r.ReservationTables!)
                .ThenInclude(rt => rt.Table!)
                    .ThenInclude(t => t.Area)
            .FirstOrDefaultAsync(r => r.ReservationId == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }
    // GET: RESERVATIONS/Create
    public IActionResult Create()
    {
       

        var model = new Reservation
        {
            ReservationDate = DateTime.Today,
            Status = "Pending",
            Areas = _context.Areas.ToList(),
            Tables = _context.Tables.ToList(),
            Sittings = _context.Sittings.ToList()
        };

        return View(model);
    }

    // POST: RESERVATIONS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ReservationId,FirstName," +
        "LastName,Email,PhoneNumber,ReservationDate," +
        "NumberOfGuests,StartTime,DurationMinutes,Status,Notes,Source," +
        "SittingId,Sitting,UserId,ReservationTables,SelectedAreaId,SelectedTableId")] Reservation reservation)
    {
        if (ModelState.IsValid)
        {
            var table = await _context.Tables
            .FindAsync(reservation.SelectedTableId);

            _context.Add(reservation);
            

            if (table == null)
            {
                ModelState.AddModelError("", "Please select a table.");

                reservation.Areas = _context.Areas.ToList();
                reservation.Tables = _context.Tables.ToList();
                reservation.Sittings = _context.Sittings.ToList();

                return View(reservation);
            }

            if (reservation.NumberOfGuests > table.Capacity)
            {
                ModelState.AddModelError(
                    "",
                    $"This table only seats {table.Capacity} guests.");

                reservation.Areas = _context.Areas.ToList();
                reservation.Tables = _context.Tables.ToList();
                reservation.Sittings = _context.Sittings.ToList();

                return View(reservation);
            }
            var sitting = await _context.Sittings
            .FindAsync(reservation.SittingId);

            if (sitting == null)
            {
                ModelState.AddModelError("", "Invalid sitting selected.");

                reservation.Areas = _context.Areas.ToList();
                reservation.Tables = _context.Tables.ToList();
                reservation.Sittings = _context.Sittings.ToList();

                return View(reservation);
            }
            var reservationEndTime =
            reservation.StartTime.AddMinutes(
                reservation.DurationMinutes);

            if (reservation.StartTime < sitting.StartTime ||
                reservationEndTime > sitting.EndTime)
            {
                ModelState.AddModelError(
                    "",
                    "Reservation must fit within the sitting time window.");

                reservation.Areas = _context.Areas.ToList();
                reservation.Tables = _context.Tables.ToList();
                reservation.Sittings = _context.Sittings.ToList();

                return View(reservation);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
        reservation.Areas = _context.Areas.ToList();
        reservation.Tables = _context.Tables.ToList();
        reservation.Sittings = _context.Sittings.ToList();
        return View(reservation);
    }

    // GET: RESERVATIONS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var reservation = await _context.Reservations.FindAsync(id);

        if (reservation == null)
        {
            return NotFound();
        }
        var reservationTable = await _context.ReservationTables
            .Include(rt => rt.Table)
            .FirstOrDefaultAsync(rt => rt.ReservationId == id);
        if (reservationTable != null)
        {
            reservation.SelectedTableId = reservationTable.TableId;
            reservation.SelectedAreaId = reservationTable.Table.AreaId;
        }
        reservation.Areas = _context.Areas.ToList();
        reservation.Tables = _context.Tables.ToList();
        reservation.Sittings = _context.Sittings.ToList();
        return View(reservation);
    }

    // POST: RESERVATIONS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
       int? reservationid,
       [Bind("ReservationId,FirstName,LastName,Email,PhoneNumber,ReservationDate,NumberOfGuests,StartTime,DurationMinutes,Status,Notes,Source,SittingId,Sitting,UserId,ReservationTables,SelectedAreaId,SelectedTableId")]
    Reservation reservation)
    {
        if (reservationid != reservation.ReservationId)
        {
            return NotFound();
        }

        // TABLE VALIDATION
        var selectedTable = await _context.Tables
            .FindAsync(reservation.SelectedTableId);

        if (selectedTable == null)
        {
            ModelState.AddModelError(
                nameof(reservation.SelectedTableId),
                "Please select a table.");
        }
        else if (reservation.NumberOfGuests > selectedTable.Capacity)
        {
            ModelState.AddModelError(
                nameof(reservation.NumberOfGuests),
                $"This table only seats {selectedTable.Capacity} guests.");
        }

        // SITTING VALIDATION
        var sitting = await _context.Sittings
            .FindAsync(reservation.SittingId);

        if (sitting != null)
        {
            var reservationEndTime =
                reservation.StartTime.AddMinutes(
                    reservation.DurationMinutes);

            if (reservation.StartTime < sitting.StartTime ||
                reservationEndTime > sitting.EndTime)
            {
                ModelState.AddModelError(
                    "",
                    "Reservation must fit within the sitting time window.");
            }
        }

        // IF ANY VALIDATION FAILED
        if (!ModelState.IsValid)
        {
            reservation.Areas = _context.Areas.ToList();
            reservation.Tables = _context.Tables.ToList();
            reservation.Sittings = _context.Sittings.ToList();

            return View(reservation);
        }

        try
        {
            var existingReservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.ReservationId == reservation.ReservationId);

            var oldStatus = existingReservation?.Status;
            _context.Update(reservation);

            await _context.SaveChangesAsync();

            if (oldStatus != "Confirmed" &&
    reservation.Status == "Confirmed")
            {
                await _emailSender.SendEmailAsync(
                    reservation.Email,
                    "Bean Scene Reservation Confirmed",
                    $@"
        <h2>☕ Bean Scene</h2>

        <p>Dear {reservation.FirstName},</p>

        <p>Your reservation has been confirmed.</p>

        <p><strong>Date:</strong>
        {reservation.ReservationDate:dd/MM/yyyy}</p>

        <p><strong>Time:</strong>
        {reservation.StartTime}</p>

        <p><strong>Guests:</strong>
        {reservation.NumberOfGuests}</p>

        <p>We look forward to welcoming you to Bean Scene.</p>

        <p>Kind regards,<br/>Bean Scene Team</p>
        ");
            }
            var reservationTable = await _context.ReservationTables
                .FirstOrDefaultAsync(rt =>
                    rt.ReservationId == reservation.ReservationId);

            if (reservation.SelectedTableId.HasValue)
            {
                if (reservationTable != null)
                {
                    _context.ReservationTables.Remove(reservationTable);
                }

                _context.ReservationTables.Add(
                    new ReservationTable
                    {
                        ReservationId = reservation.ReservationId,
                        TableId = reservation.SelectedTableId.Value
                    });
            }

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReservationExists(reservation.ReservationId))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: RESERVATIONS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(m => m.ReservationId == id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    // POST: RESERVATIONS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(
    int reservationId,
    string status)
    {
        var reservation =
            await _context.Reservations.FindAsync(reservationId);

        if (reservation == null)
        {
            return NotFound();
        }

        var oldStatus = reservation.Status;

        reservation.Status = status;

        await _context.SaveChangesAsync();

        // Send email only when changing to Confirmed
        if (oldStatus != "Confirmed" &&
            status == "Confirmed")
        {
            await _emailSender.SendEmailAsync(
                reservation.Email,
                "Bean Scene Reservation Confirmed",
                $@"
            <h2>☕ Bean Scene</h2>

            <p>Dear {reservation.FirstName},</p>

            <p>Your reservation has been confirmed.</p>

            <p><strong>Date:</strong> {reservation.ReservationDate:dd/MM/yyyy}</p>

            <p><strong>Time:</strong> {reservation.StartTime}</p>

            <p><strong>Guests:</strong> {reservation.NumberOfGuests}</p>

            <p>We look forward to welcoming you to Bean Scene.</p>

            <p>Kind regards,<br/>Bean Scene Team</p>
            ");
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ReservationExists(int? reservationid)
    {
        return _context.Reservations.Any(e => e.ReservationId == reservationid);
    }
}
