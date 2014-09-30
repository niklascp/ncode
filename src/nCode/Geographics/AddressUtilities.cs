using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace nCode.Geographics
{
    public static class AddressUtilities
    {
        private static Regex houseNoPattern = new Regex(@"\s+(\d+\s?[A-Za-z]?)");
        private static Regex streetPattern = new Regex(@"(.*)");

        public static StreetAddress ExtractStreetAddress(string address)
        {
            var houseNoMatch = houseNoPattern.Match(address);

            if (houseNoMatch.Success)
            {
                var streetNoMatch = streetPattern.Match(address, 0, houseNoMatch.Index);

                if (streetNoMatch.Success)
                {
                    return new StreetAddress()
                    {
                        StreetName = streetNoMatch.Groups[1].Value,
                        HouseNo = houseNoMatch.Groups[1].Value.Replace(" ", "")
                    };
                }
            }

            return null;
        }
    }

    public class StreetAddress
    {
        public string StreetName { get; set; }
        public string HouseNo { get; set; }

        public override string ToString()
        {
            return "StreetAddress (StreetName: " + StreetName + ", HouseNo: " + HouseNo + ")";
        }
    }
}
