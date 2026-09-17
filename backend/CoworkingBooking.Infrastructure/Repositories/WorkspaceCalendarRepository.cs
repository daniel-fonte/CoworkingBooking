using System.Globalization;
using System.Text.Json.Nodes;

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
    }
}