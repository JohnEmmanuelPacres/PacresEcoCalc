using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Spectre.Console;

namespace Pacres_EcoCalc
{
    class ManageCarbonFootprintHistory : Display
    {
        private EmissionFileHandler fileHandler;
        public ManageCarbonFootprintHistory()
        {
            fileHandler = new EmissionFileHandler();
        }
        public void display()
        {
            ReadData read = new ReadData(fileHandler);
            read.ReadUserCarbonFootprint();
        }
        public void RUD()
        {
            bool repeat = true;
            do
            {
                AnsiConsole.Clear();
                string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[cyan1]Select an option:[/]")
                .PageSize(10)
                .HighlightStyle(new Style(foreground: Color.Gold1, decoration: Decoration.Bold))
                .AddChoices(new[]
                {
                    ":calendar: [lime]Carbon Footprint History[/]", ":bar_chart: [lime]Monthly/Weekly Analytics[/]", ":magnifying_glass_tilted_left: [lime]Search Data[/]",
                    ":card_file_box:  [lime]Update Data[/]", ":cross_mark_button: [lime]Delete Data[/]", ":file_cabinet:  [lime]Export Data to External Database[/]",
                    ":cross_mark_button: [lime]Delete External Database[/]", "[yellow]Go back to Main Menu[/]"
                }
                ));

                switch (choice)
                {
                    case ":calendar: [lime]Carbon Footprint History[/]":
                        AnsiConsole.Clear();
                        display();
                        break;

                    case ":bar_chart: [lime]Monthly/Weekly Analytics[/]":
                        AnsiConsole.Clear();
                        ReadData analytics = new ReadData(fileHandler);
                        analytics.DisplayAnalytics();
                        break;

                    case ":magnifying_glass_tilted_left: [lime]Search Data[/]":
                        AnsiConsole.Clear();
                        ReadData search = new ReadData(fileHandler);
                        search.SearchData();
                        break;

                    case ":card_file_box:  [lime]Update Data[/]":
                        AnsiConsole.Clear();
                        display();
                        UpdateData update = new UpdateData(fileHandler);
                        update.UpdateUserCarbonFootprint();
                        AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to refresh display...[/]");
                        Console.ReadKey();
                        AnsiConsole.Clear();
                        display();
                        break;

                    case ":cross_mark_button: [lime]Delete Data[/]":
                        AnsiConsole.Clear();
                        display();
                        DeleteData delete = new DeleteData(fileHandler);
                        delete.RemoveUserCarbonFootprint();
                        AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to refresh display...[/]");
                        Console.ReadKey();
                        AnsiConsole.Clear();
                        display();
                        break;

                    case ":file_cabinet:  [lime]Export Data to External Database[/]":
                        AnsiConsole.Clear();
                        ReadData export = new ReadData(fileHandler);
                        export.ExportToCSV();
                        AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                        Console.ReadKey();
                        AnsiConsole.Clear();
                        break;

                    case ":cross_mark_button: [lime]Delete External Database[/]":
                        AnsiConsole.Clear();
                        DeleteData deleteDB = new DeleteData(fileHandler);
                        deleteDB.RemoveCSV();
                        AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                        Console.ReadKey();
                        AnsiConsole.Clear();
                        break;

                    case "[yellow]Go back to Main Menu[/]":
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.WriteLine("\n");
                string backToMenu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Go back to main menu?")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Yes", "No"
                }
                ));
                if ( backToMenu == "Yes")
                {
                    repeat = false;
                    AnsiConsole.MarkupLine("[cyan]Going back to menu...[/]");
                }
                else
                {
                    repeat = true;
                }
            }
            while (repeat);
            AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
        }
    }
}