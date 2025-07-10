using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtraHours.API.Model
{
    [Table("extra_hours_config")]
    public class ExtraHoursConfig
    {
        [Key]
        [Column("id")]
        public long id { get; set; }

        [Required]
        [Column("weeklyExtraHoursLimit")]
        public double weeklyExtraHoursLimit { get; set; }

        // [Required]
        // [Column("diurnalMultiplier")]
        // public double diurnalMultiplier { get; set; }

        // [Required]
        // [Column("nocturnalMultiplier")]
        // public double nocturnalMultiplier { get; set; }

        // [Required]
        // [Column("diurnalHolidayMultiplier")]
        // public double diurnalHolidayMultiplier { get; set; }

        // [Required]
        // [Column("nocturnalHolidayMultiplier")]
        // public double nocturnalHolidayMultiplier { get; set; }

        [Required]
        [Column("diurnalStart", TypeName = "time")]
        public TimeSpan diurnalStart { get; set; }

        [Required]
        [Column("diurnalEnd", TypeName = "time")]
        public TimeSpan diurnalEnd { get; set; }

        // Permitir deserialización flexible para diurnalStart y diurnalEnd
        [System.Text.Json.Serialization.JsonPropertyName("diurnalStart")]
        public string diurnalStartString
        {
            get => diurnalStart.ToString();
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Intenta parsear como HH:mm:ss
                    if (TimeSpan.TryParse(value, out var ts))
                    {
                        diurnalStart = ts;
                    }
                    // Intenta parsear como HH:mm
                    else if (TimeSpan.TryParseExact(value, "hh:mm", null, out var ts2))
                    {
                        diurnalStart = ts2;
                    }
                }
            }
        }

        [System.Text.Json.Serialization.JsonPropertyName("diurnalEnd")]
        public string diurnalEndString
        {
            get => diurnalEnd.ToString();
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Intenta parsear como HH:mm:ss
                    if (TimeSpan.TryParse(value, out var ts))
                    {
                        diurnalEnd = ts;
                    }
                    // Intenta parsear como HH:mm
                    else if (TimeSpan.TryParseExact(value, "hh:mm", null, out var ts2))
                    {
                        diurnalEnd = ts2;
                    }
                }
            }
        }
    }
}
