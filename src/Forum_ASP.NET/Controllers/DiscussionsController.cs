﻿using Forum_ASP.NET.Models;
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
			var comment = await FindCommentAsync( id );

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

		public async Task<ActionResult> Edit( int id )
		{
			Discussion discussion = await FindDiscussionAsync( id );
			if ( discussion == null )
			{
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
			discussion.DiscussionId = id;
			_context.Discussions.Attach( discussion );
			_context.Entry( discussion ).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return RedirectToAction( "Index" );
		}
	}
}