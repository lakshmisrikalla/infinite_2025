using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Web_Api_Code_Challenge_10.Models;

namespace Web_Api_Code_Challenge_10.Controllers
{
    [RoutePrefix("api/Country")]
    public class CountryController : ApiController
    {
        static List<Country> countries = new List<Country>();

        [HttpGet]
        [Route("All")]
        public IHttpActionResult GetAll()
        {
            return Ok(countries);
        }

        [HttpGet]
        [Route("ById")]
        public IHttpActionResult GetById(int id)
        {
            var country = countries.FirstOrDefault(c => c.ID == id);
            if (country == null)
                return NotFound();
            return Ok(country);
        }

        [HttpPost]
        [Route("Add")]
        public IHttpActionResult AddCountry([FromBody] Country country)
        {
            countries.Add(country);
            return Ok(countries);
        }

        [HttpPut]
        [Route("Update")]
        public IHttpActionResult UpdateCountry(int id, [FromBody] Country updated)
        {
            var country = countries.FirstOrDefault(c => c.ID == id);
            if (country == null)
                return NotFound();

            country.CountryName = updated.CountryName;
            country.Capital = updated.Capital;
            return Ok(country);
        }

        [HttpDelete]
        [Route("Delete")]
        public IHttpActionResult DeleteCountry(int id)
        {
            var country = countries.FirstOrDefault(c => c.ID == id);
            if (country == null)
                return NotFound();

            countries.Remove(country);
            return Ok(countries);
        }
    }
}
