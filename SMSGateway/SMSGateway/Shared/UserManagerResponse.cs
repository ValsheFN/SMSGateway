using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Shared
{
    public class UserManagerResponse : BaseResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
