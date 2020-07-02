using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CovidProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace CovidProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var vm = new HomeViewModel();
            var countries = GetCountriesAsync();
            vm.listItems = countries.Result;
            return View("Index",vm);
        }

        public async Task<List<SelectListItem>> GetCountriesAsync()
        {
            var countries = new List<SelectListItem>();
            string url = "https://api.covid19api.com/";
            List<CountryListModel> test = new List<CountryListModel>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(url);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                var Res =  client.GetAsync("countries");
                var result = Res.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readString = result.Content.ReadAsStringAsync().Result;

                    countries  = JsonConvert.DeserializeObject<List<CountryListModel>>(readString).Select(cn => new SelectListItem
                    {
                        Text = cn.Country,
                        Value = cn.Slug


                    }).OrderBy(cn => cn.Text).ToList();


                    return countries;

                }
                else
                {
                    return new List<SelectListItem>();
                }
            }
            
        }

        //public List<String> GetStates()
        //{
        //    var states = "Alabama,Alaska,Arizona,Arkansas,California,Colorado,Connecticut,Delaware,Florida,Georgia,Hawaii,Idaho,Illinois,Indiana,Iowa,Kansas,Kentucky,Louisiana,Maine,Maryland,Massachusetts,Michigan,Minnesota,Mississippi,Missouri,Montana,Nebraska,Nevada,New Hampshire,New Jersey,New Mexico,New York,North Carolina,North Dakota,Ohio,Oklahoma,Oregon,Pennsylvania,Rhode Island,South Carolina,South Dakota,Tennessee,Texas,Utah,Vermont,Virginia,Washington,West Virginia,Wisconsin,Wyoming";
        //    return states.Split(",").ToList();
            
        //}

        [HttpGet]
        public ActionResult Chart(string country)
        {
            var labels = new List<string>();
            var totals = new List<int>();

            string url = "https://api.covid19api.com/";
            List<CountryListModel> test = new List<CountryListModel>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(url);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                var Res = client.GetAsync("country/" + country);
                var result = Res.Result;
                Res.Wait();
                var rawData = new List<DataPointViewModel>();
                if (result.IsSuccessStatusCode)
                {
                    var readString = result.Content.ReadAsStringAsync().Result;

                    rawData = JsonConvert.DeserializeObject<List<DataPointViewModel>>(readString).Where(x => x.Active != 0).ToList();
                    var test2  = rawData.GroupBy(g => g.Date)
                    .Select(
                        g => new DataPointViewModel
                        {
                            Active = g.Sum(s => s.Active),
                            Date = g.First().Date
                        }).ToList();
                    foreach (var point in test2)
                    {
                        totals.Add(point.Active);
                        labels.Add(DateTime.Parse(point.Date).ToShortDateString());
                    }


                    return Json(new { totals = totals, labels = labels, message = "success"});
                }
                else
                {
                    return Json(new { message = "error" });

                }

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
