using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Shared
{
    public class OperationResponse<T> : BaseResponse
    {
        public T Data { get; set; }

    }
}
