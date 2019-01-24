using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using FewBox.Core.Persistence.Cache;
using FewBox.Core.Persistence.Orm;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FewBox.Core.Persistence.UnitTest
{
    [TestClass]
    public class DapperSessionUnitTest
    {
        private IOrmSession DapperSession { get; set; }
        private IAppRespository AppRespository { get; set; }

        [TestInitialize]
        public void Init()
        {
            string filePath = $"{Environment.CurrentDirectory}/FewBox.sqlite";
            if(!File.Exists(filePath))
            {
                throw new Exception($"The SQLite file '{filePath}' is not exists!");
            }
            var ormConfigurationMock = new Mock<IOrmConfiguration>();
            ormConfigurationMock.Setup(x => x.GetConnectionString()).Returns($"Data Source={filePath}"); //Server=localhost;Database=fewbox;Uid=fewbox;Pwd=fewbox;SslMode=REQUIRED;Charset=utf8;ConnectionTimeout=60;DefaultCommandTimeout=60;
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<UserInfo>(), TimeSpan.MaxValue)).Returns("");
            var currentUserMock = new Mock<ICurrentUser<string>>();
            currentUserMock.Setup(x => x.GetId()).Returns(Guid.Empty.ToString());
            this.DapperSession = new SQLiteSession(ormConfigurationMock.Object, tokenServiceMock.Object);
            this.AppRespository = new AppRespository("app", this.DapperSession, currentUserMock.Object);
        }

        [TestMethod]
        public void TestSession()
        {
            int effectRows = 0;
            string id = String.Empty;
            this.Wrapper(() => {
                id = this.AppRespository.Save(new App { Name = "OldName", Key = "Key" });
            });
            this.Wrapper(()=> {
                var app = this.AppRespository.FindOne(id);
                Assert.AreEqual("OldName", app.Name);
                app.Name = "NewName";
                this.AppRespository.Update(app);
                app = this.AppRespository.FindOne(id);
                Assert.AreEqual("NewName", app.Name);
            });
            this.Wrapper(()=> {
                effectRows = this.AppRespository.Delete(id);
                Assert.AreEqual(1, effectRows);
            });
        }

        private void Wrapper(Action action)
        {
            try
            {
                action();
                this.DapperSession.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                this.DapperSession.UnitOfWork.Rollback();
                Assert.Fail(exception.Message + exception.StackTrace);
            }
            finally
            {
                this.DapperSession.UnitOfWork.Reset();
            }
        }
    }
}
