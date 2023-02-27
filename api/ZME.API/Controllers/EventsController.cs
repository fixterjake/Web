using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
public class EventsController : ControllerBase
{
    private readonly EventRepository _eventRepository;
    private readonly RedisService _redisService;
    private readonly IValidator<Event> _eventValidator;
    private readonly IValidator<EventPosition> _eventPositionValidator;
    private readonly IValidator<EventRegistration> _eventRegistrationValidator;
    private readonly IHub _sentryHub;

    public EventsController(EventRepository eventRepository, RedisService redisService, IValidator<Event> eventValidator,
        IHub sentryHub, IValidator<EventPosition> eventPositionValidator, IValidator<EventRegistration> eventRegistrationValidator)
    {
        _eventRepository = eventRepository;
        _redisService = redisService;
        _eventValidator = eventValidator;
        _sentryHub = sentryHub;
        _eventPositionValidator = eventPositionValidator;
        _eventRegistrationValidator = eventRegistrationValidator;
    }

    [HttpPost]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(typeof(Response<Comment>), 201)]
    [ProducesResponseType(typeof(Response<IList<ValidationFailure>>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Event>>> CreateEvent(Event data)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);

            var validation = await _eventValidator.ValidateAsync(data);
            if (!validation.IsValid)
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = 400,
                    Message = "Validation failure",
                    Data = validation.Errors
                });

            var result = await _eventRepository.CreateEvent(data, Request);
            return StatusCode(201, new Response<Event>
            {
                StatusCode = 201,
                Message = $"Created event '{result.Id}'",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(Response<Event>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Event>>> GetEvent(int eventId)
    {
        try
        {
            var getClosed = await _redisService.ValidateRoles(Request.HttpContext.User, Constants.ALL_STAFF_LIST);
            var result = await _eventRepository.GetEvent(eventId, getClosed);
            return Ok(new Response<Event>
            {
                StatusCode = 200,
                Message = $"Got event '{eventId}'",
                Data = result
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("All")]
    [ProducesResponseType(typeof(ResponsePaging<IList<Event>>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Event>>> GetEvents(int page = 1, int size = 10)
    {
        try
        {
            var getClosed = await _redisService.ValidateRoles(Request.HttpContext.User, Constants.ALL_STAFF_LIST);
            var result = await _eventRepository.GetEvents(page, size, getClosed);
            return Ok(new ResponsePaging<IList<Event>>
            {
                StatusCode = 200,
                ResultCount = result.Count,
                TotalCount = await _eventRepository.GetEventsCount(getClosed),
                Message = $"Got {result.Count} events",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(typeof(Response<Event>), 200)]
    [ProducesResponseType(typeof(Response<IList<ValidationFailure>>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<Event>>> UpdateEvent(Event data)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);

            var validation = await _eventValidator.ValidateAsync(data);
            if (!validation.IsValid)
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = 400,
                    Message = "Validation failure",
                    Data = validation.Errors
                });

            var result = await _eventRepository.UpdateEvent(data, Request);
            return StatusCode(201, new Response<Event>
            {
                StatusCode = 201,
                Message = $"Created event '{result.Id}'",
                Data = result
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }


    [HttpDelete]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(typeof(Response<string?>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<string?>>> DeleteEvent(int eventId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);
            await _eventRepository.DeleteEvent(eventId, Request);
            return Ok(new Response<string?>
            {
                StatusCode = 200,
                Message = $"Deleted event '{eventId}'"
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPost("Positions")]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(typeof(Response<EventPosition>), 201)]
    [ProducesResponseType(typeof(Response<IList<ValidationFailure>>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<EventPosition>>> CreateEventPosition(EventPosition data)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);

            var validation = await _eventPositionValidator.ValidateAsync(data);
            if (!validation.IsValid)
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = 400,
                    Message = "Validation failure",
                    Data = validation.Errors
                });

            var result = await _eventRepository.CreateEventPosition(data, Request);
            return StatusCode(201, new Response<EventPosition>
            {
                StatusCode = 201,
                Message = $"Created event position '{result.Id}'",
                Data = result
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("Positions")]
    [ProducesResponseType(typeof(Response<IList<EventPosition>>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<IList<EventPosition>>>> GetEventPositions(int eventId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.ALL_STAFF_LIST))
            {
                var result = await _eventRepository.GetEventPositions(eventId, false);
                return Ok(new Response<IList<EventPosition>>
                {
                    StatusCode = 200,
                    Message = $"Got '{result.Count}' event positions",
                    Data = result
                });
            }
            else
            {
                var result = await _eventRepository.GetEventPositions(eventId, true);
                return Ok(new Response<IList<EventPosition>>
                {
                    StatusCode = 200,
                    Message = $"Got '{result.Count}' event positions",
                    Data = result
                });
            }
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpDelete("Positions")]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(typeof(Response<string?>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<string?>>> DeleteEventPosition(int eventPositionId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);

            await _eventRepository.DeleteEventPosition(eventPositionId, Request);
            return Ok(new Response<string?>
            {
                StatusCode = 200,
                Message = $"Deleted event position '{eventPositionId}'"
            });
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPost("Registrations")]
    [Authorize(Roles = Constants.CAN_REGISTER_FOR_EVENTS)]
    [ProducesResponseType(typeof(Response<EventRegistration>), 201)]
    [ProducesResponseType(typeof(Response<IList<ValidationFailure>>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<EventRegistration>>> CreateEventRegistration(EventRegistration data)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, new string[] { Constants.CAN_REGISTER_FOR_EVENTS }))
                return StatusCode(401);

            var validation = await _eventRegistrationValidator.ValidateAsync(data);
            if (!validation.IsValid)
                return BadRequest(new Response<IList<ValidationFailure>>
                {
                    StatusCode = 400,
                    Message = "Validation failure",
                    Data = validation.Errors
                });

            var result = await _eventRepository.CreateEventRegistration(data, Request);
            return StatusCode(201, new Response<EventRegistration>
            {
                StatusCode = 201,
                Message = $"Created event position '{result.Id}'",
                Data = result
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (InvalidEventRegistrationException ex)
        {
            var failures = JsonConvert.DeserializeObject<IList<ValidationFailure>>(ex.Message);
            return BadRequest(new Response<IList<ValidationFailure>>
            {
                StatusCode = 400,
                Message = "Validation failure",
                Data = failures
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("Registration/Own")]
    [Authorize]
    [ProducesResponseType(401)]
    [ProducesResponseType(typeof(Response<EventRegistration>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<EventRegistration>>> GetOwnEventRegistration(int eventId)
    {
        try
        {
            var result = await _eventRepository.GetOwnEventRegistration(eventId, Request);
            return Ok(new Response<EventRegistration>
            {
                StatusCode = 200,
                Message = $"Got event registration '{result.Id}'",
                Data = result
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (EventRegistrationNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("Registrations")]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<IList<EventRegistration>>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<IList<EventRegistration>>>> GetEventRegistrations(int eventId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);

            var result = await _eventRepository.GetEventRegistrations(eventId);
            return Ok(new Response<IList<EventRegistration>>
            {
                StatusCode = 200,
                Message = $"Got {result.Count} event registrations",
                Data = result
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut("Assign")]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<EventRegistration>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<EventRegistration>>> AssignEventRegistration(int eventRegistrationId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);

            var result = await _eventRepository.AssignEventRegistration(eventRegistrationId, Request);
            return Ok(new Response<EventRegistration>
            {
                StatusCode = 200,
                Message = $"Assign event registration '{result.Id}'",
                Data = result
            });
        }
        catch (EventRegistrationNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut("Relief")]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(Response<IList<EventRegistration>>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<EventRegistration>>> AssignReliefEventRegistration(int eventId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);

            var result = await _eventRepository.AssignReliefEventRegistrations(eventId, Request);
            return Ok(new Response<IList<EventRegistration>>
            {
                StatusCode = 200,
                Message = $"Assign {result.Count} registrations as relief",
                Data = result
            });
        }
        catch (EventRegistrationNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpDelete("Registration/Own")]
    [Authorize]
    [ProducesResponseType(401)]
    [ProducesResponseType(typeof(Response<string?>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<string>>> DeleteOwnEventRegistration(int eventId)
    {
        try
        {
            await _eventRepository.DeleteOwnEventRegistration(eventId, Request);
            return Ok(new Response<string?>
            {
                StatusCode = 200,
                Message = "Deleted own event registration"
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpDelete("Registration")]
    [Authorize(Roles = Constants.CAN_EVENTS)]
    [ProducesResponseType(401)]
    [ProducesResponseType(typeof(Response<string?>), 200)]
    [ProducesResponseType(typeof(Response<string?>), 404)]
    [ProducesResponseType(typeof(Response<string?>), 500)]
    public async Task<ActionResult<Response<string>>> DeleteEventRegistration(int eventRegistrationId)
    {
        try
        {
            if (!await _redisService.ValidateRoles(Request.HttpContext.User, Constants.CAN_EVENTS_LIST))
                return StatusCode(401);

            await _eventRepository.DeleteEventRegistration(eventRegistrationId, Request);
            return Ok(new Response<string?>
            {
                StatusCode = 200,
                Message = "Deleted event registration"
            });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (EventPositionNotFoundException ex)
        {
            return NotFound(new Response<string?>
            {
                StatusCode = 404,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }
}