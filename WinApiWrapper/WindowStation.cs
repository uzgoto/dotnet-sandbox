using System;
using System.Collections.Generic;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public class WindowStation
    {
        public IntPtr Id { get; protected set; }
        public string Name { get; protected set; }

        public static IEnumerable<WindowStation> Enumerate()
        {
            foreach (var (winStaId, winSta) in ApiWrapper.EnumWindowStations())
            {
                yield return new WindowStation()
                {
                    Id = winStaId,
                    Name = winSta,
                };
            }
        }

        public override string ToString() => $"WinSta: {this.Name}({this.Id})";
    }
}