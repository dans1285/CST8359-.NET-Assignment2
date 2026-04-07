using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventSignup.Data;
using EventSignup.Models;
using EventSignup.Services;

namespace EventSignup.Controllers
{
    public class EventController : Controller
    {
        private readonly EventDbContext _context;
        private readonly IBlobService _blobService;

        // Constructor
        public EventController(EventDbContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Event/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Date,Location")] Event eventItem, IFormFile bannerImage)
        {
            if (ModelState.IsValid)
            {
                if (bannerImage != null)
                {
                    eventItem.BannerUrl = await _blobService.UploadFileAsync(bannerImage, "eventbanner");
                }

                _context.Add(eventItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventItem);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Date,Location,BannerUrl")] Event eventItem, IFormFile bannerImage)
        {
            if (id != eventItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (bannerImage != null)
                    {
                        eventItem.BannerUrl = await _blobService.UploadFileAsync(bannerImage, "eventbanner");
                    }
                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventItem.Id))
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
            return View(eventItem);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var eventItem = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        // --- Attendee Management with Attribute Routing ---

        // GET: events/{eventId}/attendees
        [Route("events/{eventId}/attendees")]
        public async Task<IActionResult> ManageAttendees(int eventId)
        {
            var eventItem = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            ViewData["EventId"] = eventId;
            ViewData["EventName"] = eventItem.Title;
            return View(eventItem.Attendees);
        }

        // GET: events/{eventId}/attendees/create
        [Route("events/{eventId}/attendees/create")]
        public IActionResult CreateAttendee(int eventId)
        {
            ViewData["EventId"] = eventId;
            return View();
        }

        // POST: events/{eventId}/attendees/create
        [HttpPost]
        [Route("events/{eventId}/attendees/create")]
        public async Task<IActionResult> CreateAttendee(int eventId, [Bind("Name,Email")] Attendee attendee)
        {
            if (ModelState.IsValid)
            {
                attendee.EventId = eventId;
                _context.Add(attendee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageAttendees), new { eventId = eventId });
            }
            ViewData["EventId"] = eventId;
            return View(attendee);
        }

        // GET: events/{eventId}/attendees/edit/{id}
        [Route("events/{eventId}/attendees/edit/{id}")]
        public async Task<IActionResult> EditAttendee(int eventId, string id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null || attendee.EventId != eventId)
            {
                return NotFound();
            }
            ViewData["EventId"] = eventId;
            return View(attendee);
        }

        // POST: events/{eventId}/attendees/edit/{id}
        [HttpPost]
        [Route("events/{eventId}/attendees/edit/{id}")]
        public async Task<IActionResult> EditAttendee(int eventId, string id, [Bind("Id,Name,Email,EventId")] Attendee attendee)
        {
            if (id != attendee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Attendees.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageAttendees), new { eventId = eventId });
            }
            ViewData["EventId"] = eventId;
            return View(attendee);
        }

        // GET: events/{eventId}/attendees/delete/{id}
        [Route("events/{eventId}/attendees/delete/{id}")]
        public async Task<IActionResult> DeleteAttendee(int eventId, string id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null || attendee.EventId != eventId)
            {
                return NotFound();
            }
            ViewData["EventId"] = eventId;
            return View(attendee);
        }

        // POST: events/{eventId}/attendees/delete/{id}
        [HttpPost, ActionName("DeleteAttendee")]
        [Route("events/{eventId}/attendees/delete/{id}")]
        public async Task<IActionResult> DeleteAttendeeConfirmed(int eventId, string id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee != null)
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageAttendees), new { eventId = eventId });
        }
        // Temporary action to fix banners for existing local data
        [Route("update-banners")]
        public async Task<IActionResult> UpdateBanners()
        {
            var events = await _context.Events.ToListAsync();
            foreach (var e in events)
            {
                if (e.Title.Contains("Tech")) e.BannerUrl = "/uploads/eventbanner/tech_conference_new.png";
                else if (e.Title.Contains("Web Development")) e.BannerUrl = "/uploads/eventbanner/web_dev.jpg";
                else if (e.Title.Contains("AI")) e.BannerUrl = "/uploads/eventbanner/ai_summit.jpg";
                else if (e.Title.Contains("Graduation")) e.BannerUrl = "/uploads/eventbanner/graduation_new.png";
            }
            await _context.SaveChangesAsync();
            return Content("Banners updated! Go back to Index.");
        }
    }
}
