using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.SkinQuiz;
using CosmeticStore.Infrastructure.DbContext;

namespace CosmeticStore.Infrastructure.Services;

public class GeminiSkinQuizService : ISkinQuizService
{
    private readonly StoreDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ISystemLogger _logger;

    public GeminiSkinQuizService(StoreDbContext dbContext, HttpClient httpClient, IConfiguration configuration, ISystemLogger logger)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public List<SkinQuizQuestion> GetQuestions() => SkinQuizQuestions.Questions;

    public async Task<SkinQuizResult> AnalyzeSkinTypeAsync(List<QuizAnswer> answers)
    {
        try
        {
            var questions = GetQuestions();
            var prompt = SkinQuizPromptBuilder.BuildPrompt(questions, answers);
            var apiKey = _configuration["Gemini:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey)) throw new Exception("Thiếu Gemini ApiKey trong appsettings.json");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
            var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseString);
            
            // Fix cảnh báo CS8602 (kiểm tra null trước khi dùng)
            var textResult = jsonDoc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString();

            if (string.IsNullOrEmpty(textResult)) throw new Exception("AI trả về kết quả rỗng");

            var cleanJson = CleanJson(textResult);
            var aiData = JsonSerializer.Deserialize<AiSkinQuizResult>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (aiData == null) throw new Exception("Không thể Deserialize JSON từ AI");

            var baseDetails = GetSkinTypeDetails(aiData.SkinType);

            return new SkinQuizResult
            {
                DeterminedSkinType = aiData.SkinType,
                SkinTypeName = baseDetails.Name,
                Description = aiData.AiSummary,
                Characteristics = baseDetails.Characteristics,
                RecommendedIngredients = aiData.RecommendedIngredients.Any() ? aiData.RecommendedIngredients : baseDetails.RecommendedIngredients,
                IngredientsToAvoid = aiData.IngredientsToAvoid.Any() ? aiData.IngredientsToAvoid : baseDetails.IngredientsToAvoid,
                SkincareTips = baseDetails.SkincareTips,
                ConfidencePercent = 95,
                HasSkinTypeDiscount = true,
                SkinTypeDiscountPercent = 5m
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Gemini AI Quiz Error", ex);
            return new SkinQuizResult { DeterminedSkinType = SkinType.Normal, SkinTypeName = "Lỗi kết nối AI", Description = "Hệ thống AI đang bận." };
        }
    }

    public async Task<bool> SaveUserSkinTypeAsync(int userId, SkinType skinType)
    {
        try 
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return false;

            // Fix lỗi CS0200: Gọi đúng hàm CompleteSkinQuiz đã có trong User.cs
            user.CompleteSkinQuiz(skinType);

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lưu Skin Quiz cho user {userId}", ex);
            return false;
        }
    }

    public SkinTypeDetails GetSkinTypeDetails(SkinType skinType) => SkinTypeInfo.Details.GetValueOrDefault(skinType, new SkinTypeDetails { Name = "Unknown" });

    public async Task<int> CountMatchingProductsAsync(SkinType skinType) => await _dbContext.Products.CountAsync(p => p.SkinType == skinType && p.Stock > 0);

    public async Task<bool> HasCompletedQuizAsync(int userId) => (await _dbContext.Users.FindAsync(userId))?.HasCompletedSkinQuiz ?? false;

    private string CleanJson(string json) => json.Replace("```json", "").Replace("```", "").Trim();
}