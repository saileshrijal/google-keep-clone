using GoogleKeepClone.Data;
using GoogleKeepClone.Models;
using GoogleKeepClone.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoogleKeepClone.Controllers
{

    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public NotesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                //getting all the notes created by the loggedIn User
                var loggedInUser = await _userManager.GetUserAsync(User);
                var notes = await _context.Notes!
                                .Where(x => x.UserId == loggedInUser!.Id)
                                .OrderByDescending(x => x.CreatedDate)
                                .ToListAsync();
                var vm = notes.Select(x => new NotesVm()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Color = x.Color
                }).ToList();
                return View(vm);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotesVm vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(vm);
                }
                var loggedInUser = await _userManager.GetUserAsync(User);
                if (loggedInUser == null)
                {
                    throw new Exception("logged in user note found");
                }
                var notes = new Notes()
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    Color = vm.Color,
                    UserId = loggedInUser.Id,
                    CreatedDate = DateTime.UtcNow
                };
                await _context.Notes!.AddAsync(notes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var loggedInUser = await _userManager.GetUserAsync(User);
                var note = await _context.Notes!.FirstOrDefaultAsync(x => x.Id == id);
                if (note == null)
                {
                    throw new Exception("note not found");
                }
                if (note.UserId != loggedInUser!.Id)
                {
                    throw new Exception("You are not authorized to edit this note");
                }
                var vm = new EditNotesVm()
                {
                    Id = note.Id,
                    Title = note.Title,
                    Description = note.Description,
                    Color = note.Color
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditNotesVm vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(vm);
                }
                var loggedInUser = await _userManager.GetUserAsync(User);
                var note = await _context.Notes!.FirstOrDefaultAsync(x => x.Id == vm.Id);
                if (note == null)
                {
                    throw new Exception("note not found");
                }
                if (note.UserId != loggedInUser!.Id)
                {
                    throw new Exception("You are not authorized to edit this note");
                }
                note.Title = vm.Title;
                note.Description = vm.Description;
                note.Color = vm.Color;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var note = await _context.Notes!.FirstOrDefaultAsync(x => x.Id == id);
                if (note == null)
                {
                    throw new Exception("note not found");
                }
                _context.Notes!.Remove(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}