using System.Linq;
using System.Web.Http;
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
            var customers = db.Customers
                              .Where(c => c.Country == country)
                              .Select(c => new
                              {
                                  c.CustomerID,
                                  c.CompanyName,
                                  c.ContactName,
                                  c.Country,
                                  c.City,
                                  c.Phone
                              }).ToList();

            if (customers == null || customers.Count == 0)
                return NotFound();

            return Ok(customers);
        }
    }
}
