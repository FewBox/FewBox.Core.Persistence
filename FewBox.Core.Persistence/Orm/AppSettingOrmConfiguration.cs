using Microsoft.Extensions.Configuration;

namespace FewBox.Core.Persistence.Orm
{
    public class AppSettingOrmConfiguration : IOrmConfiguration
    {
        private string ConnectionString { get; set; }

        public AppSettingOrmConfiguration(IConfiguration configuration)
        {
            this.ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public string GetConnectionString()
        {
            return this.ConnectionString;
        }
    }
}
