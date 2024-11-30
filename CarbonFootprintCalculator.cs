using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Text.Json;

namespace Pacres_EcoCalc
{
    class CarbonFootprintCalculator : Display
    {
        public void display()
        {
            AnsiConsole.MarkupLine($"[chartreuse4]Here are some {":rock:"}[gold1]ten commandments[/]{":rock:"} to help reduce your carbon footprint:[/]");
            AnsiConsole.MarkupLine($"1. [lightgoldenrod2_2]Use reusable shopping bags instead of products packed in excessive plastic.[/] {":shopping_bags:"}");
            AnsiConsole.MarkupLine($"2. [lightgoldenrod2_2]Ensure that only necessities are bought, to avoid too much waste.[/] {":backhand_index_pointing_up:"}");
            AnsiConsole.MarkupLine($"3. [lightgoldenrod2_2]Take good care of your clothing; try swapping, borrowing, or buying secondhand clothes.[/] {":womans_clothes:"}");
            AnsiConsole.MarkupLine($"4. [lightgoldenrod2_2]When you have heater or air conditioner, adjust the temperature by 1°, it will already make a difference.[/] {":hot_springs:"}");
            AnsiConsole.MarkupLine($"5. [lightgoldenrod2_2]Unplug electronic devices (e.g. phone, computer, laptop) when battery is already full.[/] {":laptop:"}");
            AnsiConsole.MarkupLine($"6. [lightgoldenrod2_2]Drive wisely; plan out when and where you drive.[/] {":racing_car:"}");
            AnsiConsole.MarkupLine($"7. [lightgoldenrod2_2]Try public transport or cycling.[/] {":bicycle:"}");
            AnsiConsole.MarkupLine($"8. [lightgoldenrod2_2]Select an energy efficient label appliances with an ‘A’ label.[/] {":light_bulb:"}");
            AnsiConsole.MarkupLine($"9. [lightgoldenrod2_2]Limit meat consumptions; try local and seasonal products.[/] {":kiwi_fruit:"}");
            AnsiConsole.MarkupLine($"10. [lightgoldenrod2_2]Limit and recycle your wastes.[/] {":wastebasket:"}");
        }
        public void CalculateTotalEmission()
        {
            double totalVehicularEmission = 0;
            double averageCO2 = 10484.00;
            int i;

            var returnToMenu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Proceed to calculation or go back to menu?")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Yes, I want to proceed.",
                    "No, I want to go back to menu."
                }
                ));

            if (returnToMenu == "Yes, I want to proceed.")
            {
                var vehicleRule = new Rule("VEHICULAR EMISSION");
                vehicleRule.Centered();
                AnsiConsole.Write(vehicleRule);

                var period = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\nChoose miles per week or miles per year:")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Miles per week", "Miles per year"
                    }
                    ));

                bool unacceptable = true;
                int vehicleNum = 0;

                while (unacceptable)
                {
                    AnsiConsole.Clear();
                    AnsiConsole.Write(vehicleRule);
                    AnsiConsole.Markup("\n[cyan]How many vehicles do you have (maximum is 10)? [/]");
                    try
                    {
                        vehicleNum = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                    }

                    if (vehicleNum <= 0 || vehicleNum > 10)
                    {
                        unacceptable = true;
                        AnsiConsole.MarkupLine("[red]Please try again...[/]");
                        Console.ReadKey();
                    }
                    else
                    {
                        unacceptable = false;
                    }
                }

                for (i = 1; i <= vehicleNum; i++)
                {
                    double miles;
                    if (period == "Miles per week")
                    {
                        do
                        {
                            miles = AnsiConsole.Prompt(new TextPrompt<double>($"\n[lime]Enter the total miles driven of vehicle {i} in a week:[/]"));
                            if (miles < 0)
                            {
                                AnsiConsole.MarkupLine("[red]Please enter a non-negative number.[/]");
                            }
                        }
                        while (miles < 0);
                    }
                    else
                    {
                        do
                        {
                            miles = AnsiConsole.Prompt(new TextPrompt<double>($"\n[lime]Enter the total miles driven of vehicle {i} in a year:[/]"));
                            if (miles < 0)
                            {
                                AnsiConsole.MarkupLine("[red]Please enter a non-negative number.[/]");
                            }
                        }
                        while (miles < 0);
                    }

                    HouseholdVehicle vehicle = new HouseholdVehicle(miles);
                    double emission = 0;

                    if (period == "Miles per week")
                    {
                        emission = vehicle.CalculateHV(vehicle.Miles, EmissionPeriod.Weekly);
                        AnsiConsole.MarkupLine($"[Cyan]Estimated yearly emissions based on weekly miles for vechicle {i}:[/] [Magenta]{emission:F2} pounds of CO2/year[/]");
                    }
                    else
                    {
                        emission = vehicle.CalculateHV(vehicle.Miles, EmissionPeriod.Yearly);
                        AnsiConsole.MarkupLine($"[Cyan]Estimated yearly emissions for vechicle {i}:[/] [Magenta]{emission:F2} pounds of CO2/year[/]");
                    }

                    totalVehicularEmission += emission;
                }

                Console.WriteLine("\n");
                var rule = new Rule("[yellow]TOTAL VEHICULAR EMISSION[/]");
                rule.Justification = Justify.Left;
                AnsiConsole.Write(rule);
                if (totalVehicularEmission > averageCO2)
                {
                    AnsiConsole.MarkupLine($"Your total average household vehicular emission is [red]{totalVehicularEmission:F2}[/] pounds of carbon dioxide per year" + " :disappointed_face:");
                }
                else
                {
                    AnsiConsole.MarkupLine($"Your total average household vehicular emission is [green]{totalVehicularEmission:F2}[/] pounds of carbon dioxide per year" + " :beaming_face_with_smiling_eyes:");
                }

                AnsiConsole.MarkupLine("[yellow]Reminder: 10484 pounds of carbon dioxide is the yearly average per vehicle.[/]");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();

                //Home Energy
                var homeRule = new Rule("HOME ENERGY EMISSIOM");
                homeRule.Centered();
                AnsiConsole.Write(homeRule);

                double totalHomeEmission = 0;
                double averagePerPerson = 14020;
                var selectedEnergyTypes = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<EnergyType>()
                    .Title("\nSelect the energy sources your household use:")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Lime, decoration: Decoration.Bold))
                    .AddChoices(EnergyType.NaturalGas, EnergyType.FuelOil, EnergyType.Propane, EnergyType.Electricity)
                    );

                //storage variables for reduction portion
                double naturalGas = 0;
                double fuelOil = 0;
                double propane = 0;
                double electricity = 0;

                foreach (var energyType in selectedEnergyTypes)
                {
                    Console.WriteLine("\n");
                    var unitType = AnsiConsole.Prompt(
                        new SelectionPrompt<UnitType>()
                        .Title($"Choose units for {energyType} usage:")
                        .PageSize(10)
                        .AddChoices(energyType switch
                        {
                            EnergyType.NaturalGas => new[] { UnitType.Bill, UnitType.CubicFeet, UnitType.Therm },
                            EnergyType.FuelOil => new[] { UnitType.Bill, UnitType.Gallons },
                            EnergyType.Propane => new[] { UnitType.Bill, UnitType.Gallons },
                            EnergyType.Electricity => new[] { UnitType.Bill, UnitType.kWh },
                            _ => throw new ArgumentException("Invalid energy type.")
                        }
                        ));

                    var promptMessage = unitType == UnitType.Bill
                        ? $"Enter the monthly usage amount for {energyType} in {unitType} (₱): "
                        : $"Enter the monthly usage amount for {energyType} in {unitType}: ";

                    double input;
                    do
                    {
                        input = AnsiConsole.Prompt(
                        new TextPrompt<double>(promptMessage));

                        if (input < 0)
                        {
                            AnsiConsole.MarkupLine("[red]Please enter a non-negative number.[/]");
                        }
                    }
                    while (input < 0);


                    HomeEnergy home = new HomeEnergy(energyType, unitType, input);
                    double emission = home.CalculateHE(energyType, unitType, input);

                    if (energyType == EnergyType.NaturalGas)
                    {
                        naturalGas = emission;
                    }
                    if (energyType == EnergyType.FuelOil)
                    {
                        fuelOil = emission;
                    }
                    if (energyType == EnergyType.Propane)
                    {
                        propane = emission;
                    }
                    if (energyType == EnergyType.Electricity)
                    {
                        electricity = emission;
                    }

                    totalHomeEmission += emission;
                    AnsiConsole.MarkupLine($"[Cyan]Estimated yearly emissions for {energyType} usage:[/] [Magenta]{emission:F2} pounds of carbon dioxide per year[/]");
                }

                Console.WriteLine("\n");
                rule = new Rule("[yellow]TOTAL HOME ENERGY EMISSION[/]");
                rule.Justification = Justify.Left;
                AnsiConsole.Write(rule);

                if (totalHomeEmission > averagePerPerson)
                {
                    AnsiConsole.MarkupLine($"Your total average home energy emission is [red]{totalHomeEmission:F2}[/] pounds of carbon dioxide per year" + " :disappointed_face:");
                }
                else
                {
                    AnsiConsole.MarkupLine($"Your total average home energy emission is [green]{totalHomeEmission:F2}[/] pounds of carbon dioxide per year" + " :beaming_face_with_smiling_eyes:");
                }
                AnsiConsole.MarkupLine("[yellow]Reminder: 14020 pounds of carbon dioxide is the yearly average per household.[/]");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();

                //Waste
                var wasteRule = new Rule("WASTE EMISSION");
                wasteRule.Centered();
                AnsiConsole.Write(wasteRule);

                Console.Write("\n");
                var houseMembers = AnsiConsole.Prompt(
                    new TextPrompt<int>("\nHow many people living in your household?"));
                double averageHousehold = houseMembers * 692;

                var metalRecycle = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\nDo you recycle [green]aluminum[/] and [green]steel[/] cans?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes","No"
                    }
                    ));

                var plasticRecycle = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\nDo you recycle [green]plastic[/]?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes","No"
                    }
                    ));

                var glassRecycle = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\nDo you recycle [green]glass[/]?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes","No"
                    }
                    ));

                var newspaperRecycle = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\nDo you recycle [green]newspaper[/]?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes","No"
                    }
                    ));

                var magazineRecycle = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\nDo you recycle [green]magazines[/]?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes","No"
                    }
                    ));

                Waste waste = new Waste(houseMembers);
                double totalWasteEmission = waste.WasteEmission(houseMembers, metalRecycle,
                    plasticRecycle, glassRecycle, newspaperRecycle, magazineRecycle);

                Console.WriteLine("\n");
                rule = new Rule("[yellow]TOTAL WASTE EMISSION[/]");
                rule.Justification = Justify.Left;
                AnsiConsole.Write(rule);
                AnsiConsole.MarkupLine($"Your total waste emission is [yellow]{totalWasteEmission:F2}[/] pounds of carbon dioxide.");
                AnsiConsole.MarkupLine($"[yellow]Reminder: {averageHousehold} pounds of carbon dioxide from waste is the yearly average per household of {houseMembers}.[/]");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();

                //Current Total Emission Display Here
                var totalEmissionRule = new Rule("[yellow]TOTAL EMISSION[/]");
                rule.Justification = Justify.Center;
                AnsiConsole.Write(totalEmissionRule);
                double totalEmission = totalVehicularEmission + totalHomeEmission + totalWasteEmission;
                AnsiConsole.Write(new BarChart()
                    .Width(100)
                    .Label("\n[Cyan]Current Carbon Emission[/]\n")
                    .CenterLabel()
                    .AddItem("[green]Vehicle[/]", Math.Round(totalVehicularEmission, 2), Color.Green)
                    .AddItem("[aquamarine3]Home Energy[/]", Math.Round(totalHomeEmission, 2), Color.Aquamarine3)
                    .AddItem("[grey]Waste[/]", Math.Round(totalWasteEmission, 2), Color.Grey));

                Console.WriteLine("\n");

                AnsiConsole.Write(new BreakdownChart()
                    .ShowPercentage()
                    .Width(100)
                    .AddItem("Vehicle", Math.Round((totalVehicularEmission / totalEmission) * 100, 2), Color.Green)
                    .AddItem("Home Energy", Math.Round((totalHomeEmission / totalEmission) * 100, 2), Color.Aquamarine3)
                    .AddItem("Waste", Math.Round((totalWasteEmission / totalEmission) * 100, 2), Color.Grey));

                Console.WriteLine("\n");
                AnsiConsole.MarkupLine($"[lime]Your current overall carbon emission is [cyan]{totalEmission:F2}[/][/].");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();

                //REDUCTION
                //Vehicular
                rule = new Rule("[yellow]WHAT CAN YOU DO TO REDUCE YOUR EMISSIONS?[/]");
                rule.Justification = Justify.Center;
                AnsiConsole.Write(rule);

                double percentage = 0;
                double deductTotalVehicularEmission = 0;
                for (i = 1; i <= vehicleNum; i++)
                {
                    double newMiles;
                    do
                    {
                        newMiles = AnsiConsole.Prompt(
                        new TextPrompt<double>($"[lime]\nReduce the number of miles per week you drive [cyan]vehicle {i}[/] by:[/]"));

                        if (newMiles < 0)
                        {
                            AnsiConsole.MarkupLine("[red]Please enter a non-negative number.[/]");
                        }
                    }
                    while (newMiles < 0);
                    ReduceEmissionVehicle newVehicle = new ReduceEmissionVehicle(newMiles);
                    double newVehicleEmission = newVehicle.RE(newMiles);
                    percentage = newVehicle.REpercent(newVehicleEmission, totalEmission);
                    AnsiConsole.MarkupLine($"CO2 pounds per year for vehicle {i}: [lime]{newVehicleEmission:F2}[/]");
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");

                    //WITH MAINTENANCE CALCULATION
                    double newVehicleEmissionWithMaintenance = newVehicle.REdeduct(newMiles);
                    deductTotalVehicularEmission += newVehicleEmissionWithMaintenance;
                }

                var maintenance = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\nDo you perform regular maintenance with your vehicle?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                double reducedVehicularEmission = totalVehicularEmission;
                if (maintenance == "Yes")
                {
                    Console.WriteLine("\n");
                    reducedVehicularEmission -= deductTotalVehicularEmission;
                    ReduceEmissionVehicle newVehicle = new ReduceEmissionVehicle(reducedVehicularEmission);
                    AnsiConsole.MarkupLine($"Carbon dioxide saved through regular maintenance: [lime]{deductTotalVehicularEmission:F2}[/] lbs/year");
                    AnsiConsole.MarkupLine($"Your new vehicular total emission: [lime]{reducedVehicularEmission:F2}[/] lbs/year");
                    percentage = newVehicle.REpercent(reducedVehicularEmission, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    Console.WriteLine("\n");
                    deductTotalVehicularEmission = 0;
                    ReduceEmissionVehicle newVehicle = new ReduceEmissionVehicle();
                    AnsiConsole.MarkupLine($"Carbon dioxide saved through regular maintenance: [lime]{deductTotalVehicularEmission:F2}[/] lbs/year");
                    AnsiConsole.MarkupLine($"Your reduced vehicular total emission: [lime]{reducedVehicularEmission:F2}[/] lbs/year");
                    percentage = newVehicle.REpercent(reducedVehicularEmission, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }

                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();

                //HomeEnergy
                var rule1 = new Rule("[yellow]WHAT CAN YOU DO TO REDUCE YOUR EMISSIONS?[/]");
                rule1.Justification = Justify.Center;
                AnsiConsole.Write(rule1);
                Console.WriteLine("\n");
                //heater 
                rule1 = new Rule("[yellow]Heater[/]");
                rule1.Justification = Justify.Left;
                AnsiConsole.Write(rule1);

                var askThermostat = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Do you have a [yellow]heater[/] at home?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                double estimatedThermostat = 0;

                if (askThermostat == "Yes")
                {
                    var heatSource = AnsiConsole.Prompt(
                    new SelectionPrompt<EnergyType>()
                    .Title("Which heat source do you use?")
                    .PageSize(10)
                    .AddChoices(
                        EnergyType.NaturalGas,
                        EnergyType.FuelOil,
                        EnergyType.Propane,
                        EnergyType.Electricity
                    ));

                    double tempThermo;

                    tempThermo = AnsiConsole.Prompt(
                            new TextPrompt<double>("[aqua]Turn down your household's heater by (Celsius) during cold season:[/]"));

                    if (heatSource == EnergyType.NaturalGas)
                    {
                        ReduceEmissionHousehold heater = new ReduceEmissionHousehold(tempThermo, heatSource);
                        estimatedThermostat = heater.REheater(naturalGas, tempThermo, heatSource);
                        AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on heater: [chartreuse2]{estimatedThermostat:F2}[/] lbs/year");
                        percentage = heater.REpercent(estimatedThermostat, totalEmission);
                        AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                    }
                    else if (heatSource == EnergyType.FuelOil)
                    {
                        ReduceEmissionHousehold heater = new ReduceEmissionHousehold(tempThermo, heatSource);
                        estimatedThermostat = heater.REheater(fuelOil, tempThermo, heatSource);
                        AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on heater: [chartreuse2]{estimatedThermostat:F2}[/] lbs/year");
                        percentage = heater.REpercent(estimatedThermostat, totalEmission);
                        AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                    }
                    else if (heatSource == EnergyType.Propane)
                    {
                        ReduceEmissionHousehold heater = new ReduceEmissionHousehold(tempThermo, heatSource);
                        estimatedThermostat = heater.REheater(propane, tempThermo, heatSource);
                        AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on heater: [chartreuse2]{estimatedThermostat:F2}[/] lbs/year");
                        percentage = (estimatedThermostat / totalEmission) * 100;
                        AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                    }
                    else if (heatSource == EnergyType.Electricity)
                    {
                        ReduceEmissionHousehold heater = new ReduceEmissionHousehold(tempThermo, heatSource);
                        estimatedThermostat = heater.REheater(electricity, tempThermo, heatSource);
                        AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on heater: [chartreuse2]{estimatedThermostat:F2}[/] lbs/year");
                        percentage = heater.REpercent(estimatedThermostat, totalEmission);
                        AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                    }
                }
                else
                {
                    ReduceEmissionHousehold heater = new ReduceEmissionHousehold();
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on heater: [chartreuse2]{estimatedThermostat:F2}[/] lbs/year");
                    percentage = heater.REpercent(estimatedThermostat, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                Console.WriteLine("\n");

                //aircon
                rule1 = new Rule("[cyan2]Air Conditioner[/]");
                rule1.Justification = Justify.Left;
                AnsiConsole.Write(rule1);

                var askAirCon = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Do you have an [cyan2]air conditioner[/] at home?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                double estimatedAirCon = 0;

                if (askAirCon == "Yes")
                {
                    var tempAirCon = AnsiConsole.Prompt(
                            new TextPrompt<double>("[aqua]Turn up your household's air conditioner by (Celsius) during summer:[/]"));
                    ReduceEmissionHousehold aircon = new ReduceEmissionHousehold(tempAirCon);
                    estimatedAirCon = aircon.REairCon(electricity, tempAirCon);
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on air conditioner: [chartreuse2]{estimatedAirCon:F2}[/] lbs/year");
                    percentage = aircon.REpercent(estimatedAirCon, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on air conditioner: [chartreuse2]{estimatedAirCon:F2}[/] lbs/year");
                    ReduceEmissionHousehold aircon = new ReduceEmissionHousehold();
                    percentage = aircon.REpercent(estimatedAirCon, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                Console.WriteLine("\n");

                //computer
                rule1 = new Rule("[turquoise2]Computer[/]");
                rule1.Justification = Justify.Left;
                AnsiConsole.Write(rule1);

                var askComputer = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Do you have a [turquoise2]personal computer[/] or [turquoise2]laptop[/] at home?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                double estimatedComputer = 0;

                if (askComputer == "Yes")
                {
                    var enableSleep = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("Enable [lime]sleep feature[/] on your computer and monitor. Will you take this action?")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                        "Yes", "No"
                        }
                        ));
                    if (enableSleep == "Yes")
                    {
                        double computer = 1;
                        ReduceEmissionHousehold pc = new ReduceEmissionHousehold(computer);
                        estimatedComputer = pc.REComputer(computer);
                        AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on computer: [chartreuse2]{estimatedComputer:F2}[/] lbs/year");
                        percentage = pc.REpercent(estimatedComputer, totalEmission);
                        AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                    }
                    else
                    {
                        ReduceEmissionHousehold pc = new ReduceEmissionHousehold();
                        AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on computer: [chartreuse2]{estimatedComputer:F2}[/] lbs/year");
                        percentage = pc.REpercent(estimatedComputer, totalEmission);
                        AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                    }
                }
                else
                {
                    ReduceEmissionHousehold pc = new ReduceEmissionHousehold();
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on computer: [chartreuse2]{estimatedComputer:F2}[/] lbs/year");
                    percentage = pc.REpercent(estimatedComputer, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                Console.WriteLine("\n");

                //laundry
                rule1 = new Rule("[royalblue1]Laundry[/]");
                rule1.Justification = Justify.Left;
                AnsiConsole.Write(rule1);

                var askLaundry = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Will you wash your clothes by hand instead of using washing machine?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes","No"
                    }
                    ));

                double estimatedLaundry = 0;
                if (askLaundry == "Yes")
                {
                    int laundry;
                    do
                    {
                        laundry = AnsiConsole.Prompt(
                                new TextPrompt<int>("Enter the number of [royalblue1]laundry loads[/] you do in a week:"));
                        if (laundry < 0)
                        {
                            AnsiConsole.MarkupLine("[red]Please input non-negative numbers.[/]");
                        }
                    }
                    while (laundry < 0);

                    ReduceEmissionHousehold load = new ReduceEmissionHousehold(laundry);
                    estimatedLaundry = load.RELaundry(laundry);
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on laundry: [chartreuse2]{estimatedLaundry:F2}[/] lbs/year");
                    percentage = load.REpercent(estimatedLaundry, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    ReduceEmissionHousehold load = new ReduceEmissionHousehold();
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on laundry: [chartreuse2]{estimatedLaundry:F2}[/] lbs/year");
                    percentage = load.REpercent(estimatedLaundry, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                Console.WriteLine("\n");

                //drying rack
                rule1 = new Rule("[fuchsia]Clothesline / Drying Rack[/]");
                rule1.Justification = Justify.Left;
                AnsiConsole.Write(rule1);
                var askDryingRack = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Will you use a [fuchsia]clothesline[/] or [fuchsia]drying rack[/] for your laundry instead of dryer?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));
                double estimatedDryer = 0;
                if (askDryingRack == "Yes")
                {
                    double percent;
                    do
                    {
                        percent = AnsiConsole.Prompt(
                            new TextPrompt<double>("Enter the [lime]percentage[/] of your laundry to be dried in [fuchsia]clothesline[/] or [fuchsia]drying rack[/]:")
                            );
                        if (percent <= 0 || percent > 100)
                        {
                            AnsiConsole.MarkupLine("[red]Please input only positive percentage from 1%-100%.[/]");
                        }
                    }
                    while (percent <= 0 || percent > 100);
                    ReduceEmissionHousehold dryer = new ReduceEmissionHousehold(percent);
                    estimatedDryer = dryer.REDryer(percent);
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on laundry: [chartreuse2]{estimatedDryer:F2}[/] lbs/year");
                    percentage = dryer.REpercent(estimatedDryer, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    ReduceEmissionHousehold dryer = new ReduceEmissionHousehold();
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on laundry: [chartreuse2]{estimatedDryer:F2}[/] lbs/year");
                    percentage = dryer.REpercent(estimatedDryer, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                Console.WriteLine("\n");

                //green energy
                rule1 = new Rule("[green]Green Power[/]");
                rule1.Justification = Justify.Left;
                AnsiConsole.Write(rule1);

                double estimatedGreenEnergy = 0;
                var askGreenEnergy = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Is your household currently purchasing [green]green power[/]?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                if (askGreenEnergy == "Yes")
                {
                    double greenEnergy;
                    do
                    {
                        greenEnergy = AnsiConsole.Prompt(new
                        TextPrompt<double>("How much [lime]percentage[/] of your household's current electricity substituted with green power?")
                        );

                        if (greenEnergy <= 0 || greenEnergy > 100)
                        {
                            AnsiConsole.MarkupLine("[red]Please input only positive percentage from 1%-100%.[/]");
                        }
                    }
                    while (greenEnergy <= 0 || greenEnergy > 100);
                    ReduceEmissionHousehold GE = new ReduceEmissionHousehold(greenEnergy);
                    estimatedGreenEnergy = GE.REGreenEnergy(greenEnergy, electricity);
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on green energy: [chartreuse2]{estimatedGreenEnergy:F2}[/] lbs/year");
                    percentage = GE.REpercent(estimatedGreenEnergy, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    ReduceEmissionHousehold GE = new ReduceEmissionHousehold();
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on green energy: [chartreuse2]{estimatedGreenEnergy:F2}[/] lbs/year");
                    percentage = GE.REpercent(estimatedGreenEnergy, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                Console.WriteLine("\n");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();

                rule1 = new Rule("[yellow]WHAT CAN YOU DO TO REDUCE YOUR EMISSIONS?[/]");
                rule1.Justification = Justify.Center;
                AnsiConsole.Write(rule1);
                Console.WriteLine("\n");

                //energy star products
                var rule2 = new Rule("[lime]ENERGY STAR PRODUCTS[/]");
                rule2.Justification = Justify.Center;
                AnsiConsole.Write(rule2);
                Console.WriteLine("\n");

                //light
                rule2 = new Rule("[gold1]Light Replacement[/]");
                rule2.Justification = Justify.Left;
                AnsiConsole.Write(rule2);

                double estimatedBulbReplace = 0;
                int bulbs;
                do
                {
                    bulbs = AnsiConsole.Prompt(
                        new TextPrompt<int>("How many [gold1]incandescent light bulbs[/] will you replace with 13-watt [lime]ENERGY STAR[/] lights?"));
                    if (bulbs < 0)
                    {
                        AnsiConsole.MarkupLine("[red]Please input zero or non-negative numbers.[/]");
                    }
                }
                while (bulbs < 0);
                ReduceEmissionHousehold lightBulb = new ReduceEmissionHousehold(bulbs);
                estimatedBulbReplace = lightBulb.REBulb(bulbs);
                AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on light replacement: [chartreuse2]{estimatedBulbReplace:F2}[/] lbs/year");
                percentage = lightBulb.REpercent(estimatedBulbReplace, totalEmission);
                AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                Console.WriteLine("\n");

                //refrigerator
                rule2 = new Rule("[grey63]Refrigerator Replacement[/]");
                rule2.Justification = Justify.Left;
                AnsiConsole.Write(rule2);

                var askRefrigerator = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Replace your household's [grey63]old refrigerator[/] with an [lime]ENERGY STAR[/] model. Will you take this action?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                double estimatedRefrigerator = 0;

                if (askRefrigerator == "Yes")
                {
                    double refrigerator = 1;
                    ReduceEmissionHousehold reft = new ReduceEmissionHousehold(refrigerator);
                    estimatedRefrigerator = reft.REFridge(refrigerator);
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on refrigerator: [chartreuse2]{estimatedRefrigerator:F2}[/] lbs/year");
                    percentage = reft.REpercent(estimatedRefrigerator, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    ReduceEmissionHousehold reft = new ReduceEmissionHousehold();
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on refrigerator: [chartreuse2]{estimatedRefrigerator:F2}[/] lbs/year");
                    percentage = reft.REpercent(estimatedRefrigerator, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                Console.WriteLine("\n");

                //gas stove/furnace/boiler
                rule2 = new Rule("[grey63]Gas Stove / Furnace / Boiler[/]");
                rule2.Justification = Justify.Left;
                AnsiConsole.Write(rule2);

                var askFurnace = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Replace your household's [grey63]old gas stove/furnace/boiler[/] with an [lime]ENERGY STAR[/] model. Will you take this action?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                double estimatedFurnace = 0;

                if (askFurnace == "Yes")
                {
                    double furnace = 1;
                    var heatSource = AnsiConsole.Prompt(
                    new SelectionPrompt<EnergyType>()
                    .Title("Which heat source do you use?")
                    .PageSize(10)
                    .AddChoices(
                        EnergyType.NaturalGas,
                        EnergyType.FuelOil
                    ));
                    ReduceEmissionHousehold furn = new ReduceEmissionHousehold(furnace);
                    estimatedFurnace = furn.REFurnace(furnace, heatSource);
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on stove/furnace/boiler: [chartreuse2]{estimatedFurnace:F2}[/] lbs/year");
                    percentage = furn.REpercent(estimatedFurnace, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    ReduceEmissionHousehold furn = new ReduceEmissionHousehold();
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on stove/furnace/boiler: [chartreuse2]{estimatedFurnace:F2}[/] lbs/year");
                    percentage = furn.REpercent(estimatedFurnace, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                Console.WriteLine("\n");

                //single pane windows
                rule2 = new Rule("[cornsilk1]Window Replacement[/]");
                rule2.Justification = Justify.Left;
                AnsiConsole.Write(rule2);

                var askWindows = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Replace [cornsilk1]single-pane windows[/] with [lime]ENERGY STAR[/] windows. Will you take this action?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                double estimatedWindows = 0;

                if (askWindows == "Yes")
                {
                    double windows = 1;
                    var heatSource = AnsiConsole.Prompt(
                    new SelectionPrompt<EnergyType>()
                    .Title("Which heat source do you use?")
                    .PageSize(10)
                    .AddChoices(
                        EnergyType.NaturalGas,
                        EnergyType.FuelOil,
                        EnergyType.Propane,
                        EnergyType.Electricity
                    ));
                    ReduceEmissionHousehold singlePaneWindows = new ReduceEmissionHousehold(windows);
                    estimatedWindows = singlePaneWindows.REWindows(windows, heatSource);
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on single pane windows: [chartreuse2]{estimatedWindows:F2}[/] lbs/year");
                    percentage = singlePaneWindows.REpercent(estimatedWindows, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    ReduceEmissionHousehold singlePaneWindows = new ReduceEmissionHousehold();
                    AnsiConsole.MarkupLine($"Estimated Savings (carbon dioxide emission) on single pane windows: [chartreuse2]{estimatedWindows:F2}[/] lbs/year");
                    percentage = singlePaneWindows.REpercent(estimatedWindows, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }

                double total = 1;
                ReduceEmissionHousehold newHomeEnergy = new ReduceEmissionHousehold(total);
                double reducedHomeEnergy = newHomeEnergy.RE(total, estimatedThermostat, estimatedAirCon,
                    estimatedComputer, estimatedLaundry, estimatedDryer, estimatedGreenEnergy,
                    estimatedBulbReplace, estimatedRefrigerator, estimatedFurnace, estimatedWindows);
                Console.WriteLine("\n");
                AnsiConsole.MarkupLine($"Your reduced home energy total emission: [lime]{reducedHomeEnergy:F2}[/] lbs/year");
                percentage = newHomeEnergy.REpercent(reducedHomeEnergy, totalEmission);
                AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");

                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();

                //Waste Reduction
                AnsiConsole.Clear();
                var rule3 = new Rule("[yellow]WHAT CAN YOU DO TO REDUCE YOUR EMISSIONS?[/]");
                rule3.Justification = Justify.Center;
                AnsiConsole.Write(rule3);
                Console.WriteLine("\n");

                double reducedWaste = 0;
                var askWasteReduction = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Are you willing to start recycling the material(s) you don't currently recycle (such as newspaper, glass, plastic, metal and magazines)? ")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                if (askWasteReduction == "Yes")
                {
                    ReduceEmissionWaste wasteReduction = new ReduceEmissionWaste(houseMembers);
                    reducedWaste = wasteReduction.RE(houseMembers, metalRecycle,
                        plasticRecycle, glassRecycle, newspaperRecycle, magazineRecycle);
                    AnsiConsole.MarkupLine($"Estimated waste savings (CO2 Emission): {reducedWaste:F2}");
                    percentage = wasteReduction.REpercent(reducedWaste, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }
                else
                {
                    ReduceEmissionWaste wasteReduction = new ReduceEmissionWaste();
                    AnsiConsole.MarkupLine($"Your reduced waste total emission: [lime]{reducedWaste:F2}[/] lbs/year");
                    percentage = wasteReduction.REpercent(reducedWaste, totalEmission);
                    AnsiConsole.MarkupLine($"Percent of total emissions: [lime]{percentage:F2}%[/]");
                }

                double totalReducedEmission = reducedVehicularEmission + reducedHomeEnergy + reducedWaste;

                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();

                AnsiConsole.Clear();

                var summary = new Rule("[yellow]SUMMARY[/]");
                summary.Justification = Justify.Left;
                AnsiConsole.Write(summary);

                DateTime currentDateTime = DateTime.Now;
                double newAnnualEmission = Math.Abs(totalEmission - totalReducedEmission);
                double averagePerHM_current = totalEmission / houseMembers;
                double averagePerHM_reduced = newAnnualEmission / houseMembers;

                var table = new Table().Border(TableBorder.DoubleEdge).BorderColor(Color.Gold1);
                AnsiConsole.Live(table)
                    .AutoClear(false)
                    .Overflow(VerticalOverflow.Ellipsis)
                    .Cropping(VerticalOverflowCropping.Top)
                    .Start(ctx =>
                    {
                        table.AddColumn("Description");
                        ctx.Refresh();
                        Thread.Sleep(250);
                        table.AddColumn("Value");
                        ctx.Refresh();
                        Thread.Sleep(250);

                        table.AddRow("Current Date and Time", $"{currentDateTime}");
                        ctx.Refresh();
                        Thread.Sleep(250);
                        table.AddRow("Current Total Annual Emission", $"{totalEmission:F2} lbs");
                        ctx.Refresh();
                        Thread.Sleep(250);
                        table.AddRow("Reducible Annual Emission", $"{totalReducedEmission:F2} lbs");
                        ctx.Refresh();
                        Thread.Sleep(250);
                        table.AddRow("Potential Total Annual Emission", $"{newAnnualEmission:F2} lbs");
                        ctx.Refresh();
                        Thread.Sleep(250);
                        table.AddRow("Household Members", $"{houseMembers}");
                        ctx.Refresh();
                        Thread.Sleep(250);
                        table.AddRow("Average Emission per Household members (current)", $"{averagePerHM_current:F2} lbs");
                        ctx.Refresh();
                        Thread.Sleep(250);
                        table.AddRow("Average Emission per Household members (reduced)", $"{averagePerHM_reduced:F2} lbs");
                        ctx.Refresh();
                        Thread.Sleep(250);
                    });

                if (totalEmission <= newAnnualEmission)
                {
                    var panel = new Panel("[lime]Good job, but you can still do better...\n[/]");
                    panel.Border = BoxBorder.Double;
                    AnsiConsole.Write(panel);
                }
                else
                {
                    var panel = new Panel("[springgreen1]Don't worry, you can still do better...\n[/]");
                    panel.Border = BoxBorder.Double;
                    AnsiConsole.Write(panel);
                }

                AnsiConsole.Write(new BarChart()
                    .Width(100)
                    .Label("\n[Cyan]Current Carbon Emission[/]")
                    .CenterLabel()
                    .AddItem("[green]Vehicle[/]", Math.Round(totalVehicularEmission, 2), Color.Green)
                    .AddItem("[aquamarine3]Home Energy[/]", Math.Round(totalHomeEmission, 2), Color.Aquamarine3)
                    .AddItem("[grey]Waste[/]", Math.Round(totalWasteEmission, 2), Color.Grey));
                Console.WriteLine("\n");
                AnsiConsole.Write(new BreakdownChart()
                    .ShowPercentage()
                    .Width(100)
                    .AddItem("Vehicle", Math.Round((totalVehicularEmission / totalEmission) * 100, 2), Color.Green)
                    .AddItem("Home Energy", Math.Round((totalHomeEmission / totalEmission) * 100, 2), Color.Aquamarine3)
                    .AddItem("Waste", Math.Round((totalWasteEmission / totalEmission) * 100, 2), Color.Grey));
                Console.WriteLine("\n");
                AnsiConsole.Write(new BarChart()
                    .Width(100)
                    .Label("\n[Cyan]Reducible Carbon Emission[/]")
                    .CenterLabel()
                    .AddItem("[green]Vehicle[/]", Math.Round(reducedVehicularEmission, 2), Color.Green)
                    .AddItem("[aquamarine3]Home Energy[/]", Math.Round(reducedHomeEnergy, 2), Color.Aquamarine3)
                    .AddItem("[grey]Waste[/]", Math.Round(reducedWaste, 2), Color.Grey));
                Console.WriteLine("\n");
                AnsiConsole.Write(new BreakdownChart()
                    .ShowPercentage()
                    .Width(100)
                    .AddItem("Vehicle", Math.Round((reducedVehicularEmission / totalReducedEmission) * 100, 2), Color.Green)
                    .AddItem("Home Energy", Math.Round((reducedHomeEnergy / totalReducedEmission) * 100, 2), Color.Aquamarine3)
                    .AddItem("Waste", Math.Round((reducedWaste / totalReducedEmission) * 100, 2), Color.Grey));
                Console.WriteLine("\n");
                AnsiConsole.Write(new BarChart()
                    .Width(100)
                    .Label("\n[Cyan]Current vs Potential Emission[/]")
                    .CenterLabel()
                    .AddItem("[aqua]Current[/]", Math.Round(totalEmission, 2), Color.Aqua)
                    .AddItem("[olive]Potential[/]", Math.Round(newAnnualEmission, 2), Color.Olive)
                    );
                Console.WriteLine("\n");
                display();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Console.ReadKey();

                AnsiConsole.Clear();

                //save data
                EmissionFileHandler fileHandler = new EmissionFileHandler();
                string askSave = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Save your carbon history data?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "Yes", "No"
                    }
                    ));

                if (askSave == "Yes")
                {
                    string dataName = AnsiConsole.Prompt(new
                        TextPrompt<string>("Enter name for the data: "));
                    EmissionDataStored emissionData = new EmissionDataStored(dataName, currentDateTime,
                        totalEmission, totalReducedEmission, newAnnualEmission, houseMembers,
                        averagePerHM_reduced, averagePerHM_current, totalVehicularEmission, totalHomeEmission,
                        totalWasteEmission, reducedVehicularEmission, reducedHomeEnergy, reducedWaste)
                    {
                        DataName = dataName,
                        Date = currentDateTime,
                        TotalEmission = totalEmission,
                        ReducedAnnualEmission = totalReducedEmission,
                        NewAnnualEmission = newAnnualEmission,
                        HouseholdMembers = houseMembers,
                        AveragePerMemberCurrent = averagePerHM_current,
                        AveragePerMemberReduced = averagePerHM_reduced,
                        //additional
                        totalVehicle = totalVehicularEmission,
                        totalHE = totalHomeEmission,
                        totalWaste = totalWasteEmission,
                        reducedTotalVehicle = reducedVehicularEmission,
                        reducedTotalHE = reducedHomeEnergy,
                        reducedTotalWaste = reducedWaste
                    };

                    fileHandler.SaveData(emissionData);
                }
                //back to menu
                AnsiConsole.MarkupLine("[cyan]Going back to menu...[/]");
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
            }
        }
    }
}