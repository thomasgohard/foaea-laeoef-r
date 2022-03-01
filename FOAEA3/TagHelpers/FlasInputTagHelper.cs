using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Reflection;
using System.Text;

namespace FOAEA3.TagHelpers
{
    /// <summary>
    /// Input box tag
    /// </summary>
    /// <remarks>
    /// <b>asp-for</b> model field to use
    /// <b>required</b> mark as mandatory field
    /// <b>form-id</b> badge showing form id after textbox
    /// <b>size</b> column width to use
    /// <b>extra-fields</b> multiple field controls
    /// <b>col-sizes</b> multiple field control sizes
    /// </remarks>
    public class FlasInputTagHelper : TagHelper
    {

        public ModelExpression AspFor { get; set; }

        public string TablePrefix { get; set; }

        public bool Required { get; set; } // mark as mandatory

        public string FormID { get; set; } // display a form id after textbox

        public string Size { get; set; } // column size

        public string Placeholder { get; set; } // watermark in textbox

        public string ExtraFields { get; set; } // comma delimited additional input fields (e.g. 3 part telephone number)

        public string ColSizes { get; set; } // when multiple fields, specific the column width of each

        public bool Disabled { get; set; } = false;

        public bool Password { get; set; } = false;

        public bool NoClosing { get; set; } = false;

        public bool ExtraClosing { get; set; } = false;

