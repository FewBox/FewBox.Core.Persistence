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
        private IOrmSession OrmSession { get; set; }
        private IAppRespository AppRespository { get; set; }

        [TestInitialize]
        public void Init()
        {
            SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
            string filePath = $"{Environment.CurrentDirectory}/FewBox.sqlite";
            if(!File.Exists(filePath))
            {
                throw new Exception($"The SQLite file '{filePath}' is not exists!");
            }
            var ormConfigurationMock = new Mock<IOrmConfiguration>();
            ormConfigurationMock.Setup(x => x.GetConnectionString()).Returns($"Data Source={filePath};BinaryGUID=True;"); //Server=localhost;Database=fewbox;Uid=fewbox;Pwd=fewbox;SslMode=REQUIRED;Charset=utf8;ConnectionTimeout=60;DefaultCommandTimeout=60;
            var currentUserMock = new Mock<ICurrentUser<Guid>>();
            currentUserMock.Setup(x => x.GetId()).Returns(Guid.Empty);
            this.OrmSession = new SQLiteSession(ormConfigurationMock.Object);
            this.AppRespository = new AppRespository("app", this.OrmSession, currentUserMock.Object);
        }

        [TestMethod]
        public void TestSession()
        {
            int effectRows = 0;
            Guid id = Guid.Empty;
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
                this.OrmSession.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                this.OrmSession.UnitOfWork.Rollback();
                Assert.Fail(exception.Message + exception.StackTrace);
            }
            finally
            {
                this.OrmSession.UnitOfWork.Reset();
            }
        }
    }
}
