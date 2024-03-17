using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        private readonly UserManager<User> _userManager;
        public CommentsController (PhoneshopIdentityContext context,UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComment() {
            return await _context.Comments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments
                 .Include(c => c.User)
                 .Include(c => c.Replies)
                     .ThenInclude(reply => reply.User)
                 .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommnet(int id, Comment comment)
        {
            if(id != comment.Id)
            {
                return BadRequest();
            }
            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles ="User")]
        public async Task<ActionResult<Comment>> PostComment([FromBody]Comment comment)
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var userid = user.Id;

            var commentItem = new Comment
            {

                Content = comment.Content,
                ParentCommentId = comment.ParentCommentId,
                UserId = userid,
                PhoneModelId = comment.PhoneModelId,
            };

            _context.Comments.Add(commentItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = commentItem.Id }, commentItem);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if(comment == null) { return NotFound(); }

            comment.Status = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(c => c.Id == id);
        }
    }
}
