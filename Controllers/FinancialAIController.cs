using Microsoft.AspNetCore.Mvc;

namespace FinancialTeacherAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FinancialAIController : ControllerBase
    {
        private readonly IFinancialAIService _financialAIService;
        private readonly IEmbeddingService _embeddingService;

        public FinancialAIController(
            IFinancialAIService financialAIService,
            IEmbeddingService embeddingService
        )
        {
            _financialAIService = financialAIService;
            _embeddingService = embeddingService;
        }

        /// <summary>
        /// Get's the score of the answer based on the question
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        [HttpGet("score")]
        public async Task<IActionResult> GetScore([FromQuery] string question, string answer)
        {
            var examDTO = new ExamDTO
            {
                Question = question,
                Answer = answer
            };
            var score = await _financialAIService.GetScoreAsync(examDTO);

            return Ok(score);
        }

        /// <summary>
        /// Get's the context and stores the embeddings in Pinecone index
        /// </summary>
        /// <returns></returns>
        [HttpPost("embbedContext")]
        public async Task<IActionResult> EmbbedContext()
        {
            await _embeddingService.GenerateContextEmbeddingAsync();

            return Ok();
        }
    }
}