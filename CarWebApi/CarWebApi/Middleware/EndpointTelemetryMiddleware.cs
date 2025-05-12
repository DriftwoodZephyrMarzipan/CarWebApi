using CarWebApi.Services;

namespace CarWebApi.Middleware;

public class EndpointTelemetryMiddleware
{
    private readonly RequestDelegate _next;

    public EndpointTelemetryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRequestTelemetryService telemetry)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint is RouteEndpoint routeEndpoint)
        {
            var controllerActionDescriptor = endpoint.Metadata
                .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
                .FirstOrDefault();

            if (controllerActionDescriptor != null)
            {
                var controller = controllerActionDescriptor.ControllerName;
                var method = controllerActionDescriptor.MethodInfo.Name;

                // Serialize the method's arguments if any
                var arguments = string.Empty;

                if (controllerActionDescriptor.MethodInfo.GetParameters().Any())
                {
                    var parameterValues = context.Request.Query.ToDictionary(
                        q => q.Key,
                        q => q.Value.ToString()
                    );

                    var argumentsList = controllerActionDescriptor.MethodInfo.GetParameters()
                        .Select((param, index) =>
                            $"{param.Name}={parameterValues.ElementAtOrDefault(index).Value ?? "null"}")
                        .ToArray();

                    arguments = string.Join(", ", argumentsList);
                }

                var methodWithArgs = $"{method}({arguments})";

                telemetry.Record(controller, methodWithArgs);
            }
        }

        await _next(context);
    }
}
