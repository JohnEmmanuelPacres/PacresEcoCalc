using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    internal class ReferencePage : Display
    {
        public void display()
        {
            var table = new Table().Border(TableBorder.HeavyEdge);
            table.AddColumn(" ");
            table.AddColumn("Link");
            table.AddRow($"{":leaf_fluttering_in_wind:"} Carbon Footprint Fact Sheet", "[blue][link]https://tinyurl.com/yjf2v3j6[/][/]");
            table.AddRow($"{":leaf_fluttering_in_wind:"} GHG Equivalencies Calculator - Calculations and References", "[blue][link]https://tinyurl.com/yndjdujw[/][/]");
            table.AddRow($"{":leaf_fluttering_in_wind:"} Philippines CO2 Emissions", "[blue][link]https://tinyurl.com/mrverwsz[/][/]");
            table.AddRow($"{":leaf_fluttering_in_wind:"} Visayan Electric September-October billing", "[blue][link]https://tinyurl.com/5cre7zts[/][/]");
            table.AddRow($"{":leaf_fluttering_in_wind:"} Carbon Footprint Calculator | Climate Change | US EPA", "[blue][link]https://tinyurl.com/2mxsa2cv[/][/]");
            table.AddRow($"{":leaf_fluttering_in_wind:"} How to reduce my carbon footprint?", "[blue][link]https://tinyurl.com/hc93462p[/][/]");
            table.AddRow($"{":leaf_fluttering_in_wind:"} Fuel Economy Guide 2023", "[blue][link]https://tinyurl.com/47nchfbe[/][/]");
            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\n");
            var image = new CanvasImage("logo.png");
            image.MaxWidth(12);
            AnsiConsole.Write(Align.Center(image));
        }
    }
}