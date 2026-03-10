using System.Text;
using CosmeticStore.Core.SkinQuiz;

namespace CosmeticStore.Infrastructure.Services;


/// Utility Class: Đóng gói Prompt gửi lên LLM (Google Gemini) (Step 2)

public static class SkinQuizPromptBuilder
{
    
    /// Tạo Prompt hoàn chỉnh từ danh sách câu hỏi và câu trả lời của user
    
    /// <param name="questions">Danh sách toàn bộ câu hỏi (từ SkinQuizQuestions.cs)</param>
    /// <param name="userAnswers">Danh sách câu trả lời user gửi lên</param>
    /// <returns>Chuỗi Prompt chuẩn bị gửi cho Gemini</returns>
    public static string BuildPrompt(List<SkinQuizQuestion> questions, List<QuizAnswer> userAnswers)
    {
        var sb = new StringBuilder();

        // 1. Gán vai trò (System Persona)
        sb.AppendLine("Bạn là một chuyên gia da liễu hàng đầu với hơn 20 năm kinh nghiệm. Nhiệm vụ của bạn là phân tích loại da và tình trạng da của khách hàng dựa trên các câu trả lời trắc nghiệm.");
        sb.AppendLine();
        
        // 2. Cung cấp dữ liệu ngữ cảnh
        sb.AppendLine("Dưới đây là các thông tin khách hàng đã cung cấp:");
        
        for (int i = 0; i < userAnswers.Count; i++)
        {
            var answer = userAnswers[i];
            var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            
            if (question != null)
            {
                var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == answer.SelectedOptionId);
                var answerText = selectedOption?.Text ?? "Không rõ";
                
                sb.AppendLine($"{i + 1}. Câu hỏi: {question.Question}");
                sb.AppendLine($"   Khách hàng chọn: {answerText}");
            }
        }

        sb.AppendLine();

        // 3. Yêu cầu đầu ra (Output Formatting Requirement) - RẤT QUAN TRỌNG ĐỂ PARSE JSON
        sb.AppendLine("Dựa trên thông tin trên, hãy phân tích tình trạng da. BẮT BUỘC trả về kết quả dưới định dạng JSON nguyên chất (không sử dụng Markdown code block như ```json).");
        sb.AppendLine("Cấu trúc JSON bắt buộc phải chính xác như sau:");
        sb.AppendLine("{");
        sb.AppendLine("  \"skinType\": \"Oily | Dry | Normal | Combination | Sensitive\",");
        sb.AppendLine("  \"hydrationScore\": [Điểm độ ẩm từ 0-100],");
        sb.AppendLine("  \"pigmentationScore\": [Điểm sắc tố/đều màu từ 0-100],");
        sb.AppendLine("  \"oilinessScore\": [Điểm tiết dầu từ 0-100],");
        sb.AppendLine("  \"sensitivityScore\": [Điểm nhạy cảm từ 0-100],");
        sb.AppendLine("  \"elasticityScore\": [Điểm độ đàn hồi từ 0-100],");
        sb.AppendLine("  \"aiSummary\": \"[Đoạn văn ngắn gọn khoảng 3-4 câu nhận xét tổng quan tình trạng da, đưa ra lời khuyên chăm sóc]\",");
        sb.AppendLine("  \"recommendedIngredients\": [\"Thành phần 1\", \"Thành phần 2\", \"Thành phần 3\"],");
        sb.AppendLine("  \"ingredientsToAvoid\": [\"Thành phần A\", \"Thành phần B\"]");
        sb.AppendLine("}");

        return sb.ToString();
    }
}