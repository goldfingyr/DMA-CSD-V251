//
// NuGet packages:
// - Newtonsoft.Json
//
using CarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using System.Drawing;

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

        private void SetMyCookie(MyCookieClass myCookie)
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddMinutes(5);
            cookieOptions.Path = "/";
            Response.Cookies.Append("CarAPICookie", JsonConvert.SerializeObject(myCookie), cookieOptions);
        }


        private MyCookieClass GetMyCookie()
        {
            string? jsonString;
            try
            {
                return JsonConvert.DeserializeObject<MyCookieClass>(Request.Cookies["CarAPICookie"]);
            }
            catch
            {
                return null;
            }
            //if (jsonString != null)
            //{
            //    CarSelected = JsonConvert.DeserializeObject<string>(jsonString);
            //    return;
            //}
            //else
            //{
            //    CarSelected = null;
            //}
        }



        public string APIURI = "http://localhost:8888";
        public void OnGet()
        {
            MyCookieClass theCookie = GetMyCookie();
            if (theCookie == null)
            {
                CarSelected = null;
                return;
            }
            if (theCookie.theAction == "findCar")
            {
                // Now use API
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(APIURI);
                string result = httpClient.GetStringAsync("/apiV4/Cars/" + theCookie.licenseplate).Result;
                if (result != null)
                {
                    // we have a result
                    CarSelected = JsonConvert.DeserializeObject<Car>(result);
                }
                else
                {
                    CarSelected = null;
                }

            }
            if (theCookie.theAction == "findCarAttributes")
            {
                // Now use API
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(APIURI);
                string apiReq= $"{APIURI}/apiV4/Cars?model={theCookie.model}&make={theCookie.make}&color={theCookie.color}";
                Cars = JsonConvert.DeserializeObject<List<Car>>(httpClient.GetStringAsync( apiReq ).Result).ToList();
            }
        }

        public IActionResult OnPost(string action = "NA", string target = "NA", string make = "%", string model = "%", string color = "%")
        {
            switch (action)
            {
                case "findCarAttributes":
                    SetMyCookie(new()
                    {
                        theAction = action,
                        licenseplate = target,
                        make = (make.IsNullOrEmpty()) ? "%" : make,
                        model = (model.IsNullOrEmpty() ) ? "%" : model,
                        color = (color.IsNullOrEmpty()) ? "%" : color
                    });
                    return Redirect("/");
                    break;
                case "findCar":
                    SetMyCookie(new() { 
                    theAction= action,
                    licenseplate= target,
                    make= make,
                    model= model,
                    color= color
                    });
                    return Redirect("/");
                    break;
                default:
                    break;
            }
            return null;
        }

        public IActionResult OnPostAjax1(string data)
        {
            return new JsonResult(new { result = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt") });
        }
    }

    public class MyCookieClass : Car
    {
               public string theAction { get; set; }
       
    }
}
