using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

namespace GetLocation
{
    public partial class MainPage : ContentPage
    {
        private double latitude;
        private double longitude;

        public class OpenWeatherData
        {
            public Coord coord { get; set; }
            public Weather[] weather { get; set; }
            public string _base { get; set; }
            public Main main { get; set; }
            public int visibility { get; set; }
            public Wind wind { get; set; }
            public Clouds clouds { get; set; }
            public int dt { get; set; }
            public Sys sys { get; set; }
            public int timezone { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }
        }

        public class Coord
        {
            public float lon { get; set; }
            public float lat { get; set; }
        }

        public class Main
        {
            public float temp { get; set; }
            public float feels_like { get; set; }
            public float temp_min { get; set; }
            public float temp_max { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public int sea_level { get; set; }
            public int grnd_level { get; set; }
        }

        public class Wind
        {
            public float speed { get; set; }
            public int deg { get; set; }
            public float gust { get; set; }
        }

        public class Clouds
        {
            public int all { get; set; }
        }

        public class Sys
        {
            public int type { get; set; }
            public int id { get; set; }
            public string country { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }
        }

        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }




        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GetLocationAndWeather();
        }

        private async Task GetLocationAndWeather()
        {
            Location location = await GetLocation();
            if (location != null)
            {
                latitude = location.Latitude;
                longitude = location.Longitude;
                await GetWeatherData();
            }
            else
            {
                CityLabel.Text = "Unable to get location.";
            }
        }

        private async Task<Location> GetLocation()
        {
            try
            {
                Location location = await Geolocation.Default.GetLocationAsync();
                return location;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine("Location services not supported on this device.");
                return null;
            }
            catch (PermissionException pEx)
            {
                Console.WriteLine("Location permission denied.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to get location: {ex.Message}");
                return null;
            }
        }

        private async Task GetWeatherData()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid=13f3c4e6aaae5782ec1fd60e26c43d65";

                var response = await client.GetStringAsync(url);

                var weatherData = JsonConvert.DeserializeObject<OpenWeatherData>(response);
                CityLabel.Text = weatherData.name;
                CountryLabel.Text = weatherData.sys.country;// Display the city name
                TemperatureLabel.Text = $"{weatherData.main.temp} °C";
                WeatherDescriptionLabel.Text = weatherData.weather[0].description;
                HumidityLabel.Text = $"Humidity: {weatherData.main.humidity}%";
                WindSpeedLabel.Text = $"Wind Speed: {weatherData.wind.speed} m/s";
                WindSpeedLabel.Text = $"Wind Speed: {weatherData.wind.speed} m/s";
                string icon = weatherData.weather[0].icon;
                if (icon.EndsWith("d")) // If it's day time
                {
                    if (icon == "01d") // Clear sky
                        WeatherIcon.Source = ImageSource.FromFile("clear_sky_day_icon.png");
                    else if (icon == "02d") // Few clouds
                        WeatherIcon.Source = ImageSource.FromFile("few_clouds_day_icon.png");
                    else if (icon == "03d") // Scattered clouds
                        WeatherIcon.Source = ImageSource.FromFile("scattered_clouds_icon.jpeg");
                    else if (icon == "04d") // Broken clouds
                        WeatherIcon.Source = ImageSource.FromFile("scattered_clouds_iconpng");
                    else if (icon == "09d") // Shower rain
                        WeatherIcon.Source = ImageSource.FromFile("shower_rain_icon.png");
                    else if (icon == "10d") // Rain
                        WeatherIcon.Source = ImageSource.FromFile("rain_icon.jpeg");
                    else if (icon == "11d") // Thunderstorm
                        WeatherIcon.Source = ImageSource.FromFile("thunderstorm_icon.jpeg");
                    else if (icon == "13d") // Snow
                        WeatherIcon.Source = ImageSource.FromFile("snow_icon.jpeg");
                    else if (icon == "50d") // Mist
                        WeatherIcon.Source = ImageSource.FromFile("mist_icon.png");
                    else if (icon == "04d") // Overcast clouds
                        WeatherIcon.Source = ImageSource.FromFile("overcast.jpeg");

                    else // Unknown
                        WeatherIcon.Source = ImageSource.FromFile("unknown_icon.png");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch weather data: {ex.Message}");
            }
        }

        private async void LocationBtn_Clicked(object sender, EventArgs e)
        {
            await GetLocationAndWeather();
        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            await GetLocationAndWeather();
        }
    }
}
