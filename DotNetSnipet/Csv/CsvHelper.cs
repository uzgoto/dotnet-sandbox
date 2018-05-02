using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Uzgoto.DotNetSnipet.Csv
{
    public static class CsvHelper
    {
        public static IEnumerable<IEnumerable<string>> EnumerateItems(
            string pathToCsvFile, Encoding encoding = null)
        {
            var lines = File.ReadLines(pathToCsvFile, encoding ?? Encoding.Default);
            foreach (var item in EnumerateItems(lines))
            {
                yield return item;
            }
        }

        public static IEnumerable<IEnumerable<string>> EnumerateItems(IEnumerable<string> lines)
        {
            yield return
                lines
                    .Where(line => !line.StartsWith("#"))
                    .SelectMany(line => line.Split(','))
                    .Select(item => item.Trim());
        }
    }
}
