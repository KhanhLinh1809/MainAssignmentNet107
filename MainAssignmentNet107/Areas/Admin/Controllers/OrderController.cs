using MainAssignmentNet107.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainAssignmentNet107.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly OrderDAO _orderDAO;

        public OrderController(OrderDAO orderDAO)
        {
            _orderDAO = orderDAO;
        }

        public IActionResult Index()
        {
            // Security check
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Staff")
            {
                 return RedirectToAction("Login", "Account", new { area = "" });
            }

            var orders = _orderDAO.GetAllOrders();
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Staff") return RedirectToAction("Login", "Account", new { area = "" });

            var order = _orderDAO.GetOrderById(id);
            if (order == null) return NotFound();

            ViewBag.Details = _orderDAO.GetOrderDetails(id);
            return View(order);
        }

        public IActionResult Approve(int id)
        {
             string role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Staff") return RedirectToAction("Login", "Account", new { area = "" });

            _orderDAO.UpdateStatus(id, "Approved");
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Cancel(int id)
        {
             string role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Staff") return RedirectToAction("Login", "Account", new { area = "" });

            _orderDAO.UpdateStatus(id, "Cancelled");
            return RedirectToAction("Details", new { id = id });
        }
    }
}
