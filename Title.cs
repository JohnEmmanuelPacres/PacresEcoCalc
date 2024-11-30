using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    public class Title : Display
    {
        public void display()
        {
            var layout = new Layout("Root")
                .SplitRows(
                new Layout("Top"), new Layout("Bottom"));

            var font = FigletFont.Load("3d.flf");

            layout["Top"].Update(new Panel
                (
                Align.Center
                (
                    new FigletText(font, "Pacres EcoCalc").Color(Color.Lime),
                    VerticalAlignment.Middle))
                .Expand());

            layout["Bottom"].Update(
                new Panel(
                    Align.Center(
                        new Markup("[bold green slowblink]Press any keys to continue...[/]"),
                        VerticalAlignment.Middle))
                .Expand());

            AnsiConsole.Write(layout);
        }
    }
}