using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    class Menu : Display
    {
        public void display()
        {
            bool retry = true;

            while (retry)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("\n");
                var image = new CanvasImage("earth.png");
                image.MaxWidth(12);
                AnsiConsole.Write(Align.Center(image));

                string phrase1 = ":abacus: [lime]Calculate Carbon Footprint[/]";
                string phrase2 = ":antenna_bars: [lime]Manage Carbon Footprint History[/]";
                string phrase3 = ":bookmark_tabs: [lime]References[/]";
                string phrase4 = ":grinning_cat_with_smiling_eyes: [yellow]Exit[/]";

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("\n[aquamarine3]What do you want to do?[/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.Gold1, decoration: Decoration.Bold))
                        .AddChoices(new[]
                        {
                        phrase1,phrase2,phrase3, phrase4
                        }
                        ));

                switch (choice)
                {
                    case ":abacus: [lime]Calculate Carbon Footprint[/]":
                        Console.Clear();
                        CarbonFootprintCalculator calculator = new CarbonFootprintCalculator();
                        calculator.CalculateTotalEmission();
                        Console.ReadKey();
                        break;
                    case ":antenna_bars: [lime]Manage Carbon Footprint History[/]":
                        Console.Clear();
                        ManageCarbonFootprintHistory history = new ManageCarbonFootprintHistory();
                        history.RUD();
                        Console.ReadKey();
                        break;
                    case ":bookmark_tabs: [lime]References[/]":
                        Console.Clear();
                        ReferencePage page = new ReferencePage();
                        page.display();
                        Console.ReadKey();
                        break;
                    case ":grinning_cat_with_smiling_eyes: [yellow]Exit[/]":
                        var exit = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                            .Title("Are you sure you want to [red]exit[/] the program?")
                            .PageSize(10)
                            .AddChoices(new[]
                            {
                                "Yes", "No"
                            }
                            ));

                        if (exit == "Yes")
                        {
                            retry = false;
                        }
                        else
                        {
                            retry = true;
                        }
                        break;
                }
            }
        }
    }
}