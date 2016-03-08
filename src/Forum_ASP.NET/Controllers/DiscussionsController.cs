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
			Comment comment = new Comment()
			{
				DiscussionId = id,
			};

			return View( comment );
		}

		public async Task<ActionResult> Comment( int id )
		{
			var comment = await FindCommentsAsync( id );

			if ( comment == null )
			{
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
				discussion.CreatingDate = DateTime.Now.ToString();
				discussion.LastDate = DateTime.Now.ToString();
				discussion.Author = User.GetUserName();
                comment.Content = discussion.FirstComment;
                comment.CommentDate = discussion.CreatingDate;
                comment.CommentAuthor = discussion.Author;
                _context.Discussions.Add( discussion );
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
				comment.Discussion = await FindDiscussionAsync( comment.DiscussionId );
				comment.Discussion.LastDate = DateTime.Now.ToString();
				comment.CommentDate = DateTime.Now.ToString();
				comment.CommentAuthor = User.GetUserName();
				_context.Comment.Add( comment );
				_context.SaveChanges();
				return RedirectToAction( "Comment", new { id = comment.DiscussionId } );
			}

			return View( comment );
		}

		public async Task<ActionResult> CommentEdit( int id )
		{
			Comment comment = await FindCommentAsync( id );
			if ( comment == null )
			{
				return HttpNotFound();
			}

			ViewBag.Items = GetAuthorsListItems( comment.CommentId );
			return View( comment );
		}

		private Task<Discussion> FindDiscussionAsync( int id )
		{
			return _context.Discussions.Include( c => c.Comments ).SingleOrDefaultAsync( discussion => discussion.DiscussionId == id );
		}

		private Task<List<Comment>> FindCommentsAsync( int id )
		{
			return _context.Comment.Where( comment => comment.DiscussionId == id ).ToListAsync<Comment>();
		}

        private Task<Comment> FindCommentAsync(int id)
        {
            return _context.Comment.Include(c => c.Discussion).SingleOrDefaultAsync(comment => comment.CommentId == id);
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
		public async Task<ActionResult> CommentUpdate( int id, [Bind( "Content", "CommentDate", "CommentAuthor", "DiscussionId")] Comment comment )
		{
            comment.CommentId = id;
            _context.Comment.Attach( comment );
			_context.Entry( comment ).State = EntityState.Modified;
			await _context.SaveChangesAsync();
            return RedirectToAction("Comment", new { id = comment.DiscussionId });
        }
	}
}