namespace ExtraHours.API.Tests
{
    using System;
    using System.Linq;
    using Xunit;
    using ExtraHours.API.Service.Implementations;

    public class ColombianHolidayServiceTests
    {
        [Theory]
        [InlineData("2025-01-01", true)]  // Año Nuevo (fijo)
        [InlineData("2025-05-01", true)]  // Día del Trabajo (fijo)
        [InlineData("2025-01-06", true)]  // Reyes Magos (cae lunes, no se traslada)
        [InlineData("2025-01-13", false)] // Lunes normal (no es festivo)
        [InlineData("2025-04-13", true)]  // Domingo (cualquier domingo es festivo)
        [InlineData("2025-04-17", true)]  // Jueves Santo (dep. de Pascua, no se traslada)
        [InlineData("2025-04-18", true)]  // Viernes Santo (dep. de Pascua, no se traslada)
        [InlineData("2025-07-20", true)]  // Día de la Independencia (fijo)
        [InlineData("2025-07-21", false)] // Día normal
        [InlineData("2025-03-24", true)]  // San José trasladado (19 marzo cae miércoles → lunes 24)
        [InlineData("2025-03-19", false)] // San José fecha original (no es festivo, se traslada)
        [InlineData("2025-06-30", true)]  // San Pedro y San Pablo trasladado (29 junio cae domingo → lunes 30)
        [InlineData("2025-06-29", true)]  // San Pedro fecha original domingo (domingo siempre es festivo)
        [InlineData("2025-08-18", true)]  // La Asunción trasladado (15 agosto cae viernes → lunes 18)
        [InlineData("2025-10-13", true)]  // Día de la Raza trasladado (12 octubre cae domingo → lunes 13)
        [InlineData("2025-11-03", true)]  // Todos los Santos trasladado (1 noviembre cae sábado → lunes 3)
        [InlineData("2025-11-17", true)]  // Independencia de Cartagena trasladado (11 noviembre cae martes → lunes 17)
        public void IsPublicHoliday_MultipleDates(string dateString, bool expected)
        {
            // Arrange
            var service = new ColombianHolidayService();
            var testDate = DateTime.Parse(dateString);

            // Act
            var result = service.IsPublicHoliday(testDate);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsPublicHoliday_AllSundays_ReturnsTrue()
        {
            // Arrange
            var service = new ColombianHolidayService();
            var testDates = new[]
            {
                new DateTime(2025, 1, 5),   // Domingo
                new DateTime(2025, 2, 2),   // Domingo
                new DateTime(2025, 3, 9),   // Domingo
                new DateTime(2025, 12, 28)  // Domingo
            };

            // Act & Assert
            foreach (var date in testDates)
            {
                Assert.True(service.IsPublicHoliday(date), $"El domingo {date:yyyy-MM-dd} debería ser festivo");
            }
        }

        [Fact]
        public void IsPublicHoliday_EasterDependentHolidays_2025()
        {
            // Arrange
            var service = new ColombianHolidayService();

            // Pascua 2025 es el 20 de abril
            // Act & Assert
            Assert.True(service.IsPublicHoliday(new DateTime(2025, 4, 17)), "Jueves Santo 2025");
            Assert.True(service.IsPublicHoliday(new DateTime(2025, 4, 18)), "Viernes Santo 2025");

            // Ascensión: 43 días después de Pascua (2 de junio, lunes)
            Assert.True(service.IsPublicHoliday(new DateTime(2025, 6, 2)), "Ascensión del Señor 2025");

            // Corpus Christi: 64 días después de Pascua (23 de junio, lunes)
            Assert.True(service.IsPublicHoliday(new DateTime(2025, 6, 23)), "Corpus Christi 2025");

            // Sagrado Corazón: 71 días después de Pascua (30 de junio, lunes)
            Assert.True(service.IsPublicHoliday(new DateTime(2025, 6, 30)), "Sagrado Corazón 2025");
        }

        [Fact]
        public void GetHolidaysForYear_2025_ReturnsCorrectCount()
        {
            // Arrange
            var service = new ColombianHolidayService();

            // Act
            var holidays = service.GetHolidaysForYear(2025);

            // Assert
            Assert.Equal(18, holidays.Count); // Colombia tiene 18 festivos al año
            Assert.All(holidays, h => Assert.True(h.Date.Year == 2025));
            Assert.Contains(holidays, h => h.Name == "Año Nuevo");
            Assert.Contains(holidays, h => h.Name == "Navidad");
        }

        [Fact]
        public void EmilianiLawApplies_DifferentYears_ReturnsCorrectValue()
        {
            // Arrange
            var service = new ColombianHolidayService();

            // Act & Assert
            Assert.False(service.EmilianiLawApplies(1983)); // Antes de la ley
            Assert.True(service.EmilianiLawApplies(1984));  // Año de inicio
            Assert.True(service.EmilianiLawApplies(2025));  // Años posteriores
            Assert.True(service.EmilianiLawApplies(2030));  // Años futuros
        }

        [Theory]
        [InlineData(1899)]
        [InlineData(3001)]
        public void IsPublicHoliday_InvalidYear_ThrowsArgumentException(int year)
        {
            // Arrange
            var service = new ColombianHolidayService();
            var date = new DateTime(year, 1, 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.IsPublicHoliday(date));
        }

        [Fact]
        public void IsPublicHoliday_BeforeEmilianiLaw_NoTransfers()
        {
            // Arrange
            var service = new ColombianHolidayService();

            // En 1980, Reyes Magos caía martes 6 de enero - antes de Ley Emiliani
            var reyesMagos1980 = new DateTime(1980, 1, 6); // Martes
            var siguienteLunes1980 = new DateTime(1980, 1, 7); // Miércoles (no lunes)

            // Act & Assert
            Assert.True(service.IsPublicHoliday(reyesMagos1980)); // Se celebraba en fecha original
            Assert.False(service.IsPublicHoliday(siguienteLunes1980)); // No se trasladaba
        }
    }
}