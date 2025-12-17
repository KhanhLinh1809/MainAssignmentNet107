using Microsoft.AspNetCore.Mvc;
using MainAssignmentNet107.Models;

namespace MainAssignmentNet107.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly CustomerDAO _customerDAO;

        public CustomerController(CustomerDAO customerDAO)
        {
            _customerDAO = customerDAO;
        }

        public IActionResult Index()
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Account", new { area = "" });

            var list = _customerDAO.GetAllCustomers();
            return View(list);
        }

        public IActionResult Create()
        {
             string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Account", new { area = "" });

            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (_customerDAO.CheckUsernameExists(customer.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    return View(customer);
                }
                _customerDAO.Create(customer);
                TempData["Success"] = "Thêm tài khoản thành công!";
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public IActionResult Edit(int id)
        {
             string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Account", new { area = "" });

            var user = _customerDAO.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            // Bỏ chọn xác thực mật khẩu nếu trường này trống (nghĩa là không thay đổi).
            if (string.IsNullOrEmpty(customer.Password))
            {
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                _customerDAO.Update(customer);
                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public IActionResult Delete(int id)
        {
             string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                 _customerDAO.Delete(id);
                 TempData["Success"] = "Xóa tài khoản thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
