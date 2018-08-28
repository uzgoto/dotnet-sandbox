using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public class Desktop
    {
        public IntPtr Id { get; protected set; }
        public string Name { get; protected set; }

        public static IEnumerable<Desktop> Enumerate(WindowStation winSta)
        {
            foreach (var (desktopId, desktop) in ApiWrapper.EnumDesktops(winSta.Id))
            {
                yield return new Desktop()
                {
                    Id = desktopId,
                    Name = desktop,
                };
            }
        }

        public override string ToString() => $"Desktop: {this.Name}({this.Id})";
    }
}
