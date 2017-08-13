using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using VIC.ObjectConfig;
using VIC.ObjectConfig.Xml;

namespace Pandv.DataAccess
{
    public static class ConfigExtensions
    {
        public static IServiceCollection UseSqlServer(this IServiceCollection service)
        {
            return service.AddTransient<IDbConnection, SqlConnection>();
        }

            public static IServiceCollection UseDataAccessConfig(this IServiceCollection service, string basePath, bool isWatch = false, DbConfig[] others = null, params string[] xmlFiles)
        {
            var config = new PhysicalFileConfigBuilder()
                .SetBasePath(basePath)
                .Add(new XmlConfigFileProvider<DbConfig>(DbProvider.DbConfigKey, isWatch, i => BuildConfig(i, others), xmlFiles))
                .Build();
            service.AddSingleton<IDbProvider>(i => new DbProvider(config, i));
            return service;
        }

        public static async Task<DbConfig> BuildConfig(Task<DbConfig>[] vs, DbConfig[] others)
        {
            var config = new DbConfig()
            {
                Sqls = new Dictionary<string, DbSql>()
            };
            var css = new Dictionary<string, string>();
            if (others != null)
            {
                foreach (var i in others)
                {
                    i?.ConnectionStrings?.ForEach(x => css.Add(x.Name, x.ConnectionString));
                    i?.SqlConfigs?.ForEach(x => config.Sqls.Add(x.CommandName, x));
                }
            }
            foreach (var i in vs)
            {
                var c = await i;
                c?.ConnectionStrings?.ForEach(x => css.Add(x.Name, x.ConnectionString));
                c?.SqlConfigs?.ForEach(x => config.Sqls.Add(x.CommandName, x));
            }
            foreach (var item in config.Sqls.Values)
            {
                var connectionString = string.Empty;
                css.TryGetValue(item.ConnectionName, out connectionString);
                item.ConnectionString = connectionString;
            }
            return config;
        }
    }
}