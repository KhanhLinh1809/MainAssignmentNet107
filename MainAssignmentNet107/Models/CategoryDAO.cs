using System.Data;
using MainAssignmentNet107.Helpers;
using Microsoft.Data.SqlClient;

namespace MainAssignmentNet107.Models
{
    public class CategoryDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public CategoryDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Category> GetAll()
        {
            List<Category> categories = new List<Category>();
            string query = "SELECT * FROM Categories";
            DataTable table = _dbHelper.ExecuteQuery(query);

            foreach (DataRow row in table.Rows)
            {
                categories.Add(new Category
                {
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString()
                });
            }

            return categories;
        }
    }
}
