using MainAssignmentNet107.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainAssignmentNet107.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryMenuViewComponent(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _categoryDAO.GetAll();
            return View(categories);
        }
    }
}
