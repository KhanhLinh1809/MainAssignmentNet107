using Microsoft.AspNetCore.Mvc;
using MainAssignmentNet107.Models;

namespace MainAssignmentNet107.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly OrderDAO _orderDAO;

        public DashboardController(OrderDAO orderDAO)
        {
            _orderDAO = orderDAO;
        }

        public IActionResult Index()
        {
             string role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Staff") return RedirectToAction("Login", "Account", new { area = "" });

            try {
                ViewBag.Summary = _orderDAO.GetOrderSummary();
                ViewBag.RevenueChart = _orderDAO.GetRevenueStatistics(); // Danh sách {Ngày, Doanh thu}
                ViewBag.TopProducts = _orderDAO.GetTopSellingProducts(5);
            }
            catch {
                // Xử lý trường hợp cơ sở dữ liệu có thể trống hoặc xảy ra lỗi.
                ViewBag.Summary = new { TotalOrders = 0, PendingOrders = 0, TotalRevenue = 0, TotalCustomers = 0 };
                ViewBag.RevenueChart = new List<dynamic>();
                ViewBag.TopProducts = new List<dynamic>();
            }

            return View();
        }
    }
}
