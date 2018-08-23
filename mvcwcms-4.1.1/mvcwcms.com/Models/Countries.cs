using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MVCwCMS.Models
{
    public class Country
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }

    public class Countries
    {
        private List<Country> _AllItems;

        private List<Country> GetAllItems(bool force = false)
        {
            return AdoHelper.ExecCachedListProc<Country>("sp_cms_countries_select", force);
        }

        public Countries()
        {
            _AllItems = GetAllItems();
        }

        public List<Country> GetAllCountries(string countryCode = null, string countryName = null)
        {
            List<Country> result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where (countryCode.IsNull() || i.CountryCode.Contains(countryCode, StringComparison.OrdinalIgnoreCase))
                             && (countryName.IsNull() || i.CountryName.Contains(countryName, StringComparison.OrdinalIgnoreCase))
                          select i).ToList();
            }

            return result;
        }

        public Country GetCountryByCode(string countryCode)
        {
            Country result = null;

            if (_AllItems.IsNotNull())
            {
                result = (from i in _AllItems
                          where i.CountryCode.ToLower() == countryCode.ToLower()
                          select i).FirstOrDefault();
            }

            return result;
        }
    }
}