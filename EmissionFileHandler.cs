using Spectre.Console;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;

namespace Pacres_EcoCalc
{
    internal class EmissionFileHandler : FileHandler
    {
        private const string FilePath = "emission_data.json";
        public void ExportDataToCSV()
        {
            try
            {
                List<EmissionDataStored> dataList = ReadAllData();
                if (dataList == null || dataList.Count == 0)
                {
                    AnsiConsole.MarkupLine("No data available to export.");
                    return;
                }
                const string FilePathExternal = "emissionSummary_compilation.csv";

                using (var writer = new StreamWriter(FilePathExternal))
                {
                    writer.WriteLine("DataName,Date,TotalEmission,ReducedAnnualEmission,NewAnnualEmission,HouseholdMembers," +
                        "AveragePerMemberCurrent,AveragePerMemberReduced,Vehicular,HomeEnergy,Waste," +
                        "ReducedVehicular,ReducedHomeEnergy,ReducedWaste");
                    foreach (var data in dataList)
                    {
                        string line = string.Format(CultureInfo.InvariantCulture, "{0},{1:MM-dd-yyyy},{2:F2},{3:F2},{4:F2},{5}," +
                            "{6:F2},{7:F2},{8:F2},{9:F2},{10:F2},{11:F2},{12:F2},{13:F2}",
                            data.DataName,
                            data.Date,
                            data.TotalEmission,
                            data.ReducedAnnualEmission,
                            data.NewAnnualEmission,
                            data.HouseholdMembers,
                            data.AveragePerMemberCurrent,
                            data.AveragePerMemberReduced,
                            data.totalVehicle,
                            data.totalHE,
                            data.totalWaste,
                            data.reducedTotalVehicle,
                            data.reducedTotalHE,
                            data.reducedTotalWaste
                            );
                        writer.WriteLine(line);
                    }
                }
                AnsiConsole.MarkupLine("\n[green]Data successfully exported to CSV format at [bold]{0}[/].[/]", FilePathExternal);
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]An I/O error occurred while exporting the data!\n Error: {ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
        }
        public void SaveData(EmissionDataStored data)
        {
            List<EmissionDataStored> dataList = ReadAllData() ?? new List<EmissionDataStored>();
            bool exists = dataList.Any(d => d.DataName.Equals(data.DataName, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                string response = AnsiConsole.Prompt(new
                    SelectionPrompt<string>()
                    .Title($"A record with the name \"{data.DataName}\" already exists. Are you sure to overwrite data?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Yes", "No"
                    }
                    ));

                if (response == "Yes")
                {
                    dataList.RemoveAll(d => d.DataName.Equals(data.DataName, StringComparison.OrdinalIgnoreCase));
                    dataList.Add(data);
                    File.WriteAllText(FilePath, JsonSerializer.Serialize(dataList));
                    AnsiConsole.Progress()
                        .AutoClear(false)
                        .HideCompleted(false)
                        .Columns(new ProgressColumn[]
                        {
                            new TaskDescriptionColumn(),
                            new ProgressBarColumn(),
                            new PercentageColumn(),
                            new RemainingTimeColumn(),
                            new SpinnerColumn()
                        })
                        .Start(ctx =>
                        {
                            var task = ctx.AddTask("\n[yellow]Overwriting the existing data...[/]\n");
                            while (!ctx.IsFinished)
                            {
                                task.Increment(1.0);
                                Thread.Sleep(20);
                            }
                        });
                    AnsiConsole.MarkupLine("[gold1]Data overwritten successfully.[/]");
                    ExportDataToCSV();
                }
                else
                {
                    string response2 = AnsiConsole.Prompt(new
                    SelectionPrompt<string>()
                    .Title($"Enter new data name instead?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Yes", "No"
                    }
                    ));

                    if (response2 == "Yes")
                    {
                        string newName;
                        bool uniqueName;

                        do
                        {
                            newName = AnsiConsole.Prompt(new
                                TextPrompt<string>($"Enter new name for the data aside from \"{data.DataName}\": "));
                            uniqueName = !dataList.Any(d => d.DataName.Equals(newName, StringComparison.OrdinalIgnoreCase));
                            if (!uniqueName)
                            {
                                AnsiConsole.MarkupLine("\n[red]This name already exists. Please choose a different name.[/]");
                            }
                            else
                            {
                                data.DataName = newName;
                                dataList.Add(data);
                                File.WriteAllText(FilePath, JsonSerializer.Serialize(dataList));
                                AnsiConsole.Progress()
                                        .AutoClear(false)
                                        .HideCompleted(false)
                                        .Columns(new ProgressColumn[]
                                        {
                                    new TaskDescriptionColumn(),
                                    new ProgressBarColumn(),
                                    new PercentageColumn(),
                                    new RemainingTimeColumn(),
                                    new SpinnerColumn()
                                        })
                                        .Start(ctx =>
                                        {
                                            var task = ctx.AddTask("[green]Saving file...[/]");
                                            while (!ctx.IsFinished)
                                            {
                                                task.Increment(1.5);
                                                Thread.Sleep(30);
                                            }
                                        });
                                ExportDataToCSV();
                                AnsiConsole.MarkupLine("[gold1]Data saved successfully.[/]");
                            }
                        }
                        while (!uniqueName);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\n[grey]The data was not saved...[/]");
                    }
                }
            }
            else
            {
                dataList.Add(data);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(dataList));
                AnsiConsole.Progress()
                        .AutoClear(false)
                        .HideCompleted(false)
                        .Columns(new ProgressColumn[]
                        {
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn(),
                        new RemainingTimeColumn(),
                        new SpinnerColumn()
                        })
                        .Start(ctx =>
                        {
                            var task = ctx.AddTask("[green]Saving file...[/]");
                            while (!ctx.IsFinished)
                            {
                                task.Increment(1.5);
                                Thread.Sleep(30);
                            }
                        });
                ExportDataToCSV();
                AnsiConsole.MarkupLine("[gold1]Data saved successfully.[/]");
            }
        }
        public List<EmissionDataStored> ReadAllData()
        {
            if (File.Exists(FilePath))
            {
                string jsonData = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<EmissionDataStored>>(jsonData);
            }
            return new List<EmissionDataStored>();
        }
        public void UpdateData(string dataName, EmissionDataStored newData)
        {
            try
            {
                List<EmissionDataStored> dataList = ReadAllData();
                bool recordUpdated = false;
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (dataList[i].DataName == dataName)
                    {
                        dataList[i] = newData;
                        recordUpdated = true;
                        break;
                    }
                }
                if (recordUpdated)
                {
                    File.WriteAllText(FilePath, JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true }));
                    ExportDataToCSV();
                    AnsiConsole.MarkupLine("\n[green]Record updated successfully.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[grey]Data not found for the specified name.[/]");
                }
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]An I/O error occurred while updating the data!\n Error: {ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
        }
        public void DeleteData(string dataName)
        {
            try
            {
                List<EmissionDataStored> dataList = ReadAllData();
                int initialCount = dataList.Count;

                dataList.RemoveAll(d => d.DataName == dataName);

                if (dataList.Count < initialCount)
                {
                    File.WriteAllText(FilePath, JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true }));
                    ExportDataToCSV();
                    AnsiConsole.MarkupLine("\n[green]Record deleted successfully.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[grey]Data not found for the specified name.[/]");
                }
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]An I/O error occurred while exporting the data!\n Error: {ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }           
        }
    }
}