using MainAssignmentNet107.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainAssignmentNet107.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ProductDAO _productDAO;
        private readonly CategoryDAO _categoryDAO;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ProductDAO productDAO, CategoryDAO categoryDAO, IWebHostEnvironment webHostEnvironment)
        {
            _productDAO = productDAO;
            _categoryDAO = categoryDAO;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            // Security check
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Staff")
            {
                 return RedirectToAction("Login", "Account", new { area = "" });
            }

            var products = _productDAO.GetAll();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _categoryDAO.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile, string[] selectedColors, string[] selectedSizes)
        {
            if (ModelState.IsValid)
            {
                // Xử lý hình ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Tạo tên tệp duy nhất
                    var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(imageFile.FileName);
                    var webRootPath = _webHostEnvironment.WebRootPath;
                    var imagesPath = Path.Combine(webRootPath, "images");
                    
                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }

                    var filePath = Path.Combine(imagesPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    product.Image = fileName;
                }

                // Xử lý màu sắc
                if (selectedColors != null && selectedColors.Length > 0)
                {
                    product.Color = string.Join(", ", selectedColors);
                }

                // Kích thước tay cầm
                if (selectedSizes != null && selectedSizes.Length > 0)
                {
                    product.Size = string.Join(", ", selectedSizes);
                }

                _productDAO.Add(product);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _categoryDAO.GetAll();
            return View(product);
        }

        public IActionResult BulkUpdate()
        {
            _productDAO.BulkUpdateVariants();
            TempData["Success"] = "Đã cập nhật tự động Màu và Size cho TẤT CẢ sản phẩm!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = _productDAO.GetProductById(id);
            if (product == null) return NotFound();
            ViewBag.Categories = _categoryDAO.GetAll();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? imageFile, string[] selectedColors, string[] selectedSizes)
        {
             if (ModelState.IsValid)
            {
                // Xử lý hình ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(imageFile.FileName);
                    var webRootPath = _webHostEnvironment.WebRootPath;
                    var imagesPath = Path.Combine(webRootPath, "images");

                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }

                    var filePath = Path.Combine(imagesPath, fileName);
                    
                    try 
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        product.Image = fileName;
                        TempData["Success"] = $"Upload OK: {fileName}";
                    }
                    catch (Exception ex)
                    {
                         TempData["Error"] = $"File Error: {ex.Message}";
                    }
                }
                else
                {
                    // Giữ hình ảnh cũ
                    var oldProduct = _productDAO.GetProductById(product.ProductId);
                    product.Image = oldProduct?.Image; 
                    TempData["Warning"] = "No new file selected. Keeping old image.";
                }

                // Xử lý màu sắc
                if (selectedColors != null && selectedColors.Length > 0)
                {
                    product.Color = string.Join(", ", selectedColors);
                }
                
                // Handle Sizes
                if (selectedSizes != null && selectedSizes.Length > 0)
                {
                    product.Size = string.Join(", ", selectedSizes);
                }

                _productDAO.Update(product);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _categoryDAO.GetAll();
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            try
            {
                _productDAO.Delete(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
