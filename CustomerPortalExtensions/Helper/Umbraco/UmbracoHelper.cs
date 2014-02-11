using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Examine;
using umbraco;
using umbraco.interfaces;
using Examine.SearchCriteria;
using UmbracoExamine;

namespace CustomerPortalExtensions.Helper.Umbraco
{
    public class UmbracoHelper
    {
        public static string GetDictionaryItem(string item)
        {
            string translation;
            try
            {
                //TODO: not sure why the default language isn't working - why having to hardcode it to 2?
                 translation = new umbraco.cms.businesslogic.Dictionary.DictionaryItem(item).Value(2);
            }
            catch (Exception)
            {
                translation = string.Empty;
            }
            return translation;
        }


        private static int GetLanguageIdFromNodeId(int nodeId)
        {
            // get the domains for the node Id
            var domains = library.GetCurrentDomains(nodeId);

            // check that a domain exists
            if (domains != null && domains.Length > 0)
            {
                // return the language id from the first domain
                return domains[0].Language.id;
            }

            // otherwise return zero
            return 0;
        }



        public static DateTime? GetDateTimePropertyAlias(INode node, string alias)
        {
            DateTime date;
            return DateTime.TryParse(GetPropertyAlias(node, alias), out date) ? date : (DateTime?)null;
        }

        public static bool GetBooleanPropertyAlias(INode node, string alias, bool defaultValue)
        {
            //bool boolResult;
            return (GetPropertyAlias(node, alias) == "1");
        }

        public static string GetPropertyAlias(INode node, string alias)
        {
            if (alias == "pageName")
                return node.Name;
            if (node.GetProperty(alias) != null)
            {
                if (node.GetProperty(alias) != null)
                {
                    return (node.GetProperty(alias).Value);
                }
            }
            return String.Empty;
        }

       public static int GetIntegerPropertyAlias(ISearchResults results, string alias)
       {
           string property = GetPropertyAlias(results, alias);
           int returnInt = 0;
           int.TryParse(property, out returnInt);
           return returnInt;
       }

        public static string GetPropertyAlias(ISearchResults results, string alias)
        {
            if (results.TotalItemCount == 1)
            {
                var result = results.FirstOrDefault();
                if (result != null)
                {
                    if (alias == "pageName")
                    {
                        alias = "nodeName"; 
                    }
                    if (result.Fields.ContainsKey(alias))
                    {
                        return result.Fields[alias];
                    }
                }
                    
            }
            return String.Empty;
           
        }


        public static string InsertPropertyAliases(INode node, string alias)
        {
            //if no multiple aliases defined, just return the single alias
            if (!alias.Contains("{")) { return GetPropertyAlias(node, alias); }

            //get all aliases between {}
            Regex regexObj = new Regex(@"(?<=\{)[^{}]*(?=\})");
            var allMatchResults = regexObj.Matches(alias);
            foreach (Match match in allMatchResults)
            {
                if (match.ToString() == "pageName")
                {
                    alias = alias.Replace("{pageName}", node.Name);
                }
                else
                {
                    bool aliasExists;
                    IProperty propertyAlias = node.GetProperty(match.ToString(), out aliasExists);
                    if (aliasExists)
                    {
                        alias = alias.Replace("{" + match.ToString() + "}", propertyAlias.Value);
                    }
                }
            }
            //strip out unnecessary time fields
            alias = alias.Replace("T00:00:00", "");
            return alias;
        }
    }
}