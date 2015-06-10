using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Shared.Answer
{
    [Serializable]
    public class OperationResult
    {
        public bool Value { get; set; }
    }
}
