namespace SplatkovaKalkulacka
{
    public class CalcResult
    {
        public readonly int owed;
        public readonly int years;
        public readonly double interest;

        public readonly double monthly;
        public readonly double yearly;

        public CalcResult(int owed, int years, double interest, double monthly)
        {
            this.owed = owed;
            this.years = years;
            this.interest = interest;
            this.monthly = monthly;

            yearly = monthly * 12;
        }
    }
}