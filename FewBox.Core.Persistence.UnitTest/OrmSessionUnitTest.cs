using System;
using System.IO;
using Dapper;
using FewBox.Core.Persistence.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FewBox.Core.Persistence.UnitTest
{
    [TestClass]
    public class OrmSessionUnitTest
    {
        private ICurrentUser<Guid> CurrentUser { get; set; }
        private IOrmConfiguration OrmConfiguration { get; set; }

        [TestInitialize]
        public void Init()
        {
            SqlMapper.AddTypeHandler(new SQLiteGuidTypeHandler());
            string filePath = $"{Environment.CurrentDirectory}/FewBox.sqlite";
            if(!File.Exists(filePath))
            {
                throw new Exception($"The SQLite file '{filePath}' is not exists!");
            }
            var ormConfigurationMock = new Mock<IOrmConfiguration>();
            ormConfigurationMock.Setup(x => x.GetConnectionString()).Returns($"Data Source={filePath};BinaryGUID=True;"); //Server=localhost;Database=fewbox;Uid=fewbox;Pwd=fewbox;SslMode=REQUIRED;Charset=utf8;ConnectionTimeout=60;DefaultCommandTimeout=60;
            this.OrmConfiguration = ormConfigurationMock.Object;
            var currentUserMock = new Mock<ICurrentUser<Guid>>();
            currentUserMock.Setup(x => x.GetId()).Returns(Guid.Empty);
            this.CurrentUser = currentUserMock.Object;
        }

        [TestMethod]
        public void TestSession()
        {
            int effectRows = 0;
            Guid id = Guid.Empty;
            this.Wrapper((appRespository) => {
                id = appRespository.Save(new App { Name = "OldName", Key = "Key" });
            });
            this.Wrapper((appRespository)=> {
                var app = appRespository.FindOne(id);
                Assert.AreEqual("OldName", app.Name);
                app.Name = "NewName";
                appRespository.Update(app);
                app = appRespository.FindOne(id);
                Assert.AreEqual("NewName", app.Name);
            });
            this.Wrapper((appRespository)=> {
                effectRows = appRespository.Delete(id);
                Assert.AreEqual(1, effectRows);
            });
        }

        private void Wrapper(Action<IAppRespository> action)
        {
            var ormSession = new SQLiteSession(this.OrmConfiguration);
            var appRespository = new AppRespository("app", ormSession, this.CurrentUser);
            try
            {
                ormSession.UnitOfWork.Start();
                action(appRespository);
                ormSession.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                ormSession.UnitOfWork.Rollback();
                Assert.Fail(exception.Message + exception.StackTrace);
            }
            finally
            {
                ormSession.Close();
            }
        }
    }
}
