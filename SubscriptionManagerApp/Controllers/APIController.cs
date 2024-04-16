using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SubscriptionManagerApp.Entities;
using SubscriptionManagerApp.Messages;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        [HttpGet("/subs-api")]
        public async Task<IActionResult> GetSubsApiHome()
        {
            SubsApiHomeDTO apiHomeViewModel = new SubsApiHomeDTO()
            {
                Links = new Dictionary<string, Link>() {
                    {"self" , new Link(GenerateFullUrl("/subs-api"), "self", "GET") },
                    {"subs", new Link(GenerateFullUrl("/subs"), "subs", "GET") },
                    {"user", new Link(GenerateFullUrl("/user"), "user", "GET") },
                },
                ApiVersion = "1.0",
                Creator = "Group 4"
            };

            return Ok(apiHomeViewModel);
        }

        [HttpGet("/user/{email}")]
        public async Task<IActionResult> GetUser(string email)
        {

          List<User> user = await _SubManagerDbContext.Users.Where(u => u.Email == email)
                .Include(u=>u.UserSubscriptions)
                    .ThenInclude(s => s.Subscription)
                .Select(u=>new User()
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    UserSubscriptions = u.UserSubscriptions.Select(u => new UserSubscription()
                    {
                        Subscription = u.Subscription
                    }).ToList() 
                })
                .ToListAsync();

            UserDTO userDTO = new UserDTO()
            {
                UserLst = user
            };

            return Ok(userDTO);
        }

        
        [HttpGet("/subs")]
        public async Task<IActionResult> GetAllSubs()
        {
            List<Subscription> sub = await _SubManagerDbContext.Subscriptions
                 .Select(s => new Subscription()
                 {
                     SubscriptionId = s.SubscriptionId,
                     ServiceName = s.ServiceName,
                     Price = s.Price
                 }
             ).ToListAsync();


            SubscriptionDTO subDTO = new SubscriptionDTO()
            {
                SubLst = sub
            };

            return Ok(subDTO);
        }

        //get all subscriptions of a user
        [HttpGet("/subs/{id}")]
        public async Task<IActionResult> GetSubsOfUser(int id)
        {
            User? user = await _SubManagerDbContext.Users.Where(u => u.UserId == id)
                .Include(u => u.UserSubscriptions)
                    .ThenInclude(s => s.Subscription)
                .FirstOrDefaultAsync();

            if (user == null) { return NotFound("the user could not be found"); }

            List<Subscription> returnSubs = user.UserSubscriptions
                .Select(u => u.Subscription)
                .ToList();

            SubscriptionDTO subDTO = new SubscriptionDTO()
            {
                SubLst = returnSubs
            };

            return Ok(subDTO);

        }

        
        [HttpPost("/user")]
        public async Task<IActionResult> AddNewUser([FromBody] User UserInfo)
        {
            User user = new User()
            {
              FirstName = UserInfo.FirstName,
              LastName = UserInfo.LastName,
              Email = UserInfo.Email,
            };

            _SubManagerDbContext.Add(user);
            _SubManagerDbContext.SaveChanges();
            return Ok(user);

        }

        [HttpPost("/subs/{UserId}")]
        public async Task<IActionResult> AddNewSub([FromBody] Subscription SubInfo, int UserId)
        {
            Console.WriteLine(UserId);

            Subscription? subToAdd = await _SubManagerDbContext.Subscriptions
                .Where(s => s.SubscriptionId == SubInfo.SubscriptionId)
                .FirstOrDefaultAsync();

            if (subToAdd == null) { return NotFound("The subscription requested to add could not be found"); }

            Console.WriteLine(subToAdd.SubscriptionId + " " + subToAdd.ServiceName + " " + subToAdd.Price);

            User? user = await _SubManagerDbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == UserId);

            if (user == null) { return NotFound("The user could not be found"); }

            Console.WriteLine(user.UserId + " " + user.FirstName + " " + user.LastName + " " + user.Email);

            if (user.UserSubscriptions.Any(us => us.SubscriptionId == subToAdd.SubscriptionId))
            {
                return BadRequest("The user already has this subscription");
            }

            var newUserSubscription = new UserSubscription
            {
                SubscriptionId = SubInfo.SubscriptionId,
                UserId = UserId
            };

            _SubManagerDbContext.UserSubscriptions.Add(newUserSubscription);

            await _SubManagerDbContext.SaveChangesAsync();

            return Ok(subToAdd);
        }


        private string GenerateFullUrl(string path)
        {
            return $"{Request.Scheme}://{Request.Host}{path}";
        }
    }
}
