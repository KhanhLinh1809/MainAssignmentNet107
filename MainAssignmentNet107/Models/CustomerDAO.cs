using System.Data;
using MainAssignmentNet107.Helpers;
using Microsoft.Data.SqlClient;

namespace MainAssignmentNet107.Models
{
    public class CustomerDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public CustomerDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public Customer GetByUsername(string username)
        {
            string query = "SELECT * FROM Customers WHERE Username = @Username";
            SqlParameter[] parameters = { new SqlParameter("@Username", username) };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                return new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    Username = row["Username"].ToString(),
                    Password = row["Password"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    Role = row["Role"].ToString()
                };
            }
            return null;
        }

        public Customer Login(string username, string password)
        {
            // Lưu ý: Trong ứng dụng thực tế, sử dụng hàm băm. Ở đây, so sánh văn bản thuần túy theo yêu cầu hoặc để đơn giản hóa.
            string query = "SELECT * FROM Customers WHERE Username = @Username AND Password = @Password";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                return new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    Username = row["Username"].ToString(),
                    Password = row["Password"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    Role = row["Role"].ToString()
                };
            }
            return null;
        }

        public bool CheckUsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM Customers WHERE Username = @Username";
            SqlParameter[] parameters = { new SqlParameter("@Username", username) };
            object result = _dbHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public void Register(Customer customer)
        {
            string query = "INSERT INTO Customers (Username, Password, FullName, Email, Role) VALUES (@Username, @Password, @FullName, @Email, @Role)";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", customer.Username),
                new SqlParameter("@Password", customer.Password),
                new SqlParameter("@FullName", customer.FullName),
                new SqlParameter("@Email", customer.Email ?? (object)DBNull.Value),
                new SqlParameter("@Role", "Customer") // Vai trò mặc định
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void UpdateProfile(Customer customer)
        {
            string query = "UPDATE Customers SET FullName = @FullName, Email = @Email WHERE Username = @Username";
            SqlParameter[] parameters = {
                new SqlParameter("@FullName", customer.FullName),
                new SqlParameter("@Email", customer.Email ?? (object)DBNull.Value),
                new SqlParameter("@Username", customer.Username)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void ChangePassword(string username, string newPassword)
        {
            string query = "UPDATE Customers SET Password = @Password WHERE Username = @Username";
            SqlParameter[] parameters = {
                new SqlParameter("@Password", newPassword),
                new SqlParameter("@Username", username)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }
        public List<Customer> GetAllCustomers()
        {
            List<Customer> list = new List<Customer>();
            string query = "SELECT * FROM Customers";
            DataTable table = _dbHelper.ExecuteQuery(query);
            foreach (DataRow row in table.Rows)
            {
                list.Add(new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    Username = row["Username"].ToString(),
                    Password = row["Password"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    Role = row["Role"].ToString()
                });
            }
            return list;
        }

        public Customer GetById(int id)
        {
            string query = "SELECT * FROM Customers WHERE CustomerId = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            DataTable table = _dbHelper.ExecuteQuery(query, parameters);
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                return new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    Username = row["Username"].ToString(),
                    Password = row["Password"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    Role = row["Role"].ToString()
                };
            }
            return null;
        }

        public void Create(Customer customer)
        {
             string query = "INSERT INTO Customers (Username, Password, FullName, Email, Role) VALUES (@Username, @Password, @FullName, @Email, @Role)";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", customer.Username),
                new SqlParameter("@Password", customer.Password),
                new SqlParameter("@FullName", customer.FullName),
                new SqlParameter("@Email", customer.Email ?? (object)DBNull.Value),
                new SqlParameter("@Role", customer.Role ?? "Customer")
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void Update(Customer customer)
        {
            // Admin update (can change Role)
            string query = "UPDATE Customers SET FullName = @FullName, Email = @Email, Role = @Role WHERE CustomerId = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@FullName", customer.FullName),
                new SqlParameter("@Email", customer.Email ?? (object)DBNull.Value),
                new SqlParameter("@Role", customer.Role),
                new SqlParameter("@Id", customer.CustomerId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);

            if (!string.IsNullOrEmpty(customer.Password))
            {
                 string queryPass = "UPDATE Customers SET Password = @Password WHERE CustomerId = @Id";
                 SqlParameter[] pPass = {
                    new SqlParameter("@Password", customer.Password),
                    new SqlParameter("@Id", customer.CustomerId)
                };
                _dbHelper.ExecuteNonQuery(queryPass, pPass);
            }
        }

        public void Delete(int id)
        {
            try 
            {
                string query = "DELETE FROM Customers WHERE CustomerId = @Id";
                SqlParameter[] parameters = { new SqlParameter("@Id", id) };
                _dbHelper.ExecuteNonQuery(query, parameters);
            }
            catch
            {
                throw new Exception("Không thể xóa user này (có thể do ràng buộc khóa ngoại với đơn hàng).");
            }
        }
    }
}
