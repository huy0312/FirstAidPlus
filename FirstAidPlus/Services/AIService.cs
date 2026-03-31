using System.Text.RegularExpressions;

namespace FirstAidPlus.Services
{
    public interface IAIService
    {
        string GetReply(string userMessage);
        Task<(bool IsValid, string Message)> VerifyCertificateAsync(Microsoft.AspNetCore.Http.IFormFile file, string expectedName, int? expectedExperienceYears);
    }

    public class MockAIService : IAIService
    {
        public string GetReply(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return "Tôi có thể giúp gì cho bạn?";

            var msg = message.ToLower();

            // Greeting
            if (Regex.IsMatch(msg, @"\b(hi|hello|xin chào|chào)\b"))
                return "Xin chào! Tôi là trợ lý AI của Firstaid+. Tôi có thể giúp gì cho bạn về các khóa học sơ cấp cứu?";

            // CPR
            if (msg.Contains("cpr") || msg.Contains("hồi sức"))
                return "CPR (Hồi sức tim phổi) là kỹ thuật cứu sinh khẩn cấp. Khóa học CPR của chúng tôi bao gồm ép tim ngoài lồng ngực và hô hấp nhân tạo. Bạn có muốn đăng ký khóa học này không?";

            // Price / Cost
            if (msg.Contains("giá") || msg.Contains("bao nhiêu") || msg.Contains("chi phí"))
                return "Các khóa học của chúng tôi có giá từ 500,000 VNĐ cho khóa cơ bản đến 2,000,000 VNĐ cho các chứng chỉ chuyên sâu quốc tế. Bạn quan tâm đến khóa học nào cụ thể?";

            // Contact / Support
            if (msg.Contains("liên hệ") || msg.Contains("gặp") || msg.Contains("tư vấn") || msg.Contains("support"))
                return "Bạn có thể liên hệ hotline 1900 1818 hoặc gửi email đến support@firstaid.vn. Đội ngũ hỗ trợ của chúng tôi hoạt động 24/7.";

            // Certificates
            if (msg.Contains("chứng chỉ") || msg.Contains("bằng"))
                return "Chứng chỉ của FirstAid+ được công nhận quốc tế và có giá trị trong 2 năm. Chúng tôi liên kết với Hội Chữ Thập Đỏ và Hiệp hội Tim mạch Hoa Kỳ (AHA).";

            return "Cảm ơn câu hỏi của bạn. Tôi ở đây để giúp đỡ về thông tin các khóa đào tạo sơ cứu và CPR, chứng chỉ và dịch vụ của chúng tôi. Bạn có thể cung cấp thêm chi tiết về điều bạn cần không?";
        }

        public System.Threading.Tasks.Task<(bool IsValid, string Message)> VerifyCertificateAsync(Microsoft.AspNetCore.Http.IFormFile file, string expectedName, int? expectedExperienceYears)
        {
            return System.Threading.Tasks.Task.FromResult((true, "Chứng chỉ hợp lệ (Mock)"));
        }
    }
}
