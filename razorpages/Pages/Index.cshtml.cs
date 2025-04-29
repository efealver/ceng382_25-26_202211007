using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using razorpages.Models;
using razorpages.Helpers;

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

        // Filtering input from query
        [BindProperty(SupportsGet = true)]
        public string? FilterClassName { get; set; }

        // Pagination support
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        private const int PageSize = 10;

        // All columns available for filtering
        public List<string> AllColumns { get; set; } = new List<string> { "ClassName", "StudentCount", "Description" };

        // Selected columns for export and view
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedColumns { get; set; } = new();

        // Filtered and paginated view model
        public List<ClassInformationTable> TableData { get; set; } = new();

        public IActionResult  OnGet(int? editId, string? toggleColumn)
        {
            ClassInformationModel.GenerateFakeData();
          if (SelectedColumns == null || !SelectedColumns.Any())
            {
                SelectedColumns = AllColumns.ToList(); // Default to all columns if none are selected
            }

           // Handle column toggle
            if (!string.IsNullOrEmpty(toggleColumn))
            {
                if (SelectedColumns.Contains(toggleColumn))
                    SelectedColumns.Remove(toggleColumn);
                else
                    SelectedColumns.Add(toggleColumn);
            }
               // Ensure SelectedColumns is initialized
      


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

            // Filtering and pagination logic
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

            // Get values from session
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            // Get values from cookies
            var cookieUsername = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];
            var cookieSessionId = Request.Cookies["session_id"];

            // If any are missing or mismatched, redirect to login page
            if (sessionUsername == null || sessionToken == null || sessionId == null ||
                cookieUsername != sessionUsername ||
                cookieToken != sessionToken ||
                cookieSessionId != sessionId)
            {
                return RedirectToPage("/Login");
            }

            // Otherwise, continue loading the page
            return Page();
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

        public IActionResult OnPostExport(string exportMode)
        {
            var query = ClassInformationModel.ClassList.AsQueryable();

    // Apply filtering
            if (!string.IsNullOrWhiteSpace(FilterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterClassName, System.StringComparison.OrdinalIgnoreCase));
            }

            // Apply pagination ONLY if exporting 'unfiltered' (which means "visible screen data")
            if (exportMode == "unfiltered")
            {
                query = query
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize);
            }
            var data = query.ToList(); // Use the query directly, as it already contains the correct data
            var exportData = data.Select(item => {
                var obj = new Dictionary<string, object>();
                if (SelectedColumns == null || SelectedColumns.Count == 0 || SelectedColumns.Contains("ClassName"))
                    obj["ClassName"] = item.ClassName;
                if (SelectedColumns == null || SelectedColumns.Count == 0 || SelectedColumns.Contains("StudentCount"))
                    obj["StudentCount"] = item.StudentCount;
                if (SelectedColumns == null || SelectedColumns.Count == 0 || SelectedColumns.Contains("Description"))
                    obj["Description"] = item.Description;
                if (SelectedColumns == null || SelectedColumns.Count == 0 || SelectedColumns.Contains("Id"))
                    obj["Id"] = item.Id;
                return obj;
            }).ToList();

            var json = Utils.Instance.ExportToJson(exportData);
            var bytes = Utils.Instance.ExportToJsonBytes(exportData);
            return File(bytes, "application/json", "export.json");
        }
        public IActionResult OnPostLogout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Delete cookies
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            return RedirectToPage("/Login");
        }

        /*public IActionResult OnGet()
        {
            // Get values from session
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            // Get values from cookies
            var cookieUsername = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];
            var cookieSessionId = Request.Cookies["session_id"];

            // If any are missing or mismatched, redirect to login page
            if (sessionUsername == null || sessionToken == null || sessionId == null ||
                cookieUsername != sessionUsername ||
                cookieToken != sessionToken ||
                cookieSessionId != sessionId)
            {
                return RedirectToPage("/Login");
            }

            // Otherwise, continue loading the page
            return Page();
        }*/

    }
}
