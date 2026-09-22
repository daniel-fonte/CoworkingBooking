using System.Globalization;
using System.Text.Json.Nodes;
using CoworkingBooking.Core.WorkspaceCalendar.Constraints;
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
        private readonly WorkspaceCalendarBookingPersistenceMapper workspaceCalendarBookingPersistenceMapper; 

        public WorkspaceCalendarRepository(
            MongodbDatabaseService mongodbDatabaseService,
            WorkspaceCalendarPersistenceMapper workspaceCalendarPersistenceMapper,
            WorkspaceCalendarBookingPersistenceMapper workspaceCalendarBookingPersistenceMapper
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            _collection = _mongodbDatabaseService.GetCollection<WorkspaceCalendarModel>("workspaces_calendar");
            this.workspaceCalendarPersistenceMapper = workspaceCalendarPersistenceMapper;
            this.workspaceCalendarBookingPersistenceMapper = workspaceCalendarBookingPersistenceMapper;
        }
        
        public Task<List<WorkspaceCalendarEntity>> FindByWorkspaceId(string workspaceId)
        {
            throw new NotImplementedException();
        }

        public async Task<long> InsertMany(List<WorkspaceCalendarEntity> workspaceCalendars, IClientSessionHandle? session = null)
        {
            var insertModels = workspaceCalendars
                .Select(w => workspaceCalendarPersistenceMapper.ToModel(w))
                .Select(w => new InsertOneModel<WorkspaceCalendarModel>(w))
                .ToList();

            try
            {
                var options = new BulkWriteOptions 
                { 
                    IsOrdered = false
                };

                BulkWriteResult result;

                if (session is not null)
                {
                    result = await _collection.BulkWriteAsync(session, insertModels, options);
                }
                else
                {
                    result = await _collection.BulkWriteAsync(insertModels, options);
                }

                return result.InsertedCount;
            }
            catch (MongoBulkWriteException<WorkspaceCalendarModel> ex)
            {
                var errors = new List<Exception>();

                foreach (var error in ex.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        var workspaceCalendarFailed = insertModels[error.Index].Document;

                        var value = new JsonObject();
                        value["StartAt"] = workspaceCalendarFailed.StartAt.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture);
                        value["EndAt"] = workspaceCalendarFailed.EndAt.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture);
                        value["WorkspaceId"] = workspaceCalendarFailed.WorkspaceId;

                        errors.Add(
                            new DuplicateKeyException(
                                [
                                    WorkspaceCalendarConstraints.StartAt,
                                    WorkspaceCalendarConstraints.EndAt,
                                    WorkspaceCalendarConstraints.WorkspaceId
                                ], 
                                value.ToJsonString(), 
                                ex
                            )
                        );
                    }
                    else
                    {
                        errors.Add(new Exception(error.Message));
                    }
                }

                throw new BulkWriteException(errors, ex);
            }
        }

        public async Task<List<WorkspaceCalendarEntity>> FindByWorkspaceAvailability(string workspaceId, DateTime startAt, DateTime until)
        {
            var filter =
                Builders<WorkspaceCalendarModel>.Filter.Eq(wc => wc.WorkspaceId, workspaceId) &
                Builders<WorkspaceCalendarModel>.Filter.Gte(wc => wc.StartAt, startAt) &
                Builders<WorkspaceCalendarModel>.Filter.Lte(wc => wc.StartAt, until);

            var result = await _collection.Find(filter).ToListAsync();

            return result.Select(r => workspaceCalendarPersistenceMapper.ToEntity(r)).ToList();
        }

        public async Task<WorkspaceCalendarEntity?> FindOneById(string id)
        {
            var filter = Builders<WorkspaceCalendarModel>.Filter.Eq(wc => wc.Id, id);

            var result = await _collection.Find(filter).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            var workspaceCalendarEntity = workspaceCalendarPersistenceMapper.ToEntity(result);
            return workspaceCalendarEntity;
        }

        public async Task<WorkspaceCalendarBooking?> UpdateBooking(string id, List<WorkspaceCalendarBooking> workspaceCalendarBooking)
        {
            var modelList = workspaceCalendarBooking.Select(wc => workspaceCalendarBookingPersistenceMapper.ToModel(wc));
            var filter = Builders<WorkspaceCalendarModel>.Filter.Eq(wc => wc.Id, id);

            var update = Builders<WorkspaceCalendarModel>.Update.Set(wc => wc.Bookings, modelList);

            var options = new FindOneAndUpdateOptions<WorkspaceCalendarModel>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true,
            };

            var result = await _collection.FindOneAndUpdateAsync(filter, update, options);

            if (result == null || result.Bookings == null)
            {
                return null;
            }

            return result.Bookings
                .Select(b => workspaceCalendarBookingPersistenceMapper.ToEntity(b))
                .FirstOrDefault();
        }
    }
}