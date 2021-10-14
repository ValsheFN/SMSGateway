using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Shared
{   
    public class LoginResponse : BaseResponse
    {

        public string AccessToken { get; set; }
        public DateTime? ExpiryDate { get; set; }


    }
}
