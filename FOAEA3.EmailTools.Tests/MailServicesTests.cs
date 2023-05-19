using FOAEA3.Resources.Helpers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FOAEA3.EmailTools.Tests
{
    public class MailServicesTests
    {
        //[Fact]
        //public void SendEmailTest1()
        //{
        //    // arrange
        //    var mailService = new EWSMailService("https://%MAIL_SERVER%/EWS/Exchange.asmx".ReplaceVariablesWithEnvironmentValues());

        //    // act
        //    string result = mailService.SendMail("This is the body", "This is the subject", "dsarrazi@justice.gc.ca");

        //    // assert
        //    Assert.Equal(string.Empty, result);
        //}

        //[Fact]
        //public void SendHtmlEmailTest1()
        //{
        //    // arrange
        //    var mailService = new EWSMailService("https://%MAIL_SERVER%/EWS/Exchange.asmx".ReplaceVariablesWithEnvironmentValues());

        //    // act
        //    string result = mailService.SendMail("This is the <b>body</b>.", "This is the subject", "dsarrazi@justice.gc.ca");

        //    // assert
        //    Assert.Equal(string.Empty, result);
        //}

        //[Fact]
        //public async Task SendEmailWithAttachmentTest1()
        //{
        //    // arrange
        //    var mailService = new EWSMailService("https://%MAIL_SERVER%/EWS/Exchange.asmx".ReplaceVariablesWithEnvironmentValues());

        //    string attachmentContent = "This is an attachment";
        //    await File.WriteAllTextAsync(@"C:\MyFile.txt", attachmentContent);

        //    // act
        //    string result = mailService.SendMail("This is the body", "This is the subject", "dsarrazi@justice.gc.ca",
        //                                            @"C:\MyFile.txt", deleteFile: true);

        //    // assert
        //    Assert.Equal(string.Empty, result);
        //}

    }
}
