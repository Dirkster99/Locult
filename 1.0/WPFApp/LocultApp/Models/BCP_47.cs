namespace LocultApp.Models
{
  using System.Collections.Generic;
  using System.Globalization;

  static public class BCP_47
  {
    /// <summary>
    /// http://stackoverflow.com/questions/7978607/get-country-list-in-other-languages-besides-english
    /// method for generating a country list, say for populating
    /// a ComboBox, with country options. We return the
    /// values in a Generic List<T>
    /// </summary>
    /// <returns></returns>
    public static List<string> GetCountryList()
    {
        // create a new Generic list to hold the country names returned
        List<string> cultureList = new List<string>();

        // create an array of CultureInfo to hold all the cultures found, these include the users local culture, and all the
        // cultures installed with the .Net Framework
        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

        // loop through all the cultures found
        foreach (CultureInfo culture in cultures)
        {
            // pass the current culture's Locale ID (http://msdn.microsoft.com/en-us/library/0h88fahh.aspx)
            // to the RegionInfo contructor to gain access to the information for that culture
            RegionInfo region = new RegionInfo(culture.LCID);

            // make sure out generic list doesnt already
            // contain this country
            if (!(cultureList.Contains(region.EnglishName)))
                //not there so add the EnglishName (http://msdn.microsoft.com/en-us/library/system.globalization.regioninfo.englishname.aspx)
                //value to our generic list
                cultureList.Add(region.EnglishName);
        }

        return cultureList;
    }

    public static string ResolveRegionCultureFromFileName(string fileNamePath,
                                                          string defaultRegionCulture = "en-US")
    {
      try
      {
        string filename = System.IO.Path.GetFileName(fileNamePath);

        if (string.IsNullOrEmpty(filename) == true)
          return defaultRegionCulture;

        int iOffset = filename.LastIndexOf(".");

        if (iOffset < 0)
          return defaultRegionCulture;

        string fileExt = filename.Substring(iOffset + 1);

        int iOffset1 = filename.LastIndexOf(".", iOffset - 1);

        if (iOffset1 < 0)
          return defaultRegionCulture;

        string RegionCulture = filename.Substring(iOffset1 + 1, (iOffset - iOffset1) - 1);

        return RegionCulture;
      }
      catch
      {
      }

      return defaultRegionCulture;
    }

    /// <summary>
    /// Returns a BCP47 language code parsed from a file name, eg.: Strings.de-DE.resx
    /// or the default BCP47 code indicated by <paramref name="defaultTargetFileBCP47"/>.
    /// 
    /// File names should not have more periodes than necessary, otherwise, this method
    /// might need adjustment.
    /// </summary>
    /// <param name="fileNamePath"></param>
    /// <param name="defaultTargetFileBCP47"></param>
    /// <returns></returns>
    public static string GetLanguageRegionFromFileName(string fileNamePath,
                                                       string defaultTargetFileBCP47 = "en-US")
    {
      try
      {
        string filename = System.IO.Path.GetFileName(fileNamePath);

        string LangRegion = BCP_47.ResolveRegionCultureFromFileName(filename, defaultTargetFileBCP47);

        // There is no zh language so we return this completely
        if (LangRegion == "zh-CHT" || LangRegion == "zh-CHS" || LangRegion == "zh-Hans")
          return LangRegion;

        int i = LangRegion.IndexOf("-");

        if (i > 0)
          return LangRegion.Substring(0, i);

        return LangRegion;
      }
      catch
      {
      }

      return defaultTargetFileBCP47;
    }
  }
}
