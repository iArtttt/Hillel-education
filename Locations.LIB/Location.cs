using System;
using System.Collections.Generic;
using System.Text;

namespace Locations.LIB
{
    public record struct Location(string Country, string District, string City, double Area, int Population)
    {
        public override string ToString()
        {
            return $"{City}:{Area};{Population};{Country}({District})";
        }
    }
}
