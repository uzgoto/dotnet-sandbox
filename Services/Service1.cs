using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uzgoto.DotNetSnipet.Services;

namespace Uzgoto.DotNetSnipet.Sample
{
    class Service1 : SafeShutdownSerivceBase
    {
        private int DelaySeconds;
        public static void Main(string[] args)
        {

        }

        private Service1(string serviceName) : base(serviceName)
        {

        }

        protected override bool IsArgumentsValid(string[] args)
        {
            if(args.Length != 1) { return false; }
            return int.TryParse(args[0], out this.DelaySeconds);
        }
    }
}
