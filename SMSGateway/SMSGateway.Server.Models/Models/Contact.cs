using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Server.Models.Models
{
    public class Contact : UserRecord
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }
        public int ContactGroupId { get; set; }
    }
}
