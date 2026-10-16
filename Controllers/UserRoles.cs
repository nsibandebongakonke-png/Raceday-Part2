using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Raceday.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Raceday.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRoles : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoles(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: UserRoles
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
           var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
              var thisUserRolesViewModel = new UserRolesViewModel
              {
                  UserId = user.Id,
                  Email = user.Email ?? "N/A",
                  UserName = user.UserName ?? "N/A",
                  Roles = await _userManager.GetRolesAsync(user)
              };
              userRolesViewModel.Add(thisUserRolesViewModel);
            }

            return View(userRolesViewModel);
        }

        //GET: UserRoles/Edit/Manage?userId=someUserId
        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            
            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("Not Found");
            }
            ViewBag.UserName = user.UserName;

            var model = new List<ManageUserRolesViewModel>();

            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name ?? "N/A",
    
                };
                if (await _userManager.IsInRoleAsync(user, role.Name ?? "N/A"))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        //post: UserRoles/Edit/Manage?userId=someUserId

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Not Found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
