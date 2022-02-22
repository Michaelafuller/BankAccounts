using System;
using System.ComponentModel.DataAnnotations;

namespace BankAccounts.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "is required")]
        public double Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public User Transactor { get; set; }
    }
}