using FOAEA3.Model.Interfaces;
using FOAEA3.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FOAEA3.Model
{
    public class AssumeRoleData : IMessageList
    {
        [Display(Name = "SUBMITTER", ResourceType = typeof(LanguageResource))]
        public string AssumedRole { get; set; }

        public MessageDataList Messages => throw new NotImplementedException();
    }
}
