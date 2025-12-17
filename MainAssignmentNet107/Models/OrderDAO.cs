using System.Data;
using MainAssignmentNet107.Helpers;
using Microsoft.Data.SqlClient;

namespace MainAssignmentNet107.Models
{
    public class OrderDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public OrderDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public void CreateOrder(int customerId, List<CartItem> items, string address, string phone)
        {
            decimal totalAmount = items.Sum(i => i.Total);

            // 1. Tạo bản ghi Giỏ hàng (Đơn hàng)
            string insertCartSql = @"
                INSERT INTO Carts (CustomerId, CreatedDate, Status, TotalAmount, ReceiverAddress, ReceiverPhone) 
                VALUES (@CustomerId, GETDATE(), 'Pending', @TotalAmount, @Address, @Phone);
                SELECT SCOPE_IDENTITY();";

            SqlParameter[] cartParams = {
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@Address", address),
                new SqlParameter("@Phone", phone)
            };

            int cartId = Convert.ToInt32(_dbHelper.ExecuteScalar(insertCartSql, cartParams));

            // 2. Tạo Chi tiết Giỏ hàng
            foreach (var item in items)
            {
                string insertDetailSql = "INSERT INTO CartDetails (CartId, ProductId, Quantity, Color, Size) VALUES (@CartId, @ProductId, @Quantity, @Color, @Size)";
                SqlParameter[] detailParams = {
                    new SqlParameter("@CartId", cartId),
                    new SqlParameter("@ProductId", item.ProductId),
                    new SqlParameter("@Quantity", item.Quantity),
                    new SqlParameter("@Color", item.Color ?? ""),
                    new SqlParameter("@Size", item.Size ?? "")
                };
                _dbHelper.ExecuteNonQuery(insertDetailSql, detailParams);
            }
        }
        public List<dynamic> GetAllOrders()
        {
            List<dynamic> list = new List<dynamic>();
            string query = @"
                SELECT c.CartId, c.CreatedDate, c.Status, c.TotalAmount, c.ReceiverAddress, c.ReceiverPhone, cus.FullName 
                FROM Carts c 
                LEFT JOIN Customers cus ON c.CustomerId = cus.CustomerId 
                ORDER BY c.CreatedDate DESC";
            
            DataTable table = _dbHelper.ExecuteQuery(query);
            foreach (DataRow row in table.Rows)
            {
                list.Add(new 
                {
                    CartId = Convert.ToInt32(row["CartId"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    Status = row["Status"].ToString(),
                    TotalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0,
                    ReceiverAddress = row["ReceiverAddress"].ToString(),
                    ReceiverPhone = row["ReceiverPhone"].ToString(),
                    CustomerName = row["FullName"].ToString()
                });
            }
            return list;
        }

        public dynamic GetOrderById(int cartId)
        {
            string query = @"
                SELECT c.CartId, c.CreatedDate, c.Status, c.TotalAmount, c.ReceiverAddress, c.ReceiverPhone, cus.FullName 
                FROM Carts c 
                LEFT JOIN Customers cus ON c.CustomerId = cus.CustomerId 
                WHERE c.CartId = @CartId";
            SqlParameter[] parameters = { new SqlParameter("@CartId", cartId) };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);
            
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                return new 
                {
                    CartId = Convert.ToInt32(row["CartId"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    Status = row["Status"].ToString(),
                    TotalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0,
                    ReceiverAddress = row["ReceiverAddress"].ToString(),
                    ReceiverPhone = row["ReceiverPhone"].ToString(),
                    CustomerName = row["FullName"].ToString()
                };
            }
            return null;
        }

        public List<CartItem> GetOrderDetails(int cartId)
        {
            List<CartItem> list = new List<CartItem>();
            string query = @"
                SELECT cd.Quantity, cd.Color, cd.Size, p.Name, p.Price, p.Image, p.ProductId
                FROM CartDetails cd
                JOIN Products p ON cd.ProductId = p.ProductId
                WHERE cd.CartId = @CartId";
            
            SqlParameter[] parameters = { new SqlParameter("@CartId", cartId) };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                list.Add(new CartItem
                {
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    ProductName = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    Image = row["Image"].ToString(),
                    Color = row.Table.Columns.Contains("Color") ? row["Color"].ToString() : "",
                    Size = row.Table.Columns.Contains("Size") ? row["Size"].ToString() : ""
                });
            }
            return list;
        }

        public void UpdateStatus(int cartId, string status)
        {
            string query = "UPDATE Carts SET Status = @Status WHERE CartId = @CartId";
            SqlParameter[] parameters = { 
                new SqlParameter("@Status", status),
                new SqlParameter("@CartId", cartId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        // --- Phương pháp thống kê ---

        public List<dynamic> GetRevenueStatistics()
        {
            // 1. Lấy dữ liệu thô từ cơ sở dữ liệu
            var rawData = new Dictionary<string, decimal>();
            string query = @"
                SELECT CAST(CreatedDate AS DATE) as Date, SUM(TotalAmount) as Revenue
                FROM Carts
                WHERE Status = 'Approved' 
                AND CreatedDate >= DATEADD(day, -7, GETDATE())
                GROUP BY CAST(CreatedDate AS DATE)";
            
            DataTable table = _dbHelper.ExecuteQuery(query);
            foreach (DataRow row in table.Rows)
            {
                string dateKey = Convert.ToDateTime(row["Date"]).ToString("dd/MM");
                decimal revenue = Convert.ToDecimal(row["Revenue"]);
                if (rawData.ContainsKey(dateKey))
                    rawData[dateKey] += revenue;
                else
                    rawData.Add(dateKey, revenue);
            }

            // 2. Tạo danh sách đầy đủ 7 ngày gần đây
            List<dynamic> list = new List<dynamic>();
            for (int i = 0; i < 7; i++)
            {
                DateTime date = DateTime.Now.AddDays(-i);
                string dateString = date.ToString("dd/MM");
                decimal dailyRevenue = rawData.ContainsKey(dateString) ? rawData[dateString] : 0;
                
                list.Add(new { Date = dateString, Revenue = dailyRevenue });
            }
            
            return list;
        }

        public List<dynamic> GetTopSellingProducts(int topN)
        {
            List<dynamic> list = new List<dynamic>();
            string query = @"
                SELECT TOP (@TopN) p.Name, SUM(cd.Quantity) as TotalSold, p.Image
                FROM CartDetails cd
                JOIN Carts c ON cd.CartId = c.CartId
                JOIN Products p ON cd.ProductId = p.ProductId
                WHERE c.Status = 'Approved'
                GROUP BY p.Name, p.Image
                ORDER BY TotalSold DESC";

            SqlParameter[] parameters = { new SqlParameter("@TopN", topN) };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                list.Add(new { 
                    Name = row["Name"].ToString(), 
                    TotalSold = Convert.ToInt32(row["TotalSold"]),
                    Image = row["Image"].ToString()
                });
            }
            return list;
        }

        public dynamic GetOrderSummary()
        {
            string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM Carts) as TotalOrders,
                    (SELECT COUNT(*) FROM Carts WHERE Status = 'Pending') as PendingOrders,
                    (SELECT ISNULL(SUM(TotalAmount), 0) FROM Carts WHERE Status = 'Approved') as TotalRevenue,
                    (SELECT COUNT(*) FROM Customers) as TotalCustomers";
            
            DataTable table = _dbHelper.ExecuteQuery(query);
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                return new {
                    TotalOrders = Convert.ToInt32(row["TotalOrders"]),
                    PendingOrders = Convert.ToInt32(row["PendingOrders"]),
                    TotalRevenue = Convert.ToDecimal(row["TotalRevenue"]),
                    TotalCustomers = Convert.ToInt32(row["TotalCustomers"])
                };
            }
            return null;
        }
    }
}
