using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.TagHelpers
{
    public class FlasToggleTagHelper : TagHelper
    {
        public ModelExpression AspFor { get; set; }

        public string TablePrefix { get; set; }

        public bool Required { get; set; }

        public bool Disabled { get; set; } = false;

        public string Size { get; set; }

        public string FormID { get; set; }

        public bool SingleLine { get; set; }

        public bool ExtraClosing { get; set; } = false;

        public Dictionary<string, string> Values { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string required = (Required) ? "required" : string.Empty;
            string disabled = (Disabled) ? "disabled" : string.Empty;

            string fieldName = AspFor.Name;
            if (!string.IsNullOrEmpty(TablePrefix))
                fieldName = TablePrefix + "." + fieldName;

            string formID = string.Empty;
            if (FormID != null)
            {
                formID = $"<div class='ml-1 col-auto badge-info badge-floating rounded'>{FormID}</div>\n";
            }

            string multiLine = string.Empty;
            string singleLine = string.Empty;
            string multiLineOffset = string.Empty;
            string formInline = string.Empty;
            if (SingleLine)
            {
                singleLine = "</div>\n";
                formInline = "form-inline";
            }
            else
            {
                multiLine = "</div>\n";
                multiLineOffset = "offset-3";
            }

            string size = Size ?? "9";

            if (!ExtraClosing)
            {
                output.TagName = "div";
                output.Attributes.Add(new TagHelperAttribute("class", $"form-row {formInline} form-group {required}"));
            }
            else
            {
                output.TagName = "";
            }
            var outputContent = new StringBuilder();
            //$"<div class='offset-3 col-9'>\n" +
            if (!ExtraClosing)
                outputContent.Append($"  <label class='col-4 col-form-label justify-content-end ' for='{fieldName}'>{AspFor.Metadata.DisplayName}</label>\n");
            outputContent.Append(multiLine);
            outputContent.Append($"<div class='{multiLineOffset} col-7 ml-1 btn-group btn-group-sm btn-group-toggle padding-left-sm' data-toggle='buttons'>\n");

            int numValues = Values.Count;
            int valueCount = 0;
            string rounded = string.Empty;
            foreach (var value in Values)
            {
                var activeInfo = ((AspFor.Model != null) && (AspFor.Model.ToString() == value.Key)) ? "active" : string.Empty;
                var checkedInfo = (activeInfo == "active") ? "checked='checked'" : string.Empty;

                valueCount++;
                if (valueCount == numValues)
                    rounded = "rounded-right";

                outputContent.Append($"  <label class='btn btn-outline-info col-{size} {activeInfo} {rounded} {disabled}' {disabled}>\n");
                outputContent.Append($"    <input type='radio' id='{fieldName}{value.Key}' name='{fieldName}' value='{value.Key}' {checkedInfo} {disabled}/>{value.Value}\n");
                outputContent.Append($"  </label>\n");

            }
            outputContent.Append(formID + singleLine);

            if (ExtraClosing)
                outputContent.Append($"</div></div>\n");

            output.Content.SetHtmlContent(outputContent.ToString());
        }
    }
}
