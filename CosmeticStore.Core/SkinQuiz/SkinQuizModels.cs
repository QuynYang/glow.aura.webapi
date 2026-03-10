using System.Text.Json.Serialization;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.SkinQuiz;

public class SkinQuizQuestion
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; 
    public List<SkinQuizOption> Options { get; set; } = new();
}

public class SkinQuizOption
{
    public string OptionId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public Dictionary<SkinType, int> SkinTypeScores { get; set; } = new();
}

public class SkinQuizAnswerRequest
{
    public int? UserId { get; set; }
    public List<QuizAnswer> Answers { get; set; } = new();
}

public class QuizAnswer
{
    public int QuestionId { get; set; }
    public string SelectedOptionId { get; set; } = string.Empty;
}

public class SkinQuizResult
{
    public SkinType DeterminedSkinType { get; set; }
    public string SkinTypeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Characteristics { get; set; } = new();
    public List<string> RecommendedIngredients { get; set; } = new();
    public List<string> IngredientsToAvoid { get; set; } = new();
    public List<string> SkincareTips { get; set; } = new();
    public Dictionary<SkinType, int> DetailedScores { get; set; } = new();
    public int ConfidencePercent { get; set; }
    public int MatchingProductCount { get; set; }
    public bool HasSkinTypeDiscount { get; set; }
    public decimal SkinTypeDiscountPercent { get; set; }
}

public class AiSkinQuizResult
{
    [JsonPropertyName("skinType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SkinType SkinType { get; set; }

    [JsonPropertyName("hydrationScore")]
    public int HydrationScore { get; set; }

    [JsonPropertyName("pigmentationScore")]
    public int PigmentationScore { get; set; }

    [JsonPropertyName("oilinessScore")]
    public int OilinessScore { get; set; }

    [JsonPropertyName("sensitivityScore")]
    public int SensitivityScore { get; set; }

    [JsonPropertyName("elasticityScore")]
    public int ElasticityScore { get; set; }

    [JsonPropertyName("aiSummary")]
    public string AiSummary { get; set; } = string.Empty;

    [JsonPropertyName("recommendedIngredients")]
    public List<string> RecommendedIngredients { get; set; } = new();

    [JsonPropertyName("ingredientsToAvoid")]
    public List<string> IngredientsToAvoid { get; set; } = new();
}