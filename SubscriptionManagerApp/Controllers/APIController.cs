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

       
        [HttpGet("/user/{email}")]
        public async Task<IActionResult> GetUser(string email)
        {

          List<User> user = await _SubManagerDbContext.Users.Where(u => u.Email == email)
                .Include(u=>u.Subs)
                .Select(u=>new User()
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Subs = u.Subs
                }
            ).ToListAsync();

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
                .Include(u => u.Subs)
                .Select(u => new User()
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Subs = u.Subs
                }
            ).FirstOrDefaultAsync();

            if (user == null) { return NotFound("the user could not be found"); }

            //save to subscription list
            List<Subscription> returnSubs = user.Subs.ToList();
            
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

        [HttpPost("/sub/{id}")]
        public async Task<IActionResult> AddNewSub([FromBody] Subscription SubInfo, int UserId)
        {
           
            //create sub serviceto add to the user list of subs
           Subscription subToAdd = new Subscription()
           {
               SubscriptionId = SubInfo.SubscriptionId,
               ServiceName = SubInfo.ServiceName,
               Price = SubInfo.Price
           };

            //find the user based on the user id
            User? user = await _SubManagerDbContext.Users.Where(u => u.UserId == UserId)
                .Select(u => new User()
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }
            ).FirstOrDefaultAsync();

            if (user == null) { return NotFound("the user could not be found"); }

            //add the sub to the user
            user.Subs.Add(subToAdd);

            _SubManagerDbContext.Update(user);
            _SubManagerDbContext.SaveChanges();

            return Ok("subscription has been added successfully!");

        }
    }
}
