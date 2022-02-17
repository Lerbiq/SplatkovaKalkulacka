using System;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace TestKalkulacka
{
    public class UnitTest1
    {
        [Fact]
        public void Splatka_ValidniArgumenty_SplatkaDleVzorce()
        {
            double splatka = SplatkovaKalkulacka.Kalkulacka.CalculateMonthly(50000, 2, 2);
            Assert.Equal(2127.01, splatka, 2);
        }
    }
}