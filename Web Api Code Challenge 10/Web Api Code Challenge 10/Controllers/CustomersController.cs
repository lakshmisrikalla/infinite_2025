using System.Web.Http;
using System.Linq;
using Web_Api_Code_Challenge_10.Models;

namespace Web_Api_Code_Challenge_10.Controllers
{
    [RoutePrefix("api/Customers")]
    public class CustomersController : ApiController
    {
        NorthWindEntities db = new NorthWindEntities();

        [HttpGet]
        [Route("ByCountry")]
        public IHttpActionResult GetCustomersByCountry(string country)
        {
            var result = db.GetCustomersByCountrys(country).ToList();

            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }
    }
}

