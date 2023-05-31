using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class Administrator : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        public Administrator(RoleManager<IdentityRole> roleManager,UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ManageRole()
        {
            var lstRoles = roleManager.Roles;
            
            return View(lstRoles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleModel roleModel)
        {
            if(ModelState.IsValid)
            {
                var role = new IdentityRole()
                {
                    Name = roleModel.RoleName
                };

                var result = await roleManager.CreateAsync(role);
                if(result.Succeeded)
                {
                    ViewBag.Message = "Role Create Successfully";
                    return View("Information");
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }
            return View();
        }

        public async Task< IActionResult> AddOrRemoveUsers(string roleId)
        {
            var role =await roleManager.FindByIdAsync(roleId);
            if(role==null)
            {
                ViewBag.Message = "Role With This Id Is No Found";
                return View("Information");
            }
            ViewBag.RoleId = role.Id;
            ViewBag.RoleName = role.Name;

            var lstUsers = userManager.Users;
            List<EditUserModel> lstEditUsers = new List<EditUserModel>();
            foreach(var user in lstUsers)
            {
                EditUserModel editUser = new EditUserModel();
                editUser.UserName = user.UserName;


                if(await userManager.IsInRoleAsync(user,role.Name))
                {
                    editUser.IsSelected = true;
                }
                lstEditUsers.Add(editUser);
            }
            return View(lstEditUsers);
        }

        public async Task< IActionResult> EditUsers(List<EditUserModel> lstEditUserModel)
        {
            string roleId = Request.Form["roleId"];
            var role =await roleManager.FindByIdAsync(roleId);

            if(role==null)
            {
                ViewBag.Message = "Role With This ID Is Not Available";
                return View("Information");
            }

            var lstUsers = userManager.Users;//getting list of users

            //removing all the users that belongs to it's respective role
            for(int i=0;i<lstUsers.ToList().Count;i++)
            {
                if(await userManager.IsInRoleAsync(lstUsers.ToList()[i],role.Name))
                {
                    await userManager.RemoveFromRoleAsync(lstUsers.ToList()[i], role.Name);
                }
            }

            //adding users to the respective role 
            for(int i=0;i<lstEditUserModel.Count;i++)
            {
                if(lstEditUserModel[i].IsSelected)
                {
                    var user = await userManager.FindByEmailAsync(lstEditUserModel[i].UserName);
                    await userManager.AddToRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction("ManageRole");
        }
    }
}
