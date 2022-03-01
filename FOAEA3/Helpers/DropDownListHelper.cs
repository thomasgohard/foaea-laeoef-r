using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Helpers
{
    public class DropDownListRequiredData
    {
        public string Description_E { get; set; }
        public string Description_F { get; set; }
        public string Key { get; set; }
    }
    public class DropDownListHelper
    {
        public static string GetHTMLList(List<DropDownListRequiredData> listData)
        {
            return BuildHTMLList(GetListData(listData));
        }

        public static string GetHTMLListSortByKey(List<DropDownListRequiredData> listData)
        {
            return BuildHTMLList(GetListDataSortByKey(listData));
        }

        public static string[] GetOptionsList(List<DropDownListRequiredData> listData, string separator)
        {
            return BuildOptionsList(GetListData(listData), separator);
        }
        public static string[] BuildOptionsList(List<SelectListItem> list, string separator)
        {
            string[] HTMLList = new string[list.Count];
            int index = 0;

            foreach (SelectListItem item in list)
            {
                HTMLList[index] += string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", item.Value, separator, item.Text);
                index += 1;
            }

            return HTMLList;

        }

        public static string GetHTMLOptionList(List<DropDownListRequiredData> listData)
        {
            return BuildHTMLOptionList(GetListData(listData));
        }

        public static string BuildHTMLOptionList(List<SelectListItem> list)
        {
            string HTMLList = "";

            foreach (SelectListItem item in list)
                HTMLList += $"<OPTION value=\"{item.Value}\">{item.Text}</OPTION>";


            return HTMLList;

        }

        public static string BuildHTMLList(List<SelectListItem> list, bool useTextAsValue = false)
        {
            string all = Resources.LanguageResource.ALL_LABEL;
            string HTMLList = ":" + all + ";";

            foreach (SelectListItem item in list)
                HTMLList += string.Format(CultureInfo.CurrentCulture, "{0}:{1};", !useTextAsValue ? item.Value : item.Text, item.Text);

            HTMLList = HTMLList.Remove(HTMLList.Length - 1);

            return HTMLList;

        }

        public static List<SelectListItem> GetEmptyList()
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "---", Value = "---" }
            };
            return list;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "Should not sort accents differently")]
        public static List<SelectListItem> GetProvinceListData(List<DropDownListRequiredData> listData)
        {
            bool isEnglish = LanguageHelper.IsEnglish();

            listData.Sort((item1, item2) => string.Compare(item1.Key, item2.Key, StringComparison.CurrentCultureIgnoreCase));

            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (DropDownListRequiredData item in listData)
            {
                string description = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", isEnglish ? item.Description_E : item.Description_F, item.Key);
                SelectListItem newItem = new SelectListItem { Text = description, Value = item.Key };
                if (!listItems.Contains(newItem))
                    listItems.Add(newItem);
            }
            return listItems;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "Should not sort accents differently")]
        public static List<SelectListItem> GetListData(List<DropDownListRequiredData> listData)
        {
            bool isEnglish = LanguageHelper.IsEnglish();

            if (isEnglish)
                listData.Sort((item1, item2) => string.Compare(item1.Description_E, item2.Description_E, StringComparison.CurrentCultureIgnoreCase));
            else
                listData.Sort((item1, item2) => string.Compare(item1.Description_F, item2.Description_F, StringComparison.CurrentCultureIgnoreCase));

            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (DropDownListRequiredData item in listData)
            {
                string description = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", isEnglish ? item.Description_E : item.Description_F, item.Key);
                SelectListItem newItem = new SelectListItem { Text = description, Value = item.Key };
                if (!listItems.Contains(newItem))
                    listItems.Add(newItem);
            }
            return listItems;
        }

        public static List<SelectListItem> GetListDataSortByKey(List<DropDownListRequiredData> listData)
        {
            bool isEnglish = LanguageHelper.IsEnglish();

            listData.Sort((item1, item2) => string.Compare(item1.Key, item2.Key, StringComparison.OrdinalIgnoreCase));

            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (DropDownListRequiredData item in listData)
            {
                string description = string.Format(CultureInfo.CurrentCulture, "{1} ({0})", isEnglish ? item.Description_E : item.Description_F, item.Key);
                SelectListItem newItem = new SelectListItem { Text = description, Value = item.Key };
                if (!listItems.Contains(newItem))
                    listItems.Add(newItem);
            }
            return listItems;
        }

        public static List<SelectListItem> GetListDataSortByKey(List<DropDownListRequiredData> listData, string Selected)
        {
            bool isEnglish = LanguageHelper.IsEnglish();

            listData.Sort((item1, item2) => string.Compare(item1.Key, item2.Key, StringComparison.OrdinalIgnoreCase));

            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (DropDownListRequiredData item in listData)
            {
                string description = string.Format(CultureInfo.CurrentCulture, "{1} ({0})", isEnglish ? item.Description_E : item.Description_F, item.Key);
                bool select = false;
                if (item.Key == Selected)
                {
                    select = true;
                }
                SelectListItem newItem = new SelectListItem { Text = description, Value = item.Key, Selected = select };
                if (!listItems.Contains(newItem))
                    listItems.Add(newItem);
            }
            return listItems;
        }
    }
}
