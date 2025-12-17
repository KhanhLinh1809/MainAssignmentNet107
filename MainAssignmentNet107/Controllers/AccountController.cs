using MainAssignmentNet107.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainAssignmentNet107.Controllers
{
    public class AccountController : Controller
    {
        private readonly CustomerDAO _customerDAO;

        public AccountController(CustomerDAO customerDAO)
        {
            _customerDAO = customerDAO;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            try
            {
                var user = _customerDAO.Login(username, password);
                if (user != null)
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role ?? "Customer"); // Handle null role
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi đăng nhập: " + ex.Message;
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            try 
            {
                if (ModelState.IsValid)
                {
                    if (_customerDAO.CheckUsernameExists(customer.Username))
                    {
                        ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                        return View(customer);
                    }

                    _customerDAO.Register(customer);
                    return RedirectToAction("Login");
                }
                else
                {
                    // Aggregate validation errors
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    ViewBag.Error = "Lỗi nhập liệu: " + string.Join("; ", errors);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
            }
            return View(customer);
        }

        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return RedirectToAction("Login");
            
            var user = _customerDAO.GetByUsername(username);
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(Customer customer)
        {
            try
            {
                var username = HttpContext.Session.GetString("Username");
                if (username == null) return RedirectToAction("Login");

                // Đảm bảo xác thực tên người dùng khớp với phiên
                customer.Username = username; 
                _customerDAO.UpdateProfile(customer);
                TempData["Message"] = "Cập nhật thông tin thành công!";
            }
            catch (Exception ex)
            {
                 TempData["Error"] = "Lỗi: " + ex.Message;
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return RedirectToAction("Login");

            if (newPassword != confirmPassword)
            {
                 TempData["PassError"] = "Mật khẩu xác nhận không khớp.";
                 return RedirectToAction("Profile");
            }

            var user = _customerDAO.Login(username, oldPassword);
            if (user == null)
            {
                TempData["PassError"] = "Mật khẩu cũ không đúng.";
                 return RedirectToAction("Profile");
            }

            _customerDAO.ChangePassword(username, newPassword);
            TempData["PassMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
