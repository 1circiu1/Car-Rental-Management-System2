using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Customer
    {
        private int CustomerId { get; set; }
        private string FirstName { get; set; } = string.Empty;
        private string LastName { get; set; } = string.Empty;
        private string Email { get; set; } = string.Empty;
        private string Phone { get; set; } = string.Empty;

    }
}
