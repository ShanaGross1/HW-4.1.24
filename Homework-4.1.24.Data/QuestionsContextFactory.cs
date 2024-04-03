﻿using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_2._1._24.Data
{
    public class QuestionsDataContextFactory : IDesignTimeDbContextFactory<QuestionsDataContext>
    {
        public QuestionsDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(),
               $"..{Path.DirectorySeparatorChar}Homework-4.1.24.Web"))
               .AddJsonFile("appsettings.json")
               .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new QuestionsDataContext(config.GetConnectionString("ConStr"));
        }
    }
}
