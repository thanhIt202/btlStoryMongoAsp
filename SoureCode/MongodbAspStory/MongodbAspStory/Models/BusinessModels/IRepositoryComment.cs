using MongoDB.Bson;
using MongodbAspStory.Models.DataModels;
using MongodbAspStory.Models.ViewModels;

namespace MongodbAspStory.Models.BusinessModels
{
    public interface IRepositoryComment : IRepositoryGeneric<Comment, ObjectId>
    {
		List<CommentViewModel> GetCommentFull();
        CommentViewModel GetCommentFullById(ObjectId key);
	}
}
