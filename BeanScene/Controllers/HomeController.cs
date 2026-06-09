using BeanScene.Data;
using BeanScene.Models;
using BeanScene.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace BeanScene.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Reservation()
        {
            var model = new ReservationViewModel
            {
                ReservationDate = DateTime.Today,
                Areas = _context.Areas.ToList(),
                Tables = _context.Tables.ToList(),
                Sittings = _context.Sittings.ToList()

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservation(ReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Areas = _context.Areas.ToList();
                model.Tables = _context.Tables.ToList();
                model.Sittings = _context.Sittings.ToList();
                return View(model);
            }
            var sitting = await _context.Sittings.FindAsync(model.SittingId);
            var table = await _context.Tables
            .FindAsync(model.SelectedTableId);

                    if (table == null)
                    {
                        ModelState.AddModelError("",
                            "Please select a table.");
                        model.Areas = _context.Areas.ToList();
                        model.Tables = _context.Tables.ToList();
                        model.Sittings = _context.Sittings.ToList();

                return View(model);
                    }

            if (model.NumberOfGuests > table.Capacity)
            {
                ModelState.AddModelError("",
                    $"This table only seats {table.Capacity} guests.");

                    model.Areas = _context.Areas.ToList();
                    model.Tables = _context.Tables.ToList();
                    model.Sittings = _context.Sittings.ToList();

                return View(model);
            }

            if (sitting == null)
            {
                ModelState.AddModelError("", "Invalid sitting selected.");
                model.Areas = _context.Areas.ToList();
                model.Tables = _context.Tables.ToList();
                model.Sittings = _context.Sittings.ToList();
                return View(model);
            }
            var reservationEndTime =
                model.StartTime.AddMinutes(model.DurationMinutes);

            if (model.StartTime < sitting.StartTime ||
                reservationEndTime > sitting.EndTime)
            {
                Console.WriteLine($"Sitting: {model.SittingId}");
                Console.WriteLine($"StartTime: {model.StartTime}");
                Console.WriteLine($"Duration: {model.DurationMinutes}");
                Console.WriteLine($"Sitting Start: {sitting.StartTime}");
                Console.WriteLine($"Sitting End: {sitting.EndTime}");
                ModelState.AddModelError(
                    "",
                    "Reservation must fit within the sitting time window.");
                model.Areas = _context.Areas.ToList();
                model.Tables = _context.Tables.ToList();
                model.Sittings = _context.Sittings.ToList();

                return View(model);
            }

            var reservation = new Reservation
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,

                ReservationDate = model.ReservationDate,
                StartTime = model.StartTime,
                DurationMinutes = model.DurationMinutes,

                NumberOfGuests = model.NumberOfGuests,

                SittingId = model.SittingId,

                Notes = model.Notes,

                Status = "Pending",
                Source = "Online"
            };

            reservation.Status = "Pending";

            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();

            if (model.SelectedTableId.HasValue)
            {
                _context.ReservationTables.Add(
                    new ReservationTable
                    {
                        ReservationId = reservation.ReservationId,
                        TableId = model.SelectedTableId.Value
                    });

                await _context.SaveChangesAsync();
            }
            var newModel = new ReservationViewModel
            {
                ReservationDate = DateTime.Today,
                Areas = _context.Areas.ToList(),
                Tables = _context.Tables.ToList(),
                Sittings = _context.Sittings.ToList()
            };

            ViewBag.Message = "Reservation submitted successfully!";

            return View(newModel);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["SuccessMessage"] =
                "Thank you for contacting Bean Scene. We will get back to you shortly.";

            return RedirectToAction(nameof(Contact));
        }

        public IActionResult DrawingDemo()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
