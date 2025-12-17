using System.Data;
using MainAssignmentNet107.Helpers;
using Microsoft.Data.SqlClient;

namespace MainAssignmentNet107.Models
{
    public class ProductDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public ProductDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Product> GetAll()
        {
            List<Product> products = new List<Product>();
            string query = "SELECT p.*, c.Name as CategoryName FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.CategoryId";
            DataTable table = _dbHelper.ExecuteQuery(query);

            foreach (DataRow row in table.Rows)
            {
                products.Add(MapDataRowToProduct(row));
            }

            return products;
        }
        
        public List<Product> GetTopProducts(int count)
        {
            List<Product> products = new List<Product>();
            string query = $"SELECT TOP {count} p.*, c.Name as CategoryName FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.CategoryId ORDER BY p.ProductId DESC";
            DataTable table = _dbHelper.ExecuteQuery(query);

            foreach (DataRow row in table.Rows)
            {
                products.Add(MapDataRowToProduct(row));
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            string query = "SELECT p.*, c.Name as CategoryName FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.CategoryId WHERE p.ProductId = @ProductId";
            SqlParameter[] parameters = { new SqlParameter("@ProductId", id) };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);

            if (table.Rows.Count > 0)
            {
                return MapDataRowToProduct(table.Rows[0]);
            }
            return null;
        }

        public List<Product> SearchProducts(string keyword)
        {
            List<Product> products = new List<Product>();
            string query = "SELECT p.*, c.Name as CategoryName FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.CategoryId WHERE p.Name LIKE @Keyword";
            SqlParameter[] parameters = { new SqlParameter("@Keyword", "%" + keyword + "%") };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                products.Add(MapDataRowToProduct(row));
            }

            return products;
        }

        public List<Product> GetProductsByCategoryId(int categoryId)
        {
            List<Product> products = new List<Product>();
            string query = "SELECT p.*, c.Name as CategoryName FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.CategoryId WHERE p.CategoryId = @CategoryId";
            SqlParameter[] parameters = { new SqlParameter("@CategoryId", categoryId) };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                products.Add(MapDataRowToProduct(row));
            }

            return products;
        }

        private Product MapDataRowToProduct(DataRow row)
        {
            return new Product
            {
                ProductId = Convert.ToInt32(row["ProductId"]),
                Name = row["Name"].ToString(),
                Price = Convert.ToDecimal(row["Price"]),
                Image = row["Image"].ToString(),
                Description = row["Description"].ToString(),
                Color = row["Color"].ToString(),
                Size = row["Size"].ToString(),
                CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : 0,
                CategoryName = row.Table.Columns.Contains("CategoryName") && row["CategoryName"] != DBNull.Value ? row["CategoryName"].ToString() : ""
            };
        }
        public void Add(Product product)
        {
            string query = "INSERT INTO Products (Name, Price, Image, Description, Color, Size, CategoryId) VALUES (@Name, @Price, @Image, @Description, @Color, @Size, @CategoryId)";
            SqlParameter[] parameters = {
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@Image", product.Image ?? ""), 
                new SqlParameter("@Description", product.Description ?? ""),
                new SqlParameter("@Color", product.Color ?? ""),
                new SqlParameter("@Size", product.Size ?? ""),
                new SqlParameter("@CategoryId", product.CategoryId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void Update(Product product)
        {
            string query = "UPDATE Products SET Name=@Name, Price=@Price, Image=@Image, Description=@Description, Color=@Color, Size=@Size, CategoryId=@CategoryId WHERE ProductId=@ProductId";
            SqlParameter[] parameters = {
                new SqlParameter("@ProductId", product.ProductId),
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@Image", product.Image ?? ""),
                new SqlParameter("@Description", product.Description ?? ""),
                new SqlParameter("@Color", product.Color ?? ""),
                new SqlParameter("@Size", product.Size ?? ""),
                new SqlParameter("@CategoryId", product.CategoryId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void Delete(int id)
        {
            try
            {
                string query = "DELETE FROM Products WHERE ProductId = @Id";
                SqlParameter[] parameters = { new SqlParameter("@Id", id) };
                _dbHelper.ExecuteNonQuery(query, parameters);
            }
            catch
            {
                throw new Exception("Không thể xóa sản phẩm này vì đã có dữ liệu trong đơn hàng.");
            }
        }
        public void BulkUpdateVariants()
        {
            string query = "UPDATE Products SET Color = N'Trắng, Đen, Đỏ, Xanh Dương, Xám', Size = '38, 39, 40, 41, 42' WHERE Color IS NULL OR Color = '' OR Size IS NULL OR Size = ''";
            string queryAll = "UPDATE Products SET Color = N'Trắng, Đen, Đỏ, Xanh Dương, Xám', Size = '38, 39, 40, 41, 42'";
            _dbHelper.ExecuteNonQuery(queryAll);
        }
    }
}
