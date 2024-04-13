using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SubscriptionManagerApp.Entities;
using SubscriptionManagerApp.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SubscriptionManagerApp.Controllers
{
    public class APIController : Controller
    {
        //private sub manager db context variable
        private SubscriptionManagerContext _SubManagerDbContext;

        public APIController(SubscriptionManagerContext SubManagerDbContext)
        {
            _SubManagerDbContext = SubManagerDbContext;
        }

       
        [HttpGet("/user/{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
          
        }

        
        [HttpGet("/subs")]
        public async Task<IActionResult> GetAllSubs()
        {
           
        }

        [HttpGet("/subs/{id}")]
        public async Task<IActionResult> GetSubsOfUser(int Userid)
        {

        }

        
        [HttpPost("/user")]
        public async Task<IActionResult> AddNewUser([FromBody] User UserInfo)
        {


        }

        [HttpPost("/sub/{id}")]
        public async Task<IActionResult> AddNewSub([FromBody] Subscription SubInfo, int UserId)
        {

        }
    }
}
