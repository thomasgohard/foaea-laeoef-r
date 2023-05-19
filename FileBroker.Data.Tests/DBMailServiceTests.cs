using DBHelper;
using FileBroker.Data.DB;
using FOAEA3.Resources.Helpers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FileBroker.Data.Tests
{
    public class DBMailServiceTests
    {

        [Fact]
        public async Task SendEmailWithAttachmentTest1()
        {
            // arrange
            var fileBrokerDB = new DBToolsAsync("Server=%FOAEA_DB_SERVER%;Database=FoaeaMessageBroker;Integrated Security=SSPI;Trust Server Certificate=true;"
                                              .ReplaceVariablesWithEnvironmentValues());
            var mailService = new DBMailService(fileBrokerDB);

            string attachmentContent = "This is an attachment";
            await File.WriteAllTextAsync(@"C:\Work\MyFile.txt", attachmentContent);

            // act
            string error = await mailService.SendEmailAsync(body: "This is the body1", recipients: "dsarrazi@justice.gc.ca",
                                                 subject: "This is the subject1",
                                                 attachmentPath: @"C:\Work\MyFile.txt");

            // assert
            Assert.Equal(string.Empty, error);
        }        
        
    }
}
