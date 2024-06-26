﻿using Azure;
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
            Console.WriteLine(email);

            User user = await _SubManagerDbContext.Users
                .Include(u => u.UserSubscriptions)
                    .ThenInclude(s => s.Subscription)
                 .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            Console.WriteLine(user.UserId);

            return Ok(user);
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

            var addedUser = await _SubManagerDbContext.Users.FirstOrDefaultAsync(u => u.Email == UserInfo.Email);       // Needed to get the id from the db context

            return Ok(addedUser);

        }

        [HttpPost("/subs/{UserId}")]
        public async Task<IActionResult> AddNewSub([FromBody] Subscription SubInfo, int UserId)
        {
            Console.WriteLine(UserId);

            Subscription? subToAdd = await _SubManagerDbContext.Subscriptions
                .Where(s => s.SubscriptionId == SubInfo.SubscriptionId)
                .FirstOrDefaultAsync();

            if (subToAdd == null) { return NotFound(); }

            Console.WriteLine(subToAdd.SubscriptionId + " " + subToAdd.ServiceName + " " + subToAdd.Price);

            User? user = await _SubManagerDbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == UserId);

            if (user == null) { return NotFound("The user could not be found"); }

            Console.WriteLine(user.UserId + " " + user.FirstName + " " + user.LastName + " " + user.Email);

            if (user.UserSubscriptions.Any(us => us.SubscriptionId == subToAdd.SubscriptionId))
            {
                return BadRequest();
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

        [HttpPost("/subs")]
        public async Task<IActionResult> CreateNewSub([FromBody] Subscription SubInfo)
        {
            Subscription sub = new Subscription()
            {
                ServiceName = SubInfo.ServiceName,
                Price = SubInfo.Price
            };

            _SubManagerDbContext.Subscriptions.Add(sub);
            await _SubManagerDbContext.SaveChangesAsync();

            return Ok(sub);
        }

        [HttpDelete("/subs/{userId}")]
        public async Task<IActionResult> DeleteSub(int userId, int subId)
        {
            Console.WriteLine("userID: " + userId + "subId: " + subId);
            var user = await _SubManagerDbContext.Users.Include(u => u.UserSubscriptions)
                                                       .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                // User not found, return a 404 Not Found response
                return NotFound("User not found.");
            }

            var userSubscription = user.UserSubscriptions.FirstOrDefault(us => us.SubscriptionId == subId);
            if (userSubscription == null)
            {
                // User subscription not found, return a 404 Not Found response
                return NotFound("User subscription not found.");
            }

            _SubManagerDbContext.UserSubscriptions.Remove(userSubscription);
            await _SubManagerDbContext.SaveChangesAsync();

            // User subscription deleted successfully, return a 200 OK response
            return Ok();
        }


        private string GenerateFullUrl(string path)
        {
            return $"{Request.Scheme}://{Request.Host}{path}";
        }
    }
}
