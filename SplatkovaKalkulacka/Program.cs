using System;
using Spectre.Console;

namespace SplatkovaKalkulacka
{
    public class Kalkulacka
    {
        public static CalcResult LastResult;
        
        static void Main()
        {
            //TODO: Ask for path
            string path = ".\\hypoteky.txt";
            
            TableUi tableUi = new TableUi();
            tableUi.WriteTable();

            var owed = GetOwed();
            tableUi.UpdateValues(owed, 0, 0);
            tableUi.UpdateTable();

            var years = GetYears();
            tableUi.UpdateValues(owed, years, 0);
            tableUi.UpdateTable();

            var interest = GetInterest();
            tableUi.UpdateValues(owed, years, interest);
            tableUi.UpdateTable();


            bool quit = false;

            while (quit != true)
            {
                Action action = GetAction();

                switch (action)
                {
                    case Action.ChangeOwed:
                    {
                        owed = GetOwed();
                        tableUi.UpdateValues(owed, years, interest);
                        tableUi.UpdateTable();
                        break;
                    }
                    case Action.ChangeYears:
                    {
                        years = GetYears();
                        tableUi.UpdateValues(owed, years, interest);
                        tableUi.UpdateTable();
                        break;
                    }
                    case Action.ChangeInterest:
                    {
                        interest = GetInterest();
                        tableUi.UpdateValues(owed, years, interest);
                        tableUi.UpdateTable();
                        break;
                    }
                    case Action.Save:
                    {
                        FileUtil.WriteToFile(path, LastResult);
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
                    new TextPrompt<int>(
                            "1. Zmenit vysi pujcky\n2. Zmenit pocet let splaceni\n3. Zmenit vysi uroku\n4. Ulozit vypocet\n0. Ukoncit\n Vyber akci:")
                        .Validate(action =>
                        {
                            if (action == 1 || action == 2 || action == 3 || action == 4 || action == 0)
                            {
                                return ValidationResult.Success();
                            }
                            else
                            {
                                return ValidationResult.Error(
                                    "[bold red]Neplatna akce. Pro zvoleni zadejte cislo akce:[/]");
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
                    case 4:
                    {
                        return Action.Save;
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
                        .AddChoices(new[]
                        {
                            "Zmenit vysi pujcky", "Zmenit pocet let splaceni", "Zmenit vysi uroku", "Ulozit vypocet", "Ukoncit",
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
                    case "Ulozit vypocet":
                    {
                        return Action.Save;
                    }
                    case "Ukoncit":
                    {
                        return Action.Quit;
                    }
                }
            }

            return Action.Quit;
        }

        private enum Action
        {
            ChangeOwed,
            ChangeYears,
            ChangeInterest,
            Save,
            Quit
        }
    }
}