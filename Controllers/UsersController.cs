using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserInfo.Web.Models;
using UserInfo.Web.Data;
using UserInfo.Web.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using UserInfo.Web.Services;

namespace UserInfo.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        public UsersController(ApplicationDbContext dbContext, UserService userService)
        {
            this._userService = userService;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 4)
        {
            var (users, totalUsers) = await _userService.GetPageUsersAsync(pageNumber, pageSize);

            int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddUserViewModel viewModel)
        {
            if (!IsValidDateOfBirth(viewModel.DoB))
            {
                ModelState.AddModelError("DoB", "DateOfBirth is invalid!");
                return View(viewModel);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.AddUserAsync(viewModel);
                    return RedirectToAction("List", "Users");
                }
                catch (System.Exception)
                {
                    ModelState.AddModelError("", "Cannot add new user");
                }
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            Console.WriteLine(user);
            if (user is null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User viewModel)
        {
            var user = await _userService.GetUserByIdAsync(viewModel.Id);
            if (user is null)
            {
                return NotFound();
            }

            if (!IsValidDateOfBirth(viewModel.DoB))
            {
                ModelState.AddModelError("DoB", "DateOfBirth is invalid!");
            }

            if (ModelState.IsValid)
            {
                await _userService.EditUserByIdAsync(viewModel);
                return RedirectToAction("List", "Users");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteUserByIdAsync(id);
            return RedirectToAction("List", "Users");
        }

        private bool IsValidDateOfBirth(DateTime dateOfBirth)
        {
            return dateOfBirth >= new DateTime(1900, 1, 1) && dateOfBirth <= DateTime.Now;
        }
    }
}