using CsvHelper;
using Prediction.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Prediction.Core.Services
{
    public class DataInputService : IDataInputService
    {
        public ICollection<DataInput> ReadFile(string path)
        {
            ICollection<DataInput> results = new List<DataInput>();

            using (StreamReader reader = File.OpenText(path))
            {
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration
                {
                    Delimiter = ";",
                }))
                {
                    while (csv.Read())
                    {
                        results.Add(new DataInput
                        {
                            Id = csv.GetField<int>(0),
                            HomeTeamId = csv.GetField<int>(1),
                            HomeTeam = csv.GetField<string>(2),
                            AwayTeamId = csv.GetField<int>(3),
                            AwayTeam = csv.GetField<string>(4),
                            HomeScore = csv.GetField<int>(5),
                            AwayScore = csv.GetField<int>(6),
                            DateStart = csv.GetField<DateTime>(7),
                        });
                    }
                }
            }
            return results;
        }
    }
}
