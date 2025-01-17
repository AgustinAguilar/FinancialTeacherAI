using FinancialTeacherAI.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using UglyToad.PdfPig.Graphics;

public class EmbeddingService : IEmbeddingService
{
    private readonly Kernel _kernel;
    private readonly IPineconeService _pineconeService;
    private readonly IHuggingFaceService _huggingFaceService;

    public EmbeddingService(
        [FromKeyedServices("FinancialAIKernel")] Kernel kernel,
        IPineconeService pineconeService,
        IHuggingFaceService huggingFaceService)
    {
        _kernel = kernel;
        _pineconeService = pineconeService;
        _huggingFaceService = huggingFaceService;
    }

    /// <summary>
    /// Get's the context and stores the embeddings in Pinecone index
    /// </summary>
    /// <returns></returns>
    public async Task GenerateContextEmbeddingAsync()
    {
        string contextPath = Path.Combine(Directory.GetCurrentDirectory(), @"data\PrinciplesofFinance-WEB.pdf");
        string text = ChunkHelper.ExtractTextFromPdf(contextPath);
        text = ChunkHelper.CleanText(text);
        var chunkEmbeddings = new List<ChunkEmbedding>();
        var chunks = ChunkHelper.SplitTextIntoChunks(text, 300);

        var inputPayload = ChunkHelper.PrepareInputPayloadToAll(chunks);
        var embedding = await _huggingFaceService.GenerateEmbeddingsForChunk(inputPayload);
        float[] embeddingArray = embedding
           .Trim('[', ']')
           .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) 
           .Select(float.Parse) 
           .ToArray();

        for (int i = 0; i < chunks.Count; i++)
        {
            chunkEmbeddings.Add(new ChunkEmbedding
            {
                Text = chunks[i],
                Embedding = new float[] { embeddingArray[i] }
            });
        }
        await _pineconeService.StoreEmbeddingsAsync(chunkEmbeddings);
    }
}