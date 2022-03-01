using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace FOAEA3.TagHelpers
{
    public class FlasCommentTagHelper : TagHelper
    {
        public ModelExpression AspFor { get; set; }

        public bool Required { get; set; }
        
        public string TablePrefix { get; set; }

        public bool Disabled { get; set; } = false;

        public bool NoOffset { get; set; }

        public bool NoLabel { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string fieldName = AspFor.Name;
            if (!string.IsNullOrEmpty(TablePrefix))
                fieldName = TablePrefix + "." + fieldName;

            string required = (Required) ? "required" : string.Empty;
            string disabled = (Disabled) ? "disabled" : string.Empty;

            string offset = (NoOffset) ? "col-12" : "offset-1 col-11";

            string valueInfo = (AspFor.Model != null) ? AspFor.Model.ToString() : string.Empty;

            output.TagName = "div";
            output.Attributes.Add(new TagHelperAttribute("class", $"{offset} {required}"));
            output.Content.SetHtmlContent(
                (!NoLabel) ? $"<label class='control-label ' for='{fieldName}'>{AspFor.Metadata.DisplayName}</label>\n" : "" +
                $"<div>\n" +
                $"  <textarea class='form-control form-control-sm ' id='{fieldName}' name='{fieldName}' {disabled}>{valueInfo}</textarea>\n" +
                $"</div>\n");

        }

    }
}
