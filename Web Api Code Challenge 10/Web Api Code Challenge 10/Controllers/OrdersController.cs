using System.Web.Http;
using System.Data.SqlClient;
using System.Linq;
using Web_Api_Code_Challenge_10.Models;

namespace Web_Api_Code_Challenge_10.Controllers
{
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private readonly NorthWindEntities db = new NorthWindEntities();

        // GET: api/Orders/ByEmployee
        [HttpGet]
        [Route("ByEmployee")]
        public IHttpActionResult GetOrdersByEmployee([FromUri] int employeeId)
        {
            var orders = db.Database.SqlQuery<Order>(
                "EXEC GetOrdersByEmployee @EmployeeId",
                new SqlParameter("@EmployeeId", employeeId)).ToList();

            if (!orders.Any())
                return Content(System.Net.HttpStatusCode.NotFound, $"No orders found for EmployeeId = {employeeId}");

            return Ok(orders);
        }
    }
}



