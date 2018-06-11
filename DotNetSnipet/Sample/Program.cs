using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Interceptors;

namespace Uzgoto.DotNetSnipet.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Application.Run(new DerivedForm());
        }
    }
}
