namespace Barbearia.Middleware
{
    public sealed class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    var headers = context.Response.Headers;
        //    headers.TryAdd("X-Content-Type-Options", "nosniff");
        //    headers.TryAdd("X-Frame-Options", "DENY");
        //    headers.TryAdd("Referrer-Policy", "no-referrer");
        //    headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=(), payment=(), usb=()");
        //    headers.TryAdd("Cross-Origin-Opener-Policy", "same-origin");
        //    headers.TryAdd("Cross-Origin-Resource-Policy", "same-site");
        //    headers.TryAdd("Content-Security-Policy",
        //        "default-src 'none'; frame-ancestors 'none'; base-uri 'none'; form-action 'self'");
        //    headers.TryAdd("Cache-Control", "no-store, no-cache, must-revalidate");
        //    headers.TryAdd("Pragma", "no-cache");
        //    headers.Remove("Server");
        //    headers.Remove("X-Powered-By");

        //    await _next(context);
        //}

        public async Task InvokeAsync(HttpContext context)
        {
            var isSwagger = context.Request.Path.StartsWithSegments("/swagger");

            if (isSwagger)
            {
                context.Response.Headers.ContentSecurityPolicy =
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data:; " +
                    "font-src 'self' data:; " +
                    "connect-src 'self';";
            }
            else
            {
                context.Response.Headers.ContentSecurityPolicy =
                    "default-src 'none'; " +
                    "frame-ancestors 'none'; " +
                    "base-uri 'none'; " +
                    "form-action 'none';";
            }

            context.Response.Headers.XContentTypeOptions = "nosniff";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers["Permissions-Policy"] =
                "camera=(), microphone=(), geolocation=()";

            await _next(context);
        }
    }
}
