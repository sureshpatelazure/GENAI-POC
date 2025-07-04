﻿

using MemoryVectorStorePOC.DataModel;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using System.Threading.Tasks;
using Microsoft.Extensions.VectorData;

namespace MemoryVectorStorePOC.VectorStore
{
    public class QdrantVectorStoreService
    {
        private  QdrantVectorStore? _vectorStore;
        private  QdrantCollection<ulong, FinanceInfo>? _collection;

        public  QdrantVectorStoreService(IEmbeddingGenerator embeddingGenerator)
        {
            QdrantClient qdrantClient = new QdrantClient(new Uri("https://0bd066e5-a87b-4a8f-ac3c-3bd589fe1d3a.us-east4-0.gcp.cloud.qdrant.io:6334"), "");

            _vectorStore = new QdrantVectorStore(qdrantClient,true,
               new QdrantVectorStoreOptions
               {
                   EmbeddingGenerator = embeddingGenerator,
               });
            _collection = _vectorStore.GetCollection<ulong, FinanceInfo>("finances");

             _collection.EnsureCollectionExistsAsync().GetAwaiter().GetResult();
        }

        public async Task UpSert(string[] budgetInfo)
        {
            var records = budgetInfo.Select((input, index) => new FinanceInfo { Key = (ulong) index, Text = input });
            await _collection.UpsertAsync(records); 
        }

        public async Task Search(string query)
        {
            VectorSearchOptions<FinanceInfo> options = new VectorSearchOptions<FinanceInfo>
            {
                Filter = null, // No filter applie
            };  

            var searchResult = _collection.SearchAsync(query,top: 1, options);
            var scoreThreshold = 0.9;
            await foreach (var result in searchResult)
            {
                if (result.Score >= scoreThreshold)
                {
                    Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Text}, Score: {result.Score}");
                }
                else
                {
                    Console.WriteLine($"No results found with score above {scoreThreshold}");
                }
            }

        }

        public QdrantCollection<ulong, FinanceInfo>? Collection => _collection;
    }
}
