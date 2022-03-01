using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace FOAEA3.TagHelpers
{
    public class FlasRadioTagHelper : TagHelper
    {
        public ModelExpression AspFor { get; set; }

        public bool Disabled { get; set; } = false;

        public string Value { get; set; }

        public string Badge { get; set; }

        public bool Active { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var disabled = (Disabled) ? "disabled" : string.Empty;
            var active = (Active) ? "active" : string.Empty;

            output.TagName = "label";
            output.Attributes.Add(new TagHelperAttribute("class", $"btn btn-outline-info text-left {active} {disabled}"));
            output.Attributes.Add(new TagHelperAttribute("for", AspFor.Name + Value));

            if (Disabled)
                output.Attributes.Add(new TagHelperAttribute("disabled", disabled));

            var content = output.GetChildContentAsync().Result.GetContent();

            var badgeContent = string.Empty;
            if (!string.IsNullOrEmpty(Badge))
                badgeContent = $"  <span class='badge badge-dark create-badge mt-1' {disabled} >{Badge}</span>\n";

            output.Content.SetHtmlContent(
                    $"  <input type='radio' id='{AspFor.Name}{Value}' name='{AspFor.Name}' value='{Value}' {disabled} />\n" +
                    badgeContent +
                    $"  <label class='pl-1 align-middle' {disabled}>{content}</label>\n"
                );
        }

        /*
            <label class="btn btn-outline-info text-left @coreDisabledString @optRoleActive1" id="reason1" disabled="@coreDisabledString">
                <input type="radio" name="optRole" id="reasonCd1" value="1" @optRoleReason1 disabled="@coreDisabledString" />
                <span class='badge badge-dark create-badge mt-1' disabled='@coreDisabledString' >A</span>
                <label class="pl-1 align-middle" disabled="@coreDisabledString" >Person in default of the support provision</label>
            </label>         
         */

    }
}
