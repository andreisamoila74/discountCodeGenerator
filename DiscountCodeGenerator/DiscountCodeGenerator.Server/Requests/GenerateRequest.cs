using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCodeGenerator.Server.Requests
{
    internal class GenerateRequest
    {
        public ushort Count { get; set; }
        public byte Length { get; set; }
    }
}
