using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.SkinQuiz;

/// <summary>
/// Bộ câu hỏi trắc nghiệm xác định loại da
/// 
/// AI SKIN QUIZ:
/// - 10 câu hỏi phân tích toàn diện
/// - Mỗi câu trả lời có điểm số cho từng loại da
/// - Tổng điểm cao nhất xác định loại da chính
/// </summary>
public static class SkinQuizQuestions
{
    /// <summary>
    /// Danh sách câu hỏi
    /// </summary>
    public static readonly List<SkinQuizQuestion> Questions = new()
    {
        new SkinQuizQuestion
        {
            Id = 1,
            Question = "Sau khi rửa mặt 30 phút, da bạn cảm thấy thế nào?",
            Category = "Oiliness",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "1a", Text = "Bóng dầu, đặc biệt ở vùng T", SkinTypeScores = new() { { SkinType.Oily, 3 }, { SkinType.Combination, 1 } } },
                new() { OptionId = "1b", Text = "Căng, khô, hơi khó chịu", SkinTypeScores = new() { { SkinType.Dry, 3 } } },
                new() { OptionId = "1c", Text = "Bình thường, thoải mái", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "1d", Text = "Vùng T hơi dầu, má thì khô", SkinTypeScores = new() { { SkinType.Combination, 3 } } },
                new() { OptionId = "1e", Text = "Hơi đỏ, ngứa nhẹ", SkinTypeScores = new() { { SkinType.Sensitive, 3 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 2,
            Question = "Lỗ chân lông của bạn trông như thế nào?",
            Category = "Pores",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "2a", Text = "To, dễ thấy, đặc biệt ở mũi và má", SkinTypeScores = new() { { SkinType.Oily, 3 }, { SkinType.Combination, 1 } } },
                new() { OptionId = "2b", Text = "Nhỏ, gần như không thấy", SkinTypeScores = new() { { SkinType.Dry, 3 } } },
                new() { OptionId = "2c", Text = "Vừa phải, cân đối", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "2d", Text = "To ở vùng T, nhỏ ở má", SkinTypeScores = new() { { SkinType.Combination, 3 } } },
                new() { OptionId = "2e", Text = "Nhỏ nhưng hay bị đỏ xung quanh", SkinTypeScores = new() { { SkinType.Sensitive, 2 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 3,
            Question = "Bạn có thường xuyên bị mụn không?",
            Category = "Acne",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "3a", Text = "Có, thường xuyên bị mụn đầu đen, mụn ẩn", SkinTypeScores = new() { { SkinType.Oily, 3 } } },
                new() { OptionId = "3b", Text = "Hiếm khi, nhưng hay bị khô nứt", SkinTypeScores = new() { { SkinType.Dry, 2 } } },
                new() { OptionId = "3c", Text = "Rất ít, thỉnh thoảng 1-2 nốt", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "3d", Text = "Có, chủ yếu ở vùng trán và mũi", SkinTypeScores = new() { { SkinType.Combination, 3 } } },
                new() { OptionId = "3e", Text = "Có, nhưng kèm theo đỏ, viêm", SkinTypeScores = new() { { SkinType.Sensitive, 2 }, { SkinType.Oily, 1 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 4,
            Question = "Da bạn phản ứng thế nào với các sản phẩm skincare mới?",
            Category = "Sensitivity",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "4a", Text = "Bình thường, không có vấn đề gì", SkinTypeScores = new() { { SkinType.Normal, 2 }, { SkinType.Oily, 1 } } },
                new() { OptionId = "4b", Text = "Đôi khi bị khô hơn", SkinTypeScores = new() { { SkinType.Dry, 2 } } },
                new() { OptionId = "4c", Text = "Thường bị đỏ, ngứa, châm chích", SkinTypeScores = new() { { SkinType.Sensitive, 3 } } },
                new() { OptionId = "4d", Text = "Vùng má hay bị kích ứng hơn vùng T", SkinTypeScores = new() { { SkinType.Combination, 2 }, { SkinType.Sensitive, 1 } } },
                new() { OptionId = "4e", Text = "Có thể bị nổi mụn nếu quá dầu", SkinTypeScores = new() { { SkinType.Oily, 2 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 5,
            Question = "Đến cuối ngày, da bạn trông như thế nào?",
            Category = "EndOfDay",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "5a", Text = "Rất bóng dầu, makeup trôi", SkinTypeScores = new() { { SkinType.Oily, 3 } } },
                new() { OptionId = "5b", Text = "Căng, có thể bong tróc", SkinTypeScores = new() { { SkinType.Dry, 3 } } },
                new() { OptionId = "5c", Text = "Gần như buổi sáng, bình thường", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "5d", Text = "Vùng T bóng, má vẫn ổn hoặc hơi khô", SkinTypeScores = new() { { SkinType.Combination, 3 } } },
                new() { OptionId = "5e", Text = "Có thể đỏ hơn, mệt mỏi", SkinTypeScores = new() { { SkinType.Sensitive, 2 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 6,
            Question = "Da bạn phản ứng thế nào với thời tiết thay đổi?",
            Category = "Weather",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "6a", Text = "Dầu hơn khi nóng, bình thường khi lạnh", SkinTypeScores = new() { { SkinType.Oily, 2 } } },
                new() { OptionId = "6b", Text = "Khô hơn, nứt nẻ khi lạnh", SkinTypeScores = new() { { SkinType.Dry, 3 } } },
                new() { OptionId = "6c", Text = "Không thay đổi nhiều", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "6d", Text = "Thay đổi tùy vùng trên mặt", SkinTypeScores = new() { { SkinType.Combination, 2 } } },
                new() { OptionId = "6e", Text = "Dễ bị đỏ, kích ứng khi thời tiết khắc nghiệt", SkinTypeScores = new() { { SkinType.Sensitive, 3 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 7,
            Question = "Khi soi gương gần, bạn thấy gì?",
            Category = "Texture",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "7a", Text = "Lỗ chân lông to, mụn đầu đen", SkinTypeScores = new() { { SkinType.Oily, 3 } } },
                new() { OptionId = "7b", Text = "Vảy khô, nếp nhăn nhỏ", SkinTypeScores = new() { { SkinType.Dry, 3 } } },
                new() { OptionId = "7c", Text = "Da mịn, đều màu", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "7d", Text = "Khác biệt giữa vùng T và má", SkinTypeScores = new() { { SkinType.Combination, 3 } } },
                new() { OptionId = "7e", Text = "Mạch máu nhỏ, vùng đỏ", SkinTypeScores = new() { { SkinType.Sensitive, 3 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 8,
            Question = "Bạn cần dùng bao nhiêu kem dưỡng ẩm?",
            Category = "Hydration",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "8a", Text = "Rất ít hoặc không cần, sợ bóng dầu", SkinTypeScores = new() { { SkinType.Oily, 3 } } },
                new() { OptionId = "8b", Text = "Nhiều, vẫn cảm thấy thiếu ẩm", SkinTypeScores = new() { { SkinType.Dry, 3 } } },
                new() { OptionId = "8c", Text = "Lượng vừa phải là đủ", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "8d", Text = "Nhiều cho má, ít cho vùng T", SkinTypeScores = new() { { SkinType.Combination, 3 } } },
                new() { OptionId = "8e", Text = "Cẩn thận chọn loại dịu nhẹ", SkinTypeScores = new() { { SkinType.Sensitive, 2 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 9,
            Question = "Kem chống nắng phù hợp với bạn là?",
            Category = "Sunscreen",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "9a", Text = "Dạng gel, không dầu, kiềm dầu", SkinTypeScores = new() { { SkinType.Oily, 3 } } },
                new() { OptionId = "9b", Text = "Dạng cream, dưỡng ẩm cao", SkinTypeScores = new() { { SkinType.Dry, 3 } } },
                new() { OptionId = "9c", Text = "Bất kỳ loại nào cũng ổn", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "9d", Text = "Kết hợp: nhẹ vùng T, dưỡng ẩm vùng má", SkinTypeScores = new() { { SkinType.Combination, 2 } } },
                new() { OptionId = "9e", Text = "Mineral/Physical, hypoallergenic", SkinTypeScores = new() { { SkinType.Sensitive, 3 } } }
            }
        },
        new SkinQuizQuestion
        {
            Id = 10,
            Question = "Vấn đề da bạn lo lắng nhất là gì?",
            Category = "MainConcern",
            Options = new List<SkinQuizOption>
            {
                new() { OptionId = "10a", Text = "Mụn, bóng dầu, lỗ chân lông to", SkinTypeScores = new() { { SkinType.Oily, 3 } } },
                new() { OptionId = "10b", Text = "Khô, nếp nhăn, thiếu độ đàn hồi", SkinTypeScores = new() { { SkinType.Dry, 3 } } },
                new() { OptionId = "10c", Text = "Duy trì làn da khỏe mạnh", SkinTypeScores = new() { { SkinType.Normal, 3 } } },
                new() { OptionId = "10d", Text = "Cân bằng các vùng da khác nhau", SkinTypeScores = new() { { SkinType.Combination, 3 } } },
                new() { OptionId = "10e", Text = "Kích ứng, đỏ, dị ứng", SkinTypeScores = new() { { SkinType.Sensitive, 3 } } }
            }
        }
    };

    /// <summary>
    /// Lấy câu hỏi theo ID
    /// </summary>
    public static SkinQuizQuestion? GetQuestion(int id) => Questions.FirstOrDefault(q => q.Id == id);

    /// <summary>
    /// Lấy option theo question ID và option ID
    /// </summary>
    public static SkinQuizOption? GetOption(int questionId, string optionId)
    {
        var question = GetQuestion(questionId);
        return question?.Options.FirstOrDefault(o => o.OptionId == optionId);
    }
}

