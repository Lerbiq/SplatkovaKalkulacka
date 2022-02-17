using System;
using Spectre.Console;

namespace SplatkovaKalkulacka
{
    public class Kalkulacka
    {
        private static Table _table = null;
        private static Table _innerTable = null;
        
        static void Main()
        {
            AnsiConsole.Clear();
            _table = new Table();
            _table.AddColumn(new TableColumn("Splatkova Kalkulacka"));

            _innerTable = new Table();
            _innerTable.AddColumn(new TableColumn("[blue]Vyse Pujcky[/]").Centered());
            _innerTable.AddColumn(new TableColumn("[green]Pocet Let Splaceni[/]").Centered());
            _innerTable.AddColumn(new TableColumn("[fuchsia]Vyse uroku[/]").Centered());
            _innerTable.AddColumn(new TableColumn("[bold aqua]Mesicni[/] [aqua]Splatka[/]").Centered());
            _innerTable.AddColumn(new TableColumn("[bold aqua]Celkova[/] [aqua]Splatka[/]").Centered());
            _innerTable.AddRow("0", "0", "0", "-", "-");
            _innerTable.Border(TableBorder.Rounded);

            _table.AddRow(_innerTable);
            _table.Border(TableBorder.Rounded);

            AnsiConsole.Write(_table);

            var owed = GetOwed();
            UpdateTable(owed, 0, 0);


            var years = GetYears();
            UpdateTable(owed, years, 0);

            var interest = GetInterest();
            UpdateTable(owed, years, interest);
            
            
            bool quit = false;

            while (quit != true)
            {
                Action action = GetAction();

                switch (action)
                {
                    case Action.ChangeOwed:
                    {
                        owed = GetOwed();
                        UpdateTable(owed, years, interest);
                        break;
                    }
                    case Action.ChangeYears:
                    {
                        years = GetYears();
                        UpdateTable(owed, years, interest);
                        break;
                    }
                    case Action.ChangeInterest:
                    {
                        interest = GetInterest();
                        UpdateTable(owed, years, interest);
                        break;
                    }
                    case Action.Quit:
                    {
                        quit = true;
                        continue;
                    }
                }   
            }
        }
        
        public static double CalculateMonthly(int owed, int years, double interest)
        {
            var i = interest / 100 / 12;
            var v = 1 / (1 + i);
            return (i * owed) / (1 - Math.Pow(v, years * 12));
        }

        static int GetOwed()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>("Zadej vysi [bold blue]pujcky[/]:")
                    .Validate(owed =>
                    {
                        if (owed <= 0)
                        {
                            return ValidationResult.Error("[bold red]Dluh musi byt kladny[/]");
                        }
                        else
                        {
                            return ValidationResult.Success();
                        }
                    }));
        }

        static int GetYears()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>("Zadej [bold green]pocet let[/] splaceni:")
                    .Validate(years =>
                    {
                        if (years <= 0)
                        {
                            return ValidationResult.Error("[bold red]Pocet let musi byt kladny[/]");
                        }
                        else
                        {
                            return ValidationResult.Success();
                        }
                    }));
        }

        static double GetInterest()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<double>("Zadej [bold fuchsia]vysi uroku[/] v procentech:")
                    .Validate(interest =>
                    {
                        return interest switch
                        {
                            <= 0 => ValidationResult.Error("[bold red]Urok musi byt kladny[/]"),
                            > 100 => ValidationResult.Error("[bold red]Urok musi byt maximalne 100%[/]"),
                            _ => ValidationResult.Success(),
                        };
                    }));
        }
        
        static Action GetAction()
        {
            if (!AnsiConsole.Profile.Capabilities.Ansi)
            {
                var action = AnsiConsole.Prompt(
                    new TextPrompt<int>("1. Zmenit vysi pujcky\n2. Zmenit pocet let splaceni\n3. Zmenit vysi uroku\n0. Ukoncit\n Vyber akci:")
                        .Validate(action =>
                        {
                            if (action == 1 || action == 2 || action == 3 || action == 0)
                            {
                                return ValidationResult.Success();
                            }
                            else
                            {
                                return ValidationResult.Error("[bold red]Neplatna akce. Pro zvoleni zadejte cislo akce:[/]");
                            }
                        }));
                
                switch (action)
                {
                    case 1:
                    {
                        return Action.ChangeOwed;
                    }
                    case 2:
                    {
                        return Action.ChangeYears;
                    }
                    case 3:
                    {
                        return Action.ChangeInterest;
                    }
                    case 0:
                    {
                        return Action.Quit;
                    }
                }
            }
            else
            {
                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Vyberte akci:")
                        .PageSize(4)
                        .AddChoices(new[] {
                            "Zmenit vysi pujcky", "Zmenit pocet let splaceni", "Zmenit vysi uroku", "Ukoncit",
                        }));
                
                switch (action)
                {
                    case "Zmenit vysi pujcky":
                    {
                        return Action.ChangeOwed;
                    }
                    case "Zmenit pocet let splaceni":
                    {
                        return Action.ChangeYears;
                    }
                    case "Zmenit vysi uroku":
                    {
                        return Action.ChangeInterest;
                    }
                    case "Ukoncit":
                    {
                        return Action.Quit;
                    }
                }
            }

            return Action.Quit;
        }

        static void UpdateTable(int owed, int years, double interest)
        {
            _innerTable.UpdateCell(0, 0, owed + " Kc");
            _innerTable.UpdateCell(0, 1, years.ToString());
            _innerTable.UpdateCell(0, 2, interest + "%");

            if (owed == 0 || years == 0 || interest == 0)
            {
                _innerTable.UpdateCell(0, 3, "-");
                _innerTable.UpdateCell(0, 4, "-");
            }
            else
            {
                double monthlyPayment = CalculateMonthly(owed, years, interest);
                _innerTable.UpdateCell(0, 3, Math.Round(monthlyPayment, 3) + " Kc");
                _innerTable.UpdateCell(0, 4, Math.Round(monthlyPayment * years * 12, 3) + " Kc");   
            }
            AnsiConsole.Clear();
            AnsiConsole.Write(_table);
        }

        private enum Action
        {
            ChangeOwed,
            ChangeYears,
            ChangeInterest,
            Quit
        }
    }
}