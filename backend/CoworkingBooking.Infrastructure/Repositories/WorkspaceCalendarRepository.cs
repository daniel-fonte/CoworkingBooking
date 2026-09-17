using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Infraestructure.Mappers;
using CoworkingBooking.Infraestructure.Models;
using CoworkingBooking.Infraestructure.Providers;
using CoworkingBooking.Shared.Exceptions;
using MongoDB.Driver;

namespace CoworkingBooking.Infraestructure
{
    public class WorkspaceCalendarRepository : IWorkspaceCalendarRepository
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<WorkspaceCalendarModel> _collection;
        private readonly WorkspaceCalendarPersistenceMapper workspaceCalendarPersistenceMapper;

        public WorkspaceCalendarRepository(
            MongodbDatabaseService mongodbDatabaseService,
            WorkspaceCalendarPersistenceMapper workspaceCalendarPersistenceMapper
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            _collection = _mongodbDatabaseService.GetCollection<WorkspaceCalendarModel>("workspaces_calendar");
            this.workspaceCalendarPersistenceMapper = workspaceCalendarPersistenceMapper;
        }

        
        public Task<List<WorkspaceCalendarEntity>> FindByWorkspaceId(string workspaceId)
        {
            throw new NotImplementedException();
        }

        public async Task<long> InsertMany(List<WorkspaceCalendarEntity> workspaceCalendars, IClientSessionHandle? session = null)
        {
            try
            {
                var workspacesCalendarModels = workspaceCalendars.Select(w => workspaceCalendarPersistenceMapper.ToModel(w));

                var insertModels = workspacesCalendarModels.Select(w =>  new InsertOneModel<WorkspaceCalendarModel>(w));

                var options = new BulkWriteOptions 
                { 
                    IsOrdered = false
                };

                BulkWriteResult result = await _collection.BulkWriteAsync(session, insertModels, options);

                return result.InsertedCount;
            }
            catch (MongoBulkWriteException<WorkspaceCalendarModel> ex)
            {
                var errors = new List<Exception>();

                foreach (var error in ex.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        errors.Add(new DuplicateKeyException(error.Index.ToString(), error.Code.ToString(), ex));
                    }
                    else
                    {
                        errors.Add(new Exception(error.Message));
                    }
                }

                throw new BulkWriteException(errors, ex);
            }
        }
    }
}