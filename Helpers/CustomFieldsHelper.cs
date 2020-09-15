using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    public static class CustomFieldsHelper
    {
        const string CFG_FILE_NAME = "Settings\\customFields.json";
        static public async Task<string> GetCustomFieldNameAsync([NotNull] string defaultFieldName)
        {
            string customFieldName = defaultFieldName;

            await Task.Run(() =>
            {
                try
                {
                    var config = new ConfigurationBuilder().AddJsonFile(CFG_FILE_NAME).Build();
                    var val = config[defaultFieldName];
                    var bytes = Encoding.Default.GetBytes(val);
                    customFieldName = Encoding.UTF8.GetString(bytes);
                }
                catch (Exception ex)
                { Console.WriteLine(ex.StackTrace); }
            });

            return customFieldName;
        }
    }
}
