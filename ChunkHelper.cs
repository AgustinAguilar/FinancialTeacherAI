using System.Text.RegularExpressions;
using FinancialTeacherAI.Services.DTO;
using UglyToad.PdfPig;

public static class ChunkHelper
{
    public static List<string> ChunkTextByHeader(string text)
    {
        // Regex pattern to identify headers (e.g., #, ##)
        string headerPattern = @"(?=^#.+?$)";

        // Split the text based on headers using Regex
        var sections = Regex.Split(text, headerPattern, RegexOptions.Multiline);

        // Create a list to store chunks
        var chunks = new List<string>();

        foreach (var section in sections)
        {
            if (!string.IsNullOrWhiteSpace(section))
            {
                chunks.Add(section.Trim());
            }
        }

        return chunks;
    }

    public static string ExtractTextFromPdf(string path)
    {
        using (var pdf = PdfDocument.Open(path))
        {
            string extractedText = "";
            foreach (var page in pdf.GetPages())
            {
                extractedText += page.Text + "\n";
            }
            return extractedText;
        }
    }

    public static List<string> SplitTextIntoChunks(string text, int chunkSizeWords)
    {
        var chunks = new List<string>();
        var words = text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < words.Length; i += chunkSizeWords)
        {
            var chunk = string.Join(" ", words, i, Math.Min(chunkSizeWords, words.Length - i));
            chunks.Add(chunk);
        }

        return chunks;
    }

    public static Input PrepareInputPayloadToAll(List<string> chunks)
    {
        // Prepara el payload con el formato requerido por Hugging Face
        var input = new Input();
        input.SourceSentence = "Financials Book";
        input.Sentences = new List<string>();
        input.Sentences = chunks;
        //input.Sentences.Add(chunk);
        return input;
    }

    public static Input PrepareInputPayload(string chunk)
    {
        var input = new Input();
        input.SourceSentence = "Financials Book";
        input.Sentences = new List<string>();
        input.Sentences.Add(chunk);
        return input;
    }

    public static string CleanText(string text)
    {
        // Elimina caracteres de control y no imprimibles
        text = Regex.Replace(text, @"[\x00-\x1F\x7F]", ""); // Caracteres de control ASCII

        // Reemplaza múltiples espacios consecutivos por un único espacio
        text = Regex.Replace(text, @"\s+", " ");

        // Elimina caracteres especiales, dejando solo letras, números y espacios
        text = Regex.Replace(text, @"[^a-zA-Z0-9\s]", "").Trim();

        return text;
    }
}