using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

#nullable enable

namespace HelloWorldWeb.Models
{
    public class AuthService
    {
        private readonly HttpClient _client;
        private readonly string _url;
        private readonly string _apiKey;

        public AuthService(IConfiguration config)
        {
            Console.WriteLine("🚀 AuthService initializing...");

            _url = config["SUPABASE_URL"]!;
            _apiKey = config["SUPABASE_KEY"]!;

            if (string.IsNullOrWhiteSpace(_url) || string.IsNullOrWhiteSpace(_apiKey))
            {
                Console.WriteLine("❌ Missing Supabase ENV vars.");
                throw new Exception("Missing Supabase ENV vars.");
            }

            Console.WriteLine("✅ Supabase config OK");

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("apikey", _apiKey);
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<User?> Authenticate(string username, string password)
        {
            try
            {
                // תיקון בשמות השדות בשאילתות
                var res = await _client.GetAsync($"{_url}/rest/v1/users?username=eq.{username}&password=eq.{password}&select=*");
                var json = await res.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<User>>(json);
                return users?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Auth error: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> Register(string username, string password)
        {
            Console.WriteLine($"📥 Attempting to register user: {username}");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            if (username.Length < 5 || password.Length < 5)
                return false;

            if (!Regex.IsMatch(username, "^[a-zA-Z0-9א-ת]+$") || !Regex.IsMatch(password, "^[a-zA-Z0-9א-ת]+$"))
                return false;

            var existingUser = await GetUser(username);
            if (existingUser != null)
            {
                Console.WriteLine("⚠️ User already exists.");
                return false;
            }

            var newUser = new[]
            {
                new User
                {
                    Username = username,
                    Password = password,
                    CorrectAnswers = 0,
                    TotalAnswered = 0,
                    IsCheater = false,
                    IsBanned = false,
                    LastSeen = DateTime.UtcNow
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            var res = await _client.PostAsync($"{_url}/rest/v1/users", content);

            Console.WriteLine($"📡 Register response: {res.StatusCode}");
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Register failed: {errorContent}");
            }

            return res.IsSuccessStatusCode;
        }

        public async Task<User?> GetUser(string username)
        {
            try
            {
                var res = await _client.GetAsync($"{_url}/rest/v1/users?username=eq.{username}&select=*");
                var json = await res.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<User>>(json);
                return users?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ GetUser error: " + ex.Message);
                return null;
            }
        }

        public async Task UpdateUser(User updatedUser)
        {
            Console.WriteLine($"🔄 UpdateUser called for {updatedUser.Username}");
            Console.WriteLine($"👉 IsCheater = {updatedUser.IsCheater}, IsBanned = {updatedUser.IsBanned}, Correct = {updatedUser.CorrectAnswers}, Total = {updatedUser.TotalAnswered}");

            var patch = new[]
            {
                new {
                    CorrectAnswers = updatedUser.CorrectAnswers,
                    TotalAnswered = updatedUser.TotalAnswered,
                    IsCheater = updatedUser.IsCheater,
                    IsBanned = updatedUser.IsBanned,
                    LastSeen = updatedUser.LastSeen ?? DateTime.UtcNow
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(patch), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_url}/rest/v1/users?username=eq.{updatedUser.Username}")
            {
                Content = content
            };
            request.Headers.Add("Prefer", "return=representation");

            var response = await _client.SendAsync(request);
            Console.WriteLine($"📡 PATCH Response status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ PATCH failed: {errorContent}");
            }
            else
            {
                Console.WriteLine($"✅ User {updatedUser.Username} successfully updated.");
            }
        }

        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                var res = await _client.GetAsync($"{_url}/rest/v1/users?select=*");
                var json = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ GetAllUsers error: " + ex.Message);
                return new List<User>();
            }
        }

        public async Task<bool> DeleteUser(string username)
        {
            Console.WriteLine($"🗑️ Attempting to delete user: {username}");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{_url}/rest/v1/users?username=eq.{username}");
                request.Headers.Add("Prefer", "return=representation");
                var response = await _client.SendAsync(request);

                Console.WriteLine($"📡 DELETE Response: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("❌ DeleteUser failed: " + err);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ DeleteUser error: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> CheckConnection()
        {
            try
            {
                var res = await _client.GetAsync($"{_url}/rest/v1/users?select=username&limit=1");
                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
