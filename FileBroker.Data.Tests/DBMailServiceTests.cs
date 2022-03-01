using DBHelper;
using FileBroker.Data.DB;
using FOAEA3.Resources.Helpers;
using System.IO;
using Xunit;

namespace FileBroker.Data.Tests
{
    public class DBMailServiceTests
    {

        [Fact]
        public void SendEmailWithAttachmentTest1()
        {
            // arrange
            var fileBrokerDB = new DBTools("Server=%FOAEA_DB_SERVER%;Database=FoaeaMessageBroker;Integrated Security=SSPI;Trust Server Certificate=true;"
                                              .ReplaceVariablesWithEnvironmentValues());
            var mailService = new DBMailService(fileBrokerDB);

            string attachmentContent = "This is an attachment";
            File.WriteAllText(@"C:\Work\MyFile.txt", attachmentContent);

            // act
            string error = mailService.SendEmail(body: "This is the body", recipients: "dsarrazi@justice.gc.ca",
                                                 subject: "This is the subject",
                                                 attachmentPath: @"C:\Work\MyFile.txt");

            // assert
            Assert.Equal(string.Empty, error);
        }
    }
}
