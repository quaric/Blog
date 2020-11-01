
using System;
using System.Data.Common;
using BlogOblig.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using UnitTestProject;

namespace UnitTestProject
{
    #region SqliteInMemory
    public class SqlLiteInMemoryAccountsControllerTest : AccountsRepositoryUnitTests, IDisposable
    {
        private readonly DbConnection _connection;

        public SqlLiteInMemoryAccountsControllerTest()
            : base(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();
            
            return connection;
        }

        public void Dispose() => _connection.Dispose();
    }
    #endregion
}