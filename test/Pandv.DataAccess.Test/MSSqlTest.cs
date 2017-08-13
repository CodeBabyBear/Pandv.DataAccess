using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using System.Linq;
using Dapper;

namespace Pandv.DataAccess.Test
{
    public class MSSqlTest
    {
        private IServiceProvider _Provider;
        private IDbProvider _DB;
        private List<Student> _Students;

        public MSSqlTest()
        {
            var dir = Directory.GetCurrentDirectory();
            _Provider = new ServiceCollection()
                     .UseSqlServer()
                     .UseDataAccessConfig(dir, false, null, "db.xml")
                     .BuildServiceProvider();

            _DB = _Provider.GetService<IDbProvider>();
            _Students = GenerateStudents(500);
        }

        private List<Student> GenerateStudents(int count)
        {
            var result = new List<Student>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new Student()
                {
                    Age = i,
                    Id = i + 1,
                    JoinDate = DateTime.UtcNow,
                    Name = i.ToString(),
                    Money = i
                });
            }
            return result;
        }

        [Fact(DisplayName = "Clear")]
        public void Clear()
        {
            _DB.Execute("Clear");
        }

        [Fact(DisplayName = "BulkCopy")]
        public void BulkCopy()
        {
            _DB.ExecuteBulkCopy("BulkCopy", _Students);
        }

        [Fact(DisplayName = "SelectAll")]
        public void SelectAll()
        {
            var students =_DB.Query<Student>("SelectAll").ToList();
            Assert.Equal(500, students.Count);
        }

        [Fact(DisplayName = "SelectByName")]
        public void SelectByName()
        {
            var student = _DB.QueryFirstOrDefault<Student>("SelectByName", new { Name = new DbString() { Value = "3", IsAnsi = true } });
            Assert.Equal(3, student.Age);
        }
    }
}
