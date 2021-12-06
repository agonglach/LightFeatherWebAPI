using LightFeatherWebAPI.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LightFeatherWebAPI.Controllers
{
    [ApiController]
    public class TestApiController : ControllerBase
    {
        private List<Supervisor> _supervisors = null;
        private List<string> _formattedSupervisors = null;
        private string _invalidParamsMessage = "Invalid parameters. First Name, Last Name, and Supervisor are all required parameters";
        private string _badJSONMessage = "Request body could not be parsed. Check parameter formatting.";


        [Route("api/supervisors")]
        public async Task<IActionResult> Get()
        {
            _supervisors = new List<Supervisor>();
            _formattedSupervisors = new List<string>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://o3m5qixdng.execute-api.us-east-1.amazonaws.com/api/managers"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        _supervisors = JsonConvert.DeserializeObject<List<Supervisor>>(apiResponse);
                    }
                    catch (JsonReaderException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return BadRequest(_badJSONMessage);
                    }
                    catch (JsonSerializationException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return BadRequest(_badJSONMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return BadRequest("Invalid Request");
                    }
                }
            }
            if (_supervisors == null || _supervisors.Count == 0)
            {
                return BadRequest("The request returned 0 results");
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
        }
        private void FormatSupervisors()
        {
            foreach (Supervisor supervisor in _supervisors)
            {
                _formattedSupervisors.Add($"{supervisor.Jurisdiction} - {supervisor.LastName}, {supervisor.FirstName}");
            }
        }

        [Route("api/submit")]
        public async Task<IActionResult> Post()
        {
            Subordinate subordinate = null;

            using (var reader = new StreamReader(Request.Body))
            {
                string body = await reader.ReadToEndAsync();
                try
                {
                    subordinate = JsonConvert.DeserializeObject<Subordinate>(body);
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine(ex.Message);
                    return BadRequest(_badJSONMessage);
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine(ex.Message);
                    return BadRequest(_badJSONMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return BadRequest("Invalid Request");
                }
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
                return BadRequest(_invalidParamsMessage);
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
