using System.Linq.Expressions;
using CoworkingBooking.Core.Workspace.Constraints;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Infraestructure.Mappers;
using CoworkingBooking.Infraestructure.Models;
using CoworkingBooking.Infraestructure.Providers;
using CoworkingBooking.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoworkingBooking.Infraestructure.Repositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<WorkspaceModel> _collection;
        private readonly WorkspacePersistenceMapper workspacePersistenceMapper;
        private readonly WorkspaceAvailabilityPersistenceMapper workspaceAvailabilityPersistenceMapper;
        private readonly WorkspaceAvailabilityRecurrencePersistenceMapper workSpaceAvailabilityRecurrencePersistenceMapper;

        public WorkspaceRepository(
            MongodbDatabaseService mongodbDatabaseService, 
            WorkspacePersistenceMapper workspacePersistenceMapper,
            WorkspaceAvailabilityPersistenceMapper workspaceAvailabilityPersistenceMapper,
            WorkspaceAvailabilityRecurrencePersistenceMapper workSpaceAvailabilityRecurrencePersistenceMapper
        ) {
            this.workSpaceAvailabilityRecurrencePersistenceMapper = workSpaceAvailabilityRecurrencePersistenceMapper;
            this.workspaceAvailabilityPersistenceMapper = workspaceAvailabilityPersistenceMapper;
            this.workspacePersistenceMapper = workspacePersistenceMapper;
            _mongodbDatabaseService = mongodbDatabaseService;
            _collection = _mongodbDatabaseService.GetCollection<WorkspaceModel>("workspaces");
        }

        public Task<List<WorkspaceEntity>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<WorkspaceEntity?> FindOneById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<WorkspaceEntity?> FindOneBySlug(string slug)
        {
            var filter = Builders<WorkspaceModel>.Filter.Eq(w => w.Slug, slug) &
                Builders<WorkspaceModel>.Filter.Eq(w => w.IsInactive, false);
            
            var workspaceModel = await _collection.Find(filter).FirstOrDefaultAsync();

            if (workspaceModel == null)
            {
                return null;
            }

            var workspaceEntity = workspacePersistenceMapper.ToEntity(workspaceModel);
            return workspaceEntity;
        }

        public async Task<WorkspaceEntity> InsertOne(WorkspaceEntity workspace)
        {
            var workspaceModel = workspacePersistenceMapper.ToModel(workspace);

            try
            {    
                await _collection.InsertOneAsync(workspaceModel);

                return workspacePersistenceMapper.ToEntity(workspaceModel);
            }
            catch (MongoWriteException ex)
            {
                if(ex.WriteError.Category is ServerErrorCategory.DuplicateKey)
                {
                    throw new DuplicateKeyException([WorkspaceConstraints.Slug], workspaceModel.Name, ex);
                }

                throw;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<WorkspaceEntity?> UpdateAvailability(string workspaceId, WorkSpaceAvailability availability)
        {
            var modelAvailabilityRecurrence = workSpaceAvailabilityRecurrencePersistenceMapper.ToModel(availability.Recurrence);

            var modelAvailability = workspaceAvailabilityPersistenceMapper.ToModel(availability);

            var filter = 
                Builders<WorkspaceModel>.Filter.Eq(w => w.Id, workspaceId) &
                Builders<WorkspaceModel>.Filter.Eq(w => w.IsInactive, false);

            var update = Builders<WorkspaceModel>.Update.Set(w => w.Availability, modelAvailability);

            var options = new FindOneAndUpdateOptions<WorkspaceModel>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = false,
            };

            var result = await _collection.FindOneAndUpdateAsync(filter, update, options);

            if (result == null)
            {
                return null;
            }

            var workspaceAvailabilityRecurrenceEntity = workSpaceAvailabilityRecurrencePersistenceMapper.ToEntity(result.Availability!.Recurrence);
            var workspaceAvailabilityEntity = workspaceAvailabilityPersistenceMapper.ToEntity(result.Availability);

            return workspacePersistenceMapper.ToEntity(result);
        }

        public async Task<WorkspaceEntity> UpdateOneById(string workspaceId, WorkspaceEntity workspace)
        {
            var workspaceModel = this.workspacePersistenceMapper.ToModel(workspace);

            var filter = Builders<WorkspaceModel>.Filter.Eq(w => w.Id, workspaceId);
            var update = Builders<WorkspaceModel>.Update.Set(w => w, workspaceModel);

            var options = new FindOneAndUpdateOptions<WorkspaceModel>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = false,
            };

            var result = await _collection.FindOneAndUpdateAsync(filter, update, options);

            var workspaceAvailabilityRecurrenceEntity = workSpaceAvailabilityRecurrencePersistenceMapper.ToEntity(result.Availability!.Recurrence);
            var workspaceAvailabilityEntity = workspaceAvailabilityPersistenceMapper.ToEntity(result.Availability);

            return workspacePersistenceMapper.ToEntity(result);
        }

        public async Task<WorkspaceEntity> UpdateStatusById(string id, WorkspaceStatus status)
        {
            var filter = Builders<WorkspaceModel>.Filter.Eq(w => w.Id, id);
            var update = Builders<WorkspaceModel>.Update.Set(w => w.Status, status);

            var options = new FindOneAndUpdateOptions<WorkspaceModel>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = false,
            };

            var result = await _collection.FindOneAndUpdateAsync(filter, update, options);

            var workspaceAvailabilityRecurrenceEntity = workSpaceAvailabilityRecurrencePersistenceMapper.ToEntity(result.Availability!.Recurrence);
            var workspaceAvailabilityEntity = workspaceAvailabilityPersistenceMapper.ToEntity(result.Availability);

            return workspacePersistenceMapper.ToEntity(result);
        }
    }
}