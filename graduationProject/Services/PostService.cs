﻿using graduationProject.core.DbContext;
using graduationProject.Dtos;
using graduationProject.Models;
using graduationProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using graduationProject.DTOs;
using graduationProject.DTOs.ReactDtos;

namespace graduationProject.Services
{
    public class PostService: IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private long maxAllowedSize = 1048576;
        private readonly string _uploadsPath;

        public PostService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _uploadsPath = Path.Combine(env.WebRootPath, "uploads");
        }

        public async Task<ReturnPostDto> CreatePost(AddPostDto post, string username)
        {
            var returnPost = new ReturnPostDto();
            // var user = await _userManager.FindByNameAsync(username);
            var user = await _context.users.FindAsync(username);
            var Post = new Post
            {
                Content = post.Content,
                User = user,
            };
            if(post.Attachment != null)
            {
                var extension = Path.GetExtension(post.Attachment.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    returnPost.isSucces = false;
                    returnPost.Errors = new string[] { "Invalid file type" };
                    return returnPost;
                }
                if (post.Attachment.Length > maxAllowedSize)
                {
                    returnPost.isSucces = false;
                    returnPost.Errors = new string[] { "File size is too large" };
                    return returnPost;
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_uploadsPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await post.Attachment.CopyToAsync(fileStream);
                }
                Post.Attachment = fileName;
            }
            _context.Posts.Add(Post);
            await _context.SaveChangesAsync();

            returnPost.Id = Post.Id;
            returnPost.Content = Post.Content;
            returnPost.UserName = user.Name;
            returnPost.isSucces = true;
            if (Post.Reacts != null)
            {
                returnPost.Reacts = Post.Reacts.Count();
            }
            if (Post.Attachment != null)
            {
            returnPost.Attachment = Path.Combine(_uploadsPath, Post.Attachment);

            }

            return returnPost;
        }
        
        //public async Task<ReturnPostDto> CreatePost(AddPostDto post, string username)
        //{
        //    var returnPost = new ReturnPostDto();
        //    var user = await _userManager.FindByNameAsync(username);
        //    var Post = new Post
        //    {
        //        Content = post.Content,
        //        User = user
        //    };
        //    if (post.Attachment != null)
        //    {
        //        var extension = Path.GetExtension(post.Attachment.FileName);
        //        if (!_allowedExtensions.Contains(extension))
        //        {
        //            returnPost.isSucces = false;
        //            returnPost.Errors = ["Invalid file type"];
        //            return returnPost;
        //        }
        //        if (post.Attachment.Length > maxAllowedSize)
        //        {
        //            returnPost.isSucces = false;
        //            returnPost.Errors = ["File size is too large"];
        //            return returnPost;
        //        }
        //        var fileName = Guid.NewGuid().ToString() + extension;
        //        var filePath = Path.Combine(_uploadsPath, fileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await post.Attachment.CopyToAsync(fileStream);
        //        }
        //        Post.Attachment = fileName;
        //        returnPost.Attachment = Path.Combine("/uploads", fileName); // Corrected path combination
        //    }
        //    _context.Posts.Add(Post);
        //    await _context.SaveChangesAsync();

        //    returnPost.Id = Post.Id;
        //    returnPost.Content = Post.Content;
        //    returnPost.UserName = user.UserName;
        //    returnPost.isSucces = true;

        //    return returnPost;
        //}

        public async Task<ReturnPostDto> EditPost(PostDto post)
        {
            var Post = await _context.Posts.Include("User").Include("Comments").FirstOrDefaultAsync(c=>c.Id==post.Id);
            if (Post == null)
            {
                return new ReturnPostDto
                {
                    isSucces = false,
                    Errors = new string[] { "Post not found" }
                };
            }
            if(post.Attachment != null)
            {
                var extension = Path.GetExtension(post.Attachment.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ReturnPostDto
                    {
                        isSucces = false,
                        Errors = new string[] { "Invalid file type" }
                    };
                }
                if (post.Attachment.Length > maxAllowedSize)
                {
                    return new ReturnPostDto
                    {
                        isSucces = false,
                        Errors = new string[] { "File size is too large" }
                    };
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_uploadsPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await post.Attachment.CopyToAsync(fileStream);
                }
                Post.Attachment = fileName;
            }

            Post.Content = post.Content;
            _context.Posts.Update(Post);
            await _context.SaveChangesAsync();

            var returnpost= new ReturnPostDto
            {
                Id = Post.Id,
                Content = post.Content,
                UserName = Post.User.Name,
                isSucces = true,
                

            };
            if (Post.Reacts != null)
            {
                returnpost.Reacts = Post.Reacts.Count();
            }
            if (Post.Attachment != null)
            {
                returnpost.Attachment = Path.Combine(_uploadsPath, Post.Attachment);
            }
            return returnpost;
        }
        public async Task<ResultDto> DeletePost(int id)
        {
            var Post = await _context.Posts.FindAsync(id);
            if (Post == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Post not found"
                };
            }
            _context.Posts.Remove(Post);
            await _context.SaveChangesAsync();

            return new ResultDto
            {
                IsSuccess = true,
                Message= "Post deleted successfully"
            };
        }
        public async Task<ReturnPostDto> GetPost(int id)
        {
            var Post = await _context.Posts.Where(p => p.Id == id).Include("User").FirstOrDefaultAsync();
            if (Post == null)
            {
                return new ReturnPostDto
                {
                    isSucces = false,
                    Errors = new string[] { "Post not found" }
                };
            }
            var NewPost= new ReturnPostDto
            {
                Id = Post.Id,
                Content = Post.Content,
                isSucces = true,
                UserName = Post.User.Name,
            };
            if (Post.Reacts != null)
            {
                NewPost.Reacts = Post.Reacts.Count();
            }
            if (Post.Attachment != null)
            {
                NewPost.Attachment = Path.Combine(_uploadsPath, Post.Attachment);
            }
            return NewPost;
        }
        //public async Task<List<ReturnPostDto>> GetPosts()
        //{
        //    var Posts = await _context.Posts.ToListAsync();
        //    var returnPosts = new List<ReturnPostDto>();
        //    foreach (var Post in Posts)
        //    {
        //        returnPosts.Add(new ReturnPostDto
        //        {
        //            Id = Post.Id,
        //            Content = Post.Content,
        //            UserName = Post.User.UserName,
        //            isSucces = true
        //        });
        //    }
        //    return returnPosts;
        //}
        public async Task<List<ReturnPostDto>> GetPostsByUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var Posts = await _context.Posts.Include("User").Include("Comments").Where(p => p.User == user.User).ToListAsync();
            var returnPosts = new List<ReturnPostDto>();
            foreach (var Post in Posts)
            {
                if(Post.Comments == null)
                {
                    Post.Comments = new List<Comment>();
                }
                var returnPost = new ReturnPostDto
                {
                    Id = Post.Id,
                    Content = Post.Content,
                    UserName = Post.User.Name,
                    isSucces = true,
                };
                if (Post.Reacts != null)
                {
                    returnPost.Reacts = Post.Reacts.Count();
                }
                if (Post.Attachment != null)
                {
                    returnPost.Attachment = Path.Combine(_uploadsPath, Post.Attachment);
                }
                returnPosts.Add(returnPost);

            }
            return returnPosts;
        }
        public async Task<ReturnCommentDto> addComment(AddCommentDto comment, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var Post = await _context.Posts.FindAsync(comment.PostId);
            if (Post == null)
            {
                return new ReturnCommentDto
                {
                    isSucces = false,
                    Errors = new string[] { "Post not found" }
                };
            }
            var newComment = new Comment
            {
                Content = comment.Content,
                Post = Post,

            };
            if (comment.Attachment != null)
            {
                var extension = Path.GetExtension(comment.Attachment.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ReturnCommentDto
                    {
                        isSucces = false,
                        Errors = new string[] { "Invalid file type" }
                    };
                }
                if (comment.Attachment.Length > maxAllowedSize)
                {
                    return new ReturnCommentDto
                    {
                        isSucces = false,
                        Errors = new string[] { "File size is too large" }
                    };
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_uploadsPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await comment.Attachment.CopyToAsync(fileStream);
                }
                newComment.Attachment = fileName;
            }
            
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            var returnComment = new ReturnCommentDto
            {
                Id = newComment.Id,
                Content = newComment.Content,
                PostId = newComment.Post.Id,
                UserName = user.UserName,
                isSucces = true,
            };
            if(newComment.Attachment != null)
            {
                returnComment.Attachment = Path.Combine(_uploadsPath, newComment.Attachment);
            }
            return returnComment;
        }
        public async Task<ResultDto> deleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Comment not found"
                };
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return new ResultDto
            {
                IsSuccess = true,
                Message = "Comment deleted successfully"
            };
        }
        //public async Task<ReturnCommentDto> EditComment(CommentDto comment)
        //{
        //    var Comment = await _context.Comments.Include("Post").Include("replies").FirstOrDefaultAsync(c=>c.Id==comment.Id);
        //    if (Comment == null)
        //    {
        //        return new ReturnCommentDto
        //        {
        //            isSucces = false,
        //            Errors = ["Comment not found"]
        //        };
        //    }
        //    Comment.Content = comment.Content;
        //    _context.Comments.Update(Comment);
        //    await _context.SaveChangesAsync();

        //    return new ReturnCommentDto
        //    {
        //        Id = Comment.Id,
        //        Content = Comment.Content,
        //        PostId = Comment.Post.Id,
        //        UserName = Comment.Post.User.UserName,
        //    };
        //}
        public async Task<ReturnCommentDto> EditComment(CommentDto comment)
        {
            var commentEntity = await _context.Comments
                .Include(c => c.Post).ThenInclude(c=>c.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            if (commentEntity == null)
            {
                return new ReturnCommentDto
                {
                    isSucces = false,
                    Errors = new string[] { "Comment not found" }
                };
            }
            if(comment.Attachment != null)
            {
                var extension = Path.GetExtension(comment.Attachment.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ReturnCommentDto
                    {
                        isSucces = false,
                        Errors = new string[] { "Invalid file type" }
                    };
                }
                if (comment.Attachment.Length > maxAllowedSize)
                {
                    return new ReturnCommentDto
                    {
                        isSucces = false,
                        Errors = new string[] { "File size is too large" }
                    };
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_uploadsPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await comment.Attachment.CopyToAsync(fileStream);
                }
                commentEntity.Attachment = fileName;
            }

            commentEntity.Content = comment.Content;

            _context.Comments.Update(commentEntity);
            await _context.SaveChangesAsync();

            var returnnewComment= new ReturnCommentDto
            {
                Id = commentEntity.Id,
                Content = commentEntity.Content,
                PostId = commentEntity.Post.Id,
                UserName = commentEntity.Post.User.Name,
                isSucces = true,
            };
            if(commentEntity.Attachment != null)
            {
                returnnewComment.Attachment = Path.Combine(_uploadsPath, commentEntity.Attachment);
            }
            return returnnewComment;
        }

        //public async Task<List<ReturnCommentDto>> GetAllComments(int postId)
        //{
        //    var comments = await _context.Comments.Include("Post").Where(c => c.Post.Id == postId).ToListAsync();
        //    var returnComments = new List<ReturnCommentDto>();
        //    foreach (var comment in comments)
        //    {
        //        var user = await _userManager.FindByIdAsync(comment.Post.User.Id);
        //        returnComments.Add(new ReturnCommentDto
        //        {
        //            Id = comment.Id,
        //            Content = comment.Content,
        //            PostId = comment.Post.Id,
        //            UserName = user.UserName,
        //            isSucces = true
        //        });
        //    }
        //    return returnComments;
        //}
        public async Task<List<ReturnCommentDto>> GetAllComments(int postId)
        {
            var comments = await _context.Comments
                .Include(c => c.Post)
                .ThenInclude(p => p.User)
                .Where(c => c.Post.Id == postId)
                .ToListAsync();

            var returnComments = new List<ReturnCommentDto>();

            foreach (var comment in comments)
            {
                var user = comment.Post.User;

                if (user != null)
                {
                    var returnComment = new ReturnCommentDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        PostId = comment.Post.Id,
                        UserName = user.Name,
                        isSucces = true,
                    };
                    if (comment.Attachment != null)
                    {
                           returnComment.Attachment = Path.Combine(_uploadsPath, comment.Attachment);
                    }
                    returnComments.Add(returnComment);
                }
                else
                {
                    // Handle scenario where user is null
                    returnComments.Add(new ReturnCommentDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        PostId = comment.Post.Id,
                        UserName = "Unknown",
                        isSucces = false,
                        Errors = new string[] { "Associated user not found" }
                    });
                }
            }
            return returnComments;
        }

        //public async Task<ReturnCommentDto> GetComment(int id)
        //{
        //    var comment = await _context.Comments.Include("Post").FirstOrDefaultAsync(c => c.Id == id);
        //    if (comment == null)
        //    {
        //        return new ReturnCommentDto
        //        {
        //            isSucces = false,
        //            Errors = ["Comment not found"]
        //        };
        //    }
        //    return new ReturnCommentDto
        //    {
        //        Id = comment.Id,
        //        Content = comment.Content,
        //        PostId = comment.Post.Id,
        //        UserName = comment.Post.User.UserName,
        //        isSucces = true
        //    };
        //}
        public async Task<ReturnCommentDto> GetComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Post)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return new ReturnCommentDto
                {
                    isSucces = false,
                    Errors = new string[] { "Comment not found" }
                };
            }

            // Check if the associated post and user are null
            if (comment.Post == null || comment.Post.User == null)
            {
                return new ReturnCommentDto
                {
                    isSucces = false,
                    Errors = new string[] { "Associated post or user not found" }
                };
            }

            var returnComment= new ReturnCommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.Post.Id,
                UserName = comment.Post.User.Name,
                isSucces = true,
            };
            if(comment.Attachment != null)
            {
                returnComment.Attachment = Path.Combine(_uploadsPath, comment.Attachment);
            }
            return returnComment;
        }

        public async Task<ReturnReplyDto> addReply(AddReplyDto reply ,string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var comment = await _context.Comments.Include(c=>c.Post).ThenInclude(c=>c.User).FirstOrDefaultAsync(c=>c.Id==reply.CommentId);
            var newReply = new Reply
            {
                Content = reply.Content,
                Comment = comment
            };
            if(reply.Attachment != null)
            {
                var extension = Path.GetExtension(reply.Attachment.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ReturnReplyDto
                    {
                        isSucces = false,
                        Errors = new string[] { "Invalid file type" }
                    };
                }
                if (reply.Attachment.Length > maxAllowedSize)
                {
                    return new ReturnReplyDto
                    {
                        isSucces = false,
                        Errors = new string[] { "File size is too large" }
                    };
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_uploadsPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await reply.Attachment.CopyToAsync(fileStream);
                }
                newReply.Attachment = fileName;
            }
            _context.Replies.Add(newReply);
            await _context.SaveChangesAsync();
            var returnReply = new ReturnReplyDto
            {
                Id = newReply.Id,
                Content = newReply.Content,
                Username = user.UserName,
                CommentId = newReply.Comment.Id,
                isSucces = true,
            };
            if(newReply.Attachment != null)
            {
                returnReply.Attachment = Path.Combine(_uploadsPath, newReply.Attachment);
            }
            return returnReply;
        }
        public async Task<ResultDto> deleteReply(int id)
        {
            var reply = await _context.Replies.FindAsync(id);
            if (reply == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Reply not found"
                };
            }
            _context.Replies.Remove(reply);
            await _context.SaveChangesAsync();

            return new ResultDto
            {
                IsSuccess = true,
                Message = "Reply deleted successfully"
            };
        }
        //public async Task<ReturnReplyDto> EditReply(ReplyDto reply)
        //{
        //    var Reply = await _context.Replies.Include("Comment").FirstOrDefaultAsync(c=>c.Id==reply.Id);
        //    if (Reply == null)
        //    {
        //        return new ReturnReplyDto
        //        {
        //            isSucces = false,
        //            Errors = ["Reply not found"]
        //        };
        //    }
        //    Reply.Content = reply.Content;
        //    _context.Replies.Update(Reply);
        //    await _context.SaveChangesAsync();

        //    return new ReturnReplyDto
        //    {
        //        Id = Reply.Id,
        //        Content = Reply.Content,
        //        Username = Reply.Comment.Post.User.UserName,
        //        CommentId = Reply.Comment.Id
        //    };
        //}
        public async Task<ReturnReplyDto> EditReply(ReplyDto reply)
        {
            var replyEntity = await _context.Replies
                .Include(r => r.Comment)
                    .ThenInclude(c => c.Post)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == reply.Id);

            if (replyEntity == null)
            {
                return new ReturnReplyDto
                {
                    isSucces = false,
                    Errors = new string[] { "Reply not found" }
                };
            }
            if(reply.Attachment != null)
            {
                var extension = Path.GetExtension(reply.Attachment.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ReturnReplyDto
                    {
                        isSucces = false,
                        Errors = new string[] { "Invalid file type" }
                    };
                }
                if (reply.Attachment.Length > maxAllowedSize)
                {
                    return new ReturnReplyDto
                    {
                        isSucces = false,
                        Errors = new string[] { "File size is too large" }
                    };
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_uploadsPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await reply.Attachment.CopyToAsync(fileStream);
                }
                replyEntity.Attachment = fileName;
            }

            replyEntity.Content = reply.Content;

            _context.Replies.Update(replyEntity);
            await _context.SaveChangesAsync();

            var returnreply= new ReturnReplyDto
            {
                Id = replyEntity.Id,
                Content = replyEntity.Content,
                Username = replyEntity.Comment.Post.User.Name,
                CommentId = replyEntity.Comment.Id,
                isSucces = true,
            };
            if(replyEntity.Attachment != null)
            {
                returnreply.Attachment = Path.Combine(_uploadsPath, replyEntity.Attachment);
            }
            return returnreply;
        }
        //public async Task<List<ReturnReplyDto>> GetAllReplies(int commentId)
        //{
        //    var replies = await _context.Replies.Include("Comment").Where(c => c.Comment.Id == commentId).ToListAsync();
        //    var returnReplies = new List<ReturnReplyDto>();
        //    foreach (var reply in replies)
        //    {
        //        returnReplies.Add(new ReturnReplyDto
        //        {
        //            Id = reply.Id,
        //            Content = reply.Content,
        //            Username = reply.Comment.Post.User.UserName,
        //            CommentId = reply.Comment.Id,
        //            isSucces = true
        //        });
        //    }
        //    return returnReplies;
        //}
        public async Task<List<ReturnReplyDto>> GetAllReplies(int commentId)
        {
            var replies = await _context.Replies
                .Include(r => r.Comment)
                    .ThenInclude(c => c.Post)
                        .ThenInclude(p => p.User)
                .Where(r => r.Comment.Id == commentId)
                .ToListAsync();

            //var returnReplies = replies.Select(reply => new ReturnReplyDto
            //{
            //    Id = reply.Id,
            //    Content = reply.Content,
            //    Username = reply.Comment.Post.User.UserName,
            //    CommentId = reply.Comment.Id,
            //    isSucces = true,
            //    Attachment = Path.Combine(_uploadsPath, reply.Attachment)
            //}).ToList();
            var returnReplies = new List<ReturnReplyDto>();
            foreach(var reply in replies)
            {
                var returnReply = new ReturnReplyDto
                {
                    Id = reply.Id,
                    Content = reply.Content,
                    Username = reply.Comment.Post.User.Name,
                    CommentId = reply.Comment.Id,
                    isSucces = true,
                };
                if(reply.Attachment != null)
                {
                    returnReply.Attachment = Path.Combine(_uploadsPath, reply.Attachment);
                }
                returnReplies.Add(returnReply);
            }

            return returnReplies;
        }
        //public async Task<ReturnReplyDto> GetReply(int id)
        //{
        //    var reply = await _context.Replies.Include("Comment").FirstOrDefaultAsync(c => c.Id == id);
        //    if (reply == null)
        //    {
        //        return new ReturnReplyDto
        //        {
        //            isSucces = false,
        //            Errors = ["Reply not found"]
        //        };
        //    }
        //    return new ReturnReplyDto
        //    {
        //        Id = reply.Id,
        //        Content = reply.Content,
        //        Username = reply.Comment.Post.User.UserName,
        //        CommentId = reply.Comment.Id,
        //        isSucces = true
        //    };
        //}
        public async Task<ReturnReplyDto> GetReply(int id)
        {
            var reply = await _context.Replies
                .Include(r => r.Comment)
                    .ThenInclude(c => c.Post)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reply == null)
            {
                return new ReturnReplyDto
                {
                    isSucces = false,
                    Errors = new string[] { "Reply not found" }
                };
            }

            var returnReply= new ReturnReplyDto
            {
                Id = reply.Id,
                Content = reply.Content,
                Username = reply.Comment.Post.User.Name,
                CommentId = reply.Comment.Id,
                isSucces = true,
            };
            if(reply.Attachment != null)
            {
                returnReply.Attachment = Path.Combine(_uploadsPath, reply.Attachment);
            }
            return returnReply;
        }

        public async Task<ResultDto> AddReact(int postId,string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var Post = await _context.Posts.FindAsync(postId);
            if (Post == null)
            {
                return new ResultDto
                {
                    IsSuccess= false,
                    Message = "Post not found"
                };
            }
            if(Post.Reacts!=null)
            {
                if (Post.Reacts.Any(r => r.User == user))
                {
                    return new ResultDto
                    {
                        IsSuccess = true,
                        Message = "React Already Exists"

                    };
                }
                var newReact = new React
                {
                    Post = Post,
                    User = user
                };
                _context.Reacts.Add(newReact);
                await _context.SaveChangesAsync();

                var resultDto1 = new ResultDto
                {
                    IsSuccess = true,
                    Message = "React Added Successfully"
                };
                return resultDto1;
            }
            
            var react = new React
            {
                Post = Post,
                User=user
            };

            _context.Reacts.Add(react);
            await _context.SaveChangesAsync();

            var resultDto = new ResultDto
            {
                IsSuccess = true,
                Message = "React Added Successfully"
            };
            return resultDto;
        }
        public async Task<ResultDto> DeleteReact(int postId, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var Post = await _context.Posts.FindAsync(postId);
            if (Post == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Post not found"
                };
            }
            if (Post.Reacts.Any(r => r.User == user))
            {
                var react = await _context.Reacts.SingleOrDefaultAsync(r => r.User == user);
                _context.Reacts.Remove(react);
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "React Already Exists"

                };
            }
            return new ResultDto
            {
                IsSuccess = true,
                Message = "React is not exists"

            };
        }
        //public async Task<ReturnReactsDto> GetPostReacts(int postId)
        //{
        //    var Post =await _context.Posts.Include("User").FirstOrDefaultAsync(p=>p.Id == postId);
        //    var returnReactsDto = new ReturnReactsDto();
        //    if (Post==null)
        //    {
        //        returnReactsDto.Count = 0;
        //        return returnReactsDto;
        //    }
        //    var reacts = _context.Reacts.Include("User").Where(r => r.Post == Post);
        //    //var reacts = Post.Reacts.ToList();
        //    //var returnReactsDto = new ReturnReactsDto();
        //    returnReactsDto.Reactions = [];
        //    foreach (var reaction in reacts)
        //    {
        //        var user = reaction.User.UserName;
        //        _ = returnReactsDto.Reactions.Append(user);
        //    }
        //    if (reacts != null)
        //    {
        //        returnReactsDto.Count = Post.Reacts.Count();
        //    }
        //    //returnReactsDto.Count = reacts.Count();
        //    return returnReactsDto;
        //}
        public async Task<ReturnReactsDto> GetPostReacts(int postId)
        {
            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId);
            var returnReactsDto = new ReturnReactsDto();

            if (post == null)
            {
                returnReactsDto.Count = 0;
                return returnReactsDto;
            }

            var reacts = _context.Reacts.Include(r => r.User).Where(r => r.Post == post).ToList();
            // ToList() to execute the query and get the reacts

            returnReactsDto.Reactions = reacts.Select(r => r.User.UserName).ToArray();
            returnReactsDto.Count = reacts.Count;

            return returnReactsDto;
        }

    }
}
