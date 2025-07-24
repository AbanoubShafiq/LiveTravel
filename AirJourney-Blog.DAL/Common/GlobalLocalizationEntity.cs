using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.DAL.Common
{
    public class GlobalLocalizationEntity
    {
        public string Localize(string txtEn, string txtEs)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            if (cultureInfo.TwoLetterISOLanguageName.ToLower().Equals("es"))
                return txtEs;
            return txtEn;
        }
    }
}
