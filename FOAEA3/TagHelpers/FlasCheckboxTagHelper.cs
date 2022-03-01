using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace FOAEA3.TagHelpers
{
    public class FlasCheckboxTagHelper : TagHelper
    {
        public ModelExpression AspFor { get; set; }

        public bool Disabled { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var disabled = (Disabled) ? "disabled" : string.Empty;

            var activeInfo = ((AspFor.Model != null) && (AspFor.Model.ToString().ToLower() == "true")) ? "active" : string.Empty;
            var checkedInfo = (activeInfo == "active") ? "checked='checked'" : string.Empty;

            output.TagName = "label";
            output.Attributes.Add(new TagHelperAttribute("class", $"btn {activeInfo} btn-outline-info text-left"));

            output.Content.SetHtmlContent(
                $"  <input type='checkbox' class='form-control form-control-sm' id='{AspFor.Name}' name='{AspFor.Name}' {checkedInfo} value='true' {disabled} />\n" +
                $"  <label  class='pl-5' for='{AspFor.Name}'>{AspFor.Metadata.DisplayName}</label>\n"
            );
        }
    }

}
