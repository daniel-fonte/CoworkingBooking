using System.Text.Json;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Interfaces;

namespace CoworkingBooking.Application.Workspace
{
    public class WorkspaceRefreshCacheHandler : IRefreshCacheHandler
    {
        private readonly ICacheRepository<WorkspaceEntity> cacheRepository;
        private readonly IWorkspaceRepository workspaceRepository;

        public WorkspaceRefreshCacheHandler(
            ICacheRepository<WorkspaceEntity> cacheRepository,
            IWorkspaceRepository workspaceRepository
        )
        {
            this.cacheRepository = cacheRepository;
            this.workspaceRepository = workspaceRepository;
        }

        public string CacheType => "Workspace";

        public async Task Execute(string cacheKey, CancellationToken cancellationToken)
        {
            var slug = cacheKey.Split(':')[1];

            var workspaceFound = await workspaceRepository.FindOneBySlug(slug);

            if (workspaceFound == null)
            {
                return;
            }

            var cacheEntry = new CacheEntry<WorkspaceEntity>
            {
                Data = workspaceFound,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var cacheJson = JsonSerializer.Serialize(cacheEntry);

            await cacheRepository.UpdateByKey(cacheKey, cacheJson);
        }
    }
}