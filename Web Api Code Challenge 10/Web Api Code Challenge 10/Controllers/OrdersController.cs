using System.Linq;
using System.Web.Http;
using Web_Api_Code_Challenge_10.Models;

namespace Web_Api_Code_Challenge_10.Controllers
{
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        NorthWindEntities db = new NorthWindEntities();

        [HttpGet]
        [Route("ByEmployee")]
        public IHttpActionResult GetOrdersByEmployee(int employeeId)
        {
            var orders = db.Orders
                           .Where(o => o.EmployeeID == employeeId)
                           .Select(o => new
                           {
                               o.OrderID,
                               o.OrderDate,
                               o.ShipName,
                               o.ShipCountry
                           }).ToList();

            if (orders == null || orders.Count == 0)
                return NotFound();

            return Ok(orders);
        }
    }
}