        public bool Simple { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string required = (Required) ? "required" : string.Empty;
            string disabled = (Disabled) ? "disabled" : string.Empty;
            string inputType = (Password) ? "password" : "text";
            string closingTag = (NoClosing) ? "" : "</div>\n";

            string fieldName = AspFor.Name;
            if (!string.IsNullOrEmpty(TablePrefix))
                fieldName = TablePrefix + "." + fieldName;

            if (NoClosing)
                closingTag = string.Empty;

            string valueInfo = string.Empty;
            string formatString = AspFor.Metadata.DisplayFormatString;
            if (AspFor.Model != null)
            {
                if (!string.IsNullOrEmpty(formatString))
                {
                    dynamic value = AspFor.Model;
                    valueInfo = value.ToString(formatString);
                }
                else
                    valueInfo = AspFor.Model.ToString();
            }

            if (Simple)
            {

                output.TagName = "input";
                output.Attributes.Add(new TagHelperAttribute("class", $"form-control form-control-sm {required}"));
                output.Attributes.Add(new TagHelperAttribute("name", fieldName));
                output.Attributes.Add(new TagHelperAttribute("id", fieldName));
                output.Attributes.Add(new TagHelperAttribute("value", valueInfo));
                if (Disabled)
                    output.Attributes.Add(new TagHelperAttribute("disabled", disabled));
            }
            else // complex
            {
                string size = string.Empty;
                if (Size != null)
                {
                    size = "col-" + Size;
                }

                /*
                
                string outputHtml;

                if (FormID == null)

                    //  <flas-input asp-for="UserName" size="5" required></flas-input>
                    //  <flas-input asp-for="Password" size="5" required password></flas-input>

                    outputHtml =
                        $"<div class='form-row form-inline form-group {required}'>" +
                        $"    <label class='col-4 col-form-label justify-content-end' for='{fieldName}'>{AspFor.Metadata.DisplayName}</label>" +
                        $"    <div class='col-8'>" +
                        $"        <input class='form-control form-control-sm col-5' type='{inputType}' id='{fieldName}' name='{fieldName}' value='{valueInfo}'>" +
                        $"    </div>" +
                        $"</div>";

                else

                    // <flas-input table-prefix="@tablePrefix" asp-for="Appl_Dbtr_SurNme" form-id="[04]" size="5" required disabled="coreDisabled"></flas-input>

                    outputHtml =
                        $"<div class='form-row form-inline form-group {required}'>" +
                        $"	<label class='col-4 col-form-label justify-content-end ' for='{fieldName}'>{AspFor.Metadata.DisplayName}</label>" +
                        $"	<div class='col-8'>" +
                        $"		<div class='input-group'>" +
                        $"			<input class='form-control form-control-sm {size}' type='{inputType}' id='{fieldName}' name='{fieldName}' value='{valueInfo}'>" +
                        $"			<div class='input-group-append'>" +
                        $"			   <div class='pl-2 pr-2 badge-info rounded-right'>{FormID}</div>" +
                        $"			</div>" +
                        $"		</div>" +
                        $"	</div>" +
                        $"</div>";
                
                output.Content.SetHtmlContent(outputHtml);
                */

                // /*

                string init = string.Empty;
                if (!ExtraClosing)
                {
                    if (!NoClosing)
                    {
                        output.TagName = "div";
                        output.Attributes.Add(new TagHelperAttribute("class", $"form-row form-inline form-group {required}"));
                    }
                    else
                    {
                        output.TagName = string.Empty;
                        init = $"<div class='form-row form-inline form-group {required} nowrap'>";
                    }
                }
                else
                {
                    output.TagName = "";
                }

                string formID = string.Empty;
                string inputGroupStart = string.Empty;
                string inputGroupEnd = string.Empty;
                if (FormID != null)
                {
                    inputGroupStart = "<div class='input-group'>\n";
                    inputGroupEnd = "</div>\n";
                    formID = "<div class='input-group-append'>\n" +
                             $"   <div class='pl-2 pr-2 badge-info rounded-right'>{FormID}</div>\n" +
                             closingTag;
                }




                string placeholder = string.Empty;
                if (Placeholder != null)
                {
                    placeholder = $" placeholder='{Placeholder}'";
                }

                if (string.IsNullOrEmpty(ExtraFields))
                {
                    output.Content.SetHtmlContent(
                        init +
                        $"<label class='col-4 col-form-label justify-content-end ' for='{fieldName}'>{AspFor.Metadata.DisplayName}</label>\n" +
                        $"<div class='col-8'>\n" +
                        inputGroupStart +
                        $"  <input class='form-control form-control-sm  {size}' type='{inputType}' id='{fieldName}' name='{fieldName}' {placeholder} value='{valueInfo}' {disabled} />\n" +
                        formID +
                        inputGroupEnd +
                        closingTag);
                }
                else
                {
                    string[] fields = ExtraFields.Split(",");
                    string[] fieldColSizes = ColSizes.Split(",");

                    var htmlContent = new StringBuilder();
                    htmlContent.Append($"<label class='col-4 col-form-label justify-content-end {size}' for='{fieldName}'>{AspFor.Metadata.DisplayName}</label>\n");
                    htmlContent.Append($"<div class='col-8'>\n");

                    htmlContent.Append($"  <input class='col-{fieldColSizes[0]} form-control form-control-sm {size}' type='{inputType}' id='{fieldName}' name='{fieldName}' value='{valueInfo}' {disabled} />\n");
                    for (int i = 0; i < fields.Length; i++)
                    {
                        Type myType = AspFor.ModelExplorer.Container.Model.GetType();
                        PropertyInfo propInfo = myType.GetProperty(fields[i]);
                        valueInfo = propInfo.GetValue(AspFor.ModelExplorer.Container.Model)?.ToString();
                        if (valueInfo is null)
                            valueInfo = string.Empty;

                        htmlContent.Append($"  <input class='col-{fieldColSizes[i + 1]} ml-1 form-control form-control-sm {size}' type='{inputType}' id='{fields[i]}' name='{fields[i]}' value='{valueInfo}' {disabled} />\n");
                    }

                    if (!NoClosing)
                        htmlContent.Append(closingTag);

                    if (ExtraClosing)
                        htmlContent.Append($"</div></div>\n");

                    output.Content.SetHtmlContent(htmlContent.ToString());

                }
                // */

            }

        }

    }
}
