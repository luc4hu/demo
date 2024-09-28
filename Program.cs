using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace cons;

internal static class Program
{
    private static async Task Main()
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = "\n"
        };

        using var reader = new StreamReader("/home/luca/projects/cons/db.csv");
        await using var writer = new StreamWriter("/home/luca/projects/cons/csharp.csv");
        using var csvReader = new CsvReader(reader, csvConfig);
        await using var csvWriter = new CsvWriter(writer, csvConfig);

        // Read header
        await csvReader.ReadAsync();
        csvReader.ReadHeader();
        int headerLength = csvReader.HeaderRecord!.Length;
        
        // Write header
        foreach (string header in csvReader.HeaderRecord)
        {
            csvWriter.WriteField(header);
        }
        await csvWriter.NextRecordAsync();
        
        // Read rest
        while (await csvReader.ReadAsync())
        {
            for (int i = 0; i < headerLength; i++)
            {
                string? field = csvReader.GetField(i);
                var shouldQuote = field != "(null)";
                
                if (Guid.TryParse(field, out Guid _))
                {
                    csvWriter.WriteField(Guid.NewGuid().ToString(), shouldQuote);
                }
                else
                {
                    csvWriter.WriteField(field, shouldQuote);
                }
            }

            await csvWriter.NextRecordAsync();
        }
    }
}