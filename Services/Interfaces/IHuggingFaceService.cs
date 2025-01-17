using FinancialTeacherAI.Services.DTO;

namespace FinancialTeacherAI.Services.Interfaces
{
    public interface IHuggingFaceService
    {
        Task<string> GenerateEmbeddingsForChunk(Input inputPayload);
    }
}
