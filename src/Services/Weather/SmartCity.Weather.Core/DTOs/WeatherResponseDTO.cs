/*
 * ===================================================================================
 * TRABALHO PRÁTICO: Integração de Sistemas de Informação (ISI)
 * -----------------------------------------------------------------------------------
 * Nome: Mario Junior Manhente Portilho
 * Número: a27989
 * Curso: Engenharia de Sistemas Informáticos
 * Ano Letivo: 2025/2026
 * ===================================================================================
 */

using System.Text.Json.Serialization;

namespace SmartCity.Weather.Core.DTOs
{
    public class WeatherDto
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; } = string.Empty;

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        [JsonPropertyName("current_units")]
        public CurrentUnitsDto CurrentUnits { get; set; } = new();

        [JsonPropertyName("current")]
        public CurrentWeatherDto Current { get; set; } = new();

        [JsonPropertyName("hourly_units")]
        public HourlyUnitsDto HourlyUnits { get; set; } = new();

        [JsonPropertyName("hourly")]
        public HourlyDataDto Hourly { get; set; } = new();
    }

    public class CurrentUnitsDto
    {
        public string Time { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public string Temperature2M { get; set; } = string.Empty;
        public string WindSpeed10M { get; set; } = string.Empty;
    }

    public class CurrentWeatherDto
    {
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double Temperature2M { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed10M { get; set; }
    }

    public class HourlyUnitsDto
    {
        public string Time { get; set; } = string.Empty;
        public string Temperature2M { get; set; } = string.Empty;
    }

    public class HourlyDataDto
    {
        public List<DateTime> Time { get; set; } = new List<DateTime>();
        public List<double> Temperature2M { get; set; } = new List<double>();
    }
}