using System;
using System.Data;
using Spectre.Console;

namespace SplatkovaKalkulacka
{
    public class TableUi
    {
        private Table table = new Table();
        private Table innerTable = new Table();

        private int owed;
        private int years;
        private double interest;

        public TableUi()
        {
            table.AddColumn(new TableColumn("Splatkova Kalkulacka"));
            
            innerTable.AddColumn(new TableColumn("[blue]Vyse Pujcky[/]").Centered());
            innerTable.AddColumn(new TableColumn("[green]Pocet Let Splaceni[/]").Centered());
            innerTable.AddColumn(new TableColumn("[fuchsia]Vyse uroku[/]").Centered());
            innerTable.AddColumn(new TableColumn("[bold aqua]Mesicni[/] [aqua]Splatka[/]").Centered());
            innerTable.AddColumn(new TableColumn("[bold aqua]Celkova[/] [aqua]Splatka[/]").Centered());
            innerTable.AddRow("0", "0", "0", "-", "-");
            innerTable.Border(TableBorder.Rounded);

            table.AddRow(innerTable);
            table.Border(TableBorder.Rounded);
        }

        public void UpdateValues(int owed, int years, double interest)
        {
            this.owed = owed;
            this.years = years;
            this.interest = interest;
        }

        public void UpdateTable()
        {
            if (owed <= 0)
            {
                throw new ConstraintException("Dluh nesmi byt nula nebo zaporny");
            }
            
            innerTable.UpdateCell(0, 0, owed + " Kc");
            innerTable.UpdateCell(0, 1, years.ToString());
            innerTable.UpdateCell(0, 2, interest + "%");

            if (owed == 0 || years == 0 || interest == 0)
            {
                innerTable.UpdateCell(0, 3, "-");
                innerTable.UpdateCell(0, 4, "-");
            }
            else
            {
                double monthlyPayment = Kalkulacka.CalculateMonthly(owed, years, interest);
                innerTable.UpdateCell(0, 3, Math.Round(monthlyPayment, 3) + " Kc");
                innerTable.UpdateCell(0, 4, Math.Round(monthlyPayment * years * 12, 3) + " Kc");
                Kalkulacka.LastResult = new CalcResult(owed, years, interest, monthlyPayment);
            }
            
            AnsiConsole.Clear();
            AnsiConsole.Write(table);
        }

        public void WriteTable()
        {
            AnsiConsole.Write(table);
        }
        
    }
}