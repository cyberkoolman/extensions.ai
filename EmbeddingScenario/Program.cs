using System.Linq.Expressions;
using System.Numerics.Tensors;
using Microsoft.Extensions.AI;

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaEmbeddingGenerator(
    new Uri("http://localhost:11434"),
    modelId: "all-minilm");

// Part 1: Printing out Vectors and see the dimension size
// var result = await embeddingGenerator.GenerateEmbeddingAsync("Cats are better than dogs");
// Console.WriteLine($"Vector of length {result.Vector.Length}");
// foreach (var value in result.Vector.Span)
// {
//     Console.Write("{0:0.00}, ", value);
// }

// Part 2: Very basic vector Similarity Search
var candidates = new string[] {"Onboarding Process for New Employees", "Understanding Our Core Values", ".NET Aspire 9.0 is packed with", "New features to streamline your app development.", "New dashboard updates, custom commands, more powerful container", "start/stop/lifecycle management, new Integrations,", "Using the Office Gym", "Plan for Wellness Programss", "Participate Team building activities", "and preview support for Azure Functions are packed into this release."};

Console.WriteLine("Generating embeddings for candidates...");
var candidateEmbeddings = await embeddingGenerator.GenerateAndZipAsync(candidates);

while (true)
{
    Console.WriteLine("\nQuery: ");
    var input = Console.ReadLine()!;
    if (input == "") break;

    var inputEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(input);

    var closest =
        from candidate in candidateEmbeddings
        let similarity = TensorPrimitives.CosineSimilarity(candidate.Embedding.Vector.Span, inputEmbedding.Vector.Span)
        orderby similarity descending
        select new { Text = candidate.Value, Similarity = similarity };

    foreach (var c in closest.Take(3))
    {
        Console.WriteLine($"({c.Similarity}): {c.Text}");
    }
}

