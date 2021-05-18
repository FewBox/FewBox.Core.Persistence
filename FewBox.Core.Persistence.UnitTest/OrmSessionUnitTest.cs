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
            if (!File.Exists(filePath))
            {
                throw new Exception($"The SQLite file '{filePath}' is not exists!");
            }
            var ormConfigurationMock = new Mock<IOrmConfiguration>();
            ormConfigurationMock.Setup(x => x.GetConnectionString()).Returns($"Data Source={filePath};"); //Server=localhost;Database=fewbox;Uid=fewbox;Pwd=fewbox;SslMode=REQUIRED;Charset=utf8;ConnectionTimeout=60;DefaultCommandTimeout=60;
            this.OrmConfiguration = ormConfigurationMock.Object;
            var currentUserMock = new Mock<ICurrentUser<Guid>>();
            currentUserMock.Setup(x => x.GetId()).Returns(Guid.Empty);
            this.CurrentUser = currentUserMock.Object;
        }

        [TestMethod]
        public void TestSession()
        {
            int effectRows = 0;
            Guid record1Id = new Guid("00000000-0000-0000-0000-000000000001");
            Guid record2Id = Guid.Empty;
            Guid char36Id = Guid.NewGuid();
            this.Wrapper((appRespository) =>
            {
                if (this.VerifyTempData(appRespository))
                {
                    appRespository.Clear();
                }
            }, "app");
            this.Wrapper((appRespository) =>
            {
                // Create
                Guid id = appRespository.Save(new App { Id = record1Id, Name = "FewBox1", Key = "Landpy1", Char36Id = char36Id });
                Assert.AreEqual(record1Id, id);
                record2Id = appRespository.Save(new App { Name = "FewBox2", Key = "Landpy2", Char36Id = char36Id });
            }, "app");
            this.Wrapper((appRespository) =>
            {
                // Read
                var app2 = appRespository.FindOne(record2Id);
                Assert.AreEqual("FewBox2", app2.Name);
                Assert.AreEqual(char36Id, app2.Char36Id);
                // Update
                app2.Name = "FewBox";
                app2.Char36Id = Guid.Empty;
                appRespository.Update(app2);
            }, "app");
            this.Wrapper((appRespository) =>
            {
                // Verify FindOne
                var app2 = appRespository.FindOne(record2Id);
                Assert.AreEqual("FewBox", app2.Name);
                Assert.AreEqual(Guid.Empty, app2.Char36Id);
                // Verify FindAll
                var apps = appRespository.FindAll();
                Assert.IsTrue(apps.AsList().Count == 2);
                Assert.AreEqual("FewBox", apps.AsList()[1].Name);
                Assert.AreEqual(Guid.Empty, apps.AsList()[1].Char36Id);
                // Verify FindAll CreatedBy
                var createdByApps = appRespository.FindAllByCreatedBy(Guid.Empty, 1, 1 );
                // Verify Count
                int count = appRespository.Count();
                Assert.IsTrue(count == 2);
                // Verify Recycle
                appRespository.Recycle(record2Id);
                count = appRespository.Count();
                Assert.IsTrue(count == 1);
            }, "app");
            this.Wrapper((appRecycleRespository) =>
            {
                // Verify Recycle
                int count = appRecycleRespository.Count();
                Assert.IsTrue(count == 1);
                // Truncate
                appRecycleRespository.Clear();
            }, "app_recycle");
            this.Wrapper((appRespository) =>
            {
                // FindOne
                var app = appRespository.FindOne(record1Id);
                // Delete
                effectRows = appRespository.Delete(record1Id);
                Assert.AreEqual(1, effectRows);
            }, "app");
        }

        private bool VerifyTempData(IAppRespository appRespository)
        {
            return appRespository.Count() > 0;
        }

        private void Wrapper(Action<IAppRespository> action, string tableName)
        {
            var ormSession = new SQLiteSession(this.OrmConfiguration);
            var appRespository = new AppRespository(tableName, ormSession, this.CurrentUser);
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
                ormSession.UnitOfWork.Stop();
            }
        }
    }
}
