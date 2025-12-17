using MainAssignmentNet107.Helpers;
using MainAssignmentNet107.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainAssignmentNet107.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductDAO _productDAO;
        private readonly OrderDAO _orderDAO;
        private readonly CustomerDAO _customerDAO;

        public CartController(ProductDAO productDAO, OrderDAO orderDAO, CustomerDAO customerDAO)
        {
            _productDAO = productDAO;
            _orderDAO = orderDAO;
            _customerDAO = customerDAO;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        public IActionResult AddToCart(int productId, string color, string size)
        {
            var product = _productDAO.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            // Kiểm tra xem mặt hàng có cùng ID, màu sắc VÀ kích thước có tồn tại hay không
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId && c.Color == color && c.Size == size);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    Image = product.Image,
                    Color = color,
                    Size = size
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult UpdateCart(int productId, string color, string size, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId && c.Color == color && c.Size == size);
                if (item != null)
                {
                    item.Quantity = quantity;
                    if (item.Quantity <= 0)
                    {
                        cart.Remove(item);
                    }
                }
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId, string color, string size)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId && c.Color == color && c.Size == size);
                if (item != null)
                {
                    cart.Remove(item);
                }
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Checkout()
        {
            // Check login
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            return View(cart);
        }


        [HttpPost]
        public IActionResult Checkout(string receiverAddress, string receiverPhone)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(receiverAddress) || string.IsNullOrEmpty(receiverPhone))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin giao hàng.";
                return View(cart);
            }

            // Lấy CustomerId từ cơ sở dữ liệu bằng tên người dùng.
            string username = HttpContext.Session.GetString("Username");

            // Giả sử chúng ta có thể lấy được đối tượng hoặc ID Khách hàng.

            // Để đơn giản, hãy lấy khách hàng bằng tên người dùng thông qua CustomerDAO.

            // Chúng ta cần thêm GetCustomerByUsername vào CustomerDAO hoặc chỉ cần đơn giản hóa việc đăng nhập trả về ID.

            // Giả sử chúng ta có thể lấy được nó.

            // Khắc phục nhanh: Thêm GetCustomerByUsername vào CustomerDAO.

            var customer = _customerDAO.Login(username, ""); // Tái sử dụng một cách thiếu chuyên nghiệp hay phương pháp mới
            // Phương thức Login yêu cầu mật khẩu. Trước tiên, thêm GetByUsername vào CustomerDAO.
            // Hoặc chỉ cần lưu CustomerId vào Session trong quá trình đăng nhập!
            
            // Dựa vào Session để xử lý logic sau này. Hiện tại, sử dụng phương pháp mới để lấy dữ liệu.
            var cust = _customerDAO.GetByUsername(username);
            
            try
            {
                _orderDAO.CreateOrder(cust.CustomerId, cart, receiverAddress, receiverPhone);
                // Xóa giỏ hàng
                HttpContext.Session.Remove("Cart");
                return RedirectToAction("OrderSuccess");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi xử lý đơn hàng: " + ex.Message + ". Vui lòng liên hệ quản trị viên.";
                // Giữ lại giỏ hàng để người dùng có thể thử lại.
                return View(cart);
            }
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
