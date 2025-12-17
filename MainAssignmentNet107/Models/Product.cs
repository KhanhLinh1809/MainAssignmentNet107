namespace MainAssignmentNet107.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int CategoryId { get; set; }

        // Thuộc tính điều hướng nếu cần, nhưng giữ cho nó đơn giản cho ADO.
        public string? CategoryName { get; set; } 
    }
}
