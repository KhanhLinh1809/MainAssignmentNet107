using System.Diagnostics;
using MainAssignmentNet107.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainAssignmentNet107.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductDAO _productDAO;
        private readonly CategoryDAO _categoryDAO;
        private readonly MainAssignmentNet107.Helpers.DatabaseHelper _dbHelper;

        // Updated constructor to include DatabaseHelper
        public HomeController(ILogger<HomeController> logger, ProductDAO productDAO, CategoryDAO categoryDAO, MainAssignmentNet107.Helpers.DatabaseHelper dbHelper)
        {
            _logger = logger;
            _productDAO = productDAO;
            _categoryDAO = categoryDAO;
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            var model = new MainAssignmentNet107.ViewModels.HomeViewModel
            {
                Categories = _categoryDAO.GetAll(),
                FeaturedProducts = _productDAO.GetTopProducts(12) // Get top 12 products
            };
            return View(model);
        }

        public IActionResult FixDatabase()
        {
            try
            {
                // 1. Chạy tập lệnh hiện có nếu cần (tùy chọn)

                // 2. Cập nhật bảng Chi tiết giỏ hàng
                string valColor = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CartDetails' AND COLUMN_NAME = 'Color') BEGIN ALTER TABLE CartDetails ADD Color NVARCHAR(50) END";
                _dbHelper.ExecuteNonQuery(valColor);

                string valSize = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CartDetails' AND COLUMN_NAME = 'Size') BEGIN ALTER TABLE CartDetails ADD Size NVARCHAR(50) END";
                _dbHelper.ExecuteNonQuery(valSize);

                return Content("Cập nhật Database thành công! Đã thêm cột Color và Size vào bảng CartDetails.");
            }
            catch (Exception ex)
            {
                return Content("Lỗi cập nhật DB: " + ex.Message);
            }
        }

        public IActionResult Details(int id)
        {
            var product = _productDAO.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Search(string query)
        {
            var products = _productDAO.SearchProducts(query);
            return View("Index", new MainAssignmentNet107.ViewModels.HomeViewModel
            {
                Categories = _categoryDAO.GetAll(),
                FeaturedProducts = products
            });
        }

        public IActionResult Category(int id)
        {
            var products = _productDAO.GetProductsByCategoryId(id);
            var category = _categoryDAO.GetAll().FirstOrDefault(c => c.CategoryId == id);
            ViewData["CategoryName"] = category?.Name;

            return View("Index", new MainAssignmentNet107.ViewModels.HomeViewModel
            {
                Categories = _categoryDAO.GetAll(),
                FeaturedProducts = products
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
