using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Server.Models.Models
{
    public class SmsTemplate : UserRecord
    {
        [Required]
        public string SmsTemplateName { set; get; }
        [Required]
        public string Content { set; get; }
        //Foreign Keys
    }
}
