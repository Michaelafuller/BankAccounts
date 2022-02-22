using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace BankAccounts.Controllers
{
    [Route("transaction")]
    public class BankController : Controller
    {
        private bool inSession
        {
            get 
            {
                return HttpContext.Session.GetInt32("UUID") != null;
            }
        }
        private User loggedIn
        {
            get
            {
                return db.Users.Include(u => u.AllTransactions)
                    .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UUID"));
            }
        }

        private BankAccountsContext db;
        public BankController(BankAccountsContext context)
        {
            db = context;
        }


        [HttpGet("/account")]
        public IActionResult Account()
        {
            if (!inSession)
            {
                return RedirectToAction("LoginForm", "Home");
            }

            ViewBag.loggedIn = loggedIn;
            ViewBag.AllTransactions = db.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.UserId == loggedIn.UserId);
            
            // List<Transaction> allTransactions = db.Transactions
            //     .Where(t => t.UserId == HttpContext.Session.GetInt32("UUID"))
            //     .ToList();
            return View("Account");
        }

        [HttpPost("/transaction/deposit")]
        public IActionResult TransactionDeposit(Transactions newTransaction)
        {
            if (!inSession)
            {
                return RedirectToAction("LoginForm", "Home");
            }

            if(ModelState.IsValid == false)
            {
                return View("Account");
            }
            ViewBag.loggedIn = loggedIn;
            ViewBag.AllTransactions = db.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.UserId == loggedIn.UserId);

            db.Transactions.Add(newTransaction);
            db.SaveChanges();
            return RedirectToAction("Account");
        }

        [HttpPost("/transaction/withdraw")]
        public IActionResult TransactionWithdraw(Transactions newTransaction)
        {
            if (!inSession)
            {
                return RedirectToAction("LoginForm", "Home");
            }

            if(ModelState.IsValid == false)
            {
                return View("Account");
            }
            ViewBag.loggedIn = loggedIn;
            ViewBag.AllTransactions = db.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.UserId == loggedIn.UserId);
            if (newTransaction.Amount - loggedIn.Balance >= 0)
            {
                db.Transactions.Add(newTransaction);
                db.SaveChanges();
                return RedirectToAction("Account");
            }
            else
            {
                ModelState.AddModelError("Amount", "Insufficient Funds");
                return View("Account");
            }
        }

    }
}