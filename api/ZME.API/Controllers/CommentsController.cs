using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using ZME.API.Extensions;
using ZME.API.Repositories;
using ZME.API.Services;
using ZME.API.Shared.Models;
using ZME.API.Shared.Responses;
using ZME.API.Shared.Utils;
using Constants = ZME.API.Shared.Utils.Constants;

namespace ZME.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly CommentRepository _commentRepository;
    private readonly RedisService _redisService;
    private readonly IValidator<Comment> _commentValidator;
    private readonly IHub _sentryHub;

    public CommentsController(CommentRepository commentRepository, RedisService redisService, IValidator<Comment> commentValidator, IHub sentryHub)
    {
        _commentRepository = commentRepository;
        _redisService = redisService;
        _commentValidator = commentValidator;
        _sentryHub = sentryHub;
    }

    [HttpPost]
    [Authorize(Roles = Constants.CAN_COMMENT)]
    [ProducesResponseType(typeof(Response<Comment>), 201)]
    [ProducesResponseType(typeof(Response<IList<ValidationFailure>>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Comment>>> CreateComment(Comment data)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_COMMENT_LIST))
                return StatusCode(401);

            // Check if they can add a confidential comment 
            if (data.Confidential && !await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_COMMENT_CONFIDENTIAL_LIST))
                return StatusCode(401);

            var validation = await _commentValidator.ValidateAsync(data);
            if (!validation.IsValid)
            {
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = 400,
                    Message = "Validation failure",
                    Data = validation.Errors
                });
            }

            var result = await _commentRepository.CreateComment(data, Request);
            return StatusCode(201, new Response<Comment>
            {
                StatusCode = 201,
                Message = $"Created comment '{result.Id}'",
                Data = result
            });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message,
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet]
    [Authorize(Roles = $"{Constants.CAN_COMMENT},{Constants.CAN_COMMENT_CONFIDENTIAL}")]
    [ProducesResponseType(typeof(ResponsePaging<IList<Comment>>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<IList<Comment>>>> GetComments(int userId, int page = 1, int size = 10)
    {
        try
        {
            if (await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_COMMENT_CONFIDENTIAL_LIST))
            {
                var result = await _commentRepository.GetComments(userId, page, size, true);
                var totalCount = await _commentRepository.GetCommentCount(userId);
                return Ok(new ResponsePaging<IList<Comment>>
                {
                    StatusCode = 200,
                    ResultCount = result.Count,
                    TotalCount = totalCount,
                    Message = $"Got {result.Count} comments",
                    Data = result
                });
            }
            else if (await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_COMMENT_LIST))
            {
                var result = await _commentRepository.GetComments(userId, page, size, false);
                var totalCount = await _commentRepository.GetCommentCount(userId);
                return Ok(new ResponsePaging<IList<Comment>>
                {
                    StatusCode = 200,
                    ResultCount = result.Count,
                    TotalCount = totalCount,
                    Message = $"Got {result.Count} comments",
                    Data = result
                });
            }
            return StatusCode(401);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message,
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpDelete]
    [Authorize(Roles = Constants.SENIOR_STAFF)]
    [ProducesResponseType(typeof(Response<string?>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<string?>>> DeleteComment(int commentId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.SENIOR_STAFF_LIST))
                return StatusCode(401);

            await _commentRepository.DeleteComment(commentId, Request);

            return Ok(new Response<string?>
            {
                StatusCode = 200,
                Message = $"Deleted commend '{commentId}'"
            });
        }
        catch (CommentNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message,
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }
}
