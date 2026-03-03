//
// NuGet packages:
// - Newtonsoft.Json
//
using CarAPI.Models;
using CarUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string? CarSelectedLicenceplate { get; set; }

        [BindProperty]
        public Car CarSelected { get; set; }

        [BindProperty]
        public List<Car> Cars { get; set; }



        private void SetMyCookie(APIAction myAction)
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddMinutes(5);
            cookieOptions.Path = "/";
            Response.Cookies.Append("CarAPICookie", JsonConvert.SerializeObject(myAction), cookieOptions);
        }


        private APIAction GetMyCookie()
        {
            string? jsonString;
            try
            {
                return JsonConvert.DeserializeObject<APIAction>(Request.Cookies["CarAPICookie"]);
            }
            catch
            {
                return null;
            }
        }



        public string APIURI = "http://localhost:8888";
        public void OnGet()
        {
            APIAction myAction = GetMyCookie();
            if (myAction != null)
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(APIURI);
                switch (myAction.action)
                {
                    case "findCar":
                        CarSelectedLicenceplate = myAction.target;
                        // Now use API
                        string result1 = httpClient.GetStringAsync("/apiV4/Cars/" + myAction.target).Result;
                        if (result1 != null)
                        {
                            // we have a result
                            CarSelected = JsonConvert.DeserializeObject<Car>(result1);
                        }
                        else
                        {
                            CarSelected = null;
                        }
                        break;
                    case "findCarByColor":
                        string result2 = httpClient.GetStringAsync("/apiV4/Cars?color=" + myAction.target).Result;
                        if (result2 != null)
                        {
                            // we have a result
                            Cars = JsonConvert.DeserializeObject<List<Car>>(result2);
                        }
                        else
                        {
                            Cars = null;
                        }

                        break;
                    default:
                        CarSelectedLicenceplate = null;
                        break;
                }
 
            }
        }

        public IActionResult OnPost(string action = "NA", string target = "NA")
        {
            APIAction myAction = new()
            {
                action = action,
                target = target
            };
            SetMyCookie(myAction);
            return Redirect("/");
        }

        public IActionResult OnPostAjax1(string data)
        {
            return new JsonResult(new { result = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt") });
        }
    }
}
