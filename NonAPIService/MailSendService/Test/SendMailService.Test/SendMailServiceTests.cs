using Xunit;
using SendMailService.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.Extensions.Logging;

namespace SendMailService.Service.Tests
{
    public class SendMailServiceTests
    {
        [Fact()]
        public async void SendMailAsyncTest() 
        {
            var mock = new Mock<ILogger<SendMailService>>();
            ILogger<SendMailService> logger = mock.Object;

            //or use this short equivalent 
            logger = Mock.Of<ILogger<SendMailService>>();
            var sendMail = new Service.SendMailService(logger);

            sendMail.SendMailSettings(
                        new System.Net.Mail.SmtpClient
                        {
                            Host = "smtp.office365.com",
                            Port = 587,
                            UseDefaultCredentials = false,
                            EnableSsl=true,
                        },
                        new System.Net.NetworkCredential
                        {
                            Password = "mikaretina",
                            UserName = "smikaric@msn.com",
                        },
                        new System.Net.Mail.MailMessage
                        {
                            From = new System.Net.Mail.MailAddress("smikaric@msn.com"),
                        });

            await sendMail.SendMailAsync("smikaric@gmail.com", "", "aa");

            Assert.True(true, "This test needs an implementation");
        }
        [Fact()]
        public void SendMailTest()
        {
            var mock = new Mock<ILogger<SendMailService>>();
            ILogger<SendMailService> logger = mock.Object;

            //or use this short equivalent 
            logger = Mock.Of<ILogger<SendMailService>>();
            var sendMail = new Service.SendMailService(logger);
            sendMail.OnMailSent += SendMail_OnMailSent;
            sendMail.SendMailSettings(
                        new System.Net.Mail.SmtpClient
                        {
                            Host = "smtp.office365.com",
                            Port = 587,
                            UseDefaultCredentials = false,
                            EnableSsl = true,
                        },
                        new System.Net.NetworkCredential
                        {
                            Password = "mikaretina",
                            UserName = "smikaric@msn.com",
                        },
                        new System.Net.Mail.MailMessage
                        {
                            From = new System.Net.Mail.MailAddress("smikaric@msn.com"),
                        });

            sendMail.SendMail("smikaric@gmail.com", "", "aa");

            Assert.True(true, "This test needs an implementation");
        }

        private void SendMail_OnMailSent(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {

            }else if (e.Cancelled)
            {

            }
            else
            {
                var a = e.UserState;
            }
        }
    }
}