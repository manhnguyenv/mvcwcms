using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;
using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MVCwCMS.Models
{
    public class GeoIpCity
    {
        public int? GeoNameId { get; set; }
        public string CountryIsoCode { get; set; }
        public string CountryName { get; set; }
        public string SubdivisionIsoCode { get; set; }
        public string SubdivisionName { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    /// <summary>
    /// Update the "~/App_Data/GeoLite2-City.mmdb" file from here: http://geolite.maxmind.com/download/geoip/database/GeoLite2-City.mmdb.gz
    /// </summary>
    public class GeoIpCities
    {
        private static object ThisLock = new object();

        private DatabaseReader _AllItems;

        private DatabaseReader GetAllItems(bool force = false)
        {
            HttpContext context = HttpContext.Current;

            if (force || context.Cache["DatabaseReader"].IsNull()) //Double check locking
            {
                lock (ThisLock)
                {
                    if (force || context.Cache["DatabaseReader"].IsNull()) //Double check locking
                    {
                        context.Cache.Insert("DatabaseReader", new DatabaseReader(context.Server.MapPath("~/App_Data/GeoLite2-City.mmdb")));
                    }
                }
            }

            return context.Cache["DatabaseReader"] as DatabaseReader;
        }

        public GeoIpCities()
        {
            _AllItems = GetAllItems();
        }

        public GeoIpCity GetGeoIpCityByIp(string ipAddress)
        {
            GeoIpCity result = new GeoIpCity();
            
            if (_AllItems.IsNotNull())
            {
                try
                {
                    CityResponse cityResponse = _AllItems.City(ipAddress);

                    if (cityResponse.IsNotNull())
                    {
                        result.GeoNameId = cityResponse.City.GeoNameId;
                        result.CountryIsoCode = cityResponse.Country.IsoCode;
                        result.CountryName = cityResponse.Country.Name;
                        result.SubdivisionIsoCode = cityResponse.MostSpecificSubdivision.IsoCode;
                        result.SubdivisionName = cityResponse.MostSpecificSubdivision.Name;
                        result.CityName = cityResponse.City.Name;
                        result.PostalCode = cityResponse.Postal.Code;
                        result.Latitude = cityResponse.Location.Latitude;
                        result.Longitude = cityResponse.Location.Longitude;
                    }
                }
                catch (AddressNotFoundException) { /* Ignore only the AddressNotFoundException */ }
            }

            return result;
        }
    }
}