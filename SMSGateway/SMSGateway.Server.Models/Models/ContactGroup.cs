using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Server.Models.Models
{
    public class ContactGroup : UserRecord
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
