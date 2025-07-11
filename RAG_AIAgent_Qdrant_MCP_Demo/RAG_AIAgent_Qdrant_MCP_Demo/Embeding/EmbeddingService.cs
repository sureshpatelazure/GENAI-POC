﻿using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using RAG_AIAgent_Qdrant_MCP_Demo.DataLoader;
using RAG_AIAgent_Qdrant_MCP_Demo.VectorStore;

namespace RAG_AIAgent_Qdrant_MCP_Demo.Embeding
{
    public class EmbeddingService
    {
        private readonly Kernel _kernel;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly IDataLoader _dataLoader;
        private readonly IVectorStoreService _vectorStoreService;
        public EmbeddingService(Kernel kernel, IDataLoader dataLoader, IVectorStoreService vectorStoreService)
        {
            _kernel = kernel;
            _embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            _dataLoader = dataLoader;
            _vectorStoreService = vectorStoreService;
        }

        public void UploadEmbedding(string[] filePaths, int blockDivistion, int batchSize)
        {
            int counter = 0;    
            foreach (var path in filePaths)
            {
                Console.WriteLine($"Processing file {++counter} of {filePaths.Length}: {path}");    
                var dataContent = _dataLoader.LoadData(path, blockDivistion, batchSize).GetAwaiter().GetResult();
                _vectorStoreService.UpSert(dataContent).GetAwaiter().GetResult();
            }
        }
    }
}