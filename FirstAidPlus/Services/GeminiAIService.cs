using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FirstAidPlus.Services
{
    public class GeminiAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"];
        }

        public string GetReply(string userMessage)
        {
            // Sync wrapper for async method (simplified for this interface)
            return GetReplyAsync(userMessage).Result;
        }

        private async Task<string> GetReplyAsync(string userMessage)
        {
            if (string.IsNullOrEmpty(_apiKey)) return "Chưa cấu hình API Key.";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = "Bạn là FirstAid Supporter - Trợ lý ảo chuyên nghiệp của FirstAidPlus. Hãy trả lời ngắn gọn, thân thiện và hữu ích bằng tiếng Việt. Câu hỏi: " + userMessage }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Attempt to parse the error message from Google
                    try {
                         var errorNode = JsonNode.Parse(responseString);
                         string errorMsg = errorNode?["error"]?["message"]?.ToString();
                         
                         if (responseString.Contains("Permission denied") || (errorMsg != null && errorMsg.Contains("Permission denied")))
                         {
                             return "Lỗi cấu hình: Vui lòng bật 'Generative Language API' trong Google Cloud Console.";
                         }

                         if(!string.IsNullOrEmpty(errorMsg)) return $"Lỗi AI: {errorMsg}";
                    } catch {}
                    
                    return $"Xin lỗi, sự cố kết nối AI ({response.StatusCode}). Check API Key.";
                }

                var root = JsonNode.Parse(responseString);
                string text = root?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                
                return text ?? "Xin lỗi, tôi không hiểu câu hỏi.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Xin lỗi, hệ thống đang bận. Vui lòng thử lại sau.";
            }
        }

        public async Task<(bool IsValid, string Message)> VerifyCertificateAsync(Microsoft.AspNetCore.Http.IFormFile file, string expectedName, int? expectedExperienceYears)
        {
            if (string.IsNullOrEmpty(_apiKey)) return (false, "Chưa cấu hình API Key cho quá trình xác thực AI.");
            if (file == null || file.Length == 0) return (false, "Vui lòng tải lên ảnh chứng chỉ.");

            // Convert IFormFile to base64
            string base64Image;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var imageBytes = ms.ToArray();
                base64Image = Convert.ToBase64String(imageBytes);
            }

            var mimeType = file.ContentType;
            if (string.IsNullOrEmpty(mimeType) || !mimeType.StartsWith("image/"))
            {
                return (false, "File không phải là định dạng hình ảnh hợp lệ.");
            }

            // Using gemini-2.5-flash as requested
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var systemPrompt = $@"Bạn là một chuyên gia kiểm duyệt chứng chỉ y tế tại Việt Nam. 
Nhiệm vụ của bạn là kiểm tra xem hình ảnh được cung cấp CÓ PHẢI là 'CHỨNG CHỈ HÀNH NGHỀ KHÁM BỆNH, CHỮA BỆNH' hợp lệ tại Việt Nam hay không, VÀ đối chiếu với thông tin ứng viên cung cấp.

Các tiêu chí cần có:
1. Phải có chữ 'CHỨNG CHỈ HÀNH NGHỀ KHÁM BỆNH, CHỮA BỆNH'.
2. Thông tin 'Họ và tên' trên chứng chỉ PHẢI khớp hoặc rất gần giống với tên đăng ký là '{expectedName}'.
3. Phải hiển thị rõ 'Phạm vi chuyên môn' (ví dụ: Nội tổng quát, Răng Hàm Mặt, Y học cổ truyền...).
4. Phải có con dấu và chữ ký của cơ quan có thẩm quyền (Sở Y Tế hoặc Bộ Y Tế).
5. Số năm kinh nghiệm công tác mà ứng viên khai báo là {expectedExperienceYears} năm phải hợp lý so với ngày cấp trên chứng chỉ. Nếu ngày cấp gần đây nhưng khai báo quá nhiều năm kinh nghiệm, hãy đánh dấu là không hợp lệ.
6. Hình ảnh không được quá mờ, cắt xén làm mất thông tin quan trọng.

Hãy trả về phản hồi CHỈ BAO GỒM một chuỗi JSON hợp lệ với định dạng sau, không kèm bất kỳ markdown hoặc văn bản nào khác:
{{
  ""isValid"": true/false,
  ""message"": ""Lý do chi tiết bằng tiếng Việt. Nếu hợp lệ, ghi 'Chứng chỉ hợp lệ'. Nếu không hợp lệ, giải thích rõ lý do tại sao (ví dụ: 'Hình ảnh mờ', 'Không khớp tên {expectedName}', 'Số năm kinh nghiệm không hợp lý so với ngày cấp', 'Thiếu dấu đỏ của cơ quan cấp').""
}}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = systemPrompt },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = mimeType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.1,
                    response_mime_type = "application/json"
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Gemini API Error: {responseString}");
                    return (false, $"Lỗi kết nối đến dịch vụ AI ({response.StatusCode}). Vui lòng thử lại sau.");
                }

                var root = JsonNode.Parse(responseString);
                var modelText = root?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                if (string.IsNullOrEmpty(modelText))
                {
                    return (false, "Không nhận được phản hồi từ AI kiểm duyệt.");
                }

                // Parse the JSON output from AI
                try
                {
                    // Clean up markdown in case AI ignored instructions
                    modelText = modelText.Replace("```json", "").Replace("```", "").Trim();
                    var resultNode = JsonNode.Parse(modelText);
                    
                    bool isValid = resultNode?["isValid"]?.GetValue<bool>() ?? false;
                    string message = resultNode?["message"]?.ToString() ?? "Không thể phân tích kết quả xác thực.";
                    
                    return (isValid, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Parse error for AI output: {modelText} - {ex.Message}");
                    return (false, "Lỗi phân tích kết quả từ hệ thống kiểm duyệt.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Verification Error: {ex.Message}");
                return (false, "Đã xảy ra lỗi hệ thống khi xác thực chứng chỉ. Vui lòng thử lại.");
            }
        }
    }
}
