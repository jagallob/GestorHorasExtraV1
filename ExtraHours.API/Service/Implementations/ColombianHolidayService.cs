namespace ExtraHours.API.Service.Implementations
{
    public class ColombianHolidayService
    {
        // Año desde el cual aplica la Ley Emiliani (traslados de festivos)
        private const int EMILIANI_LAW_START_YEAR = 1984;

        // Festivos fijos en Colombia
        private static readonly Dictionary<(int month, int day), string> FixedHolidays = new Dictionary<(int month, int day), string>
        {
            { (1, 1), "Año Nuevo" },
            { (5, 1), "Día del Trabajo" },
            { (7, 20), "Día de la Independencia" },
            { (8, 7), "Batalla de Boyacá" },
            { (12, 8), "Día de la Inmaculada Concepción" },
            { (12, 25), "Navidad" }
        };

        // Festivos que se trasladan al siguiente lunes (solo desde 1984)
        private static readonly Dictionary<(int month, int day), string> MovableHolidays = new Dictionary<(int month, int day), string>
        {
            { (1, 6), "Día de los Reyes Magos" },
            { (3, 19), "Día de San José" },
            { (6, 29), "San Pedro y San Pablo" },
            { (8, 15), "La Asunción" },
            { (10, 12), "Día de la Raza" },
            { (11, 1), "Día de Todos los Santos" },
            { (11, 11), "Independencia de Cartagena" }
        };

        // Festivos que dependen de la Pascua
        private static readonly Dictionary<int, string> EasterDependentHolidays = new Dictionary<int, string>
        {
            { -3, "Jueves Santo" },
            { -2, "Viernes Santo" },
            { 43, "Ascensión del Señor" },
            { 64, "Corpus Christi" },
            { 71, "Sagrado Corazón" }
        };

        /// <summary>
        /// Verifica si una fecha es festivo en Colombia
        /// </summary>
        /// <param name="date">Fecha a verificar</param>
        /// <returns>True si es festivo, False en caso contrario</returns>
        /// <exception cref="ArgumentException">Si el año es anterior a 1900 o posterior a 3000</exception>
        public bool IsPublicHoliday(DateTime date)
        {
            // Validación de rango de años razonable
            if (date.Year < 1900 || date.Year > 3000)
            {
                throw new ArgumentException($"Año {date.Year} fuera del rango soportado (1900-3000)", nameof(date));
            }

            // Validar si es domingo
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return true;

            // Verificar festivos fijos
            if (FixedHolidays.ContainsKey((date.Month, date.Day)))
                return true;

            // Verificar festivos trasladables (solo aplica desde 1984 - Ley Emiliani)
            if (date.Year >= EMILIANI_LAW_START_YEAR)
            {
                foreach (var holiday in MovableHolidays)
                {
                    var originalDate = new DateTime(date.Year, holiday.Key.month, holiday.Key.day);

                    // Si la fecha original es lunes, se celebra ese día
                    if (originalDate.DayOfWeek == DayOfWeek.Monday)
                    {
                        if (date.Date == originalDate.Date)
                            return true;
                    }
                    else
                    {
                        // Si no es lunes, se traslada al siguiente lunes
                        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)originalDate.DayOfWeek + 7) % 7;
                        if (daysUntilMonday == 0) daysUntilMonday = 7;

                        var movedDate = originalDate.AddDays(daysUntilMonday);
                        if (date.Date == movedDate.Date)
                            return true;
                    }
                }
            }
            else
            {
                // Antes de 1984, los festivos trasladables se celebraban en su fecha original
                foreach (var holiday in MovableHolidays)
                {
                    var originalDate = new DateTime(date.Year, holiday.Key.month, holiday.Key.day);
                    if (date.Date == originalDate.Date)
                        return true;
                }
            }

            // Verificar festivos que dependen de la Pascua
            var easterSunday = CalculateEasterSunday(date.Year);
            foreach (var kvp in EasterDependentHolidays)
            {
                int offset = kvp.Key;
                var holidayDate = easterSunday.AddDays(offset);

                // Jueves Santo y Viernes Santo nunca se trasladan
                if (offset == -3 || offset == -2)
                {
                    if (date.Date == holidayDate.Date)
                        return true;
                }
                else if (date.Year >= EMILIANI_LAW_START_YEAR)
                {
                    // Otros festivos de Pascua se trasladan al lunes si no caen en lunes (desde 1984)
                    if (holidayDate.DayOfWeek == DayOfWeek.Monday)
                    {
                        if (date.Date == holidayDate.Date)
                            return true;
                    }
                    else
                    {
                        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)holidayDate.DayOfWeek + 7) % 7;
                        if (daysUntilMonday == 0) daysUntilMonday = 7;

                        var movedDate = holidayDate.AddDays(daysUntilMonday);
                        if (date.Date == movedDate.Date)
                            return true;
                    }
                }
                else
                {
                    // Antes de 1984, se celebraban en su fecha original
                    if (date.Date == holidayDate.Date)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Obtiene todos los festivos de un año específico
        /// </summary>
        /// <param name="year">Año para el cual obtener los festivos</param>
        /// <returns>Lista de fechas de festivos (excluyendo domingos regulares)</returns>
        public List<(DateTime Date, string Name)> GetHolidaysForYear(int year)
        {
            if (year < 1900 || year > 3000)
            {
                throw new ArgumentException($"Año {year} fuera del rango soportado (1900-3000)", nameof(year));
            }

            var holidays = new List<(DateTime Date, string Name)>();

            // Agregar festivos fijos
            foreach (var holiday in FixedHolidays)
            {
                holidays.Add((new DateTime(year, holiday.Key.month, holiday.Key.day), holiday.Value));
            }

            // Agregar festivos trasladables
            foreach (var holiday in MovableHolidays)
            {
                var originalDate = new DateTime(year, holiday.Key.month, holiday.Key.day);

                if (year >= EMILIANI_LAW_START_YEAR)
                {
                    if (originalDate.DayOfWeek == DayOfWeek.Monday)
                    {
                        holidays.Add((originalDate, holiday.Value));
                    }
                    else
                    {
                        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)originalDate.DayOfWeek + 7) % 7;
                        if (daysUntilMonday == 0) daysUntilMonday = 7;

                        var movedDate = originalDate.AddDays(daysUntilMonday);
                        holidays.Add((movedDate, $"{holiday.Value} (trasladado)"));
                    }
                }
                else
                {
                    holidays.Add((originalDate, holiday.Value));
                }
            }

            // Agregar festivos de Pascua
            var easterSunday = CalculateEasterSunday(year);
            foreach (var kvp in EasterDependentHolidays)
            {
                int offset = kvp.Key;
                var holidayDate = easterSunday.AddDays(offset);

                if (offset == -3 || offset == -2)
                {
                    holidays.Add((holidayDate, kvp.Value));
                }
                else if (year >= EMILIANI_LAW_START_YEAR)
                {
                    if (holidayDate.DayOfWeek == DayOfWeek.Monday)
                    {
                        holidays.Add((holidayDate, kvp.Value));
                    }
                    else
                    {
                        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)holidayDate.DayOfWeek + 7) % 7;
                        if (daysUntilMonday == 0) daysUntilMonday = 7;

                        var movedDate = holidayDate.AddDays(daysUntilMonday);
                        holidays.Add((movedDate, $"{kvp.Value} (trasladado)"));
                    }
                }
                else
                {
                    holidays.Add((holidayDate, kvp.Value));
                }
            }

            return holidays.OrderBy(h => h.Date).ToList();
        }

        /// <summary>
        /// Calcula la fecha del Domingo de Pascua para un año dado usando el algoritmo de Butcher
        /// Funciona correctamente para años 1900-3000
        /// </summary>
        private DateTime CalculateEasterSunday(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Indica si la Ley Emiliani (traslado de festivos) aplica para un año dado
        /// </summary>
        /// <param name="year">Año a verificar</param>
        /// <returns>True si aplica la ley de traslados</returns>
        public bool EmilianiLawApplies(int year) => year >= EMILIANI_LAW_START_YEAR;
    }
}