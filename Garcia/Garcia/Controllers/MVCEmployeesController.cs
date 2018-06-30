using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Garcia.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Garcia.Controllers
{
    public class MVCEmployeesController : Controller
    {
        string tokenAdress = "http://localhost:54754/token";
        string apiBaseAddress = "http://localhost:54754/api/";

        string userId = "63187171E3E548F0ABB846C7197D07BA";
        string password = "B8A1634";

        string GetToken(string userName, string password)
        {

            var pairs = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>( "grant_type", "password" ),
                            new KeyValuePair<string, string>( "userName", userName ),
                            new KeyValuePair<string, string> ( "password", password )
                        };
            var content = new FormUrlEncodedContent(pairs);
            using (var client = new HttpClient())
            {
                var response =
                    client.PostAsync(tokenAdress, content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MVCEmployees
        public ActionResult Index()
        {
            //return View(db.Employees.ToList());
            IEnumerable<Employee> employees = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                //HTTP GET
                var responseTask = client.GetAsync("Employees");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Employee>>();
                    readTask.Wait();

                    employees = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    employees = Enumerable.Empty<Employee>();

                    ModelState.AddModelError(string.Empty, "Web Api error. Please contact administrator.");
                }
            }
            return View(employees);

        }

        // GET: MVCEmployees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Employee emp = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                //HTTP GET
                var responseTask = client.GetAsync("Employees/"+id);
                responseTask.Wait();
                
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Employee>();
                    readTask.Wait();

                    emp = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    ModelState.AddModelError(string.Empty, "Web Api error. Please contact administrator.");
                }
            }

            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }

        // GET: MVCEmployees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MVCEmployees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Surname")] Employee employee)
        {                                   
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Employee>("Employees",employee);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        ModelState.AddModelError(string.Empty, "Web Api error. Please contact administrator.");
                    }
                }
            }

            return View(employee);
        }

        // GET: MVCEmployees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Employee emp = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                //HTTP GET
                var responseTask = client.GetAsync("Employees/" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Employee>();
                    readTask.Wait();

                    emp = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    ModelState.AddModelError(string.Empty, "Web Api error. Please contact administrator.");
                }
            }

            if (emp == null)
            {
                return HttpNotFound();
            }

            return View(emp);
        }

        // POST: MVCEmployees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname")] Employee employee)
        {
            var token = GetToken(userId, password);
            var bearerToken = JsonConvert.DeserializeObject<Token>(token).access_token;

            if (ModelState.IsValid)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
                    //HTTP PUT
                    var postTask = client.PutAsJsonAsync<Employee>("Employees/"+employee.Id, employee);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        ModelState.AddModelError(string.Empty, "Web Api error. Please contact administrator.");
                    }
                }

                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: MVCEmployees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        [JsonProperty(".issued")]
        public string issued { get; set; }
        [JsonProperty(".expires")]
        public string expires { get; set; }
    }
}
