namespace Api.Middleware;

public class CustomExceptionHandlingMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlingMiddleware> logger)
{
	private const int MaxMessageLength = 255;

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			await HandleGlobalExceptionAsync(context, ex);
		}
	}

	private async Task HandleGlobalExceptionAsync(HttpContext context, Exception ex)
	{
		string requestId = context.TraceIdentifier;
		string message = ex.Message;
		if (message.Length > MaxMessageLength)
			message = $"{message[..MaxMessageLength]}...";
		if (ex is ApplicationException)
		{
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			requestId = string.Empty;
		}
		else
		{
			logger.LogError(ex, "{Message} (requestId: {RequestId})", ex.Message, requestId);
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		}
		await context.Response.WriteAsJsonAsync(new { Id = requestId, Message = message });
	}
}
