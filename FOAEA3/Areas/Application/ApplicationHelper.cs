using System;

namespace FOAEA3.Areas.Application
{
    public static class ApplicationHelper
    {
        public static string GetControllerForCategory(string category)
        {
            string controller = category switch
            {
                "T01" => "Tracing",
                "I01" => "Interception",
                "L01" => "LicenceDenial",
                "L03" => "LicenceDenialTermination",
                _ => throw new ApplicationException($"Invalid category: {category}")
            };

            return controller;
        }

    }
}
