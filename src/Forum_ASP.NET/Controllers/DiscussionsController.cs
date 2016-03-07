using Forum_ASP.NET.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Forum_ASP.NET.Controllers
{
	public class DiscussionsController : Controller
	{
		private ApplicationDbContext _context;

		public DiscussionsController( ApplicationDbContext context )
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View( _context.Discussions.ToList() );
		}

		public IActionResult Create()
		{
			return View();
		}

		public IActionResult CreateComment( int id )
		{
			//var m = await FindCommentAsync(id);
			Comment comment = new Comment()
			{
				DiscussionId = id,
			};
			//var comment = new Comment()
			//{
			//    DiscussionId = id
			//};
			return View( comment );
		}

		// Details in the Tutorial (Should be the same (an right solution :D))
		public async Task<ActionResult> Comment( int id )
		{
			var comment = await FindCommentAsync( id );
			//Discussion discussion = await _context.Discussions
			//.Include(b => b.Comment)
			//.SingleOrDefaultAsync(b => b.DiscussionId == id);
			if ( comment == null )
			{
				// Logger: see iLogger or getting started > Fundamentals > logging
				//Logger.LogInformation("Details: Item not found {0}", id);
				return HttpNotFound();
			}
			return View( comment );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create( Discussion discussion, Comment comment )
		{
			if ( ModelState.IsValid )
			{

				comment.Content = "First Comment";
				// List type ? in Discussion?
				//discussion.Comments.Add("Test");
				discussion.CreatingDate = DateTime.Now.ToString();
				discussion.LastDate = DateTime.Now.ToString();
				discussion.Author = User.GetUserName();
				//_context.Comment.Add(comment);
				_context.Discussions.Add( discussion );
				//_context.SaveChanges();
				comment.DiscussionId = discussion.DiscussionId;
				_context.Comment.Add( comment );
				_context.SaveChanges();
				return RedirectToAction( "Index" );
			}

			return View( discussion );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateComment( Comment comment )
		{
			if ( ModelState.IsValid )
			{
				//comment.DiscussionId = id;
				comment.Discussion = await FindDiscussionAsync( comment.DiscussionId );
				//discussion.LastDate = DateTime.Now.ToString();
				comment.Discussion.LastDate = DateTime.Now.ToString();
				comment.CommentDate = DateTime.Now.ToString();
				comment.CommentAuthor = User.GetUserName();
				_context.Comment.Add( comment );
				_context.SaveChanges();
				return RedirectToAction( "Comment", new { id = comment.DiscussionId } );
			}

			return View( comment );
		}

		public async Task<ActionResult> Edit( int id )
		{
			Discussion discussion = await FindDiscussionAsync( id );
			if ( discussion == null )
			{
				// Logger: see iLogger or getting started > Fundamentals > logging
				//Logger.LogInformation("Edit: Item not found {0}", id);
				return HttpNotFound();
			}

			ViewBag.Items = GetAuthorsListItems( discussion.DiscussionId );
			return View( discussion );
		}

		private Task<Discussion> FindDiscussionAsync( int id )
		{
			return _context.Discussions.Include( c => c.Comments ).SingleOrDefaultAsync( discussion => discussion.DiscussionId == id );
		}

		private Task<List<Comment>> FindCommentAsync( int id )
		{
			return _context.Comment.Where( comment => comment.DiscussionId == id ).ToListAsync<Comment>();
		}

		private IEnumerable<SelectListItem> GetAuthorsListItems( int selected = -1 )
		{
			var tmp = _context.Discussions.ToList();

			return tmp
					.OrderBy( discussion => discussion.LastDate )
					.Select( discussion => new SelectListItem
					{
						Text = String.Format( "{0}, {1}", discussion.DiscussionName, discussion.Author ),
						Value = discussion.CreatingDate.ToString(),
						Selected = discussion.DiscussionId == selected
					} );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Update( int id, [Bind( "DiscussionName", "CreatingDate", "LastDate", "Comments", "Author" )] Discussion discussion )
		{
			//try
			//{
			discussion.DiscussionId = id;
			_context.Discussions.Attach( discussion );
			_context.Entry( discussion ).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return RedirectToAction( "Index" );
			//}
			// DataStoreExeption No Idea how to use it
			//catch (DataStoreException)
			//{
			//    ModelState.AddModelError(string.Empty, "Unable to save changes.");
			//}
			//return View(discussion);
		}
	}
}