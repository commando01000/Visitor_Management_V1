using Microsoft.Graph;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.AccountRepository;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;

namespace Visitor_Management_Portal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        private readonly IAccountService _accountService;
        public AccountController(IAccountRepository accountRepository, IAccountService accountService)
        {
            _accountRepository = accountRepository;
            _accountService = accountService;
        }

        // GET: Account
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
            return View();
        }

        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                // Signal OWIN to send an authorization request to Azure
                Request.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = Url.Action("FinalizeLogin", "Account", null, Request.Url.Scheme) },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public ActionResult FinalizeLogin()
        {
            try
            {
                var email = ClaimsManager.GetUserEmailFromSession();
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Logout");
                }

                var userVm = _accountService.FindUserByEmail(email);
                if (userVm == null)
                {
                    ViewBag.ErrorMessage = "Invalid Email Or Password";
                    return View("Index");
                }

                var claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.NameIdentifier, userVm.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, userVm.FullName));
                claims.Add(new Claim(ClaimTypes.Role, userVm.RoleName));
                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Utilities.AuthenticationMethod.Microsoft.ToString()));
                claims.Add(new Claim(ClaimsManager.OrganizationId, userVm.OranizationId.ToString()));
                claims.Add(new Claim(ClaimsManager.OrganizationName, userVm.OrganizationName));

                var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                authenticationManager.SignIn(claimsIdentity);

                return RedirectToAction("Dashboard", "Dashboard");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Something Went Wrong , Please Try Again Later";
                return View("Index");
            }
        }

        [HttpPost]
        public JsonResult ForgetPasswordOTP(string Email)
        {
            var user = _accountService.Authenticate(Email);

            if (user != null)
            {
                var result = _accountService.SendOTP(user);
                if (result == false)
                {
                    return Json(new { success = false, message = "OTP Not Sent." });
                }
                else
                {
                    return Json(new { success = true, message = "OTP Sent Successfully." });
                }
            }
            else
            {
                return Json(new { success = false, message = "User Not Found." });
            }
        }

        [HttpGet]
        public JsonResult ValidateOTP(string Email, string OTP)
        {
            var result = _accountService.ValidateOTP(Email, OTP);

            if (result)
            {
                // set temp data to make sure the call is valid
                TempData["AllowResetPassword"] = true;
                return Json(new Models.OperationResult { Status = true, Message = "OTP Validated Successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = result, message = "Invalid OTP." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ResetPassword(string Email, string password)
        {
            bool allowResetPassword = TempData["AllowResetPassword"] as bool? ?? false;

            if (allowResetPassword)
            {
                var result = _accountService.ResetPassword(Email, password);
                if (result)
                {
                    return Json(new Models.OperationResult { Status = true, Message = "Password Reset Successfully." });
                }
                else
                {
                    return Json(new Models.OperationResult { Status = false, Message = "Password Reset Failed." });
                }
            }
            else
            {
                return Json(new Models.OperationResult { Status = false, Message = "Action Not Allowed." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Register(string fullName, string email, string password, string organizationName, string organizationDomain)
        {
            bool success = _accountRepository.Register(fullName, email, password, organizationName, organizationDomain);
            if (success)
            {
                return Json(new { success = true, message = "Registration successful." });
            }
            else
            {
                return Json(new { success = false, message = "This organization is already registered" });
            }
        }

        [HttpPost]
        public JsonResult Login(string email, string password)
        {
            OrganizationUserVM user = _accountService.FindUserByEmailAndPassword(email, password);

            if (user != null)
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, user.FullName));
                claims.Add(new Claim(ClaimTypes.Role, user.RoleName));
                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Utilities.AuthenticationMethod.Normal.ToString()));
                claims.Add(new Claim(ClaimsManager.OrganizationId, user.OranizationId.ToString()));
                claims.Add(new Claim(ClaimsManager.OrganizationName, user.OrganizationName));

                var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties
                {
                    IsPersistent = true
                }, claimsIdentity);

                return Json(new { success = true, message = "Login successful" });
            }
            else
            {
                return Json(new { success = false, message = "Invalid email or password." });
            }
        }

        public ActionResult Logout()
        {
            Utilities.AuthenticationMethod authenticationMethod = (Utilities.AuthenticationMethod)
                Enum.Parse(typeof(Utilities.AuthenticationMethod), ClaimsManager.GetAuthenticationMethod());

            var authenticationManager = Request.GetOwinContext().Authentication;

            switch (authenticationMethod)
            {
                case Utilities.AuthenticationMethod.Microsoft:
                    authenticationManager.SignOut(
                        new AuthenticationProperties
                        {
                            RedirectUri = Url.Action("Index", "Account", null, Request.Url.Scheme)
                        },
                        OpenIdConnectAuthenticationDefaults.AuthenticationType, // Sign out from Microsoft authentication
                        CookieAuthenticationDefaults.AuthenticationType // Sign out from cookie authentication
                    );
                    break;

                case Utilities.AuthenticationMethod.Normal:
                default:
                    authenticationManager.SignOut(
                        CookieAuthenticationDefaults.AuthenticationType // Sign out from cookie authentication
                    );
                    break;
            }

            // Clear session and cache to ensure full logout
            Session.Clear();
            Session.Abandon();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
            {
                var cookie = new HttpCookie(".AspNet.ApplicationCookie")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Account");
        }


        private Guid GetCurrentUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("User identity not found");
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return new Guid(userIdClaim);
        }

    }
}