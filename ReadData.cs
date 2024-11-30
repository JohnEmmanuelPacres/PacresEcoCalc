using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Spectre.Console;
using System.Globalization;

namespace Pacres_EcoCalc
{
    class ReadData
    {
        protected EmissionFileHandler fileHandler;
        public ReadData(EmissionFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
        }

        public void ReadUserCarbonFootprint()
        {
            try
            {
                List<EmissionDataStored> history = fileHandler.ReadAllData();

                if (history.Count == 0)
                {
                    AnsiConsole.MarkupLine("[grey]No carbon footprint history found.[/]");
                    return;
                }
                AnsiConsole.MarkupLine("[yellow]Carbon Footprint History:[/]");
                foreach (var record in history)
                {
                    Console.WriteLine("\n");
                    TableChart(record);
                }
            }
            catch(FileNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[grey]The file does not exist {":white_question_mark:"}[/]");
                AnsiConsole.MarkupLine($"[grey]Error:[/] [red]{ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
            catch(IOException ex)
            {
                AnsiConsole.MarkupLine($"[grey]There is an error in the input or output operation {":double_exclamation_mark:"}[/]");
                AnsiConsole.MarkupLine($"[grey]Error:[/] [red]{ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
            catch(Exception ex) 
            {
                AnsiConsole.MarkupLine("[red]Failed to read the file...[/]");
                AnsiConsole.MarkupLine($"[grey]Error:[/] [red]{ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
        }
        public void TableChart(EmissionDataStored record)
        {
            var table = new Table()
                        .Border(TableBorder.DoubleEdge);
            table.AddColumn("Description");
            table.AddColumn("Value");
            table.AddRow("Data Name", $"{record.DataName}");
            table.AddRow("Date", $"{record.Date:MM-dd-yyyy}");
            table.AddRow("Current Total Annual Emission", $"{record.TotalEmission:F2} lbs");
            table.AddRow("Reducible Emission", $"{record.ReducedAnnualEmission:F2} lbs");
            table.AddRow("Potential Total Annual Emission", $"{record.NewAnnualEmission:F2} lbs");
            table.AddRow("Household Members", $"{record.HouseholdMembers}");
            table.AddRow("Average Emission per Household Member (Current)", $"{record.AveragePerMemberCurrent:F2} lbs");
            table.AddRow("Average Emission per Household Member (Reduced)", $"{record.AveragePerMemberReduced:F2} lbs");
            AnsiConsole.Write(table);

            AnsiConsole.Write(new BarChart()
                .Width(100)
                .Label("\n[green underline] [aqua]Current[/] vs [olive]Potential[/] Total Annual Emission[/]")
                .CenterLabel()
                .AddItem("[aqua]Current[/]", Math.Round(record.TotalEmission, 2), Color.Aqua)
                .AddItem("[olive]Potential[/]", Math.Round(record.NewAnnualEmission, 2), Color.Olive)
                );
            AnsiConsole.MarkupLine("\n");
            AnsiConsole.MarkupLine("[aqua]Current:[/]");
            AnsiConsole.Write(new BreakdownChart()
                .ShowPercentage()
                .Width(100)
                .AddItem("Vehicle", Math.Round(record.totalVehicle / record.TotalEmission * 100, 2), Color.Green)
                .AddItem("Home Energy", Math.Round(record.totalHE / record.TotalEmission * 100, 2), Color.Aquamarine3)
                .AddItem("Waste", Math.Round(record.totalWaste / record.TotalEmission * 100, 2), Color.Grey)
                );
            AnsiConsole.MarkupLine("\n");
            AnsiConsole.MarkupLine("[olive]Reducible:[/]");
            AnsiConsole.Write(new BreakdownChart()
                .ShowPercentage()
                .Width(100)
                .AddItem("Vehicle", Math.Round(record.reducedTotalVehicle / record.ReducedAnnualEmission * 100, 2), Color.Green)
                .AddItem("Home Energy", Math.Round(record.reducedTotalHE / record.ReducedAnnualEmission * 100, 2), Color.Aquamarine3)
                .AddItem("Waste", Math.Round(record.reducedTotalWaste / record.ReducedAnnualEmission * 100, 2), Color.Grey)
                );
        }
        public void SearchData()
        {
            string searchName = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the name of the record you want to read: "));
            try
            {
                List<EmissionDataStored> history = fileHandler.ReadAllData();
                var matchingRecords = history.FindAll(record => record.DataName.Equals(searchName, StringComparison.OrdinalIgnoreCase));

                AnsiConsole.MarkupLine($"\n[gold1]Results for {searchName}[/]:");
                if (matchingRecords.Count == 0)
                {
                    AnsiConsole.MarkupLine("[grey]No records found with the specified data name.[/]");
                }
                foreach (var record in matchingRecords)
                {
                    TableChart(record);
                }
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[grey]The file that you are looking for does not exist {":white_question_mark:"}[/]");
                AnsiConsole.MarkupLine($"[grey]Error:[/] [red]{ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Failed to read the file...[/]");
                AnsiConsole.MarkupLine($"[grey]Error:[/] [red]{ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
        }
        public void ExportToCSV()
        {
            string export = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Do you want to export your current records to csv?")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Yes", "No"
                }
                ));

            if (export == "Yes")
            {
                try
                {
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
                            var task = ctx.AddTask("\n[yellow]Exporting existing records...[/]\n");
                            while (!ctx.IsFinished)
                            {
                                task.Increment(1.0);
                                Thread.Sleep(20);
                            }
                        });
                    fileHandler.ExportDataToCSV();
                }
                catch (FileNotFoundException ex)
                {
                    AnsiConsole.MarkupLine($"[grey]The file could not be found.[/]");
                    AnsiConsole.MarkupLine($"[grey]Error:[/] [red]{ex.Message}[/]");
                    Console.ReadKey();
                    AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                    Menu menu = new Menu();
                    menu.display();
                }
                catch (IOException ex)
                {
                    AnsiConsole.MarkupLine($"[grey]An error occurred during export.[/]");
                    AnsiConsole.MarkupLine($"[grey]Error:[/] [red]{ex.Message}[/]");
                    Console.ReadKey();
                    AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                    Menu menu = new Menu();
                    menu.display();
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine("[red]Data exportation to CSV failed...[/]");
                    AnsiConsole.MarkupLine($"[grey]Error:[/] [red]{ex.Message}[/]");
                    Console.ReadKey();
                    AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                    Menu menu = new Menu();
                    menu.display();
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]Export to CSV was canceled.[/]");
            }
        }

        public void DisplayAnalytics()
        {
            EmissionPeriod selectedPeriod = AnsiConsole.Prompt(
                new SelectionPrompt<EmissionPeriod>()
                .Title("Choose time period for analytics: ")
                .AddChoices(EmissionPeriod.Weekly, EmissionPeriod.Monthly)
                );

            try
            {
                List<EmissionDataStored> history = fileHandler.ReadAllData();
                if(!history.Any())
                {
                    AnsiConsole.MarkupLine("[grey]No available records found for analysis.[/]");
                    AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue.[/]");
                    Console.ReadKey();
                    return;
                }

                var groupedData = selectedPeriod switch
                {
                    EmissionPeriod.Weekly => history.GroupBy(record => new
                    {
                        Year = record.Date.Year,
                        Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            record.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday
                            )
                    })
                    .Select(g => new
                    {
                        Label = $"Week {g.Key.Week} of Year {g.Key.Year}",
                        CurrentEmission = g.Average(record => record.TotalEmission),
                        PotentialEmission = g.Average(record => record.NewAnnualEmission),
                        AveragePerMember_current = g.Average(record => record.AveragePerMemberCurrent),
                        AveragePerMember_reduced = g.Average(record => record.AveragePerMemberReduced),
                        //breakdown
                        CurrentEmissionSum = g.Sum(record => record.TotalEmission),
                        Vehicle = g.Sum(record => record.totalVehicle),
                        HE = g.Sum(record => record.totalHE),
                        Waste = g.Sum(record => record.totalWaste)
                    }),
                    EmissionPeriod.Monthly => history.GroupBy(record => new
                    {
                        Year = record.Date.Year,
                        Month = record.Date.Month
                    })
                    .Select(g => new
                    {
                        Label = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month)} of Year {g.Key.Year}",
                        CurrentEmission = g.Average(record => record.TotalEmission),
                        PotentialEmission = g.Average(record => record.NewAnnualEmission),
                        AveragePerMember_current = g.Average(record => record.AveragePerMemberCurrent),
                        AveragePerMember_reduced = g.Average(record => record.AveragePerMemberReduced),
                        //breakdown
                        CurrentEmissionSum = g.Sum(record => record.TotalEmission),
                        Vehicle = g.Sum(record => record.totalVehicle),
                        HE = g.Sum(record => record.totalHE),
                        Waste = g.Sum(record => record.totalWaste)
                    }),
                    _ => throw new ArgumentOutOfRangeException()
                };

                AnsiConsole.MarkupInterpolated($"\nCarbon Emission [green]{selectedPeriod} Analytics[/]:\n");
                foreach (var group in groupedData)
                {
                    AnsiConsole.Write(new BarChart()
                        .Width(150)
                        .Label($"[lime]{group.Label}[/]")
                        .CenterLabel()
                        .AddItem($"[aqua]Current Emission[/]", Math.Round(group.CurrentEmission, 2), Color.Aqua)
                        .AddItem($"[olive]Potential Emission[/]", Math.Round(group.PotentialEmission, 2), Color.Olive)
                        .AddItem($"[pink1]Current Emission per Household Member[/]", Math.Round(group.AveragePerMember_current, 2), Color.Pink1)
                        .AddItem($"[darkolivegreen1]Reduced Emission Per Household Member[/]", Math.Round(group.AveragePerMember_reduced, 2), Color.DarkOliveGreen1));

                    AnsiConsole.MarkupLine("\n");

                    AnsiConsole.Write(new BreakdownChart()
                            .ShowPercentage()
                            .Width(150)
                            .AddItem("Vehicle", Math.Round((group.Vehicle / group.CurrentEmissionSum) * 100, 2), Color.Green)
                            .AddItem("Home Energy", Math.Round((group.HE / group.CurrentEmissionSum) * 100, 2), Color.Aquamarine3)
                            .AddItem("Waste", Math.Round((group.Waste / group.CurrentEmissionSum) * 100, 2), Color.Grey));

                    AnsiConsole.MarkupLine("\n");
                }
            }
            catch(ArgumentOutOfRangeException ex)
            {
                AnsiConsole.MarkupLine("[red]An error has been encountered during analysis.[/]");
                AnsiConsole.MarkupLine($"Error: {ex.Message}");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                Menu menu = new Menu();
                menu.display();
            }
            catch(Exception ex)
            {
                AnsiConsole.MarkupLine($"Error: {ex.Message}");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                Menu menu = new Menu();
                menu.display();
            }
        }
    }
    class UpdateData : ReadData
    {
        public UpdateData(EmissionFileHandler fileHandler) : base(fileHandler) { }
        public void UpdateUserCarbonFootprint()
        {
            Console.WriteLine("\n");
            string dataName = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the name of the record you want to modify: "));

            double totalEmission, reducedEmission, newAnnualEmission, houseMembers, averagePerHM_current, 
                averagePerHM_reduced, totalVehicularEmission, totalHomeEmission, totalWasteEmission, reducedVehicularEmission, reducedHomeEnergy, reducedWaste;
            //new vehicular
            do
            {
                totalVehicularEmission = AnsiConsole.Prompt(
                new TextPrompt<double>("\nEnter total vehicular emission: "));

                if (totalVehicularEmission <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Invalid value. Please input a positive number.[/]");
                }
            }
            while (totalVehicularEmission <= 0);
            //new home energy
            do
            {
                totalHomeEmission = AnsiConsole.Prompt(
                new TextPrompt<double>("\nEnter total home energy emission: "));

                if (totalHomeEmission <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Invalid value. Please input a positive number.[/]");
                }
            }
            while (totalHomeEmission <= 0);
            //new waste
            do
            {
                totalWasteEmission = AnsiConsole.Prompt(
                new TextPrompt<double>("\nEnter total waste emission: "));

                if (totalWasteEmission <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Invalid value. Please input a positive number.[/]");
                }
            }
            while (totalWasteEmission <= 0);

            totalEmission = totalVehicularEmission + totalHomeEmission + totalWasteEmission;
            AnsiConsole.MarkupLine($"\n[gold1]Your new current total emission is {totalEmission}...[/]");

            //new reduced vehicular
            do
            {
                reducedVehicularEmission = AnsiConsole.Prompt(
                new TextPrompt<double>("\nEnter reduced vehicular emission: "));

                if (reducedVehicularEmission <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Invalid value. Please input a positive number.[/]");
                }
            }
            while (reducedVehicularEmission <= 0);
            //new reduced HE
            do
            {
                reducedHomeEnergy = AnsiConsole.Prompt(
                new TextPrompt<double>("\nEnter reduced home energy: "));

                if (reducedHomeEnergy <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Invalid value. Please input a positive number.[/]");
                }
            }
            while (reducedHomeEnergy <= 0);
            //new reduced waste
            do
            {
                reducedWaste = AnsiConsole.Prompt(
                new TextPrompt<double>("\nNew value for Reducible Emission: "));

                if (reducedWaste <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Invalid value. Please input a positive number.[/]");
                }
            }
            while (reducedWaste <= 0);

            reducedEmission = reducedVehicularEmission + reducedHomeEnergy + reducedWaste;
            AnsiConsole.MarkupLine($"\n[gold1]Your new total reduced emission is {totalEmission}...[/]");

            newAnnualEmission = Math.Abs(totalEmission - reducedEmission);

            do
            {
                houseMembers = AnsiConsole.Prompt(
                new TextPrompt<double>("\nNew value for Household members: "));

                if (houseMembers <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Invalid value. Please input a positive number.[/]");
                }
            }
            while (houseMembers <= 0);

            averagePerHM_current = totalEmission / houseMembers;
            averagePerHM_reduced = newAnnualEmission / houseMembers;

            EmissionDataStored newData = new EmissionDataStored(dataName, DateTime.Now, totalEmission, 
                reducedEmission, newAnnualEmission, houseMembers, averagePerHM_current, 
                averagePerHM_reduced, totalVehicularEmission, totalHomeEmission, totalWasteEmission,
                reducedVehicularEmission, reducedHomeEnergy, reducedWaste);
            fileHandler.UpdateData(dataName, newData);
        }
    }
    class DeleteData : ReadData
    {
        public DeleteData(EmissionFileHandler fileHandler) : base(fileHandler) { }

        public void RemoveUserCarbonFootprint()
        {
            Console.WriteLine("\n");
            AnsiConsole.Markup("Enter the name of the record to [red]delete[/]: ");
            string dataName = Console.ReadLine();
            var askDelete = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Are you sure to [red]delete[/] current record?")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Yes",
                    "No"
                }
                ));
            if (askDelete == "Yes")
            {
                fileHandler.DeleteData(dataName);
            }
        }

        public void RemoveCSV()
        {
            const string FilePathExternal = "emissionSummary_compilation.csv";
            try
            {
                var askDelete = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Are you sure to [red]delete[/] current external database?")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Yes",
                    "No"
                }
                ));

                if (askDelete == "Yes")
                {
                    File.Delete(FilePathExternal);
                    AnsiConsole.MarkupLine("[green]CSV file deleted successfully...[/]");
                }
            }
            catch (FileNotFoundException ex)
            {
                AnsiConsole.MarkupLine("[red]File is not found.[/]");
                AnsiConsole.MarkupLine($"Error:[red] {ex.Message}[/]");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                Menu menu = new Menu();
                menu.display();
            }
        }
    }
}