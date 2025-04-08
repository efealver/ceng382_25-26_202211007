using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using razorpages.Models;

namespace razorpages.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int? EditId { get; set; }

        [BindProperty]
        [Required]
        public string ClassName { get; set; }

        [BindProperty]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Student count must be at least 1")]
        public int StudentCount { get; set; }

        [BindProperty]
        public string Description { get; set; }

        //  Filtering input from query
        [BindProperty(SupportsGet = true)]
        public string? FilterClassName { get; set; }

        //  Pagination support
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        private const int PageSize = 10;

        // Filtered and paginated view model
        public List<ClassInformationTable> TableData { get; set; } = new();

        public void OnGet(int? editId)
        {
            ClassInformationModel.GenerateFakeData();

            // Pre-fill form for editing
            if (editId.HasValue)
            {
                var classInfo = ClassInformationModel.GetClassById(editId.Value);
                if (classInfo != null)
                {
                    EditId = classInfo.Id;
                    ClassName = classInfo.ClassName;
                    StudentCount = classInfo.StudentCount;
                    Description = classInfo.Description;
                }
            }

            //  Filtering and pagination logic
            var query = ClassInformationModel.ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FilterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterClassName, System.StringComparison.OrdinalIgnoreCase));
            }

            TotalPages = (int)System.Math.Ceiling(query.Count() / (double)PageSize);

            var paged = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            TableData = paged.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (EditId.HasValue)
            {
                ClassInformationModel.UpdateClass(EditId.Value, ClassName, StudentCount, Description);
            }
            else
            {
                ClassInformationModel.AddClass(ClassName, StudentCount, Description);
            }

            return RedirectToPage(new { FilterClassName, PageNumber });
        }

        public IActionResult OnPostDelete(int id)
        {
            ClassInformationModel.DeleteClass(id);
            return RedirectToPage(new { FilterClassName, PageNumber });
        }
    }
}
