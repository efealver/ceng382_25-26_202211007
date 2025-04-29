using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using razorpages.Models;
using System.Text.Json;

namespace razorpages.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInput Input { get; set; }

        public string ErrorMessage { get; set; }

        public class LoginInput
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public IActionResult OnGet()
        {
            // If already logged in, redirect
            if (HttpContext.Session.GetString("username") != null)
            {
                return RedirectToPage("/Index"); // or your table page
            }
            return Page();
        }

        public IActionResult OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var usersPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data/users.json");
            var json = System.IO.File.ReadAllText(usersPath);
            var users = JsonSerializer.Deserialize<List<User>>(json);

            var user = users.FirstOrDefault(u =>
                u.Username == Input.Username &&
                u.Password == Input.Password &&
                u.IsActive);

            if (user != null)
            {
                string token = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                Response.Cookies.Append("username", user.Username, cookieOptions);
                Response.Cookies.Append("token", token, cookieOptions);
                Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

                return RedirectToPage("/Index"); // redirect to main page after login
            }

            ErrorMessage = "Invalid username or password.";
            return Page();
        }
    }
}
