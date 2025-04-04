using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YourProjectName.Models;

namespace YourProjectName.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int? EditId { get; set; } // Stores the Id of the class being edited

        [BindProperty]
        [Required]
        public string ClassName { get; set; }

        [BindProperty]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Student count must be at least 1")]
        public int StudentCount { get; set; }

        [BindProperty]
        public string Description { get; set; }

        public List<ClassInformationModel> ClassList => ClassInformationModel.ClassList;

        public void OnGet(int? editId)
        {
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
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (EditId.HasValue)
            {
                // Update existing class
                ClassInformationModel.UpdateClass(EditId.Value, ClassName, StudentCount, Description);
            }
            else
            {
                // Add new class
                ClassInformationModel.AddClass(ClassName, StudentCount, Description);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            ClassInformationModel.DeleteClass(id);
            return RedirectToPage();
        }
    }
}
