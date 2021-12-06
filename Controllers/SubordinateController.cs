using LightFeatherWebAPI.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LightFeatherWebAPI.Controllers
{
    [ApiController]
    public class SubordinateController : ControllerBase
    {
        private List<Supervisor> _supervisors = new List<Supervisor>();
        private List<string> _formattedSupervisors = new List<string>();

        //private string _testPostData = "firstName=Mark&lastName=Johnson&phoneNumber=770-965-8858&email=test@test.com&supervisor={FirstName: Liam, LastName: Stevens, Jurisdiction: c}";
        //private string _testURI = "https://localhost:7214/api/submit";

        [Route("api/supervisors")]
        public async Task<IActionResult> Get()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://o3m5qixdng.execute-api.us-east-1.amazonaws.com/api/managers"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        _supervisors = JsonConvert.DeserializeObject<List<Supervisor>>(apiResponse);
                    }
                    catch (JsonSerializationException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return BadRequest(ex.Message);
                    }
                    //PrintSupervisors();
                }
            }
            SortSupervisors();
            FormatSupervisors();
            return Ok(_formattedSupervisors);
        }

        private void SortSupervisors()
        {
            //cull numeric jurisdictions from the list
            //not sure what value types the nums are, so we check for a floating point to cover all cases
            double attemptedValue;
            _supervisors.RemoveAll(s => double.TryParse(s.Jurisdiction, out attemptedValue) == true);
            //sort by jurisdiction, lastname, then firstname
            _supervisors = _supervisors.OrderBy(s => s.Jurisdiction).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            //PrintSupervisors();
        }

        [Route("api/submit")]
        public async Task<IActionResult> Post()
        {
            string body;
            Subordinate subordinate = null;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            try
            {
                subordinate = JsonConvert.DeserializeObject<Subordinate>(body);
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
            if (subordinate != null && subordinate.Supervisor != null)
            {
                Console.WriteLine(subordinate.ToString());
            }

            if (CheckForValidParameters(subordinate))
            {
                return Ok();
            }
            else
            {
                return BadRequest("Invalid parameters. First Name, Last Name, and Supervisor details are all required parameters");
            }
        }
        //[Route("api/test")]
        //public async Task<IActionResult> PostTest()
        //{
        //    Supervisor supervisor = new Supervisor();
        //    string testStringSupervisor = JsonConvert.SerializeObject(supervisor);

        //    var values = new Dictionary<string, string>
        //    {
        //        { "firstName", "Tom" },
        //        { "lastName", "world" },
        //        { "email", "Tom" },
        //        { "phoneNumber", "world" },
        //        { "supervisor", testStringSupervisor }
        //    };

        //    var content = new FormUrlEncodedContent(values);


        //    using (var httpClient = new HttpClient())
        //    {
        //        var response = await httpClient.PostAsync("https://localhost:7214/api/submit", content);
        //        var responseString = await response.Content.ReadAsStringAsync();
        //    }
        //    return Ok();
        //}

        //[Route("api/submit")]
        //public IActionResult Post()
        //{
        //    return Ok();
        //}

        //private void PrintSupervisors()
        //{
        //    foreach (Supervisor supervisor in _supervisors)
        //    {
        //        Console.WriteLine($"{supervisor.Jurisdiction} - {supervisor.LastName}, {supervisor.FirstName}");
        //    }
        //}

        private void FormatSupervisors()
        {
            foreach (Supervisor supervisor in _supervisors)
            {
                _formattedSupervisors.Add($"{supervisor.Jurisdiction} - {supervisor.LastName}, {supervisor.FirstName}");
            }
        }

        private bool CheckForValidParameters(Subordinate subordinate)
        {
            if (subordinate == null)
            {
                return false;
            }
            else if (subordinate.FirstName == null || subordinate.LastName == null || subordinate.Supervisor == null)
            {
                return false;
            }
            else if (subordinate.FirstName == String.Empty || subordinate.LastName == String.Empty)
            {
                return false;
            }

            return true;

        }
    }
}
