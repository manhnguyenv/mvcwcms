using CsvHelper;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;
using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MVCwCMS.Models
{
    public class GeoIpCountry
    {
        public string CountryIsoCode { get; set; }
        public string CountryName { get; set; }
    }

    /// <summary>
    /// Update the "~/App_Data/iso3166.csv" file from here: http://dev.maxmind.com/static/csv/codes/iso3166.csv
    /// Remember to add the following line at the top of the csv file: CountryIsoCode,CountryName
    /// </summary>
    public class GeoIpCountries
    {
        private static object ThisLock = new object();

        private List<GeoIpCountry> _AllItems;

        private List<GeoIpCountry> GetAllItems(bool force = false)
        {
            HttpContext context = HttpContext.Current;

            if (force || context.Cache["GeoIpCountries"].IsNull()) //Double check locking
            {
                lock (ThisLock)
                {
                    if (force || context.Cache["GeoIpCountries"].IsNull()) //Double check locking
                    {
                        TextReader textReader = File.OpenText(context.Server.MapPath("~/App_Data/iso3166.csv"));
                        CsvReader csvReader = new CsvReader(textReader);
                        context.Cache.Insert("GeoIpCountries", csvReader.GetRecords<GeoIpCountry>().OrderBy(i => i.CountryName).ToList());
                    }
                }
            }

            return context.Cache["GeoIpCountries"] as List<GeoIpCountry>;
        }

        public GeoIpCountries()
        {
            _AllItems = GetAllItems();
        }

        public List<GeoIpCountry> GetAllGeoIpCountries()
        {
            return _AllItems;
        }

        public GeoIpCountry GetCountryByCode(string countryCode)
        {
            GeoIpCountry result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.CountryIsoCode.ToLower() == countryCode.ToLower()
                          select i).FirstOrDefault();
            }

            return result;
        }
    }
}